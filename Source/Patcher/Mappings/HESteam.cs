using S6Patcher.Source.Helpers;
using System;
using System.Collections.Generic;

namespace S6Patcher.Source.Patcher.Mappings
{
    internal class HESteam(execID ID, BinaryParser Parser) : MappingBase(ID, Parser)
    {
        public override Dictionary<UInt32, byte[]> GetTextureResolutionMapping(uint Resolution) => 
            base.GetTextureResolutionMapping(Resolution, [0x2D4D74, 0x2D4D7B, 0x2D4D82]);
        public override Dictionary<UInt32, byte[]> GetAutoSaveMapping(double Time) =>
            base.GetAutoSaveMapping(Time, [0x1C6045, 0xEB95C0]);

        public override Dictionary<UInt32, byte[]> GetZoomLevelMapping(double ZoomLevel, float ClutterFarDistance)
        {
            Dictionary<UInt32, byte[]> Mapping = Parser.ParseBinaryWrapper(ID, "ZLM");
            Mapping[0xC4F9EC] = BitConverter.GetBytes(ClutterFarDistance);
            Mapping[0x270E76] = BitConverter.GetBytes(ClutterFarDistance + 4800f);

            return Mapping;
        }
    }
}
