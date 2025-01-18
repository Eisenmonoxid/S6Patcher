using S6Patcher.Source.Helpers;
using System.Collections.Generic;
using System.Text;

namespace S6Patcher.Source.Patcher
{
    internal class Modloader
    {
        /*
         *  mod.bba is always loaded, no matter if game is base or extra1 
         *  When executable is patched, the mod.bba has to be existent, otherwise the game won't start
         *  Feature can be toggled on/off with the Patcher (or the player can start the backup executable)
         *  Make bba containing game fixes available
         *  TODO: Directory for History Editions!
        */
        Modloader(execID ID)
        {
        }
        public Mappings.PatchEntry GetOVLoader()
        {
            Mappings.PatchEntry Loader = new Mappings.PatchEntry()
            {
                Name = "Activate Modloader",
                Mapping = new Dictionary<long, byte[]>()
                {
                    // .data segment
                    {0x53F5BC, Encoding.ASCII.GetBytes("modloader\\bba\\mod.bba\0\0")}, // Add mod.bba file path
                    // .text segment
                    {0x23E916, new byte[] {0xEB}}, // Always jump to ModelViewer
                    {0x23E936, new byte[] {0xEB, 0xE0, 0x90, 0x90}} // Return to original loader after mod
                }
            };

            return Loader;
        }
        /*
        public Mappings.PatchEntry GetSteamHELoader()
        {
            Mappings.PatchEntry Loader = new Mappings.PatchEntry()
            {
                Name = "Activate Modloader",
                Mapping = new Dictionary<long, byte[]>()
                {
                    // .data segment
                    {0x53F5BC, Encoding.ASCII.GetBytes("modloader\\bba\\mod.bba\0\0")}, // Add mod.bba file path
                    // .text segment
                    {0x23E916, new byte[] {0xEB}}, // Always jump to ModelViewer
                    {0x23E936, new byte[] {0xEB, 0xE0, 0x90, 0x90}} // Return to original loader after mod
                }
            };

            return Loader;
        }

        public Mappings.PatchEntry GetUbiHELoader()
        {
            Mappings.PatchEntry Loader = new Mappings.PatchEntry()
            {
                Name = "Activate Modloader",
                Mapping = new Dictionary<long, byte[]>()
                {
                    // .data segment
                    {0x53F5BC, Encoding.ASCII.GetBytes("modloader\\bba\\mod.bba\0\0")}, // Add mod.bba file path
                    // .text segment
                    {0x23E916, new byte[] {0xEB}}, // Always jump to ModelViewer
                    {0x23E936, new byte[] {0xEB, 0xE0, 0x90, 0x90}} // Return to original loader after mod
                }
            };

            return Loader;
        }
        */
    }
}
