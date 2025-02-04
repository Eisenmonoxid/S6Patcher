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
        public abstract Dictionary<long, byte[]> GetModloaderMapping();
        public abstract Dictionary<long, byte[]> GetZoomLevelMapping(double ZoomLevel, float ClutterFarDistance);
        public abstract UInt32[] GetAutoSaveMapping();
        public abstract UInt32[] GetTextureResolutionMapping();
        public abstract Dictionary<long, byte[]> GetOverrideUserScriptMapping();
        public static MappingBase GetMappingsByID(execID ID)
        {
            switch (ID)
            {
                case execID.OV:
                    return new OV();
                case execID.OV_OFFSET:
                    return new OV();
                case execID.HE_UBISOFT:
                    return new HE_Ubi();
                case execID.HE_STEAM:
                    return new HE_Steam();
                case execID.ED:
                    return new ED();
                default:
                    throw new NotImplementedException("Error: No valid execID passed!");
            }
        }
    }
}
