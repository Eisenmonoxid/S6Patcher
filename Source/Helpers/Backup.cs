using S6Patcher.Source.Patcher;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace S6Patcher.Source.Helpers
{
    public static class Backup
    {
        public static bool CreateBackup(string Filepath)
        {
            string BackupPath = GetBackupPath(Filepath);
            if (File.Exists(BackupPath) == false)
            {
                try
                {
                    File.Copy(Filepath, BackupPath, false);
                }
                catch (Exception ex)
                {
                    Logger.Instance.Log(ex.ToString());
                    return false;
                }
            }

            Logger.Instance.Log("CreateBackup(): Backup creation successful! Path: " + BackupPath);
            return true;
        }

        private static void DeleteUserConfiguration(string[] Options)
        {
            UserScriptHandler.GetUserScriptDirectories().ForEach(Element =>
            {
                DeleteSectionFromOptions(Path.Combine(Element, "Config"), Options);
                string ScriptPath = Path.Combine(Element, "Script");
                if (Directory.Exists(ScriptPath) == true)
                {
                    foreach (string Entry in UserScriptHandler.ScriptFiles)
                    {
                        try
                        {
                            string CurrentFile = Path.Combine(ScriptPath, Entry);
                            File.Delete(CurrentFile);
                            Logger.Instance.Log("DeleteUserConfiguration(): File sucessfully deleted: " + CurrentFile);
                        }
                        catch (Exception ex) // Errors here do not matter
                        {
                            Logger.Instance.Log("DeleteUserConfiguration(): " + ex.Message);
                            continue;
                        }
                    }
                }
            });
        }

        private static void DeleteSectionFromOptions(string CurrentPath, string[] Options)
        {
            CurrentPath = Path.Combine(CurrentPath, "Options.ini");
            if (File.Exists(CurrentPath) == false)
            {
                Logger.Instance.Log("DeleteSectionFromOptions(): File " + CurrentPath + " NOT found! Aborting ...");
                return;
            }

            List<string> Lines;
            try
            {
                Lines = [.. File.ReadAllLines(CurrentPath)];
            }
            catch (Exception ex)
            {
                Logger.Instance.Log(ex.ToString());
                return;
            }

            foreach (string Element in Options)
            {
                Lines.RemoveAll(Line => Line.Contains(Element));
            }

            try
            {
                File.WriteAllLines(CurrentPath, Lines);
            }
            catch (Exception ex)
            {
                Logger.Instance.Log(ex.ToString());
                return;
            }

            Logger.Instance.Log("DeleteSectionFromOptions(): File " + CurrentPath + " sucessfully updated!");
        }

        public static bool RestoreBackup(FileStream Stream, string[] Options)
        {
            DeleteUserConfiguration(Options); // Delete Userscript & Config Section from Documents folder

            string FilePath = Stream.Name;
            string FinalPath = GetOldBackupPath(FilePath);

            if (File.Exists(FinalPath) == false)
            {
                Logger.Instance.Log("RestoreBackup(): OLD File " + FinalPath + " NOT found! Retry ...");

                FinalPath = GetBackupPath(FilePath);
                if (File.Exists(FinalPath) == false)
                {
                    Logger.Instance.Log("RestoreBackup(): NEW File " + FinalPath + " NOT found! Aborting ...");
                    return false;
                }
            }

            IOFileHandler.Instance.CloseStream(Stream);

            try
            {
                File.Replace(FinalPath, FilePath, null);
            }
            catch (Exception ex)
            {
                Logger.Instance.Log(ex.ToString());
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }

            Logger.Instance.Log("RestoreBackup(): File " + FinalPath + " successfully restored!");
            return true;
        }

        private static string GetBackupPath(string Filepath) => 
            Path.Combine(Path.GetDirectoryName(Filepath), Path.GetFileNameWithoutExtension(Filepath) + ".backup");

        private static string GetOldBackupPath(string Filepath) =>
            Path.Combine(Path.GetDirectoryName(Filepath), Path.GetFileNameWithoutExtension(Filepath) + "_BACKUP.exe");
    }
}
