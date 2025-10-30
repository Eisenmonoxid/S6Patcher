using S6Patcher.Source.Helpers;
using System;
using System.Collections.Generic;

namespace S6Patcher.Source.Patcher.Mappings
{
    internal class ED(execID ID, BinaryParser Parser) : MappingBase(ID, Parser)
    {
        public override Dictionary<UInt32, byte[]> GetTextureResolutionMapping(uint Resolution) => throw new NotImplementedException();
        public override Dictionary<UInt32, byte[]> GetZoomLevelMapping(double ZoomLevel, float ClutterFarDistance) => 
            throw new NotImplementedException();
        public override Dictionary<UInt32, byte[]> GetAutoSaveMapping(double Time) => throw new NotImplementedException();
    }
}