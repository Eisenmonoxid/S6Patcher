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
        public static IOFileHandler Instance
        {
            get
            {
                return _instance;
            }
        }

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
        public OpenFileDialog CreateOFDialog()
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                CheckFileExists = true,
                ShowHelp = false,
                CheckPathExists = true,
                DereferenceLinks = true,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                Multiselect = false,
                ShowReadOnly = false,
            };

            return ofd;
        }
        public bool CreateBackup(string Filepath)
        {
            string FinalPath = Path.Combine(Path.GetDirectoryName(Filepath), Path.GetFileNameWithoutExtension(Filepath) + "_BACKUP.exe");
            if (File.Exists(FinalPath) == false)
            {
                try
                {
                    File.Copy(Filepath, FinalPath, false);
                    Logger.Instance.Log("CreateBackup(): Backup creation successful! Path: " + FinalPath);
                }
                catch (Exception ex)
                {
                    Logger.Instance.Log(ex.ToString());
                    MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }
            }

            return true;
        }
        private void DeleteUserConfiguration(string[] Options)
        {
            string[] ScriptFiles = {"UserScriptLocal.lua", "EMXBinData.s6patcher"};
            Helpers.GetUserScriptDirectories().ForEach(Element =>
            {
                DeleteSectionFromOptions(Path.Combine(Element, "Config"), Options);
                string ScriptPath = Path.Combine(Element, "Script");
                if (Directory.Exists(ScriptPath) == true)
                {
                    foreach (string Entry in ScriptFiles)
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
            string FinalPath = Path.Combine(Path.GetDirectoryName(FilePath), Path.GetFileNameWithoutExtension(FilePath) + "_BACKUP.exe");

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
            List<string> Directories = Helpers.GetUserScriptDirectories();
            foreach (string Element in Directories)
            {
                string CurrentPath = Path.Combine(Path.Combine(Element, "Config"), Name);
                if (File.Exists(CurrentPath) == false)
                {
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
                    Logger.Instance.Log("UpdateEntryInOptionsFile(): Updated file " + CurrentPath);
                }
                catch (Exception ex)
                {
                    Logger.Instance.Log(ex.ToString());
                    MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }
            }

            return true;
        }
    }
}
