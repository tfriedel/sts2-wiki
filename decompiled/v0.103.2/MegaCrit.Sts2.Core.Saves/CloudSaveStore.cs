using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Debug;
using MegaCrit.Sts2.Core.Logging;

namespace MegaCrit.Sts2.Core.Saves;

public class CloudSaveStore : ICloudSaveStore, ISaveStore
{
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass35_0
	{
		public CloudSaveStore _003C_003E4__this;

		public string directoryPath;

		internal int _003COverwriteCloudWithLocalDirectory_003Eb__0(string p1, string p2)
		{
			return _003C_003E4__this.LocalStore.GetLastModifiedTime(directoryPath + "/" + p2).CompareTo(_003C_003E4__this.LocalStore.GetLastModifiedTime(directoryPath + "/" + p1));
		}
	}

	[CompilerGenerated]
	private sealed class _003COverwriteCloudWithLocalDirectory_003Ed__35 : IEnumerable<Task>, IEnumerable, IEnumerator<Task>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private Task _003C_003E2__current;

		private int _003C_003El__initialThreadId;

		public CloudSaveStore _003C_003E4__this;

		private string directoryPath;

		public string _003C_003E3__directoryPath;

		private _003C_003Ec__DisplayClass35_0 _003C_003E8__1;

		private int? byteLimit;

		public int? _003C_003E3__byteLimit;

		private int? fileLimit;

		public int? _003C_003E3__fileLimit;

		private HashSet<string> _003CfilePathsRead_003E5__2;

		private string[] _003C_003E7__wrap2;

		private int _003C_003E7__wrap3;

		private int _003CtotalFilesWritten_003E5__5;

		private List<string>.Enumerator _003C_003E7__wrap5;

		private int _003CbytesToWrite_003E5__7;

		Task IEnumerator<Task>.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		[DebuggerHidden]
		public _003COverwriteCloudWithLocalDirectory_003Ed__35(int _003C_003E1__state)
		{
			this._003C_003E1__state = _003C_003E1__state;
			_003C_003El__initialThreadId = Environment.CurrentManagedThreadId;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
			int num = _003C_003E1__state;
			if (num == -3 || num == 2)
			{
				try
				{
				}
				finally
				{
					_003C_003Em__Finally1();
				}
			}
			_003C_003E8__1 = null;
			_003CfilePathsRead_003E5__2 = null;
			_003C_003E7__wrap2 = null;
			_003C_003E7__wrap5 = default(List<string>.Enumerator);
			_003C_003E1__state = -2;
		}

