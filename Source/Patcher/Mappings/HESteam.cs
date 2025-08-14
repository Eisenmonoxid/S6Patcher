using System;
using System.Collections.Generic;
using System.Text;

namespace S6Patcher.Source.Patcher.Mappings
{
    internal class HESteam : MappingBase
    {
        public override List<PatchEntry> GetMapping() => new List<PatchEntry>
        {
            new PatchEntry
            {
                Name = "cbDevMode",
                Mapping = new Dictionary<long, byte[]>()
                {
                    {0x1E0C94, new byte[] {0xEB, 0x19}}, // Enable Development-Mode without command line argument -DevM
                    {0x205250, new byte[] {0xC6, 0x05}}, // Set global DevMachine to 1
                    {0x205256, new byte[] {0x01}}, // Set global DevMachine to 1
                }
            },
            new PatchEntry
            {
                Name = "cbScriptBugFixes",
                Mapping = new Dictionary<long, byte[]>()
                {
                    {0x33D6D7, new byte[] {0x08}}, // Crash fix when dismissing entertainer, push EntityID on stack
                    {0xC3F081, Encoding.ASCII.GetBytes("EMXBinData.s6patcher")}, // Make game load .s6patcher binary file in MAINMENU lua state
                    {0xC3F095, new byte[] {0x00, 0x00, 0x00}}, // Make game load .s6patcher binary file in MAINMENU lua state
                    {0x1C5897, new byte[] {0x90, 0x90}}, // Always load userscript, even when not in dev mode
                }
            },
            new PatchEntry
            {
                Name = "cbLimitedEdition",
                Mapping = new Dictionary<long, byte[]>()
                {
                    {0x25D4D5, new byte[] {0x90, 0x90}}, // Override JZ, always set Special Edition to 1
                    {0x25D4DB, new byte[] {0x90, 0x90, 0xFF, 0xC3}}, // Override JNZ, always set Special Edition to 1
                }
            },
        };

        public override Dictionary<long, byte[]> GetEasyDebugMapping() => new Dictionary<long, byte[]>()
        {
            {0x1E09E1, new byte[] {0x90, 0x90}}, // Override JGE, show no data folder found MessageBox (halt thread)
            {0x1E0A5C, new byte[] {0xEB, 0x03, 0x90, 0x90, 0x90}}, // Jump to original instruction
        };

        public override Dictionary<long, byte[]> GetModloaderMapping() => new Dictionary<long, byte[]>()
        { 
            // .data segment
            {0xC50A14, Encoding.ASCII.GetBytes("S6Patcher: Adding ModLoader Directories...\n\0\0")}, // Print to log
            {0xC50A44, Encoding.ASCII.GetBytes("..\\ModLoader\\base\\shr\0")}, // Add directory path
            {0xC50A5A, Encoding.ASCII.GetBytes("..\\ModLoader\\extra1\\shr\0")}, // Add directory path
            {0xC50A72, Encoding.ASCII.GetBytes("..\\ModLoader\\shr\0\0")}, // Add directory path
            // .text segment -> Check what loading method to use
            {0x25E2D0, new byte[] {0xEB}}, // Always jump to ModelViewer
            {0x25E2D4, new byte[] {0xEB}}, // Always jump to ModelViewer
            {0x25E304, new byte[] {0xE8, 0x41, 0x00, 0x00, 0x00, 0xEB, 0xC7, 0x90}}, // Call ModelViewer method
            // .text segment -> Override the ModelViewer func
            {0x25E376, new byte[] {0x72}}, // Return to original loader after mod
            {0x25E3A6, new byte[] {0x20}}, // Return to original loader after mod
            {0x25E3AB, new byte[] {0x7A, 0x68, 0x5A}}, // Return to original loader after mod
            {0x25E3D1, new byte[] {0xEB, 0x53, 0x90, 0x90, 0x90}}, // Return to original loader after mod
        };

        public override Dictionary<long, byte[]> GetTextureResolutionMapping(uint Resolution)
        {
            Dictionary<long, byte[]> Mapping = new Dictionary<long, byte[]>();
            uint i = 0;
            foreach (var Element in new uint[] {0x2D4D74, 0x2D4D7B, 0x2D4D82})
            {
                Mapping.Add(Element, BitConverter.GetBytes(Resolution / Convert.ToUInt32(Math.Pow(2, i++))));
            }

            return Mapping;
        }

        public override Dictionary<long, byte[]> GetZoomLevelMapping(double ZoomLevel, float ClutterFarDistance) => new Dictionary<long, byte[]>()
        {
            {0xC4F9EC, BitConverter.GetBytes(ClutterFarDistance)}, // This is not an error, the HE uses the zoomlevel as single instead of double like in the OV ...
            {0x270E73, new byte[] {0xC7, 0x45, 0xF0}},
            {0x270E76, BitConverter.GetBytes(ClutterFarDistance + 4800f)},
            {0x270E7A, new byte[] {0xC7, 0x45, 0xF4}},
            {0x270E7D, BitConverter.GetBytes(3700f)},
            {0x270E81, new byte[] {0x90}},
            {0x270E87, new byte[] {0x90, 0x90, 0x90}},
            {0x2540B8, new byte[] {0x50, 0xB8, 0x00, 0x40, 0x9C, 0xC5, 0x89, 0x81, 
                0x9C, 0x00, 0x00, 0x00, 0x58, 0xC6, 0x81, 0x98, 0x00, 0x00, 0x00, 0x01, 0x90, 0x90}},
        };

        public override Dictionary<long, byte[]> GetAutoSaveMapping(double Time) => new Dictionary<long, byte[]>()
        {
            {0x1C6045, (Time == 0.0) ? new byte[] {0xEB} : new byte[] {0x76}}, // Switch autosave on or off
            {0xEB95C0, BitConverter.GetBytes(Time * 60000)},
        }; 
    }
}
