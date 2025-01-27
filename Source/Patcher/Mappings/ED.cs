﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace S6Patcher.Source.Patcher.Mappings
{
    internal class ED : Mapping
    {
        public ED() {}
        public override List<PatchEntry> GetMapping()
        {
            return new List<PatchEntry>
            {
                new PatchEntry
                {
                    Name = "High - Resolution Textures:",
                    Mapping = new Dictionary<long, byte[]>()
                    {
                        {0x27A7AD, new byte[] {0xEB, 0x00}}, // Set ground texture resolution
                        {0x25B100, new byte[] {0x09, 0x7C, 0x03, 0x6A, 0x00, 0x58, 0x83, 0xC7, 0x20, 0x89, 0x46, 0x3C, 0x8B, 0x0F, 0xC7, 0x07, 0x00, 0x00, 0x00, 0x00, 0xEB}}, // General texture resolution
                    }
                },
                new PatchEntry
                {
                    Name = "Free Scaling and Placing of Entities",
                    Mapping = new Dictionary<long, byte[]>()
                    {
                        {0x21285, new byte[] {0xEB}}, // Scaling lower limit
                        {0x212A0, new byte[] {0xEB}}, // Scaling upper limit
                        {0x21D3A, new byte[] {0xEB}}, // Scaling lower limit
                        {0x21D55, new byte[] {0xEB}}, // Scaling upper limit
                        {0x4B879, new byte[] {0xEB}}, // Override placeable check
                        {0x4BBE8, new byte[] {0x3B, 0xFB}}, // Override placeable check
                        {0x20FBE, new byte[] {0x66, 0x90}}, // Override placeable check
                        {0xE7BC6, new byte[] {0xEB, 0x09}}, // WallGate placement check
                        {0x2E06D, new byte[] {0x90, 0x90}}, // Wall placement check
                        {0xC094E, new byte[] {0xB8, 0x01, 0x00, 0x00, 0x00}}, // General placement check
                        {0xC1745, new byte[] {0xB8, 0x01, 0x00, 0x00, 0x00}}, // General placement check
                        {0x32239, new byte[] {0xEB, 0x17}}, // WallGate placement check
                        {0x2E3AA, new byte[] {0xEB, 0x17}}, // Wall segment placement check
                        {0x2A716, new byte[] {0x90, 0x90}}, // Placement override
                        {0x1F2AD, new byte[] {0x90, 0x90}}, // Placement override
                        {0x45474, new byte[] {0xEB, 0x07}}, // Placement override
                        //{0x31B5C, new byte[] {0xEB}}, // -> Works, but useless (All Clutter in Editor Dialog)
                    }
                },
                new PatchEntry
                {
                    Name = "Higher Entity Limits",
                    Mapping = new Dictionary<long, byte[]>()
                    {
                        {0x4A710, new byte[] {0x00, 0x00, 0xF0}}, // Higher general entity limit
                        {0x42FC60, new byte[] {0x1F, 0x04}}, // Movable entities at the same time
                    }
                },
                new PatchEntry
                {
                    Name = "Usable Black Map Border",
                    Mapping = new Dictionary<long, byte[]>()
                    {
                        {0x3AEAC, new byte[] {0xEB, 0x08}}, // Override map border check at saving
                    }
                },
                new PatchEntry
                {
                    Name = "Activate Development-Mode Permanently",
                    Mapping = new Dictionary<long, byte[]>()
                    {
                        {0x20A0FB, new byte[] {0xC6, 0x05, 0xBC, 0x79, 0x97, 0x00, 0x01, 0xEB, 0x7C, 0x90}}, // Set DevMachine to 1
                        {0x13B4A, new byte[] {0xE9, 0xDE, 0xFE, 0xFF, 0xFF, 0xEB, 0x5C}}, // Validate Map -> Lost Feature
                        {0xCC36, new byte[] {0x74, 0x7A}}, // Validate Map -> Lost Feature
                        {0x4C5A9, new byte[] {0xEB, 0x1C}}, // Remove lag when using windows in map editor (ThreadSleep 250 ms)
                        {0x2AE72, new byte[] {0xEB, 0x23}}, // Internal Window override
                        {0x2199F3, new byte[] {0xEB}}, // Override XML parsing error, enables "protected" maps to be loaded in the editor
                    }
                },
                new PatchEntry
                {
                    Name = "Show All Entities and Textures in Editor",
                    Mapping = new Dictionary<long, byte[]>()
                    {
                        {0x20615, new byte[] {0x90, 0x90, 0x90, 0x90, 0x90, 0x90}}, // Show all Entities in editor placement window
                        {0x20629, new byte[] {0xEB, 0x10}}, // Show all Entities in editor placement window
                        {0x24EEE, new byte[] {0x90, 0x90, 0x90, 0x90, 0x90, 0x90}}, // Show all Textures in the texture placement window
                        {0x24F05, new byte[] {0x90, 0x90, 0x90, 0x90, 0x90, 0x90}}, // Show all Textures in the texture placement window
                    }
                }
            };
        }
    }
}
