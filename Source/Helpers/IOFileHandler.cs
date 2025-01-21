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
                MessageBox.Show("OpenFileStream:\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return null;
            }

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
            string FileName = Path.GetFileNameWithoutExtension(Filepath);
            string FinalPath = Path.Combine(Path.GetDirectoryName(Filepath), FileName + "_BACKUP.exe");

            if (File.Exists(FinalPath) == false)
            {
                try
                {
                    File.Copy(Filepath, FinalPath, false);
                }
                catch (Exception ex)
                {
                    Logger.Instance.Log(ex.ToString());
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }
            }
            return true;
        }
        private void DeleteUserConfiguration()
        {
            string[] ScriptFiles = {"UserScriptLocal.lua", "EMXBinData.s6patcher"};
            List<string> Directories = Helpers.GetUserScriptDirectories();
            foreach (string Element in Directories)
            {
                DeleteSectionFromOptions(Path.Combine(Element, "Config"));
                string ScriptPath = Path.Combine(Element, "Script");
                if (Directory.Exists(ScriptPath) == false)
                {
                    continue;
                }
                foreach (string Entry in ScriptFiles)
                {
                    try
                    {
                        File.Delete(Path.Combine(ScriptPath, Entry));
                    }
                    catch (Exception ex) // Errors here do not matter
                    {
                        Logger.Instance.Log(ex.ToString());
                        continue;
                    }
                }
            }
        }
        private void DeleteSectionFromOptions(string CurrentPath)
        {
            CurrentPath = Path.Combine(CurrentPath, "Options.ini");
            if (File.Exists(CurrentPath) == false)
            {
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

            Lines.RemoveAll(Line => Line.Contains("S6Patcher"));
            Lines.RemoveAll(Line => Line.Contains("ExtendedKnightSelection"));

            try
            {
                File.WriteAllLines(CurrentPath, Lines);
            }
            catch (Exception ex)
            {
                Logger.Instance.Log(ex.ToString());
                return;
            }
        }
        public bool RestoreBackup(ref FileStream Stream)
        {
            DeleteUserConfiguration(); // Delete Userscript & Config Section from Documents folder

            string FilePath = Stream.Name;
            string FileName = Path.GetFileNameWithoutExtension(FilePath);
            string FinalPath = Path.Combine(Path.GetDirectoryName(FilePath), FileName + "_BACKUP.exe");

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
                Logger.Instance.Log(ex.ToString());
                MessageBox.Show("RestoreBackup:\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }

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
                    MessageBox.Show("UpdateEntryInOptionsFile:\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                Lines.RemoveAll(Line => Line.Contains(Section));
                Lines.RemoveAll(Line => Line.Contains(Key));
                Lines.Add("[" + Section + "]");
                Lines.Add(Key + "=" + ((Entry == true) ? "1" : "0"));

                try
                {
                    File.WriteAllLines(CurrentPath, Lines);
                }
                catch (Exception ex)
                {
                    Logger.Instance.Log(ex.ToString());
                    MessageBox.Show("UpdateEntryInOptionsFile:\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }
            }

            return true;
        }
    }
}
