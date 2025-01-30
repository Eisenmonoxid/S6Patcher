using System;
using System.Collections.Generic;
using System.Text;

namespace S6Patcher.Source.Patcher.Mappings
{
    internal class OV : MappingBase
    {
        public OV() {}
        public override List<PatchEntry> GetMapping()
        {
            return new List<PatchEntry>
            {
                new PatchEntry
                {
                    Name = "High - Resolution Textures:",
                    Mapping = new Dictionary<long, byte[]>()
                    {
                        {0x288ADF, new byte[] {0xE9, 0x1D, 0xD2, 0x26, 0x00, 0x90, 0x90, 0x90}}, // High entity resolution
                        {0x4F5D01, new byte[] {0xE8, 0x87, 0x3D, 0xD9, 0xFF, 0xC7, 0x46, 0x40, 0x00, 0x70, 0x9B, 0x3C, 0xE9, 0xD5, 0x2D, 0xD9, 0xFF, 0x90}}, // High entity resolution
                    }
                },
                new PatchEntry
                {
                    Name = "Activate Development-Mode Permanently",
                    Mapping = new Dictionary<long, byte[]>()
                    {
                        {0x732FA, new byte[] {0xC6, 0x05, 0x28, 0xBF, 0xAA, 0x00, 0x01, 0xEB, 0x7C, 0x90}}, // Set global DevMachine to 1
                        {0xBE11, new byte[] {0x66, 0x90}}, // Enable Development-Mode without command line argument -DevM
                    }
                },
                new PatchEntry
                {
                    Name = "Activate Script and Code Bugfixes",
                    Mapping = new Dictionary<long, byte[]>()
                    {
                        {0x1B5E36, new byte[] {0x08}}, // Crash fix when dismissing entertainer, push EntityID on stack
                        {0x4FD5A9, Encoding.ASCII.GetBytes("EMXBinData.s6patcher")}, // Make game load .s6patcher binary file in MAINMENU lua state
                        {0x4FD5BD, new byte[] {0x00, 0x00, 0x00}}, // Make game load .s6patcher binary file in MAINMENU lua state
                    }
                },
                new PatchEntry
                {
                    Name = "Activate Limited/Special Edition",
                    Mapping = new Dictionary<long, byte[]>()
                    {
                        {0x23D64D, new byte[] {0x90, 0x90}}, // Override JZ, always set Special Edition to 1
                        {0x23D655, new byte[] {0x90, 0x90}}, // Override JNZ, always set Special Edition to 1
                    }
                }
            };
        }

        public override UInt32[] GetTextureResolutionMapping()
        {
            return new UInt32[] {0x2BE177, 0x2BE17E, 0x2BE185};
        }
        public override Dictionary<long, byte[]> GetZoomLevelMapping(double ZoomLevel, float ClutterFarDistance)
        {
            float Offset = 4800;
            float TransitionFactor = 3700;

            return new Dictionary<long, byte[]>()
            {
                {0x545400, BitConverter.GetBytes(ZoomLevel)},
                {0x2B334E, new byte[] {0xC7, 0x45, 0x64}},
                {0x2B3351, BitConverter.GetBytes(ClutterFarDistance + Offset)},
                {0x2B3355, new byte[] {0xC7, 0x45, 0x6C}},
                {0x2B3358, BitConverter.GetBytes(TransitionFactor)},
                {0x2B335C, new byte[] {0x90, 0x90}},
                {0x27AC99, new byte[] {0x50, 0xB8, 0x00, 0x40, 0x9C, 0xC5, 0x89, 0x81, 0x9C, 0x00, 0x00, 0x00, 0x58, 0xC6, 0x81, 0x98, 0x00, 0x00, 0x00, 0x01, 0xC2, 0x08, 0x00}},
            };
        }
        public override UInt32[] GetAutoSaveMapping()
        {
            throw new NotImplementedException();
        }
    }
}
