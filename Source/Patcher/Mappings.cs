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
            UpdateMappingValueAtIndex(ref Mapping, 0x00, Time == 0.0 ? [0xEB] : [0x76]);
            UpdateMappingValueAtIndex(ref Mapping, 0x01, BitConverter.GetBytes(Time * 60000));

            return Mapping;
        }

        public Dictionary<uint, byte[]> GetZoomLevelMapping(double ZoomLevel, float ClutterFarDistance)
        {
            byte[] Level = _ID == execID.OV ? BitConverter.GetBytes(ZoomLevel) : BitConverter.GetBytes(ClutterFarDistance);
            Dictionary<uint, byte[]> Mapping = _Parser.ParseBinaryWrapper(_ID, "ZLM");
            UpdateMappingValueAtIndex(ref Mapping, 0x00, Level);
            UpdateMappingValueAtIndex(ref Mapping, 0x01, BitConverter.GetBytes(ClutterFarDistance + 4800f));

            return Mapping;
        }

        public Dictionary<uint, byte[]> GetDocumentsFolderMapping(string FolderPath)
        {
            Dictionary<uint, byte[]> Mapping = _Parser.ParseBinaryWrapper(_ID, "DCP");
            byte[] UnicodePath = Encoding.Unicode.GetBytes(FolderPath + "\0");
            UpdateMappingValueAtIndex(ref Mapping, 0xCC, UnicodePath);

            return Mapping;
        }

        public Dictionary<uint, byte[]> GetScriptFileMapping() => _Parser.ParseBinaryWrapper(_ID, "SFM");
        public Dictionary<uint, byte[]> GetEasyDebugMapping() => _Parser.ParseBinaryWrapper(_ID, "EDG");
        public Dictionary<uint, byte[]> GetModloaderMapping() => _Parser.ParseBinaryWrapper(_ID, "MDL");

        private void UpdateMappingValueAtIndex(ref Dictionary<uint, byte[]> Mapping, byte Value, byte[] Replacement)
        {
            uint Key = Mapping.FirstOrDefault(Element => Element.Value[0] == Value).Key;
            Mapping[Key] = Replacement;
        }
    }
}
