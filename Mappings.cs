﻿using System.Collections.Generic;

namespace S6Patcher
{
    internal class Mappings
    {
        public static readonly Dictionary<long, byte[]> OVMapping = new Dictionary<long, byte[]>()
        {
            {0x288ADF, new byte[] {0xE9, 0x1D, 0xD2, 0x26, 0x00, 0x90, 0x90, 0x90}},
            {0x4F5D01, new byte[] {0xE8, 0x87, 0x3D, 0xD9, 0xFF, 0xC7, 0x46, 0x40, 0x00,
                    0x70, 0x9B, 0x3C, 0xE9, 0xD5, 0x2D, 0xD9, 0xFF, 0x90}},
            {0x2BE177, new byte[] {0x00, 0x08, 0x00, 0x00}},
            {0x2BE17E, new byte[] {0x00, 0x04, 0x00, 0x00}},
            {0x2BE185, new byte[] {0x00, 0x02, 0x00, 0x00}},
            {0x732FA, new byte[] {0xC6, 0x05, 0x28, 0xBF, 0xAA, 0x00, 0x01, 0xEB, 0x7C, 0x90}},
        };
        public static readonly Dictionary<long, byte[]> HEMapping = new Dictionary<long, byte[]>()
        {
            {0x1E0837, new byte[] {0xEB, 0x19}},
            {0x204B57, new byte[] {0xC6, 0x05}},
            {0x204B5D, new byte[] {0x01}},
        };
        public static readonly Dictionary<long, byte[]> EditorMapping = new Dictionary<long, byte[]>()
        {
            {0x27A7AD, new byte[] {0xEB, 0x00}},
            {0x25B100, new byte[] {0x09, 0x7C, 0x03, 0x6A, 0x00, 0x58, 0x83, 0xC7, 0x20, 0x89,
                    0x46, 0x3C, 0x8B, 0x0F, 0xC7, 0x07, 0x00, 0x00, 0x00, 0x00, 0xEB}},
            {0x21285, new byte[] {0xEB}},
            {0x212A0, new byte[] {0xEB}},
            {0x21D3A, new byte[] {0xEB}},
            {0x21D55, new byte[] {0xEB}},
            {0x4B879, new byte[] {0xEB}},
            {0x4BBE8, new byte[] {0x3B, 0xFB}},
            {0x4A710, new byte[] {0x00, 0x00, 0xF0}},
            {0x42FC60, new byte[] {0x1F, 0x04}},
            {0x20FBE, new byte[] {0x66, 0x90}},
            {0xE7BC6, new byte[] {0xEB, 0x09}},
            {0x2E06D, new byte[] {0x90, 0x90}},
            {0xC094E, new byte[] {0xB8, 0x01, 0x00, 0x00, 0x00}},
            {0xC1745, new byte[] {0xB8, 0x01, 0x00, 0x00, 0x00}},
            {0x3AEAC, new byte[] {0xEB, 0x08}},
            {0x20A0FB, new byte[] {0xC6, 0x05, 0xBC, 0x79, 0x97, 0x00, 0x01, 0xEB, 0x7C, 0x90}},
            {0x13B4A, new byte[] {0xE9, 0xDE, 0xFE, 0xFF, 0xFF, 0xEB, 0x5C}},
            {0xCC36, new byte[] {0x74, 0x7A}},
            {0x4C5A9, new byte[] {0xEB, 0x1C}},
            {0x32239, new byte[] {0xEB, 0x17}},
            {0x2E3AA, new byte[] {0xEB, 0x17}},
            {0x2A716, new byte[] {0x90, 0x90}},
            {0x1F2AD, new byte[] {0x90, 0x90}},
            {0x45474, new byte[] {0xEB, 0x07}}, 
            {0x2AE72, new byte[] {0xEB, 0x23}},
            //{0x31B5C, new byte[] {0xEB}}, // -> Works, but useless (All Clutter in Editor Dialog)
        };
    }
}
