using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace S6Patcher
{
    public enum execID {OV = 0, HE = 1, ED = 2}; // Identify the current game release version
    public class Helpers
    {
        public static bool IsSteamOV = false; // This does not actually refer to the OV from Steam
        public static bool IsSteamHE = false; // This however, does

        public static void WriteBytes(ref FileStream Stream, long Position, byte[] Bytes)
        {
            Stream.Position = (IsSteamOV == false) ?  Position :  Position - 0x3F0000;
            try
            {
                Stream.Write(Bytes, 0, Bytes.Length);
            }
            catch (Exception ex)
            {
                MessageBox.Show("WriteToFileStream:\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        public static List<string> GetUserScriptDirectories()
        {
            // <Documents>/THE SETTLERS - Rise of an Empire/Script/UserScriptGlobal.lua && UserScriptLocal.lua are always loaded by the game when a map is started!
            string DocumentPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            List<string> Directories = new List<string>();
            foreach (string Element in Directory.GetDirectories(DocumentPath))
            {
                if (Element.Contains("Aufstieg eines") || Element.Contains("Rise of an"))
                {
                    Directories.Add(Path.Combine(DocumentPath, Element));
                }
            }

            return Directories;
        }
    }
}