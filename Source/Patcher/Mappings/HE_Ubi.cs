﻿using System;
using System.Collections.Generic;
using System.Text;

namespace S6Patcher.Source.Patcher.Mappings
{
    internal class HE_Ubi : MappingBase
    {
        public override List<PatchEntry> GetMapping()
        {
            return new List<PatchEntry>
            {
                new PatchEntry
                {
                    Name = "cbDevMode",
                    Mapping = new Dictionary<long, byte[]>()
                    {
                        {0x1E0837, new byte[] {0xEB, 0x19}}, // Enable Development-Mode without command line argument -DevM
                        {0x204B57, new byte[] {0xC6, 0x05}}, // Set global DevMachine to 1
                        {0x204B5D, new byte[] {0x01}}, // Set global DevMachine to 1
                    }
                },
                new PatchEntry
                {
                    Name = "cbScriptBugFixes",
                    Mapping = new Dictionary<long, byte[]>()
                    {
                        {0x33C533, new byte[] {0x08}}, // Crash fix when dismissing entertainer, push EntityID on stack
                        {0xC3E661, Encoding.ASCII.GetBytes("EMXBinData.s6patcher")}, // Make game load .s6patcher binary file in MAINMENU lua state
                        {0xC3E675, new byte[] {0x00, 0x00, 0x00}}, // Make game load .s6patcher binary file in MAINMENU lua state
                        {0x1C577C, new byte[] {0x90, 0x90}}, // Always load userscript, even when not in dev mode
                    }
                },
                new PatchEntry
                {
                    Name = "cbLimitedEdition",
                    Mapping = new Dictionary<long, byte[]>()
                    {
                        {0x25C6FA, new byte[] {0x90, 0x90}}, // Override JZ, always set Special Edition to 1
                        {0x25C700, new byte[] {0x90, 0x90, 0xFF, 0xC3}}, // Override JNZ, always set Special Edition to 1
                    }
                }
            };
        }

        public override Dictionary<long, byte[]> GetModloaderMapping()
        {
            byte[] CurrentPath = Encoding.ASCII.GetBytes("..\\modloader\\shr\0\0");
            return new Dictionary<long, byte[]>()
            {
                // .data segment
                {0xC4FC74, Encoding.ASCII.GetBytes("ModLoader: Adding primary directory.\n\0\0")}, // Print to log
                {0xC4FCB8, CurrentPath}, // Add primary directory path
                {0xC4FCA4, CurrentPath}, // Add primary directory path
                // .text segment -> Check what loading method to use
                {0x25D526, new byte[] {0xEB}}, // Always jump to ModelViewer
                {0x25D55A, new byte[] {0xE8, 0x3E, 0x00, 0x00, 0x00, 0xEB, 0xC7, 0x90}}, // Return to original loader after mod
                // .text segment -> Override the ModelViewer func
                {0x25D600, new byte[] {0xEB, 0x7A}}, // Return to original loader after mod
            };
        }

        public override Dictionary<long, byte[]> GetTextureResolutionMapping(uint Resolution)
        {
            Dictionary<long, byte[]> Mapping = new Dictionary<long, byte[]>();

            uint i = 0;
            foreach (var Element in new uint[] {0x2D4188, 0x2D418F, 0x2D4196})
            {
                Mapping.Add(Element, BitConverter.GetBytes(Resolution / Convert.ToUInt32(Math.Pow(2, i++))));
            }

            return Mapping;
        }

        public override Dictionary<long, byte[]> GetZoomLevelMapping(double ZoomLevel, float ClutterFarDistance)
        {
            float Offset = 4800;
            float TransitionFactor = 3700;

            return new Dictionary<long, byte[]>()
            {
                {0xC4EC4C, BitConverter.GetBytes(ClutterFarDistance)}, // This is not an error, the HE uses the zoomlevel as single instead of double like in the OV ...
                {0x270311, new byte[] {0xC7, 0x45, 0xF0}},
                {0x270314, BitConverter.GetBytes(ClutterFarDistance + Offset)},
                {0x270318, new byte[] {0xC7, 0x45, 0xF4}},
                {0x27031B, BitConverter.GetBytes(TransitionFactor)},
                {0x27031F, new byte[] {0x90}},
                {0x270325, new byte[] {0x90, 0x90, 0x90}},
                {0x2532F7, new byte[] {0x50, 0xB8, 0x00, 0x40, 0x9C, 0xC5, 0x89, 0x81, 0x9C, 0x00, 0x00, 0x00, 0x58, 0xC6, 0x81, 0x98, 0x00, 0x00, 0x00, 0x01, 0x90, 0x90}},
            };
        }

        public override Dictionary<long, byte[]> GetAutoSaveMapping(double Time)
        {
            byte[] Interval = BitConverter.GetBytes(Time * 60000);
            return new Dictionary<long, byte[]>()
            {
                {0x1C5F2A, (Time == 0.0) ? new byte[] {0xEB} : new byte[] {0x76}}, // Switch autosave on or off
                {0xEB83C0, Interval},
            };
        }
    }
}
