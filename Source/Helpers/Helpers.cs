using IWshRuntimeLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace S6Patcher.Source.Helpers
{
    public enum execID
    {
        NONE = 0,
        OV = 1,
        OV_OFFSET = 2,
        HE_UBISOFT = 3,
        HE_STEAM = 4,
        ED = 5
    };
    public static class Helpers
    {
        public static UInt32 GlobalOffset = 0x3F0000;
        public static execID CurrentID = execID.NONE;
        public static void WriteBytes(ref FileStream Stream, long Position, byte[] Bytes)
        {
            Stream.Position = (CurrentID == execID.OV_OFFSET) ? Position - GlobalOffset : Position;
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
            // EMXBinData.s6patcher is the minified and precompiled MainMenuUserScript.lua
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
        public static bool SetCurrentExecutableID(ref FileStream Stream)
        {
            string ExpectedVersion = "1, 71, 4289, 0";
            byte[] Result = new byte[30];

            Dictionary<UInt32, execID> Mapping = new Dictionary<UInt32, execID>()
            {
                {0x6ECADC, execID.OV},
                {0x2FCADC, execID.OV_OFFSET},
                {0xF531A4, execID.HE_UBISOFT},
                {0xF545A4, execID.HE_STEAM},
                {0x6D06A8, execID.ED},
            };

            foreach (var Element in Mapping)
            {
                if (Stream.Length < Element.Key)
                {
                    continue;
                }

                Stream.Position = Element.Key;
                Stream.Read(Result, 0, Result.Length);
                string Version = Encoding.Unicode.GetString(Result).Substring(0, ExpectedVersion.Length);

                if (Version == ExpectedVersion)
                {
                    CurrentID = Element.Value;
                    return true;
                };
            }

            CurrentID = execID.NONE;
            return false;
        }
        public static void CreateDesktopShortcuts(string Filepath)
        {
            string Desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            WshShell Shell = new WshShell();

            string PatchedLink = Desktop + @"\Settlers 6 - Patched.lnk";
            string UnpatchedLink = Desktop + @"\Settlers 6 - Unpatched.lnk";

            IWshShortcut UnpatchedShortcut = (IWshShortcut)Shell.CreateShortcut(UnpatchedLink);
            IWshShortcut PatchedShortcut = (IWshShortcut)Shell.CreateShortcut(PatchedLink);

            UnpatchedShortcut.Description = "Launches UNPatched Settlers 6";
            PatchedShortcut.Description = "Launches Patched Settlers 6";

            PatchedShortcut.TargetPath = Filepath;
            UnpatchedShortcut.TargetPath = Filepath.Replace(Path.GetExtension(Filepath), "") + "_BACKUP.exe";

            try
            {
                UnpatchedShortcut.Save();
                PatchedShortcut.Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show("CreateDesktopShortcuts:\n\n" + ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); 
                return;
            }
        }
        public static string GetRootPathFromFile(string Filepath, execID ID)
        {
            int Index = Filepath.LastIndexOf(Path.GetFileName(Filepath)) - 1;
            Filepath = Filepath.Remove(Index, Filepath.Length - Index);

            for (short i = 0; true; i++)
            {
                if ((ID == execID.OV || ID == execID.OV_OFFSET) && (i == 2))
                {
                    break;
                }
                else if (i == 3)
                {
                    break;
                }

                Index = Filepath.LastIndexOf(Path.DirectorySeparatorChar);
                Filepath = Filepath.Remove(Index, Filepath.Length - Index);
            }
            
            return Filepath;
        }
    }
}