using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using static S6Patcher.Helpers;

namespace S6Patcher
{
    public sealed class Backup
    {
        private static readonly Backup _instance = new Backup();
        private Backup() {}
        public static Backup Instance
        {
            get
            {
                return _instance;
            }
        }
        public bool CreateBackup(string Filepath)
        {
            string FileName = Path.GetFileNameWithoutExtension(Filepath);
            string DirectoryPath = Path.GetDirectoryName(Filepath);
            string FinalPath = Path.Combine(DirectoryPath, FileName + "_BACKUP.exe");

            if (File.Exists(FinalPath) == false)
            {
                try
                {
                    File.Copy(Filepath, FinalPath, false);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }
            }
            return true;
        }
        public void RemoveUserScriptFiles()
        {
            string[] ScriptFiles = { "UserScriptLocal.lua", "EMXBinData.s6patcher" };
            List<string> Directories = GetUserScriptDirectories();
            foreach (string Element in Directories)
            {
                string ScriptPath = Path.Combine(Element, "Script");
                if (!Directory.Exists(ScriptPath))
                {
                    continue;
                }
                foreach (string Entry in ScriptFiles)
                {
                    try
                    {
                        File.Delete(Path.Combine(ScriptPath, Entry));
                    }
                    catch (Exception) // Errors here do not matter
                    {
                        continue;
                    }
                }
            }
        }
        public bool RestoreBackup(ref FileStream Stream)
        {
            RemoveUserScriptFiles(); // Delete Userscript from Folders

            string FilePath = Stream.Name;
            string FileName = Path.GetFileNameWithoutExtension(FilePath);
            string DirectoryPath = Path.GetDirectoryName(FilePath);
            string FinalPath = Path.Combine(DirectoryPath, FileName + "_BACKUP.exe");

            if (File.Exists(FinalPath) == false)
            {
                return false;
            }

            Stream.Close();
            Stream.Dispose();

            try
            {
                File.Replace(FinalPath, FilePath, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }

            return true;
        }
    }
}
