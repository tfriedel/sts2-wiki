using System.IO;
using Godot;
using MegaCrit.Sts2.Core.Saves;

namespace MegaCrit.Sts2.Core.Modding;

public class ModManagerFileIo : IModManagerFileIo
{
	public string[] GetFilesAt(string path)
	{
		return DirAccess.GetFilesAt(path);
	}

	public string[] GetDirectoriesAt(string path)
	{
		return DirAccess.GetDirectoriesAt(path);
	}

	public bool FileExists(string path)
	{
		return FileAccess.FileExists(path);
	}

	public bool DirectoryExists(string path)
	{
		return DirAccess.DirExistsAbsolute(path);
	}

	public Stream OpenStream(string path, ModeFlags mode)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return new FileAccessStream(path, mode);
	}
}
