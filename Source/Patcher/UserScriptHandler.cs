using S6Patcher.Properties;
using S6Patcher.Source.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace S6Patcher.Source.Patcher
{
    public sealed class UserScriptHandler
    {
        // <Documents>/THE SETTLERS - Rise of an Empire/Script/UserScriptGlobal.lua && UserScriptLocal.lua are ...
        // ... always loaded by the game when a map is started!
        // EMXBinData.s6patcher is the minified and precompiled MainMenuUserScript.lua

        private UserScriptHandler() {}
        public static UserScriptHandler Instance {get;} = new();

        private readonly Dictionary<string, byte[]> Scripts = new()
        {
            {"UserScriptLocal.lua", Resources.UserScriptLocal},
            {"EMXBinData.s6patcher", Resources.EMXBinData},
            {"UserScriptGlobal.lua", Resources.UserScriptGlobal}
        };
        private string[] ScriptFiles => [.. Scripts.Keys];
        private byte[][] ScriptResources => [.. Scripts.Values];
        public string GlobalDocuments {get; set;} = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        public bool DoesUserScriptDirectoryExist() => GetUserScriptDirectories().Count != 0;

        public List<string> GetUserScriptDirectories() => !string.IsNullOrEmpty(GlobalDocuments) ? SelectUserScriptDirectories(GlobalDocuments) : [];

        private static List<string> SelectUserScriptDirectories(string Documents) => [.. Directory.GetDirectories(Documents)
                .Where(Element => Element.Contains("Aufstieg eines") || Element.Contains("Rise of an"))
                .Select(Element => {Element = Path.Combine(Documents, Element); return Element;})];

        public async Task CreateUserScriptFiles()
        {
            foreach (string Element in GetUserScriptDirectories())
            {
                string CurrentPath = Path.Combine(Element, "Script");
                try
                {
                    Directory.CreateDirectory(CurrentPath);
                    for (int i = 0; i < ScriptFiles.Length; i++)
                    {
                        string FilePath = Path.Combine(CurrentPath, ScriptFiles[i]);
                        await File.WriteAllBytesAsync(FilePath, ScriptResources[i]);
                        Logger.Instance.Log("Finished writing ScriptFile named " + ScriptFiles[i] + " to " + CurrentPath);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Instance.Log(ex.ToString());
                    ErrorTracking.Increment();
                    continue;
                }
            }
        }
    }
}
