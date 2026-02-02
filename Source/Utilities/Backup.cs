using System;
using System.IO;

namespace S6Patcher.Source.Helpers
{
    internal static class Backup
    {
        public static event Action<string> ShowMessage;

        public static bool Create(string Path)
        {
            string Backup = GetBackupPath(Path, false);
            if (File.Exists(Backup) == false)
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

        public static void Restore(string Path)
        {
            string Backup = GetBackupPath(Path, true);
            if (File.Exists(Backup) == false)
            {
                Logger.Instance.Log("OLD File " + Backup + " NOT found! Retry ...");

                Backup = GetBackupPath(Path, false);
                if (File.Exists(Backup) == false)
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

            Logger.Instance.Log("File " + Path + " successfully restored!");
            ShowMessage.Invoke("Backup successfully restored!");
        }

        private static string GetBackupPath(string Filepath, bool Old) =>
            Path.Combine(Path.GetDirectoryName(Filepath), 
                Path.GetFileNameWithoutExtension(Filepath) + ((Old == true) ? "_BACKUP.exe" : ".backup"));
    }
}
