using System;
using System.Runtime.InteropServices;

namespace S6Patcher.Source.Archive
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public unsafe struct BBADirectoryDefinition
	{
		public UInt32 HeaderSize;
		public UInt32 OffsetFileEntries;
		public UInt32 OffsetFileHashtable;
		public fixed byte Null1[52];
		public fixed byte ArchiveHeader[3];
		public byte Version;
		public byte Unknown5;
		public UInt32 HeaderSize2;
		public UInt32 HeaderEncryptionID;
		public fixed byte Null2[16];
		public UInt32 DirectoryEncryptionID;
		public UInt32 Unknown1;
		public fixed byte Null3[40];
		public fixed UInt32 EncryptionIDs[8];
		public fixed byte Null4[96];
		public UInt32 NumberOfFiles;
		// Entries follow (dynamic length)
	}

	internal class BBADirectory
	{
		private BBADirectoryDefinition Definition;
		public ref BBADirectoryDefinition GetDefinition() => ref Definition;
		public ReadOnlySpan<byte> Serialize() => Helpers.Serialize(Definition);
		public static readonly int Size = Helpers.GetSerializedSize<BBADirectoryDefinition>();

		public BBADirectory(ReadOnlySpan<byte> Data)
		{
			Definition = Helpers.Parse<BBADirectoryDefinition>(Data);
		}

		public BBADirectory(bool IsBBAArchiveFile, UInt32 OffsetFileEntries, UInt32 OffsetFileHashtable, UInt32 NumberOfFiles)
		{
			Definition = new BBADirectoryDefinition()
			{
				HeaderSize = 64,
				Version = 4,
				Unknown5 = 5,
				HeaderSize2 = 64,
				HeaderEncryptionID = Crypt.S6_HEAD_CRYPTID,
				DirectoryEncryptionID =  IsBBAArchiveFile ? Crypt.S6_BBA_CRYPTID : Crypt.S6_MAP_CRYPTID,
				Unknown1 = 0xD1C81BB5,
				OffsetFileEntries = OffsetFileEntries,
				OffsetFileHashtable = OffsetFileHashtable,
				NumberOfFiles = NumberOfFiles
			};

			unsafe
			{
				fixed (byte* Pointer = Definition.ArchiveHeader)
				{
        			"BAF"u8.CopyTo(new Span<byte>(Pointer, 3));
				}

				fixed (uint* Pointer = Definition.EncryptionIDs)
				{
					new Span<UInt32>(Pointer, 8)[1] = Crypt.S6_FILE_CRYPTID;
				}

				Helpers.FillBufferWithZeroes(ref Definition.Null1[0], 52);
				Helpers.FillBufferWithZeroes(ref Definition.Null2[0], 16);
				Helpers.FillBufferWithZeroes(ref Definition.Null3[0], 40);
				Helpers.FillBufferWithZeroes(ref Definition.Null4[0], 96);
			}
		}
	}
}
