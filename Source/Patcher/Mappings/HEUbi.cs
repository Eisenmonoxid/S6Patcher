using S6Patcher.Source.Helpers;
using System;
using System.Collections.Generic;

namespace S6Patcher.Source.Patcher.Mappings
{
    internal class HEUbi(execID ID, BinaryParser Parser) : MappingBase(ID, Parser)
    {
        public override Dictionary<UInt32, byte[]> GetTextureResolutionMapping(uint Resolution) => 
            base.GetTextureResolutionMapping(Resolution, [0x2D4188, 0x2D418F, 0x2D4196]);
        public override Dictionary<UInt32, byte[]> GetAutoSaveMapping(double Time) =>
            base.GetAutoSaveMapping(Time, [0x1C5F2A, 0xEB83C0]);

        public override Dictionary<UInt32, byte[]> GetZoomLevelMapping(double ZoomLevel, float ClutterFarDistance)
        {
            Dictionary<UInt32, byte[]> Mapping = _Parser.ParseBinaryWrapper(_ID, "ZLM");
            Mapping[0xC4EC4C] = BitConverter.GetBytes(ClutterFarDistance);
            Mapping[0x270314] = BitConverter.GetBytes(ClutterFarDistance + 4800f);

            return Mapping;
        }
    }
}
