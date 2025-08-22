using S6Patcher.Source.Helpers;
using System;
using System.Collections.Generic;

namespace S6Patcher.Source.Patcher.Mappings
{
    public abstract class MappingBase
    {
        public struct PatchEntry
        {
            public string Name;
            public Dictionary<long, byte[]> Mapping;
        }
        public abstract List<PatchEntry> GetMapping();
        public abstract Dictionary<long, byte[]> GetEasyDebugMapping();
        public abstract Dictionary<long, byte[]> GetModloaderMapping();
        public abstract Dictionary<long, byte[]> GetZoomLevelMapping(double ZoomLevel, float ClutterFarDistance);
        public abstract Dictionary<long, byte[]> GetAutoSaveMapping(double Time);
        public abstract Dictionary<long, byte[]> GetTextureResolutionMapping(uint Resolution);
        public static MappingBase GetMappingsByID(execID ID) => ID switch
        {
            execID.OV => new OV(),
            execID.OV_OFFSET => new OV(),
            execID.HE_UBISOFT => new HEUbi(),
            execID.HE_STEAM => new HESteam(),
            execID.ED => new ED(),
            _ => throw new NotImplementedException("Error: No valid execID passed!"),
        };
    }
}
