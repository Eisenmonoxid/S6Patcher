using S6Patcher.Source.Helpers;
using S6Patcher.Source.Patcher.Mappings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public static Mapping GetMappingsByID(execID ID)
        {
            switch (ID)
            {
                case execID.OV:
                    return new OV();
                case execID.OV_OFFSET:
                    return new OV();
                case execID.HE_UBISOFT:
                    return new UbiHE();
                case execID.HE_STEAM:
                    return new SteamHE();
                case execID.ED:
                    return new ED();
                default:
                    throw new NotImplementedException("Error: No valid execID passed!");
            }
        }
    }
}
