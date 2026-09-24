using System;
using System.IO;
using System.Threading.Tasks;

namespace S6Patcher.Source.Utilities
{
    internal static class Backup
    {
        public static event Action<string> ShowMessage;
        public static event Func<string, Task<bool>> ShowMessagePrompt;

        public static bool Create(string Path)
        {
            string Backup = GetBackupPath(Path, false);
            if (!File.Exists(Backup))
            {
                try
                {
                    File.Copy(Path, Backup, false);
                }
                catch (Exception ex)
                {
                    Logger.Instance.Log(ex.ToString());
                    return false;
                }
            }

            Logger.Instance.Log("Backup creation successful! Path: " + Backup);
            return true;
        }

        public static async Task Restore(string Path, execID GlobalID)
        {
            string Backup = GetBackupPath(Path, true);
            if (!File.Exists(Backup))
            {
                Logger.Instance.Log("OLD File " + Backup + " NOT found! Retry ...");

                Backup = GetBackupPath(Path, false);
                if (!File.Exists(Backup))
                {
                    Logger.Instance.Log("NEW File " + Backup + " NOT found! Aborting ...");
                    ShowMessage.Invoke("Could not restore Backup. No file found!");
                    return;
                }
            }

            try
            {
                File.Replace(Backup, Path, null);
            }
            catch (Exception ex)
            {
                Logger.Instance.Log(ex.ToString());
                ShowMessage.Invoke(ex.Message);
                return;
            }

            bool KeepModFiles = await AskForModDeletion();
            if (!KeepModFiles && GlobalID != execID.NONE)
            {
                string ModLoaderPath = IOFileHandler.Instance.GetModLoaderDirectory(GlobalID, Path);
                if (Directory.Exists(ModLoaderPath))
                {
                    try
                    {
                        Directory.Delete(ModLoaderPath, true);
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.Log(ex.ToString());
                        ShowMessage.Invoke("Could not delete Mod folder: " + ex.Message);
                    }
                }
            }

            Logger.Instance.Log("File " + Path + " successfully restored!");
            ShowMessage.Invoke("Backup successfully restored!");
        }

        private static async Task<bool> AskForModDeletion() => await ShowMessagePrompt.Invoke("Keep Mod files? (Recommended)");
        private static string GetBackupPath(string Filepath, bool Old) =>
            Path.Combine(Path.GetDirectoryName(Filepath), 
                Path.GetFileNameWithoutExtension(Filepath) + (Old ? "_BACKUP.exe" : ".backup"));
    }
}
