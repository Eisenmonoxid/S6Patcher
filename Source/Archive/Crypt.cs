using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace S6Patcher.Source.Archive
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct XXTEAKeyDefinition
	{
		public UInt32 EncryptionID;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public UInt32[] Keys;
		public UInt32 Start;
		public UInt32 Delta;
	}

	public static unsafe class Crypt
	{
		public const UInt32 S6_HEAD_CRYPTID = 0x29D58DC5;
		public const UInt32 S6_MAP_CRYPTID = 0x161680E5;
		public const UInt32 S6_BBA_CRYPTID = 0x605E90C5;
		public const UInt32 S6_FILE_CRYPTID = 0x9BB3F065;

		private static readonly UInt32[] S6FileKey =
		[
			0xAB0FDAE6,
			0x8A423249,
			0x8A5EC670,
			0x67F8261D
		];

		private static readonly XXTEAKeyDefinition[] Keys =
		[
			new XXTEAKeyDefinition
			{
				EncryptionID = S6_HEAD_CRYPTID,
				Start = 0xDAA66D2B,
				Delta = 0x61C88647,
				Keys = [0x912201AB, 0x81D90511, 0xFA842E62, 0x21475421]
			},
			new XXTEAKeyDefinition
			{
				EncryptionID = S6_BBA_CRYPTID,
				Start = 0x0915E073,
				Delta = 0x6C6A96CB,
				Keys = [0x3AD22B55, 0xC5263F15, 0x459E4F55, 0xFC5969CC]
			},
			new XXTEAKeyDefinition
			{
				EncryptionID = S6_MAP_CRYPTID,
				Start = 0x5384540F,
				Delta = 0x61C88647,
				Keys = [0x804B7535, 0x953F7021, 0x32A58E66, 0x04DB71E8]
			}
		];

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static ref readonly XXTEAKeyDefinition GetKeyFromID(uint EncryptionID)
		{
			for (int i = 0; i < Keys.Length; i++)
			{
				if (Keys[i].EncryptionID == EncryptionID)
				{
					return ref Keys[i];
				}
			}

			throw new KeyNotFoundException();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static uint FEISTEL(uint s, uint v0, uint v1, uint n, uint[] k)
			=> ((s ^ v0) + (v1 ^ k[((s >> 2) & 3) ^ (n & 3)])) ^ (((v1 << 4) ^ (v0 >> 3)) + ((v1 >> 5) ^ (v0 << 2)));

		[MethodImpl(MethodImplOptions.AggressiveOptimization)]
		public static bool Decrypt(Span<byte> Buffer, UInt32 EncryptionID)
		{
			ref readonly XXTEAKeyDefinition Key = ref GetKeyFromID(EncryptionID);
			fixed (byte* Pointer = Buffer)
			{
				XXTeaDecrypt((uint*)Pointer, (uint)Buffer.Length >> 2, in Key);
			}

			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveOptimization)]
		public static bool Encrypt(Span<byte> Buffer, UInt32 EncryptionID)
		{
			ref readonly XXTEAKeyDefinition Key = ref GetKeyFromID(EncryptionID);
			fixed (byte* Pointer = Buffer)
			{
				XXTeaEncrypt((uint*)Pointer, (uint)Buffer.Length >> 2, in Key);
			}

			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveOptimization)]
		private static void XXTeaDecrypt(uint* Data, uint Blocks, in XXTEAKeyDefinition Key)
		{
			uint Sum = Key.Start;
			uint Last = Data[0];

			while (Sum != 0)
			{
				for (uint Position = Blocks - 1; Position > 0; Position--)
				{
					Last = Data[Position] -= FEISTEL(Sum, Last, Data[Position - 1], Position, Key.Keys);
				}

				Last = Data[0] -= FEISTEL(Sum, Last, Data[Blocks - 1], 0, Key.Keys);
				Sum += Key.Delta;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveOptimization)]
		private static void XXTeaEncrypt(uint* Data, uint Blocks, in XXTEAKeyDefinition Key)
		{
			uint Sum = 0;
			uint Last = Data[Blocks - 1];

			while (Sum != Key.Start)
			{
				Sum -= Key.Delta;
				for (uint Position = 0; Position < Blocks - 1; Position++)
				{
					Last = Data[Position] += FEISTEL(Sum, Data[Position + 1], Last, Position, Key.Keys);
				}

				Last = Data[Blocks - 1] += FEISTEL(Sum, Data[0], Last, Blocks - 1, Key.Keys);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void DecryptFileHeader(Span<uint> Data, Span<uint> Entry)
		{
			for (int i = 3; i >= 0; i--)
			{
				Data[i] ^= Data[(3 + i) & 3] ^ Entry[i] ^ S6FileKey[3 - i];
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void EncryptFileHeader(Span<uint> Data, Span<uint> Entry)
		{
			for (int i = 0; i < 4; i++)
			{
				Data[i] ^= Data[(3 + i) & 3] ^ Entry[i] ^ S6FileKey[3 - i];
			}
		}
	}
}
