using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Hashing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace S6Patcher.Source.Archive
{
	public static class Helpers
	{
		public static string SanitizePath(string Base, string Result) => Result.Replace(Base, "").Replace("/", "\\").TrimStart('\\').ToLower();
		
		public static UInt64 GetLastWriteTimeFromFileOrDirectory(string FileOrDirectoryPath)
		{
			Func<string, DateTime> GetWriteTime = File.Exists(FileOrDirectoryPath) ? File.GetLastWriteTime : Directory.GetLastWriteTime;
			try
			{
				return BBADataEntry.GetTimeStamp(GetWriteTime(FileOrDirectoryPath));
			}
			catch
			{
				return 0;
			}
		}
		
		public static List<string> GetFiles(string Input)
		{
			List<string> Files = [.. Directory.GetFiles(Input)];
			foreach (string Element in Directory.GetDirectories(Input))
			{
                Files.Add(Element);
                Files.AddRange(GetFiles(Element));
			}

			return Files;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref readonly T Parse<T>(ReadOnlySpan<byte> Buffer) where T : unmanaged => ref MemoryMarshal.AsRef<T>(Buffer);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int GetSerializedSize<T>() where T : unmanaged => Unsafe.SizeOf<T>();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Span<byte> Serialize<T>(in T Struct, int Size = 0) where T : unmanaged
		{
			byte[] Buffer = new byte[Size == 0 ? GetSerializedSize<T>() : Size];
			MemoryMarshal.Write(Buffer, in Struct);
			return Buffer.AsSpan();
		}

		public static unsafe void FillBufferWithZeroes(ref byte Buffer, int Length)
		{
			fixed (byte* Pointer = &Buffer)
			{
				new Span<byte>(Pointer, Length).Clear();
			}
		}

		public static UInt32 GetCRC32OfData(ReadOnlySpan<byte> Data)
		{
			Crc32 CRC = new();
			CRC.Append(Data);
			return CRC.GetCurrentHashAsUInt32();
		}

		public static UInt32 GetCombinedCRC32OfData(ReadOnlySpan<byte> Data1, ReadOnlySpan<byte> Data2)
		{
			Crc32 CRC = new();
			CRC.Append(Data1);
			CRC.Append(Data2);
			return CRC.GetCurrentHashAsUInt32();
		}
    }
}
