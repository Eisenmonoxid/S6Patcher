using S6Patcher.Source.Helpers;
using System;
using System.Collections.Generic;

namespace S6Patcher.Source.Patcher.Mappings
{
    internal class OV(execID ID, BinaryParser Parser) : MappingBase(ID, Parser)
    {
        public override Dictionary<uint, byte[]> GetAutoSaveMapping(double Time) => throw new NotImplementedException();

        public override Dictionary<UInt32, byte[]> GetTextureResolutionMapping(uint Resolution) =>
            base.GetTextureResolutionMapping(Resolution, [0x2BE177, 0x2BE17E, 0x2BE185]);

        public override Dictionary<UInt32, byte[]> GetZoomLevelMapping(double ZoomLevel, float ClutterFarDistance)
        {
            Dictionary<UInt32, byte[]> Mapping = _Parser.ParseBinaryWrapper(_ID, "ZLM");
            Mapping[0x545400] = BitConverter.GetBytes(ZoomLevel);
            Mapping[0x2B3351] = BitConverter.GetBytes(ClutterFarDistance + 4800f);

            return Mapping;
        }
    }
}
