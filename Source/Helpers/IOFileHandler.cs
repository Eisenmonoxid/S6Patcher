using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using static S6Patcher.Helpers;

namespace S6Patcher
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

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern long WritePrivateProfileString(string Section, string Key, string Value, string FilePath);
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetPrivateProfileString(string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);

        public FileStream OpenFileStream(string Path)
        {
            FileStream Stream;
            try
            {
                Stream = new FileStream(Path, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (Exception ex)
            {
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
        private void DeleteUserConfiguration()
        {
            string[] ScriptFiles = {"UserScriptLocal.lua", "EMXBinData.s6patcher"};
            List<string> Directories = GetUserScriptDirectories();
            foreach (string Element in Directories)
            {
                DeleteSectionFromOptions(Path.Combine(Element, "Config"));
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
        private void DeleteSectionFromOptions(string CurrentPath)
        {
            CurrentPath = Path.Combine(CurrentPath, "Options.ini");
            if (!File.Exists(CurrentPath))
            {
                return;
            }

            StringBuilder Builder = new StringBuilder(255);
            GetPrivateProfileString("S6Patcher", "ExtendedKnightSelection", "", Builder, 255, CurrentPath);
            if (Builder.ToString().Length <= 0)
            {
                return; // Section Entry does not exist
            }

            // Open configuration file, remove Section
            try
            {
                WritePrivateProfileString("S6Patcher", null, null, CurrentPath);
            }
            catch (Exception)
            {
                return;
            }
        }
        public bool RestoreBackup(ref FileStream Stream)
        {
            DeleteUserConfiguration(); // Delete Userscript & Config Section from Documents folder

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
                MessageBox.Show("RestoreBackup:\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }

            return true;
        }
        public bool UpdateEntryInOptionsFile(string Section, string Key, bool Entry)
        {
            string Name = "Options.ini";
            List<string> Directories = GetUserScriptDirectories();
            foreach (string Element in Directories)
            {
                string CurrentPath = Path.Combine(Path.Combine(Element, "Config"), Name);
                if (!File.Exists(CurrentPath))
                {
                    continue;
                }

                // Open configuration file, update Section, Key and Value
                try
                {
                    WritePrivateProfileString(Section, Key, (Entry == true) ? 1.ToString() : 0.ToString(), CurrentPath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("UpdateEntryInOptionsFile:\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }
            }

            return true;
        }
    }
}
