using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace S6Patcher.Source.Archive
{
	internal class BBAArchiveFile
    {
		private readonly bool IsArchiveFileOrMap;
		private readonly BBAHeader GlobalHeader;
		private readonly BBADirectoryHeader GlobalDirectoryHeader;
		private readonly BBADirectory GlobalDirectory;
		private readonly BBADataEntry[] GlobalFileData;
		private readonly BBAFileHashTable GlobalHashTable;
		private readonly Stream ArchiveStream;
		private readonly BinaryReader GlobalArchiveReader;

		public BBAArchiveFile(Stream Archive, bool ArchiveFileOrMap, bool BuildHashTableMapping = true)
		{
			ArchiveStream = Archive;
			GlobalArchiveReader = new(ArchiveStream);
			ArchiveStream.Seek(0, SeekOrigin.Begin);

			IsArchiveFileOrMap = ArchiveFileOrMap;

			GlobalHeader = new BBAHeader(GlobalArchiveReader.ReadBytes(BBAHeader.Size));

			Span<byte> DirectoryHeader = GlobalArchiveReader.ReadBytes(BBADirectoryHeader.Size);
			Crypt.Decrypt(DirectoryHeader, GlobalHeader.GetDefinition().EncryptionID);
			GlobalDirectoryHeader = new BBADirectoryHeader(DirectoryHeader);

			GlobalArchiveReader.BaseStream.Position = GlobalDirectoryHeader.GetDefinition().Offset;
			Span<byte> FileDictionary = GlobalArchiveReader.ReadBytes((int)GlobalDirectoryHeader.GetDefinition().Length);
			Crypt.Decrypt(FileDictionary, GlobalDirectoryHeader.GetDefinition().EncryptionID);

			ref CompressedHeaderDefinition CompressedHeader = ref new BBACompression(FileDictionary).GetDefinition();
			ReadOnlySpan<byte> DecompressedHeader = BBACompression.Decompress(FileDictionary[BBACompression.Size..].ToArray(), (int)CompressedHeader.DecompressedSize);
			GlobalDirectory = new BBADirectory(DecompressedHeader);

			uint AmountOfFiles = GlobalDirectory.GetDefinition().NumberOfFiles;
			GlobalFileData = new BBADataEntry[AmountOfFiles];
			ReadOnlySpan<byte> DataEntriesSpan = DecompressedHeader[(int)(GlobalDirectory.GetDefinition().OffsetFileEntries + 4)..];
			ParseDataEntriesFromHeader(DataEntriesSpan, AmountOfFiles);

			ReadOnlySpan<byte> HashTableEntriesSpan = DecompressedHeader[(int)GlobalDirectory.GetDefinition().OffsetFileHashtable..];
			uint AmountOfHashTableEntries = MemoryMarshal.Read<uint>(HashTableEntriesSpan[..4]);

			GlobalHashTable = new BBAFileHashTable(AmountOfHashTableEntries);
			GlobalHashTable.ParseHashTableFromHeader(HashTableEntriesSpan[4..]);

			if (BuildHashTableMapping)
			{
				GlobalHashTable.LinkHashTableEntriesToDataEntries(GlobalFileData.AsSpan());
			}

			// Console.WriteLine($"[INFO] Loaded BBArchive with {GlobalDirectory.GetDefinition().NumberOfFiles} files.");
		}

		public BBAArchiveFile(Stream Archive, string FolderPath, bool ArchiveFileOrMap)
		{
			ArchiveStream = Archive;
			ArchiveStream.Seek(0, SeekOrigin.Begin);

			IsArchiveFileOrMap = ArchiveFileOrMap;

			List<string> Entries = Helpers.GetFiles(FolderPath);
			GlobalFileData = new BBADataEntry[Entries.Count + 1];

			uint HashTableSize = BBAFileHashTable.CalculateHashTableSize((uint)Entries.Count);
			GlobalHashTable = new BBAFileHashTable(HashTableSize);

			MemoryStream FileDataStream = PackFolderFilesIntoArchiveFile(Entries, FolderPath);
			WriteBBAArchiveFileMetadata(FileDataStream);
		}

		private void ParseDataEntriesFromHeader(ReadOnlySpan<byte> Data, uint AmountOfFiles)
		{
			int FileEntryOffset = 0;
			for (uint i = 0; i < AmountOfFiles; i++)
			{
				BBADataEntry Element = new(Data[FileEntryOffset..]);
				GlobalFileData[i] = Element;

				int Padding = (int)(4 - (Element.GetDefinition().FileNameLength % 4));
				FileEntryOffset += (int)(BBADataEntry.Size + Element.GetDefinition().FileNameLength + Padding);
			}
		}

		public void UnpackAllDataEntriesFromArchive(string ArchiveOutputDirectoryPath)
		{
			for (uint i = 0; i < GlobalFileData.Length; i++)
			{
				BBADataEntry Element = GlobalFileData[i];
				ref BBADataEntryDefinition Definition = ref Element.GetDefinition();
				string OutputPath = Path.Combine(ArchiveOutputDirectoryPath, Element.FileName).Replace('\\', Path.DirectorySeparatorChar);

				if (Definition.FileType == (uint)BBADataEntry.BBAArchiveFileType.Directory)
				{
					Directory.CreateDirectory(OutputPath);
				}
				else
				{
					FileStream CurrentFile;
					try
					{
						Directory.CreateDirectory(Path.GetDirectoryName(OutputPath));
						CurrentFile = File.OpenWrite(OutputPath);
					}
					catch (Exception ex)
					{
						Console.WriteLine("[ERROR] Could not open file for writing: " + OutputPath + ": " + ex.Message);
						continue;
					}
				    
					using BinaryWriter Writer = new(CurrentFile, Encoding.UTF8, false);
					if (!Directory.Exists(Path.GetDirectoryName(OutputPath)))
					{
						Directory.CreateDirectory(Path.GetDirectoryName(OutputPath));
					}

					if (Definition.CompressedFileSize != 0)
					{
						ReadOnlySpan<byte> Data = ReadDataEntryFromArchive(Element);
						Writer.Write(Data);
					}

					new FileInfo(OutputPath)
					{
						LastWriteTime = new DateTime(1600, 1, 1).AddTicks((long)Definition.TimeStamp)
					};
				}
			}

			Console.WriteLine("\n[INFO] All files extracted to: " + ArchiveOutputDirectoryPath);
		}

		private MemoryStream PackFolderFilesIntoArchiveFile(List<string> Entries, string FolderPath)
        {
            uint Offset = 1;
			uint FileOffset = 0;
			int FHOffset = sizeof(UInt32) * 2; // 8 bytes for compressed file header

			Span<byte> DummyBuffer = new byte[FHOffset];
			DummyBuffer.Clear();

			GlobalFileData[0] = CreateBBADataEntryForDirectory(FolderPath, "."); // Root directory entry

			MemoryStream FileDataStream = new();
			foreach (var Element in Entries)
			{
				BBADataEntry Entry;
                if (File.Exists(Element))
                {
					byte[] Data = File.ReadAllBytes(Element);
					Entry = CreateBBADataEntry(Data, FolderPath, Element);
					ref BBADataEntryDefinition Definition = ref Entry.GetDefinition();

					if (Data.Length == 0)
					{
						GlobalFileData[Offset++] = Entry;
						continue;
					}

					FileDataStream.Seek(FileOffset, SeekOrigin.Begin);
					if (Definition.FileType == (uint)BBADataEntry.BBAArchiveFileType.Compressed)
					{
						byte[] CompressedData = BBACompression.Compress(Data);
						Definition.CompressedFileSize = (uint)(CompressedData.Length + FHOffset);

						FileDataStream.Write(DummyBuffer);
						FileDataStream.Write(CompressedData);
						FileDataStream.Seek(FileOffset, SeekOrigin.Begin);

						// Write File Header
						byte[] FileHeader = WriteFileHeader(ref Definition, CompressedData.AsSpan()[..FHOffset]);
						FileDataStream.Write(FileHeader);

						Definition.CompressedCRC32 = Helpers.GetCombinedCRC32OfData(FileHeader, CompressedData);
					}
					else
					{
						Definition.CompressedFileSize = Definition.DecompressedFileSize;
						Definition.CompressedCRC32 = Definition.DecompressedCRC32;
						FileDataStream.Write(Data);
					}

					Definition.FileOffset = FileOffset;
					FileOffset += Definition.CompressedFileSize;
					Definition.NextSibling = (int)FileOffset;
				}
				else if (Directory.Exists(Element))
				{
					Entry = CreateBBADataEntryForDirectory(FolderPath, Element);
				}
				else				
				{
					Console.WriteLine("[ERROR] File or directory " + Element + " does not exist! Skipping ...");
					return FileDataStream;
				}

				GlobalFileData[Offset++] = Entry;
			}

			return FileDataStream;
        }

		private byte[] WriteFileHeader(ref BBADataEntryDefinition Definition, ReadOnlySpan<byte> Data)
		{
			int USize = sizeof(UInt32);
			BBAArchiveFileHeaderDefinition FileHeader = new()
			{
				CompressedSize = Definition.CompressedFileSize - 8,
				DecompressedSize = Definition.DecompressedFileSize
			};

			Span<uint> Entry = MemoryMarshal.CreateSpan(ref Unsafe.As<BBADataEntryDefinition, uint>(ref Definition), USize); 
			Span<BBAArchiveFileHeaderDefinition> FileHeaderDefinition = MemoryMarshal.CreateSpan(ref FileHeader, 1);
            Span<UInt32> Header = MemoryMarshal.Cast<BBAArchiveFileHeaderDefinition, UInt32>(FileHeaderDefinition);

			Header[2] = BinaryPrimitives.ReadUInt32LittleEndian(Data[0..USize]);
			Header[3] = BinaryPrimitives.ReadUInt32LittleEndian(Data[USize..(USize * 2)]);

			Crypt.EncryptFileHeader(Header, Entry);
			return MemoryMarshal.AsBytes(Header).ToArray();
		}

		private BBADataEntry CreateBBADataEntry(ReadOnlySpan<byte> Data, string FolderPath, string ElementPath)
		{
			UInt32 Value = Helpers.GetCRC32OfData(Data);
			string SanitizedPath = Helpers.SanitizePath(FolderPath, ElementPath);

			BBADataEntry DataEntry = new(SanitizedPath);
			ref BBADataEntryDefinition Definition = ref DataEntry.GetDefinition();

			Definition.TimeStamp = Helpers.GetLastWriteTimeFromFileOrDirectory(ElementPath);
			Definition.DecompressedFileSize = (uint)Data.Length;
			Definition.DecompressedCRC32 = Value;
			Definition.CompressedFileSize = Definition.DecompressedFileSize;
			Definition.CompressedCRC32 = Definition.DecompressedCRC32;
			Definition.FileType = BBADataEntry.IsCompressed(Path.GetExtension(ElementPath)) 
				? (uint)BBADataEntry.BBAArchiveFileType.Compressed : (uint)BBADataEntry.BBAArchiveFileType.None;
			Definition.FileNameLength = (uint)SanitizedPath.Length;
			Definition.FileNameOffset = (uint)SanitizedPath.Count(C => C == '\\');

			return DataEntry;
        }

		private BBADataEntry CreateBBADataEntryForDirectory(string FolderPath, string ElementPath)
		{
			string SanitizedPath = Helpers.SanitizePath(FolderPath, ElementPath);

			BBADataEntry DataEntry = new(SanitizedPath);
			ref BBADataEntryDefinition Definition = ref DataEntry.GetDefinition();

			Definition.TimeStamp = Helpers.GetLastWriteTimeFromFileOrDirectory(ElementPath);
			Definition.DecompressedFileSize = 0;
			Definition.DecompressedCRC32 = 0;
			Definition.CompressedFileSize = 0;
			Definition.CompressedCRC32 = 0;
			Definition.FileType = (uint)BBADataEntry.BBAArchiveFileType.Directory;
			Definition.FileNameLength = (uint)SanitizedPath.Length;
			Definition.FileNameOffset = (uint)SanitizedPath.Count(C => C == '\\');

			return DataEntry;
        }

		private ReadOnlySpan<byte> ReadDataEntryFromArchive(BBADataEntry DataEntry)
		{
			ref BBADataEntryDefinition Definition = ref DataEntry.GetDefinition();

			GlobalArchiveReader.BaseStream.Position = Definition.FileOffset;
			if (Definition.FileType == (uint)BBADataEntry.BBAArchiveFileType.Compressed)
			{
				return DecompressDataEntryFromArchive(ref Definition);
			}
			else
			{
				return GlobalArchiveReader.ReadBytes((int)Definition.CompressedFileSize);
			}
		}

		private ReadOnlySpan<byte> DecompressDataEntryFromArchive(ref BBADataEntryDefinition Definition)
		{
			int USize = sizeof(UInt32);
			int FileHeaderSize = Helpers.GetSerializedSize<BBAArchiveFileHeaderDefinition>();
			byte[] CurrentFileHeader = new byte[FileHeaderSize];

			GlobalArchiveReader.Read(CurrentFileHeader, 0, FileHeaderSize);

			Span<uint> Data = MemoryMarshal.Cast<byte, uint>(CurrentFileHeader.AsSpan());
			Span<uint> Entry = MemoryMarshal.CreateSpan(ref Unsafe.As<BBADataEntryDefinition, uint>(ref Definition), USize);

			Crypt.DecryptFileHeader(Data, Entry);

			GlobalArchiveReader.BaseStream.Position = Definition.FileOffset + (USize * 2); // Skip first two uints of file header

			byte[] Source = GlobalArchiveReader.ReadBytes((int)Definition.CompressedFileSize);

			BinaryPrimitives.WriteUInt32LittleEndian(Source.AsSpan()[..USize], Data[2]);
			BinaryPrimitives.WriteUInt32LittleEndian(Source.AsSpan()[USize..(USize * 2)], Data[3]);

			return BBACompression.Decompress(Source, (int)Definition.DecompressedFileSize);
		}

		private byte[] GetSerializedFileData()
		{
			using MemoryStream Memory = new();
			using BinaryWriter Writer = new(Memory);

			uint FileEntryOffset = (uint)(BBAHeader.Size + BBADirectoryHeader.Size);
			for (int i = 0; i < GlobalFileData.Length; i++)
			{
				BBADataEntry DataEntry = GlobalFileData[i];
				ref BBADataEntryDefinition Definition = ref DataEntry.GetDefinition();
				Definition.FileOffset += FileEntryOffset;
				Writer.Write(DataEntry.Serialize());
			}

			return Memory.ToArray();
		}

		private byte[] GetDecompressedHeaderData(bool IsBBAFile, Span<byte> SerializedFileData, Span<byte> SerializedHashTableData)
		{
			using MemoryStream Memory = new();
			using BinaryWriter Writer = new(Memory);

			Writer.BaseStream.Seek(BBADirectory.Size, SeekOrigin.Begin);

			Writer.Write(SerializedFileData.Length);
			Writer.Write(SerializedFileData);

			uint HashTableOffset = (uint)Writer.BaseStream.Position;
			Writer.Write(SerializedHashTableData.Length / Helpers.GetSerializedSize<BBAArchiveHashTableEntryDefinition>());
			Writer.Write(SerializedHashTableData);

			BBADirectory Directory = new(IsBBAFile, (uint)BBADirectory.Size, HashTableOffset, (uint)GlobalFileData.Length);
			Writer.BaseStream.Seek(0, SeekOrigin.Begin);
			Writer.Write(Directory.Serialize());

			return Memory.ToArray();
		}

		private byte[] GetEncryptedFileDictionary(bool IsBBAFile, byte[] CompressedHeaderBytes, Span<byte> CompressedHeaderData)
		{
			byte[] FileDictionary = [..CompressedHeaderBytes, ..CompressedHeaderData];
			int Length = FileDictionary.Length;
			int Padding = Length % 4;
			if (Padding != 0)
			{
				Padding = 4 - Padding;
			}

			Array.Resize(ref FileDictionary, Length + Padding);
			Crypt.Encrypt(FileDictionary, IsBBAFile ? Crypt.S6_BBA_CRYPTID : Crypt.S6_MAP_CRYPTID);

			return FileDictionary;
		}

		private void WriteBBAArchiveFileMetadata(MemoryStream FileDataStream)
		{
			Span<byte> SerializedHashTableData = GlobalHashTable.SerializeHashTable(GlobalFileData);
			Span<byte> SerializedFileData = GetSerializedFileData();
			Span<byte> DecompressedHeaderData = GetDecompressedHeaderData(IsArchiveFileOrMap, SerializedFileData, SerializedHashTableData);
			Span<byte> CompressedHeaderData = BBACompression.Compress(DecompressedHeaderData);

			CompressedHeaderDefinition CompressedHeader = new()
			{
				DecompressedSize = (uint)DecompressedHeaderData.Length,
				CompressedSize = (uint)CompressedHeaderData.Length,
			};

			byte[] CompressedHeaderBytes = new byte[BBACompression.Size];
			MemoryMarshal.Write(CompressedHeaderBytes, in CompressedHeader);
			Span<byte> FileDictionaryData = GetEncryptedFileDictionary(IsArchiveFileOrMap, CompressedHeaderBytes, CompressedHeaderData);
			uint CRC32 = Helpers.GetCRC32OfData(FileDictionaryData);

			UInt32 Offset = (uint)(BBAHeader.Size + BBADirectoryHeader.Size + FileDataStream.Length);
			BBADirectoryHeader DirectoryHeader = new(IsArchiveFileOrMap, Offset, (uint)FileDictionaryData.Length, CRC32);

			BBAHeader Header = new();

			Span<byte> SerializedDirectoryHeader = DirectoryHeader.Serialize();
			Crypt.Encrypt(SerializedDirectoryHeader, Header.GetDefinition().EncryptionID);

			ArchiveStream.Seek(0, SeekOrigin.Begin);
			ArchiveStream.Write(Header.Serialize());
			ArchiveStream.Write(SerializedDirectoryHeader);

			FileDataStream.Seek(0, SeekOrigin.Begin);
			FileDataStream.CopyTo(ArchiveStream);
			FileDataStream.Dispose();

			ArchiveStream.Write(FileDictionaryData);
			ArchiveStream.Flush();
		}

		private BBADataEntry GetDataEntryByName(string FileName)
		{
			foreach (var Element in GlobalFileData)
			{
				if (Element.FileName == FileName)
				{
					return Element;
				}
			}

			return default;
		}

		public byte[] GetFileDataByDataEntryName(string FileName)
		{
			BBADataEntry DataEntry = GetDataEntryByName(FileName);
			ReadOnlySpan<byte> Data = ReadDataEntryFromArchive(DataEntry);
			return Data.ToArray();
		}
	}
}
