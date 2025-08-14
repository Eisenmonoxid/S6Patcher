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
        public static MappingBase GetMappingsByID(execID ID)
        {
            switch (ID)
            {
                case execID.OV:
                    return new OV();
                case execID.OV_OFFSET:
                    return new OV();
                case execID.HE_UBISOFT:
                    return new HEUbi();
                case execID.HE_STEAM:
                    return new HESteam();
                case execID.ED:
                    return new ED();
                default:
                    throw new NotImplementedException("Error: No valid execID passed!");
            }
        }
    }
}
