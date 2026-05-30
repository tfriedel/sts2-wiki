using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using MegaCrit.Sts2.Core.Exceptions;
using MegaCrit.Sts2.Core.Logging;

namespace MegaCrit.Sts2.Core.Saves;

public class GodotFileIo : ISaveStore
{
	public string SaveDir { get; set; }

	public GodotFileIo(string saveDir)
	{
		SaveDir = saveDir;
		CreateDirectory(SaveDir);
	}

	public string GetFullPath(string filename)
	{
		if (filename.StartsWith(SaveDir))
		{
			return filename;
		}
		return SaveDir + "/" + filename;
	}

	public string? ReadFile(string path)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Invalid comparison between Unknown and I8
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		path = GetFullPath(path);
		FileAccess val = FileAccess.Open(path, (ModeFlags)1);
		try
		{
			if (val == null)
			{
				Error openError = FileAccess.GetOpenError();
				if ((long)openError == 7)
				{
					Log.Warn("Tried to read file at " + path + ", but there was no such file");
					return null;
				}
				throw new SaveException($"Failed to open file for reading. path='{path}' error={openError}");
			}
			string asText = val.GetAsText(false);
			val.Close();
			return asText;
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public async Task<string?> ReadFileAsync(string path)
	{
		path = GetFullPath(path);
		ValidateGodotFilePath(path);
		FileAccessStream stream = new FileAccessStream(path, (ModeFlags)1);
		string @string;
		try
		{
			using MemoryStream memoryStream = new MemoryStream();
			await stream.CopyToAsync(memoryStream);
			@string = Encoding.UTF8.GetString(memoryStream.ToArray());
		}
		finally
		{
			if (stream != null)
			{
				await stream.DisposeAsync();
			}
		}
		return @string;
	}

	public DateTimeOffset GetLastModifiedTime(string path)
	{
		path = GetFullPath(path);
		return DateTimeOffset.FromUnixTimeSeconds((long)FileAccess.GetModifiedTime(path));
	}

	public int GetFileSize(string path)
	{
		path = GetFullPath(path);
		return (int)FileAccess.GetSize(path);
	}

	public void SetLastModifiedTime(string path, DateTimeOffset time)
	{
		path = GetFullPath(path);
		File.SetLastWriteTimeUtc(ProjectSettings.GlobalizePath(path), time.UtcDateTime);
	}

	public void WriteFile(string path, string content)
	{
		if (string.IsNullOrWhiteSpace(content))
		{
			Log.Error("The content is empty for path='" + path + "'");
		}
		else
		{
			WriteFile(path, Encoding.UTF8.GetBytes(content));
		}
	}

	public void WriteFile(string path, byte[] bytes)
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		path = GetFullPath(path);
		ValidateGodotFilePath(path);
		CopyBackup(path);
		string text = path + ".tmp";
		FileAccess val = FileAccess.Open(text, (ModeFlags)2);
		try
		{
			if (val == null)
			{
				throw new SaveException($"Failed to open file for writing. path='{text}' error={FileAccess.GetOpenError()}");
			}
			if (val.StoreBuffer(bytes))
			{
				val.Close();
				RenameFile(text, path);
				Log.Info($"Wrote {bytes.Length} bytes to path={path} save_dir={SaveDir}");
				return;
			}
			throw new SaveException($"Failed to write {bytes.Length} bytes to path={path} save_dir={SaveDir}. Error: {val.GetError()}");
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public Task WriteFileAsync(string path, string content)
	{
		return WriteFileAsync(path, Encoding.UTF8.GetBytes(content));
	}

	public async Task WriteFileAsync(string path, byte[] bytes)
	{
		path = GetFullPath(path);
		ValidateGodotFilePath(path);
		CopyBackup(path);
		string tempPath = path + ".tmp";
		FileAccessStream stream = new FileAccessStream(tempPath, (ModeFlags)2);
		try
		{
			await stream.WriteAsync(bytes);
			long position = stream.Position;
			stream.Close();
			RenameFile(tempPath, path);
			Log.Info($"Wrote {position} bytes to path={path} save_dir={SaveDir}");
		}
		finally
		{
			if (stream != null)
			{
				await stream.DisposeAsync();
			}
		}
	}

	public bool FileExists(string path)
	{
		return FileAccess.FileExists(GetFullPath(path));
	}

	public bool DirectoryExists(string path)
	{
		return DirAccess.DirExistsAbsolute(GetFullPath(path));
	}

	public void DeleteFile(string path)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		Error val = DirAccess.RemoveAbsolute(GetFullPath(path));
		if ((int)val != 0)
		{
			Log.Error($"Error deleting path {path}: {val}");
		}
	}

	public void RenameFile(string sourcePath, string destinationPath)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		if (!FileExists(sourcePath))
		{
			throw new SaveException("Cannot rename file: source does not exist. source=" + GetFullPath(sourcePath));
		}
		sourcePath = GetFullPath(sourcePath);
		destinationPath = GetFullPath(destinationPath);
		for (int i = 1; i <= 4; i++)
		{
			Error val = DirAccess.RenameAbsolute(sourcePath, destinationPath);
			if ((int)val == 0)
			{
				break;
			}
			if (FileAccess.FileExists(destinationPath))
			{
				Log.Warn($"Rename reported error={val} but destination exists, treating as success. source={sourcePath}");
				break;
			}
			if (i < 4)
			{
				Log.Warn($"Rename failed (attempt {i}/{4}), retrying. error={val} source={sourcePath}");
				Thread.Sleep(50);
				continue;
			}
			throw new SaveException($"Failed to rename file. error={val} source={sourcePath} destination={destinationPath} source_exists={FileAccess.FileExists(sourcePath)} destination_exists={FileAccess.FileExists(destinationPath)}");
		}
	}

