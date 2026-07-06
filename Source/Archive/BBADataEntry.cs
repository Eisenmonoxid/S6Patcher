using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace S6Patcher.Source.Archive
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public unsafe struct BBADataEntryDefinition
	{
		public UInt64 TimeStamp;
		public UInt32 DecompressedFileSize;
		public UInt32 DecompressedCRC32;
		public UInt32 FileType;
		public fixed byte Null1[4];
		public UInt32 FileOffset;
		public fixed byte Null2[4];
		public UInt32 CompressedFileSize;
		public UInt32 CompressedCRC32;
		public fixed byte Null3[8];
		public UInt32 FileNameLength;
		public UInt32 FileNameOffset;
		public Int32 FirstChild;
		public Int32 NextSibling;
		// string FileName follows (dynamic length)
	}

	internal class BBADataEntry
	{
		public enum BBAArchiveFileType
		{
			None = 0,
			Compressed = 1,
			Directory = 256
		}

		private static readonly HashSet<string> CompressedFileTypes = ["xml", "bin", "fdb", "lua", "fx", "anm", "dff", "spt", "dds", "txt", "cs"];
        public static bool IsCompressed(string Extension) => CompressedFileTypes.Contains(Extension.TrimStart('.').ToLowerInvariant());

		private BBADataEntryDefinition Definition;
		public readonly string FileName;
		public ref BBADataEntryDefinition GetDefinition() => ref Definition;
		public static readonly int Size = Helpers.GetSerializedSize<BBADataEntryDefinition>();
		public static UInt64 GetTimeStamp(DateTime Date) => BitConverter.ToUInt64(BitConverter.GetBytes(Date.Ticks), 0);
		public uint GetPaddedSize() => (uint)(Size + Definition.FileNameLength + (4 - (Definition.FileNameLength % 4)));

		public BBADataEntry(ReadOnlySpan<byte> Data)
		{
			Definition = Helpers.Parse<BBADataEntryDefinition>(Data);
			FileName = Encoding.ASCII.GetString(Data[Size..(int)(Size + Definition.FileNameLength)]).TrimEnd('\0');
		}

		public BBADataEntry(string Name)
		{
			FileName = Name;
			Definition = new BBADataEntryDefinition();

			unsafe
			{
				Helpers.FillBufferWithZeroes(ref Definition.Null1[0], 4);
				Helpers.FillBufferWithZeroes(ref Definition.Null2[0], 4);
				Helpers.FillBufferWithZeroes(ref Definition.Null3[0], 8);
			}
		}

        public ReadOnlySpan<byte> Serialize()
		{
			byte[] Name = Encoding.ASCII.GetBytes(FileName);
			Span<byte> Result = Helpers.Serialize(Definition, (int)GetPaddedSize());
			Name.CopyTo(Result[Size..]);
			return Result;
		}
	}
}
