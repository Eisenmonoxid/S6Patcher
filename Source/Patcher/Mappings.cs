using S6Patcher.Source.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace S6Patcher.Source.Patcher
{
    public class Mappings(execID _ID, BinaryParser _Parser)
    {
        public Dictionary<uint, byte[]> GetMapping(List<string> Features)
        {
            Dictionary<uint, byte[]> Mapping = [];
            foreach (string Feature in Features)
            {
                Mapping = Mapping.Union(_Parser.ParseBinaryWrapper(_ID, Feature)).ToDictionary(K => K.Key, V => V.Value);
            }

            return Mapping;
        }

        public Dictionary<uint, byte[]> GetTextureResolutionMapping(uint Resolution)
        {
            Dictionary<uint, byte[]> Mapping = _Parser.ParseBinaryWrapper(_ID, "GTS");

            uint Index = 0;
            foreach (var Element in Mapping)
            {
                uint Key = Mapping.FirstOrDefault(Element => Element.Value[0] == (byte)Index).Key;
                Mapping[Key] = BitConverter.GetBytes(Resolution / Convert.ToUInt32(Math.Pow(2, Index++)));
            }

            return Mapping;
        }

        public Dictionary<uint, byte[]> GetAutoSaveMapping(double Time)
        {
            Dictionary<uint, byte[]> Mapping = _Parser.ParseBinaryWrapper(_ID, "ATS");

            uint Key = Mapping.FirstOrDefault(Element => Element.Value[0] == 0x0).Key;
            Mapping[Key] = Time == 0.0 ? [0xEB] : [0x76];

            Key = Mapping.FirstOrDefault(Element => Element.Value[0] == 0x1).Key;
            Mapping[Key] = BitConverter.GetBytes(Time * 60000);

            return Mapping;
        }

        public Dictionary<uint, byte[]> GetZoomLevelMapping(double ZoomLevel, float ClutterFarDistance)
        {
            byte[] Level = _ID == execID.OV ? BitConverter.GetBytes(ZoomLevel) : BitConverter.GetBytes(ClutterFarDistance);
            Dictionary<uint, byte[]> Mapping = _Parser.ParseBinaryWrapper(_ID, "ZLM");

            uint Key = Mapping.FirstOrDefault(Element => Element.Value[0] == 0x0).Key;
            Mapping[Key] = Level;

            Key = Mapping.FirstOrDefault(Element => Element.Value[0] == 0x1).Key;
            Mapping[Key] = BitConverter.GetBytes(ClutterFarDistance + 4800f);

            return Mapping;
        }

        public Dictionary<uint, byte[]> GetDocumentsFolderMapping(string FolderPath)
        {
            Dictionary<uint, byte[]> Mapping = _Parser.ParseBinaryWrapper(_ID, "DCP");

            byte[] UTF8Path = Encoding.Unicode.GetBytes(FolderPath + "\0");
            Mapping.Add(0x4FF938, UTF8Path); // TODO: Get value from binary file instead of hardcoding it here

            return Mapping;
        }

        public Dictionary<uint, byte[]> GetScriptFileMapping() => _Parser.ParseBinaryWrapper(_ID, "SFM");
        public Dictionary<uint, byte[]> GetEasyDebugMapping() => _Parser.ParseBinaryWrapper(_ID, "EDG");
        public Dictionary<uint, byte[]> GetModloaderMapping() => _Parser.ParseBinaryWrapper(_ID, "MDL");
    }
}
