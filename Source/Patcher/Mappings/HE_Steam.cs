using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace S6Patcher.Source.Patcher.Mappings
{
    internal class HE_Steam : Mapping
    {
        public HE_Steam() {}
        public override List<PatchEntry> GetMapping()
        {
            return new List<PatchEntry>
            {
                new PatchEntry
                {
                    Name = "Activate Development-Mode Permanently",
                    Mapping = new Dictionary<long, byte[]>()
                    {
                        {0x1E0C94, new byte[] {0xEB, 0x19}}, // Enable Development-Mode without command line argument -DevM
                        {0x205250, new byte[] {0xC6, 0x05}}, // Set global DevMachine to 1
                        {0x205256, new byte[] {0x01}}, // Set global DevMachine to 1
                    }
                },
                new PatchEntry
                {
                    Name = "Activate Script and Code Bugfixes",
                    Mapping = new Dictionary<long, byte[]>()
                    {
                        {0x33D6D7, new byte[] {0x08}}, // Crash fix when dismissing entertainer, push EntityID on stack
                        {0xC3F081, Encoding.ASCII.GetBytes("EMXBinData.s6patcher")}, // Make game load .s6patcher binary file in MAINMENU lua state
                        {0xC3F095, new byte[] {0x00, 0x00, 0x00}}, // Make game load .s6patcher binary file in MAINMENU lua state
                    }
                },
                new PatchEntry
                {
                    Name = "Activate Limited/Special Edition",
                    Mapping = new Dictionary<long, byte[]>()
                    {
                        {0x25D4D5, new byte[] {0x90, 0x90}}, // Override JZ, always set Special Edition to 1
                        {0x25D4DB, new byte[] {0x90, 0x90, 0xFF, 0xC3}}, // Override JNZ, always set Special Edition to 1
                    }
                }
            };
        }

        public override UInt32[] GetTextureResolutionMapping()
        {
            return new UInt32[] {0x2D4D74, 0x2D4D7B, 0x2D4D82};
        }
        public override Dictionary<long, byte[]> GetZoomLevelMapping(double ZoomLevel, float ClutterFarDistance)
        {
            float Offset = 4800;
            float TransitionFactor = 3700;

            return new Dictionary<long, byte[]>()
            {
                {0xC4F9EC, BitConverter.GetBytes(ClutterFarDistance)}, // The HE uses the zoomlevel as single ...
                {0x270E73, new byte[] {0xC7, 0x45, 0xF0}},
                {0x270E76, BitConverter.GetBytes(ClutterFarDistance + Offset)},
                {0x270E7A, new byte[] {0xC7, 0x45, 0xF4}},
                {0x270E7D, BitConverter.GetBytes(TransitionFactor)},
                {0x270E81, new byte[] {0x90}},
                {0x270E87, new byte[] {0x90, 0x90, 0x90}},
                {0x2540B8, new byte[] {0x50, 0xB8, 0x00, 0x40, 0x9C, 0xC5, 0x89, 0x81, 0x9C, 0x00, 0x00, 0x00, 0x58, 0xC6, 0x81, 0x98, 0x00, 0x00, 0x00, 0x01, 0x90, 0x90}},
            };
        }
        public override UInt32[] GetAutoSaveMapping()
        {
            return new UInt32[] {0x1C6045, 0xEB95C0};
        }
    }
}
