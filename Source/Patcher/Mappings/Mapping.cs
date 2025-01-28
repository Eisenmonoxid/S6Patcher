using S6Patcher.Source.Helpers;
using System;
using System.Collections.Generic;

namespace S6Patcher.Source.Patcher.Mappings
{
    public abstract class Mapping
    {
        public struct PatchEntry
        {
            public string Name;
            public Dictionary<long, byte[]> Mapping;
        }
        public abstract List<PatchEntry> GetMapping();
        public abstract Dictionary<long, byte[]> GetZoomLevelMapping(double ZoomLevel, float ClutterFarDistance);
        public abstract UInt32[] GetAutoSaveMapping();
        public abstract UInt32[] GetTextureResolutionMapping();
        public static Mapping GetMappingsByID(execID ID)
        {
            return ID switch
            {
                execID.OV => new OV(),
                execID.OV_OFFSET => new OV(),
                execID.HE_UBISOFT => new HE_Ubi(),
                execID.HE_STEAM => new HE_Steam(),
                execID.ED => new ED(),
                _ => throw new NotImplementedException("Error: No valid execID passed!"),
            };
        }
    }
}
