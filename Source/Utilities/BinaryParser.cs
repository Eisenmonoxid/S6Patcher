using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace S6Patcher.Source.Utilities
{
    public class BinaryParser
    {
        private readonly byte[] Magic = Encoding.UTF8.GetBytes("EMX");
        private readonly BinaryReader GlobalReader = null;
        private readonly uint BlockOffset = 0x0;

        public void Dispose() => GlobalReader?.Dispose();

        public BinaryParser(string Definition)
        {
            Stream BinaryStream = Utility.GetEmbeddedResourceDefinition(Definition);
            if (BinaryStream == null || BinaryStream.CanRead == false)
            {
                throw new Exception("[ERROR] Invalid binary stream.");
            }

            BlockOffset = (uint)(Magic.Length + sizeof(byte));
            GlobalReader = new BinaryReader(GetDecompressedStream(BinaryStream));
            BinaryStream.Dispose();

            if (!IsValidBinaryFile())
            {
                Dispose();
                throw new Exception("[ERROR] Invalid binary file.");
            }
        }

        private MemoryStream GetDecompressedStream(Stream BinaryStream)
        {
            BinaryStream.Seek(0, SeekOrigin.Begin);

            using GZipStream DecompressionStream = new(BinaryStream, CompressionMode.Decompress);
            MemoryStream DecompressedStream = new();

            DecompressionStream.CopyTo(DecompressedStream);
            DecompressedStream.Seek(0, SeekOrigin.Begin);

            return DecompressedStream;
        }

        private bool IsValidBinaryFile()
        {
            byte[] Result = new byte[Magic.Length];
            GlobalReader.BaseStream.Seek(0, SeekOrigin.Begin);
            GlobalReader.Read(Result, 0, Result.Length);
            return Result.SequenceEqual(Magic);
        }

        public byte GetFileVersion()
        {
            GlobalReader.BaseStream.Position = Magic.Length;
            return GlobalReader.ReadByte();
        }

        public Dictionary<uint, byte[]> ParseBinaryWrapper(execID ID, string Identifier)
        {
            if (!ParseBinaryFileContent((byte)ID, out Dictionary<uint, byte[]> Mapping, Identifier))
            {
                throw new Exception("[ERROR] Could not parse binary file content.");
            }

            return Mapping;
        }

        public bool ParseBinaryFileContent(byte Identifier, out Dictionary<UInt32, byte[]> Result, string ID = "")
        {
            Result = [];
            byte[] IDBytes = (ID == "") ? [0x0, 0x0, 0x0] : Encoding.UTF8.GetBytes(ID);

            GlobalReader.BaseStream.Position = BlockOffset;
            byte BlockID = GlobalReader.ReadByte();
            while (BlockID != Identifier)
            {
                UInt32 Size = GlobalReader.ReadUInt32();
                if ((GlobalReader.BaseStream.Position + Size) > GlobalReader.BaseStream.Length)
                {
                    return false;
                }

                GlobalReader.BaseStream.Position += Size;
                BlockID = GlobalReader.ReadByte();
            }

            // Read blocks
            UInt32 BlockSize = GlobalReader.ReadUInt32();
            long Position = GlobalReader.BaseStream.Position;

            UInt32 Address;
            byte[] Data;
            byte[] EntryID;
            while ((Position + BlockSize) > GlobalReader.BaseStream.Position)
            {
                EntryID = GlobalReader.ReadBytes(IDBytes.Length);
                if (EntryID.SequenceEqual(IDBytes))
                {
                    Address = GlobalReader.ReadUInt32();
                    Data = GlobalReader.ReadBytes(GlobalReader.ReadUInt16());
                    Result.Add(Address, Data);
                }
                else
                {
                    GlobalReader.BaseStream.Position += sizeof(UInt32);
                    UInt16 Length = GlobalReader.ReadUInt16();
                    GlobalReader.BaseStream.Position += Length;
                }
            }

            return true;
        }
    }
}