	public string[] GetFilesInDirectory(string directoryPath)
	{
		directoryPath = GetFullPath(directoryPath);
		return DirAccess.GetFilesAt(directoryPath);
	}

	public string[] GetDirectoriesInDirectory(string directoryPath)
	{
		directoryPath = GetFullPath(directoryPath);
		return DirAccess.GetDirectoriesAt(directoryPath);
	}

	public void CreateDirectory(string directoryPath)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		directoryPath = GetFullPath(directoryPath);
		if (!DirAccess.DirExistsAbsolute(directoryPath))
		{
			DirAccess.MakeDirRecursiveAbsolute(directoryPath);
		}
	}

	public void DeleteDirectory(string directoryPath)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		directoryPath = GetFullPath(directoryPath);
		if (!DirAccess.DirExistsAbsolute(directoryPath))
		{
			return;
		}
		DirAccess val = DirAccess.Open(directoryPath);
		try
		{
			val.IncludeHidden = true;
			string[] files = val.GetFiles();
			foreach (string text in files)
			{
				Error val2 = val.Remove(text);
				if ((int)val2 != 0)
				{
					throw new InvalidOperationException($"Got error {val2} trying to delete file {text} in directory {directoryPath}");
				}
			}
			string[] directories = val.GetDirectories();
			foreach (string text2 in directories)
			{
				DeleteDirectory(directoryPath + "/" + text2);
			}
			Error val3 = val.Remove("");
			if ((int)val3 != 0)
			{
				throw new InvalidOperationException($"Got error {val3} trying to delete directory {directoryPath}");
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void DeleteTemporaryFiles(string directoryPath)
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		directoryPath = GetFullPath(directoryPath);
		DirAccess val = DirAccess.Open(directoryPath);
		try
		{
			if (val == null)
			{
				return;
			}
			string[] files = val.GetFiles();
			foreach (string text in files)
			{
				if (text.EndsWith(".tmp"))
				{
					Log.Info("Cleaning up orphaned " + text + " in " + directoryPath);
					Error val2 = val.Remove(text);
					if ((int)val2 != 0)
					{
						Log.Warn($"Couldn't delete temporary file {text} in {directoryPath}, error={val2}");
					}
				}
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private static void CopyBackup(string fullPath)
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		if (!FileAccess.FileExists(fullPath))
		{
			return;
		}
		string text = fullPath + ".backup";
		FileAccess val = FileAccess.Open(fullPath, (ModeFlags)1);
		try
		{
			if (val == null)
			{
				Log.Warn($"Failed to open source for backup copy. path={fullPath} error={FileAccess.GetOpenError()}");
				return;
			}
			byte[] buffer = val.GetBuffer((long)val.GetLength());
			val.Close();
			FileAccess val2 = FileAccess.Open(text, (ModeFlags)2);
			try
			{
				if (val2 == null)
				{
					Log.Warn($"Failed to open backup for writing. path={text} error={FileAccess.GetOpenError()}");
					return;
				}
				if (!val2.StoreBuffer(buffer))
				{
					Log.Warn($"Copying backup from {fullPath} to {text} failed: {val2.GetError()}");
				}
				val2.Close();
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private static void ValidateGodotFilePath(string godotFilePath)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if (!godotFilePath.Contains("://"))
		{
			throw new SaveException("The path='" + godotFilePath + "' is not a godot file path");
		}
		string baseDir = StringExtensions.GetBaseDir(godotFilePath);
		if (!DirAccess.DirExistsAbsolute(baseDir))
		{
			DirAccess.MakeDirRecursiveAbsolute(baseDir);
		}
	}
}
