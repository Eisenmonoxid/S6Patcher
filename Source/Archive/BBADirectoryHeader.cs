using System;
using System.Runtime.InteropServices;

namespace S6Patcher.Source.Archive
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public unsafe struct BBADirectoryHeaderDefinition
	{
		public UInt32 Offset;
		public UInt32 Null;
		public UInt32 Length;
		public UInt32 CRC32;
		public UInt32 EncryptionID;
		public UInt32 Unknown1;
		public fixed byte Unknown2[40];
	}
	
	internal class BBADirectoryHeader
	{
		private BBADirectoryHeaderDefinition Definition;
		public ref BBADirectoryHeaderDefinition GetDefinition() => ref Definition;
		public Span<byte> Serialize() => Helpers.Serialize(Definition);
		public static readonly int Size = Helpers.GetSerializedSize<BBADirectoryHeaderDefinition>();

		public BBADirectoryHeader(ReadOnlySpan<byte> Data)
		{
			Definition = Helpers.Parse<BBADirectoryHeaderDefinition>(Data);
		}

		public BBADirectoryHeader(bool IsBBAArchiveFile, UInt32 Offset, UInt32 Length, UInt32 CRC32)
		{
			Definition = new BBADirectoryHeaderDefinition()
			{
				Offset = Offset,
				Null = 0,
				Length = Length,
				CRC32 = CRC32,
				EncryptionID = IsBBAArchiveFile ? Crypt.S6_BBA_CRYPTID : Crypt.S6_MAP_CRYPTID,
				Unknown1 = 0xD1C81BB5
			};

			unsafe
			{
				Helpers.FillBufferWithZeroes(ref Definition.Unknown2[0], 40);
			}
		}
	}
}
