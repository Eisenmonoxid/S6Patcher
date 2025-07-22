using S6Patcher.Source.Patcher;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace S6Patcher.Source.Helpers
{
    public sealed class IOFileHandler
    {
        private static readonly IOFileHandler _instance = new IOFileHandler();
        private IOFileHandler() {}
        public static IOFileHandler Instance => _instance;
        public string InitialDirectory = String.Empty;

        public FileStream OpenFileStream(string Path)
        {
            FileStream Stream;
            try
            {
                Stream = new FileStream(Path, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (Exception ex)
            {
                Logger.Instance.Log(ex.ToString());
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return null;
            }

            Logger.Instance.Log("OpenFileStream(): Returning Stream: " + Stream.Name);
            return Stream;
        }

        public OpenFileDialog CreateOFDialog(string Filter, Environment.SpecialFolder Folder)
        {
            return new OpenFileDialog
            {
                CheckFileExists = true,
                ShowHelp = false,
                CheckPathExists = true,
                DereferenceLinks = true,
                InitialDirectory = (InitialDirectory == String.Empty || Folder == Environment.SpecialFolder.MyDocuments) ? Environment.GetFolderPath(Folder) : InitialDirectory,
                Multiselect = false,
                ShowReadOnly = false,
                Filter = Filter
            };
        }

        public bool CreateBackup(string Filepath)
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
                    MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }
            }

            Logger.Instance.Log("CreateBackup(): Backup creation successful! Path: " + BackupPath);
            return true;
        }

        private void DeleteUserConfiguration(string[] Options)
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

        private void DeleteSectionFromOptions(string CurrentPath, string[] Options)
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
                Lines = File.ReadAllLines(CurrentPath).ToList();
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

        public bool RestoreBackup(FileStream Stream, string[] Options)
        {
            DeleteUserConfiguration(Options); // Delete Userscript & Config Section from Documents folder

            string FilePath = Stream.Name;
            string FinalPath = GetBackupPath(FilePath);

            if (File.Exists(FinalPath) == false)
            {
                Logger.Instance.Log("RestoreBackup(): File " + FinalPath + " NOT found!");
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
                Logger.Instance.Log(ex.ToString());
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }

            Logger.Instance.Log("RestoreBackup(): File " + FinalPath + " successfully restored!");
            return true;
        }

        public bool UpdateEntryInOptionsFile(string Section, string Key, bool Entry)
        {
            string Name = "Options.ini";
            List<string> Directories = UserScriptHandler.GetUserScriptDirectories();
            foreach (string Element in Directories)
            {
                string CurrentPath = Path.Combine(Element, "Config", Name);
                if (File.Exists(CurrentPath) == false)
                {
                    Logger.Instance.Log("UpdateEntryInOptionsFile(): Skipping! Did not find Options file " + CurrentPath);
                    continue;
                }

                // Open configuration file, update Section, Key and Value
                List<string> Lines;
                try
                {
                    Lines = File.ReadAllLines(CurrentPath).ToList();
                }
                catch (Exception ex)
                {
                    Logger.Instance.Log(ex.ToString());
                    MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                Lines.RemoveAll(Line => Line.Contains(Key));
                if (Lines.Contains(Section) == false)
                {
                    Lines.Add(Section);
                }
                Lines.Insert(Lines.IndexOf(Section) + 1, Key + "=" + ((Entry == true) ? "1" : "0"));

                try
                {
                    File.WriteAllLines(CurrentPath, Lines);
                }
                catch (Exception ex)
                {
                    Logger.Instance.Log(ex.ToString());
                    MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                Logger.Instance.Log("UpdateEntryInOptionsFile(): Updated file " + CurrentPath);
            }

            return true;
        }

        public string GetRootDirectory(string Filepath, uint Depth)
        {
            Logger.Instance.Log("GetRootPathFromFile(): Called with Depth: " + Depth.ToString() + " and Input: " + Filepath);

            DirectoryInfo Info = new DirectoryInfo(Path.GetDirectoryName(Filepath));
            for (; Depth > 0; Depth--)
            {
                Info = Info.Parent;
            }

            Logger.Instance.Log("GetRootPathFromFile(): Returning Path: " + Info.FullName);
            return Info.FullName;
        }

        private string GetBackupPath(string Filepath)
        {
            return Path.Combine(Path.GetDirectoryName(Filepath), Path.GetFileNameWithoutExtension(Filepath) + ".backup");
        }
    }
}
