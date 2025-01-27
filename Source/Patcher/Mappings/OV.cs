using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace S6Patcher.Source.Patcher.Mappings
{
    internal class OV : Mapping
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
    }
}
