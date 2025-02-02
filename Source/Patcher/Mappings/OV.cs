using System;
using System.Collections.Generic;
using System.Text;

namespace S6Patcher.Source.Patcher.Mappings
{
    internal class OV : MappingBase
    {
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

        public override Dictionary<long, byte[]> GetModloaderMapping()
        {
            byte[] CurrentPath = Encoding.ASCII.GetBytes("modloader\\bba\\mod.bba\0\0");
            return new Dictionary<long, byte[]>()
            {
                // .data segment
                {0x53F5BC, CurrentPath}, // Add mod.bba file path
                // .text segment -> Check what loading method to use
                {0x23E916, new byte[] {0xEB}}, // Always jump to ModelViewer
                {0x23E931, new byte[] {0xE8, 0x00, 0xF9, 0xFF, 0xFF}}, // Always jump to ModelViewer
                {0x23E936, new byte[] {0xEB, 0xE0, 0x90, 0x90}}, // Return to original loader after mod
                // .text segment -> Override the ModelViewer func
                {0x23E236, new byte[] {0x55, 0x89, 0xE5, 0x83, 0xEC, 0x28, 0x8D, 0x4D, 0xDC, 0x68, 0x00, 0x01, 0x00, 0x00}}, // Return to original loader after mod
                {0x23E244, new byte[] {0xE8, 0x35, 0xE6, 0xE2, 0xFF, 0x8D, 0x4D, 0xDC, 0x68, 0xBC, 0xF5, 0x93, 0x00}}, // Return to original loader after mod
                {0x23E251, new byte[] {0xE8, 0xC2, 0xE7, 0xE2, 0xFF, 0xEB, 0x1F, 0x90, 0x90, 0x6A, 0x00, 0x8D, 0x7C, 0x25, 0xDC}}, // Return to original loader after mod
                {0x23E260, new byte[] {0x31, 0xC0, 0xE8, 0x06, 0xF0, 0xFF, 0xFF, 0xEB, 0x19, 0x90, 0x90, 0xE8, 0xF0, 0xE3, 0xE2, 0xFF}}, // Return to original loader after mod
                {0x23E270, new byte[] {0x83, 0xC4, 0x2A, 0x89, 0xEC, 0x5D, 0xC3, 0x8B, 0x45, 0xDC, 0x89, 0x45, 0xD8, 0x89, 0x4D, 0xDC}}, // Return to original loader after mod
                {0x23E280, new byte[] {0xEB, 0xD8, 0x8B, 0x45, 0xD8, 0x89, 0x45, 0xDC, 0x8D, 0x4D, 0xDC, 0xEB, 0xDE, 0x90, 0x90, 0x90}}, // Return to original loader after mod
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
