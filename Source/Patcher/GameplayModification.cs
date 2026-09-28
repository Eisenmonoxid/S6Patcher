using S6Patcher.Source.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using S6Packer.Source;
using System.Xml;

namespace S6Patcher.Source.Patcher
{
    public static class GameplayModification
    {
        private sealed record XmlModification(string[] Tags, string Value);

        public static readonly List<FileDataEntry> ModifiableFileData = [];
        private static readonly List<XmlModification> XmlModifications = [];

        public static void ClearModifiableFileData()
        {
            ModifiableFileData.Clear();
            XmlModifications.Clear();
        }

        public static byte[] UpdateFileContent(FileDataEntry Entry, byte[] FileContent)
        {
            using MemoryStream Stream = new(FileContent, writable: false);
            XmlDocument Document = LoadXmlDocument(Stream);
            return UpdateTags(Document, Entry.Data);
        }

        private static byte[] UpdateTags(XmlDocument Document, Dictionary<UInt32, byte[]> Data)
        {
            Dictionary<string, int> TagOccurrences = [];
            foreach (uint Index in Data.Keys.OrderBy(Index => Index))
            {
                XmlModification Modification = XmlModifications[(int)Index];
                string[] Tags = Modification.Tags;
                string TagPath = string.Join('\0', Tags);

                List<XmlNode> Nodes = [];
                foreach (XmlNode Node in Document.GetElementsByTagName(Tags[0]))
                {
                    Nodes.Add(Node);
                }

                for (int TagIndex = 1; TagIndex < Tags.Length; TagIndex++)
                {
                    List<XmlNode> ChildNodes = [];
                    foreach (XmlNode Node in Nodes)
                    {
                        foreach (XmlNode ChildNode in Node.ChildNodes)
                        {
                            if (ChildNode.NodeType == XmlNodeType.Element && ChildNode.Name == Tags[TagIndex])
                            {
                                ChildNodes.Add(ChildNode);
                            }
                        }
                    }

                    Nodes = ChildNodes;
                }

                TagOccurrences.TryGetValue(TagPath, out int Occurrence);
                TagOccurrences[TagPath] = Occurrence + 1;

                if (Occurrence < Nodes.Count)
                {
                    Nodes[Occurrence].InnerText = Modification.Value;
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

        private static void AddModifiableFileData(string Archive, string Path, string[] XMLTags, string Value)
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

            uint Index = (uint)XmlModifications.Count;
            XmlModifications.Add(new(XMLTags, Value));
            Entry.Data.Add(Index, []);
        }

        public static void ModifyPlayerColor(Avalonia.Media.Color Color)
        {
            // First file entry is always zero
            AddModifiableFileData("shrgcfg0.bba", "config\\playercolor.xml", ["Red"], "0");
            AddModifiableFileData("shrgcfg0.bba", "config\\playercolor.xml", ["Green"], "0");
            AddModifiableFileData("shrgcfg0.bba", "config\\playercolor.xml", ["Blue"], "0");

            // Custom player color
            AddModifiableFileData("shrgcfg0.bba", "config\\playercolor.xml", ["Red"], Color.R.ToString());
            AddModifiableFileData("shrgcfg0.bba", "config\\playercolor.xml", ["Green"], Color.G.ToString());
            AddModifiableFileData("shrgcfg0.bba", "config\\playercolor.xml", ["Blue"], Color.B.ToString());
        }

        public static void ModifySettlerLimits(uint[] Values)
        {
            AddModifiableFileData("shrgcfg1.bba", "config\\logic.xml", ["SettlerLimit"], "50"); // No cathedral
            foreach (uint Value in Values)
            {
                AddModifiableFileData("shrgcfg1.bba", "config\\logic.xml", ["SettlerLimit"], Value.ToString());
            }
        }

        public static void ModifySoldierLimits(uint[] Values)
        {
            string BasePath = "config\\entities\\b_castle_";
            foreach (string DefinitionFile in new string[] {"me.xml", "na.xml", "ne.xml", "se.xml"})
            {
                string FullPath = BasePath + DefinitionFile;
                foreach (uint Value in Values)
                {
                    AddModifiableFileData("shrgcfg0.bba", FullPath, ["SoldierLimits", "Limit"], Value.ToString());
                }
            }

            foreach (uint Value in Values)
            {
                AddModifiableFileData("shrgcfge10.bba", BasePath + "as.xml", ["SoldierLimits", "Limit"], Value.ToString());
            }
        }

        public static void ModifyStorehouseLimits(uint[] Values)
        {
            string StorePath = "config\\entities\\b_storehouse.xml";
            foreach (uint Value in Values)
            {
                AddModifiableFileData("shrgcfge10.bba", StorePath, ["OutStockCapacities", "OutStockCapacity"], Value.ToString());
            }

            // The following is necessary for the extra1 storehouse to be loaded in the base game, otherwise the game crashes
            AddModifiableFileData("shrgcfge10.bba", "config\\goodsex.xml", ["Worth"], "0"); 
        }
    }
}
