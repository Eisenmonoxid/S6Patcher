using System;
using System.Collections.Generic;
using System.IO;
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
        public FileStream OpenFileStream(string Path, execID ID)
        {
            FileStream Stream;
            try
            {
                Stream = new FileStream(Path, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
        public void RemoveUserScriptFiles()
        {
            string[] ScriptFiles = {"UserScriptLocal.lua", "EMXBinData.s6patcher"};
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