		private bool MoveNext()
		{
			try
			{
				int num = _003C_003E1__state;
				CloudSaveStore cloudSaveStore = _003C_003E4__this;
				List<string> list;
				switch (num)
				{
				default:
					return false;
				case 0:
				{
					_003C_003E1__state = -1;
					_003C_003E8__1 = new _003C_003Ec__DisplayClass35_0();
					_003C_003E8__1._003C_003E4__this = _003C_003E4__this;
					_003C_003E8__1.directoryPath = directoryPath;
					Log.Debug("Writing all files in directory " + _003C_003E8__1.directoryPath + " to cloud");
					_003CfilePathsRead_003E5__2 = new HashSet<string>();
					string[] array = Array.Empty<string>();
					try
					{
						if (cloudSaveStore.CloudStore.DirectoryExists(_003C_003E8__1.directoryPath))
						{
							array = cloudSaveStore.CloudStore.GetFilesInDirectory(_003C_003E8__1.directoryPath);
						}
					}
					catch (Exception ex)
					{
						Log.Warn("Failed to list cloud files in " + _003C_003E8__1.directoryPath + ", skipping cloud delete sync: " + ex.Message);
						SentryService.CaptureException(ex);
					}
					_003C_003E7__wrap2 = array;
					_003C_003E7__wrap3 = 0;
					goto IL_0163;
				}
				case 1:
					_003C_003E1__state = -1;
					_003C_003E7__wrap3++;
					goto IL_0163;
				case 2:
					{
						_003C_003E1__state = -3;
						_003C_003E7__wrap3 += _003CbytesToWrite_003E5__7;
						_003CtotalFilesWritten_003E5__5++;
						goto IL_036c;
					}
					IL_036c:
					while (_003C_003E7__wrap5.MoveNext())
					{
						string current = _003C_003E7__wrap5.Current;
						if (!_003CfilePathsRead_003E5__2.Contains(current) && !current.EndsWith(".backup"))
						{
							string path = _003C_003E8__1.directoryPath + "/" + current;
							_003CbytesToWrite_003E5__7 = cloudSaveStore.LocalStore.GetFileSize(path);
							bool flag = (byteLimit.HasValue && _003C_003E7__wrap3 + _003CbytesToWrite_003E5__7 > byteLimit.Value) || (fileLimit.HasValue && _003CtotalFilesWritten_003E5__5 + 1 > fileLimit.Value);
							if (flag)
							{
								Log.Info($"File {current} will be immediately forgotten after writing to cloud. Bytes written:{_003C_003E7__wrap3 + _003CbytesToWrite_003E5__7}. Files written: {_003CtotalFilesWritten_003E5__5 + 1}");
							}
							_003C_003E2__current = cloudSaveStore.OverwriteCloudWithLocal(path, flag);
							_003C_003E1__state = 2;
							return true;
						}
					}
					_003C_003Em__Finally1();
					_003C_003E7__wrap5 = default(List<string>.Enumerator);
					break;
					IL_0163:
					if (_003C_003E7__wrap3 < _003C_003E7__wrap2.Length)
					{
						string text = _003C_003E7__wrap2[_003C_003E7__wrap3];
						_003CfilePathsRead_003E5__2.Add(text);
						_003C_003E2__current = cloudSaveStore.OverwriteCloudWithLocal(_003C_003E8__1.directoryPath + "/" + text);
						_003C_003E1__state = 1;
						return true;
					}
					_003C_003E7__wrap2 = null;
					if (!cloudSaveStore.LocalStore.DirectoryExists(_003C_003E8__1.directoryPath))
					{
						break;
					}
					list = cloudSaveStore.LocalStore.GetFilesInDirectory(_003C_003E8__1.directoryPath).ToList();
					_003C_003E7__wrap3 = 0;
					_003CtotalFilesWritten_003E5__5 = 0;
					if (byteLimit.HasValue || fileLimit.HasValue)
					{
						list.Sort((string p1, string p2) => _003C_003E8__1._003C_003E4__this.LocalStore.GetLastModifiedTime(_003C_003E8__1.directoryPath + "/" + p2).CompareTo(_003C_003E8__1._003C_003E4__this.LocalStore.GetLastModifiedTime(_003C_003E8__1.directoryPath + "/" + p1)));
					}
					_003C_003E7__wrap5 = list.GetEnumerator();
					_003C_003E1__state = -3;
					goto IL_036c;
				}
				return false;
			}
			catch
			{
				//try-fault
				((IDisposable)this).Dispose();
				throw;
			}
		}

