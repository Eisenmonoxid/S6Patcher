using S6Patcher.Source.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using S6Packer.Source;
using System.Xml;
using System.Buffers.Binary;

namespace S6Patcher.Source.Patcher
{
    public static class GameplayModification
    {
        public static readonly List<FileDataEntry> ModifiableFileData = [];
        public static byte[] UpdateFileContent(FileDataEntry Entry, byte[] FileContent)
        {
            using MemoryStream Stream = new(FileContent, writable: false);
            XmlDocument Document = LoadXmlDocument(Stream);
            return UpdateTags(Document, Entry.Data);
        }

        private static byte[] UpdateTags(XmlDocument Document, Dictionary<UInt32, byte[]> Data)
        {
            Dictionary<string, int> TagOccurrences = [];
            foreach (var Entry in Data)
            {
                byte[] Value = Entry.Value;
                ushort TagLength = BinaryPrimitives.ReadUInt16LittleEndian(Value);
                string Tag = Encoding.UTF8.GetString(Value, sizeof(ushort), TagLength);
                string NewValue = Encoding.UTF8.GetString(Value, sizeof(ushort) + TagLength, Value.Length - sizeof(ushort) - TagLength);

                XmlNodeList Nodes = Document.GetElementsByTagName(Tag);
                TagOccurrences.TryGetValue(Tag, out int Occurrence);
                TagOccurrences[Tag] = Occurrence + 1;

                if (Occurrence < Nodes.Count)
                {
                    Nodes[Occurrence].InnerText = NewValue;
                }
            }

            return SerializeXmlDocument(Document);
        }

        private static XmlDocument LoadXmlDocument(Stream Stream)
        {
            XmlReaderSettings Settings = new()
            {
                DtdProcessing = DtdProcessing.Prohibit,
                XmlResolver = null
            };

            XmlDocument Document = new()
            {
                PreserveWhitespace = true
            };

            using XmlReader Reader = XmlReader.Create(Stream, Settings);
            Document.Load(Reader);
            return Document;
        }

        private static byte[] SerializeXmlDocument(XmlDocument Document)
        {
            using MemoryStream Stream = new();
            XmlWriterSettings Settings = new()
            {
                Encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false),
                Indent = false
            };

            using (XmlWriter Writer = XmlWriter.Create(Stream, Settings))
            {
                Document.Save(Writer);
            }

            return Stream.ToArray();
        }

        private static FileDataEntry GetModifiableFileData(string Archive, string Path)
        {
            foreach (FileDataEntry Entry in ModifiableFileData)
            {
                if (Entry.BBArchiveName == Archive && Entry.FilePath == Path)
                {
                    return Entry;
                }
            }

            return null;
        }

        private static void AddModifiableFileData(string Archive, string Path, string XMLTag, string Value)
        {
            FileDataEntry Entry = GetModifiableFileData(Archive, Path);
            if (Entry == null)
            {
                Entry = new()
                {
                    IsDataFile = false,
                    BBArchiveName = Archive,
                    FilePath = Path,

                    OriginalFileCRC = 0x0, // Ignored when !IsDataFile
                    EntryCount = 0, // Ignored when !IsDataFile
                    Data = []
                };

                ModifiableFileData.Add(Entry);
            }

            byte[] TagBytes = Encoding.UTF8.GetBytes(XMLTag);
            byte[] ValueBytes = Encoding.UTF8.GetBytes(Value);
            ushort TagLength = (ushort)TagBytes.Length;
            byte[] Data = new byte[sizeof(ushort) + TagBytes.Length + ValueBytes.Length];

            BinaryPrimitives.WriteUInt16LittleEndian(Data, TagLength);
            Buffer.BlockCopy(TagBytes, 0, Data, sizeof(ushort), TagBytes.Length);
            Buffer.BlockCopy(ValueBytes, 0, Data, sizeof(ushort) + TagBytes.Length, ValueBytes.Length);

            Entry.Data.Add((uint)(Entry.Data.Count + 1), Data);
        }

        public static void ModifyPlayerColor(Avalonia.Media.Color Color)
        {
            // First file entry is always zero
            AddModifiableFileData("shrgcfg0.bba", "config\\playercolor.xml", "Red", "0");
            AddModifiableFileData("shrgcfg0.bba", "config\\playercolor.xml", "Green", "0");
            AddModifiableFileData("shrgcfg0.bba", "config\\playercolor.xml", "Blue", "0");

            // Custom player color
            AddModifiableFileData("shrgcfg0.bba", "config\\playercolor.xml", "Red", Color.R.ToString());
            AddModifiableFileData("shrgcfg0.bba", "config\\playercolor.xml", "Green", Color.G.ToString());
            AddModifiableFileData("shrgcfg0.bba", "config\\playercolor.xml", "Blue", Color.B.ToString());
        }

        public static void ModifySettlerLimits(uint[] Values)
        {
            AddModifiableFileData("shrgcfg0.bba", "config\\logic.xml", "SettlerLimit", "50"); // No cathedral
            foreach (uint Value in Values)
            {
                AddModifiableFileData("shrgcfg0.bba", "config\\logic.xml", "SettlerLimit", Value.ToString());
            }
        }

        public static void ModifySoldierLimits(uint[] Values)
        {
            AddModifiableFileData("shrgcfg0.bba", "config\\logic.xml", "SettlerLimit", "50"); // No cathedral
            foreach (uint Value in Values)
            {
                AddModifiableFileData("shrgcfg0.bba", "config\\logic.xml", "SettlerLimit", Value.ToString());
            }
        }
    }
}
