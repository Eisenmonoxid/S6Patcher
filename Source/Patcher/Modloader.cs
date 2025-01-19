using S6Patcher.Source.Helpers;
using System.Collections.Generic;
using System.Text;

namespace S6Patcher.Source.Patcher
{
    internal class Modloader : Mappings
    {
        /*
        *  mod.bba is always loaded, no matter if game is base or extra1.
        *  When executable is patched, the mod.bba has to be existent, otherwise the game won't start.
        *  Feature can be toggled on/off with the Patcher (or the player can start the backup executable).
        *  Make bba containing game fixes available.
        */
        private Dictionary<long, byte[]> GetOVMappings()
        {
            return new Dictionary<long, byte[]>()
            {
                // .data segment
                {0x53F5BC, Encoding.ASCII.GetBytes("modloader\\bba\\mod.bba\0\0") }, // Add mod.bba file path
                // .text segment -> Check what loading method to use
                {0x23E916, new byte[] {0xEB}}, // Always jump to ModelViewer
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
        private Dictionary<long, byte[]> GetSteamHEMappings()
        {
            return new Dictionary<long, byte[]>()
            { 
                // .data segment
                {0xC50A14, Encoding.ASCII.GetBytes("ModLoader: Adding primary directory.\0\0")}, // Print to log
                {0xC50A58, Encoding.ASCII.GetBytes("modloader\\shr\0\0")}, // Add primary directory path
                // .text segment -> Check what loading method to use
                {0x25E2D0, new byte[] {0xEB}}, // Always jump to ModelViewer
                {0x25E304, new byte[] {0xE8, 0x41, 0x00, 0x00, 0x00, 0xEB, 0xC7, 0x90}}, // Return to original loader after mod
                // .text segment -> Override the ModelViewer func
                {0x25E3AA, new byte[] {0xEB}}, // Return to original loader after mod
            };
        }
        private Dictionary<long, byte[]> GetUbiHEMappings()
        {
            return new Dictionary<long, byte[]>()
            {
                // .data segment
                {0xC4FC74, Encoding.ASCII.GetBytes("ModLoader: Adding primary directory.\0\0")}, // Print to log
                {0xC4FCB8, Encoding.ASCII.GetBytes("modloader\\shr\0\0")}, // Add primary directory path
                // .text segment -> Check what loading method to use
                {0x25D526, new byte[] {0xEB}}, // Always jump to ModelViewer
                {0x25D55A, new byte[] {0xE8, 0x3E, 0x00, 0x00, 0x00, 0xEB, 0xC7, 0x90}}, // Return to original loader after mod
                // .text segment -> Override the ModelViewer func
                {0x25D600, new byte[] {0xEB}}, // Return to original loader after mod
            };
        }
        public new Dictionary<long, byte[]> GetMappingsByID(execID ID)
        {
            switch (ID)
            {
                case execID.OV:
                    return GetOVMappings();
                case execID.OV_OFFSET:
                    return GetOVMappings();
                case execID.HE_UBISOFT:
                    return GetUbiHEMappings();
                case execID.HE_STEAM:
                    return GetSteamHEMappings();
                default:
                    return null;
            }
        }
    }
}
