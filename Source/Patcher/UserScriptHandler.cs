using S6Patcher.Properties;
using S6Patcher.Source.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace S6Patcher.Source.Patcher
{
    public static class UserScriptHandler
    {
        private static readonly Dictionary<string, byte[]> Scripts = new Dictionary<string, byte[]>()
        {
            {"UserScriptLocal.lua", Resources.UserScriptLocal},
            {"EMXBinData.s6patcher", Resources.EMXBinData},
            {"UserScriptGlobal.lua", Resources.UserScriptGlobal}
        };
        public static string[] ScriptFiles => Scripts.Keys.ToArray();
        public static byte[][] ScriptResources => Scripts.Values.ToArray();
        private static string MonoGlobalDocumentsPath = String.Empty;

        public static List<string> GetUserScriptDirectories()
        {
            // <Documents>/THE SETTLERS - Rise of an Empire/Script/UserScriptGlobal.lua && UserScriptLocal.lua are always loaded by the game when a map is started!
            // EMXBinData.s6patcher is the minified and precompiled MainMenuUserScript.lua
            string DocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (Program.IsMono)
            {
                if (MonoGlobalDocumentsPath == String.Empty)
                {
                    MessageBox.Show(Resources.MonoOptionsFile, "Select Options.ini file ...", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    OpenFileDialog ofd = IOFileHandler.Instance.CreateOFDialog("Configuration file|*.ini", Environment.SpecialFolder.MyDocuments);

                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        DirectoryInfo Info = new DirectoryInfo(ofd.FileName);
                        MonoGlobalDocumentsPath = Info.Parent.Parent.Parent.FullName;
                    }
                    else
                    {
                        Logger.Instance.Log("GetUserScriptDirectories(): User did not select a file! Aborting ...");
                        ofd.Dispose();
                        return new List<string>();
                    }
                    ofd.Dispose();
                }

                DocumentsPath = MonoGlobalDocumentsPath;
            }

            return SelectUserScriptDirectories(DocumentsPath);
        }

        private static List<string> SelectUserScriptDirectories(string DocumentsPath)
        {
            return Directory.GetDirectories(DocumentsPath)
                .Where(Element => Element.Contains("Aufstieg eines") || Element.Contains("Rise of an"))
                .Select(Element => {Element = Path.Combine(DocumentsPath, Element); return Element;})
                .ToList();
        }
    }
}