		bool IEnumerator.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			return this.MoveNext();
		}

		private void _003C_003Em__Finally1()
		{
			_003C_003E1__state = -1;
			((IDisposable)_003C_003E7__wrap5).Dispose();
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}

		[DebuggerHidden]
		IEnumerator<Task> IEnumerable<Task>.GetEnumerator()
		{
			_003COverwriteCloudWithLocalDirectory_003Ed__35 _003COverwriteCloudWithLocalDirectory_003Ed__;
			if (_003C_003E1__state == -2 && _003C_003El__initialThreadId == Environment.CurrentManagedThreadId)
			{
				_003C_003E1__state = 0;
				_003COverwriteCloudWithLocalDirectory_003Ed__ = this;
			}
			else
			{
				_003COverwriteCloudWithLocalDirectory_003Ed__ = new _003COverwriteCloudWithLocalDirectory_003Ed__35(0)
				{
					_003C_003E4__this = _003C_003E4__this
				};
			}
			_003COverwriteCloudWithLocalDirectory_003Ed__.directoryPath = _003C_003E3__directoryPath;
			_003COverwriteCloudWithLocalDirectory_003Ed__.byteLimit = _003C_003E3__byteLimit;
			_003COverwriteCloudWithLocalDirectory_003Ed__.fileLimit = _003C_003E3__fileLimit;
			return _003COverwriteCloudWithLocalDirectory_003Ed__;
		}

		[DebuggerHidden]
		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<Task>)this).GetEnumerator();
		}
	}

	[CompilerGenerated]
	private sealed class _003CSyncCloudToLocalDirectory_003Ed__33 : IEnumerable<Task>, IEnumerable, IEnumerator<Task>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private Task _003C_003E2__current;

		private int _003C_003El__initialThreadId;

		private string directoryPath;

		public string _003C_003E3__directoryPath;

		public CloudSaveStore _003C_003E4__this;

		private HashSet<string> _003CfilePathsRead_003E5__2;

		private string[] _003C_003E7__wrap2;

		private int _003C_003E7__wrap3;

		Task IEnumerator<Task>.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		[DebuggerHidden]
		public _003CSyncCloudToLocalDirectory_003Ed__33(int _003C_003E1__state)
		{
			this._003C_003E1__state = _003C_003E1__state;
			_003C_003El__initialThreadId = Environment.CurrentManagedThreadId;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
			_003CfilePathsRead_003E5__2 = null;
			_003C_003E7__wrap2 = null;
			_003C_003E1__state = -2;
		}

		private bool MoveNext()
		{
			int num = _003C_003E1__state;
			CloudSaveStore cloudSaveStore = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
			{
				_003C_003E1__state = -1;
				Log.Debug("Syncing all files in " + directoryPath + " from cloud to local");
				_003CfilePathsRead_003E5__2 = new HashSet<string>();
				string[] array = Array.Empty<string>();
				try
				{
					if (cloudSaveStore.CloudStore.DirectoryExists(directoryPath))
					{
						array = cloudSaveStore.CloudStore.GetFilesInDirectory(directoryPath);
					}
				}
				catch (Exception ex)
				{
					Log.Warn("Failed to list cloud files in " + directoryPath + ", skipping cloud sync: " + ex.Message);
					SentryService.CaptureException(ex);
				}
				_003C_003E7__wrap2 = array;
				_003C_003E7__wrap3 = 0;
				goto IL_012a;
			}
			case 1:
				_003C_003E1__state = -1;
				_003C_003E7__wrap3++;
				goto IL_012a;
			case 2:
				{
					_003C_003E1__state = -1;
					goto IL_01e1;
				}
				IL_01ef:
				if (_003C_003E7__wrap3 < _003C_003E7__wrap2.Length)
				{
					string text = _003C_003E7__wrap2[_003C_003E7__wrap3];
					string text2 = directoryPath + "/" + text;
					if (!_003CfilePathsRead_003E5__2.Contains(text2))
					{
						Log.Debug("Checking file " + text2 + " in local saves");
						_003C_003E2__current = cloudSaveStore.SyncCloudToLocal(text2);
						_003C_003E1__state = 2;
						return true;
					}
					goto IL_01e1;
				}
				_003C_003E7__wrap2 = null;
				break;
				IL_012a:
				if (_003C_003E7__wrap3 < _003C_003E7__wrap2.Length)
				{
					string text3 = _003C_003E7__wrap2[_003C_003E7__wrap3];
					string text4 = directoryPath + "/" + text3;
					_003CfilePathsRead_003E5__2.Add(text4);
					Log.Debug("Checking file " + text4 + " in cloud saves");
					_003C_003E2__current = cloudSaveStore.SyncCloudToLocal(text4);
					_003C_003E1__state = 1;
					return true;
				}
				_003C_003E7__wrap2 = null;
				if (!cloudSaveStore.LocalStore.DirectoryExists(directoryPath))
				{
					break;
				}
				_003C_003E7__wrap2 = cloudSaveStore.LocalStore.GetFilesInDirectory(directoryPath);
				_003C_003E7__wrap3 = 0;
				goto IL_01ef;
				IL_01e1:
				_003C_003E7__wrap3++;
				goto IL_01ef;
			}
			return false;
		}

		bool IEnumerator.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			return this.MoveNext();
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}

		[DebuggerHidden]
		IEnumerator<Task> IEnumerable<Task>.GetEnumerator()
		{
			_003CSyncCloudToLocalDirectory_003Ed__33 _003CSyncCloudToLocalDirectory_003Ed__;
			if (_003C_003E1__state == -2 && _003C_003El__initialThreadId == Environment.CurrentManagedThreadId)
			{
				_003C_003E1__state = 0;
				_003CSyncCloudToLocalDirectory_003Ed__ = this;
			}
			else
			{
				_003CSyncCloudToLocalDirectory_003Ed__ = new _003CSyncCloudToLocalDirectory_003Ed__33(0)
				{
					_003C_003E4__this = _003C_003E4__this
				};
			}
			_003CSyncCloudToLocalDirectory_003Ed__.directoryPath = _003C_003E3__directoryPath;
			return _003CSyncCloudToLocalDirectory_003Ed__;
		}

		[DebuggerHidden]
		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<Task>)this).GetEnumerator();
		}
	}

	public ISaveStore LocalStore { get; }

	public ICloudSaveStore CloudStore { get; }

	public CloudSaveStore(ISaveStore localStore, ICloudSaveStore cloudStore)
	{
		LocalStore = localStore;
		CloudStore = cloudStore;
	}

	public string? ReadFile(string path)
	{
		return LocalStore.ReadFile(path);
	}

	public Task<string?> ReadFileAsync(string path)
	{
		return LocalStore.ReadFileAsync(path);
	}

	public bool FileExists(string path)
	{
		return LocalStore.FileExists(path);
	}

	public bool DirectoryExists(string path)
	{
		return LocalStore.DirectoryExists(path);
	}

	public void WriteFile(string path, string content)
	{
		LocalStore.WriteFile(path, content);
		try
		{
			CloudStore.WriteFile(path, content);
			SyncLocalTimestamp(path);
		}
		catch (Exception ex)
		{
			Log.Warn("Cloud write failed for " + path + ", local file preserved: " + ex.Message);
			SyncLocalTimestamp(path);
		}
	}

	public void WriteFile(string path, byte[] bytes)
	{
		LocalStore.WriteFile(path, bytes);
		try
		{
			CloudStore.WriteFile(path, bytes);
			SyncLocalTimestamp(path);
		}
		catch (Exception ex)
		{
			Log.Warn("Cloud write failed for " + path + ", local file preserved: " + ex.Message);
			SyncLocalTimestamp(path);
		}
	}

	public async Task WriteFileAsync(string path, string content)
	{
		await LocalStore.WriteFileAsync(path, content);
		try
		{
			await CloudStore.WriteFileAsync(path, content);
			SyncLocalTimestamp(path);
		}
		catch (Exception ex)
		{
			Log.Warn("Cloud write failed for " + path + ", local file preserved: " + ex.Message);
			SyncLocalTimestamp(path);
		}
	}

	public async Task WriteFileAsync(string path, byte[] bytes)
	{
		await LocalStore.WriteFileAsync(path, bytes);
		try
		{
			await CloudStore.WriteFileAsync(path, bytes);
			SyncLocalTimestamp(path);
		}
		catch (Exception ex)
		{
			Log.Warn("Cloud write failed for " + path + ", local file preserved: " + ex.Message);
			SyncLocalTimestamp(path);
		}
	}

	public void DeleteFile(string path)
	{
		LocalStore.DeleteFile(path);
		try
		{
			CloudStore.DeleteFile(path);
		}
		catch (Exception ex)
		{
			Log.Warn("Cloud delete failed for " + path + ", local delete preserved: " + ex.Message);
		}
	}

	public void RenameFile(string sourcePath, string destinationPath)
	{
		LocalStore.RenameFile(sourcePath, destinationPath);
		try
		{
			CloudStore.RenameFile(sourcePath, destinationPath);
		}
		catch (Exception ex)
		{
			Log.Warn($"Cloud rename failed for {sourcePath} -> {destinationPath}, local rename preserved: {ex.Message}");
		}
	}

	public string[] GetFilesInDirectory(string directoryPath)
	{
		return LocalStore.GetFilesInDirectory(directoryPath);
	}

	public string[] GetDirectoriesInDirectory(string directoryPath)
	{
		return LocalStore.GetDirectoriesInDirectory(directoryPath);
	}

	public void CreateDirectory(string directoryPath)
	{
		LocalStore.CreateDirectory(directoryPath);
		try
		{
			CloudStore.CreateDirectory(directoryPath);
		}
		catch (Exception ex)
		{
			Log.Warn("Cloud create directory failed for " + directoryPath + ": " + ex.Message);
		}
	}

	public void DeleteDirectory(string directoryPath)
	{
		LocalStore.DeleteDirectory(directoryPath);
		try
		{
			CloudStore.DeleteDirectory(directoryPath);
		}
		catch (Exception ex)
		{
			Log.Warn("Cloud delete directory failed for " + directoryPath + ": " + ex.Message);
		}
	}

	public void DeleteTemporaryFiles(string directoryPath)
	{
		LocalStore.DeleteTemporaryFiles(directoryPath);
		try
		{
			CloudStore.DeleteTemporaryFiles(directoryPath);
		}
		catch (Exception ex)
		{
			Log.Warn("Cloud delete temporary files failed for " + directoryPath + ": " + ex.Message);
		}
	}

	public DateTimeOffset GetLastModifiedTime(string path)
	{
		return LocalStore.GetLastModifiedTime(path);
	}

	public int GetFileSize(string path)
	{
		return LocalStore.GetFileSize(path);
	}

	public void SetLastModifiedTime(string path, DateTimeOffset time)
	{
		LocalStore.SetLastModifiedTime(path, time);
	}

	public string GetFullPath(string filename)
	{
		return LocalStore.GetFullPath(filename);
	}

	public bool HasCloudFiles()
	{
		return CloudStore.HasCloudFiles();
	}

	public void ForgetFile(string path)
	{
		try
		{
			CloudStore.ForgetFile(path);
		}
		catch (Exception ex)
		{
			Log.Warn("Cloud forget failed for " + path + ": " + ex.Message);
		}
	}

	public bool IsFilePersisted(string path)
	{
		return CloudStore.IsFilePersisted(path);
	}

	public void BeginSaveBatch()
	{
		CloudStore.BeginSaveBatch();
	}

	public void EndSaveBatch()
	{
		CloudStore.EndSaveBatch();
	}

	public async Task SyncCloudToLocal(string path)
	{
		try
		{
			await SyncCloudToLocalInternal(path);
		}
		catch (Exception ex)
		{
			Log.Warn("SteamRemoteStorage: Failed to sync " + path + " from cloud, skipping: " + ex.Message);
			SentryService.CaptureException(ex);
		}
	}

	private async Task SyncCloudToLocalInternal(string path)
	{
		bool flag = CloudStore.FileExists(path);
		bool flag2 = LocalStore.FileExists(path);
		if (flag)
		{
			DateTimeOffset lastModifiedTime = CloudStore.GetLastModifiedTime(path);
			DateTimeOffset? dateTimeOffset = (flag2 ? new DateTimeOffset?(LocalStore.GetLastModifiedTime(path)) : null);
			if (!flag2 || lastModifiedTime != dateTimeOffset)
			{
				Log.Info($"Copying {path} from cloud to local. Local file exists: {flag2} Cloud save time: {lastModifiedTime} Local save time: {dateTimeOffset}");
				string content = await CloudStore.ReadFileAsync(path);
				await LocalStore.WriteFileAsync(path, content);
				SyncLocalTimestamp(path);
			}
			else
			{
				Log.Debug($"Skipping sync for {path}, last modified time matches on local and remote ({lastModifiedTime})");
			}
		}
		else if (flag2)
		{
			Log.Info("Deleting " + path + " because it does not exist on remote");
			LocalStore.DeleteFile(path);
			LocalStore.DeleteFile(path + ".backup");
		}
		else
		{
			Log.Debug("Skipping sync for " + path + ", it doesn't exist on either local or cloud");
		}
	}

	[IteratorStateMachine(typeof(_003CSyncCloudToLocalDirectory_003Ed__33))]
	public IEnumerable<Task> SyncCloudToLocalDirectory(string directoryPath)
	{
		//yield-return decompiler failed: Unexpected instruction in Iterator.Dispose()
		return new _003CSyncCloudToLocalDirectory_003Ed__33(-2)
		{
			_003C_003E4__this = this,
			_003C_003E3__directoryPath = directoryPath
		};
	}

	public async Task OverwriteCloudWithLocal(string path, bool forgetImmediately = false)
	{
		if (LocalStore.FileExists(path))
		{
			Log.Debug("Writing file " + path + " to cloud");
			string content = await LocalStore.ReadFileAsync(path);
			try
			{
				await CloudStore.WriteFileAsync(path, content);
				if (forgetImmediately)
				{
					try
					{
						Log.Debug("Immediately forgetting " + path);
						CloudStore.ForgetFile(path);
					}
					catch (Exception ex)
					{
						Log.Warn("Cloud forget failed for " + path + ": " + ex.Message);
					}
				}
				SyncLocalTimestamp(path);
				return;
			}
			catch (Exception ex2)
			{
				Log.Warn("Cloud write failed for " + path + ", local file preserved: " + ex2.Message);
				SyncLocalTimestamp(path);
				return;
			}
		}
		try
		{
			if (CloudStore.FileExists(path))
			{
				Log.Debug("Deleting file " + path + " from cloud because it doesn't exist on local");
				CloudStore.DeleteFile(path);
			}
		}
		catch (Exception ex3)
		{
			Log.Warn("Cloud delete failed for " + path + ": " + ex3.Message);
		}
	}

	[IteratorStateMachine(typeof(_003COverwriteCloudWithLocalDirectory_003Ed__35))]
	public IEnumerable<Task> OverwriteCloudWithLocalDirectory(string directoryPath, int? byteLimit, int? fileLimit)
	{
		//yield-return decompiler failed: Unexpected instruction in Iterator.Dispose()
		return new _003COverwriteCloudWithLocalDirectory_003Ed__35(-2)
		{
			_003C_003E4__this = this,
			_003C_003E3__directoryPath = directoryPath,
			_003C_003E3__byteLimit = byteLimit,
			_003C_003E3__fileLimit = fileLimit
		};
	}

	public void ForgetFilesInDirectoryBeforeWritingIfNecessary(string directoryPath, int bytesToBeWritten, int byteLimit, int fileLimit)
	{
		try
		{
			ForgetFilesInDirectoryBeforeWritingIfNecessaryInternal(directoryPath, bytesToBeWritten, byteLimit, fileLimit);
		}
		catch (Exception ex)
		{
			Log.Warn("Cloud quota management failed for " + directoryPath + ": " + ex.Message);
			SentryService.CaptureException(ex);
		}
	}

	private void ForgetFilesInDirectoryBeforeWritingIfNecessaryInternal(string directoryPath, int bytesToBeWritten, int byteLimit, int fileLimit)
	{
		int num = bytesToBeWritten;
		int num2 = 1;
		string[] filesInDirectory = CloudStore.GetFilesInDirectory(directoryPath);
		List<string> list = new List<string>();
		string[] array = filesInDirectory;
		foreach (string text in array)
		{
			string text2 = directoryPath + "/" + text;
			if (CloudStore.IsFilePersisted(text2))
			{
				list.Add(text2);
				num += CloudStore.GetFileSize(text2);
				num2++;
			}
		}
		if (num > byteLimit || num2 > fileLimit)
		{
			list.Sort((string p1, string p2) => GetLastModifiedTime(p2).CompareTo(GetLastModifiedTime(p1)));
			while (num > byteLimit || num2 > fileLimit)
			{
				string text3 = list[list.Count - 1];
				num -= CloudStore.GetFileSize(text3);
				num2--;
				Log.Info($"Forgetting file {text3} from cloud storage because we're past our quota. Bytes after forgetting: {num}. Files after forgetting: {num2}");
				CloudStore.ForgetFile(text3);
				list.RemoveAt(list.Count - 1);
			}
		}
	}

	private void SyncLocalTimestamp(string path)
	{
		try
		{
			LocalStore.SetLastModifiedTime(path, CloudStore.GetLastModifiedTime(path));
		}
		catch (Exception ex)
		{
			Log.Warn("Failed to sync timestamp for " + path + ", will re-sync on next launch: " + ex.Message);
		}
	}
}
