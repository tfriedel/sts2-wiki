using System;
using System.IO;
using Godot;

namespace MegaCrit.Sts2.Core.Saves;

public class FileAccessStream : Stream
{
	private readonly FileAccess _file;

	private readonly ModeFlags _flags;

	private readonly string _filePath;

	public override bool CanRead
	{
		get
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			if (_file.IsOpen())
			{
				return ((Enum)_flags).HasFlag((Enum)(object)(ModeFlags)1);
			}
			return false;
		}
	}

	public override bool CanSeek => _file.IsOpen();

	public override bool CanWrite
	{
		get
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			if (_file.IsOpen())
			{
				return ((Enum)_flags).HasFlag((Enum)(object)(ModeFlags)2);
			}
			return false;
		}
	}

	public override long Length => (long)_file.GetLength();

	public override long Position
	{
		get
		{
			return (long)_file.GetPosition();
		}
		set
		{
			_file.Seek((ulong)value);
		}
	}

	public FileAccessStream(string filePath, ModeFlags flags)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		_flags = flags;
		_filePath = filePath;
		FileAccess val = FileAccess.Open(filePath, flags);
		_file = val ?? throw new IOException($"Opening file {filePath} with flags {flags} failed: {FileAccess.GetOpenError()}");
	}

	public override void Flush()
	{
		_file.Flush();
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		byte[] buffer2 = _file.GetBuffer((long)count);
		int num = Math.Min(count, buffer2.Length);
		Array.Copy(buffer2, 0, buffer, offset, num);
		return num;
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		int num = Math.Min(buffer.Length - offset, count);
		bool flag;
		if (offset == 0 && buffer.Length <= count)
		{
			flag = _file.StoreBuffer(buffer);
		}
		else
		{
			byte[] array = new byte[num];
			Array.Copy(buffer, offset, array, 0, num);
			flag = _file.StoreBuffer(array);
		}
		if (!flag)
		{
			throw new IOException($"Failed to write {num} bytes to file {_filePath}: {_file.GetError()}");
		}
	}

	public override long Seek(long offset, SeekOrigin origin)
	{
		switch (origin)
		{
		case SeekOrigin.Begin:
			_file.Seek((ulong)offset);
			break;
		case SeekOrigin.Current:
			_file.Seek(_file.GetPosition() + (ulong)offset);
			break;
		case SeekOrigin.End:
			_file.Seek(_file.GetLength() - (ulong)offset);
			break;
		}
		return (long)_file.GetPosition();
	}

	public override void SetLength(long value)
	{
		throw new NotImplementedException();
	}

	protected override void Dispose(bool disposing)
	{
		_file.Close();
	}
}
