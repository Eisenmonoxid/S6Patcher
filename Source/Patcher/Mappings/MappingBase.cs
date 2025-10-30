using S6Patcher.Source.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace S6Patcher.Source.Patcher.Mappings
{
    public abstract class MappingBase(execID ID, BinaryParser Parser)
    {
        public Dictionary<UInt32, byte[]> GetMapping(List<string> Features)
        {
            Dictionary<UInt32, byte[]> Mapping = [];
            foreach (string Feature in Features)
            {
                Mapping = Mapping.Union(Parser.ParseBinaryWrapper(ID, Feature)).ToDictionary(K => K.Key, V => V.Value);
            }

            return Mapping;
        }

        protected Dictionary<UInt32, byte[]> GetTextureResolutionMapping(uint Resolution, UInt32[] Offsets)
        {
            Dictionary<UInt32, byte[]> Mapping = [];
            uint i = 0;
            foreach (var Element in Offsets)
            {
                Mapping.Add(Element, BitConverter.GetBytes(Resolution / Convert.ToUInt32(Math.Pow(2, i++))));
            }

            return Mapping;
        }

        protected Dictionary<UInt32, byte[]> GetAutoSaveMapping(double Time, UInt32[] Offsets) => new()
        {
            {Offsets[0], Time == 0.0 ? [0xEB] : [0x76]}, // Switch autosave on or off
            {Offsets[1], BitConverter.GetBytes(Time * 60000)},
        };

        public Dictionary<UInt32, byte[]> GetScriptFileMapping() => Parser.ParseBinaryWrapper(ID, "SFM");
        public Dictionary<UInt32, byte[]> GetEasyDebugMapping() => Parser.ParseBinaryWrapper(ID, "EDG");
        public Dictionary<UInt32, byte[]> GetModloaderMapping() => Parser.ParseBinaryWrapper(ID, "MDL");

        public abstract Dictionary<UInt32, byte[]> GetZoomLevelMapping(double ZoomLevel, float ClutterFarDistance);
        public abstract Dictionary<UInt32, byte[]> GetTextureResolutionMapping(uint Resolution);
        public abstract Dictionary<UInt32, byte[]> GetAutoSaveMapping(double Time);

        public static MappingBase GetMappingsByID(execID ID, BinaryParser Parser) => ID switch
        {
            execID.OV => new OV(ID, Parser),
            execID.HE_UBISOFT => new HEUbi(ID, Parser),
            execID.HE_STEAM => new HESteam(ID, Parser),
            execID.ED => new ED(ID, Parser),
            _ => null,
        };
    }
}
