using Microsoft.Win32.SafeHandles;
using S6Patcher.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace S6Patcher.Source.Helpers
{
    public sealed class IOFileHandler
    {
        private static readonly IOFileHandler _instance = new IOFileHandler();
        private IOFileHandler() {}
        public static IOFileHandler Instance => _instance;

        private static readonly Dictionary<string, byte[]> Scripts = new Dictionary<string, byte[]>()
        {
            {"UserScriptLocal.lua", Resources.UserScriptLocal},
            {"EMXBinData.s6patcher", Resources.EMXBinData},
            {"UserScriptGlobal.lua", Resources.UserScriptGlobal}
        };
        public static string[] ScriptFiles => Scripts.Keys.ToArray();
        public static byte[][] ScriptResources => Scripts.Values.ToArray();
        private string MonoGlobalDocumentsPath = String.Empty;

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
                InitialDirectory = Environment.GetFolderPath(Folder),
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
            GetUserScriptDirectories().ForEach(Element =>
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
            List<string> Directories = IOFileHandler.Instance.GetUserScriptDirectories();
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

        public void CreateModLoader(FileStream GlobalStream, execID GlobalID)
        {
            char Separator = Path.DirectorySeparatorChar;
            uint DirectoryDepth = (GlobalID == execID.OV || GlobalID == execID.OV_OFFSET) ? 2U : 3U;
            string ModPath = Helpers.GetRootDirectory(GlobalStream.Name, DirectoryDepth);
            ModPath += (Separator + "modloader");

            try
            {
                Directory.CreateDirectory(ModPath);
            }
            catch (Exception ex)
            {
                Logger.Instance.Log(ex.ToString());
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Logger.Instance.Log("SetModLoader(): Directory created or already existed: " + ModPath);

            if (GlobalID == execID.HE_UBISOFT || GlobalID == execID.HE_STEAM)
            {
                ModPath += (Separator + "shr");
                Directory.CreateDirectory(ModPath);
                Logger.Instance.Log("SetModLoader(): Directory created " + ModPath);
            }
            else
            {
                ModPath += (Separator + "bba" + Separator);
                Directory.CreateDirectory(ModPath);
                try
                {
                    File.WriteAllBytes(ModPath + "mod.bba", Resources.mod);
                    Logger.Instance.Log("SetModLoader(): Written mod.bba to Path " + ModPath);
                }
                catch (Exception ex)
                {
                    Logger.Instance.Log(ex.ToString());
                    MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private string GetBackupPath(string Filepath)
        {
            return Path.Combine(Path.GetDirectoryName(Filepath), Path.GetFileNameWithoutExtension(Filepath) + ".backup");
        }

        public List<string> GetUserScriptDirectories()
        {
            // <Documents>/THE SETTLERS - Rise of an Empire/Script/UserScriptGlobal.lua && UserScriptLocal.lua are always loaded by the game when a map is started!
            // EMXBinData.s6patcher is the minified and precompiled MainMenuUserScript.lua
            string DocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (Program.IsMono)
            {
                if (MonoGlobalDocumentsPath == String.Empty)
                {
                    MessageBox.Show(Resources.MonoOptionsFile, "Select Options.ini file ...", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    OpenFileDialog ofd = CreateOFDialog("Configuration file|*.ini", Environment.SpecialFolder.MyDocuments);

                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        DirectoryInfo Info = new DirectoryInfo(ofd.FileName);
                        MonoGlobalDocumentsPath = Info.Parent.Parent.Parent.FullName;
                    }
                    else
                    {
                        Logger.Instance.Log("GetUserScriptDirectories(): User did not select a file! Aborting ...");
                        ofd.Dispose();
                        return new List<string>();
                    }
                    ofd.Dispose();
                }

                DocumentsPath = MonoGlobalDocumentsPath;
            }

            return SelectUserScriptDirectories(DocumentsPath);
        }

        private List<string> SelectUserScriptDirectories(string DocumentsPath)
        {
            return Directory.GetDirectories(DocumentsPath)
                .Where(Element => Element.Contains("Aufstieg eines") || Element.Contains("Rise of an"))
                .Select(Element => {Element = Path.Combine(DocumentsPath, Element); return Element;})
                .ToList();
        }

        [DllImport("imagehlp.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int CheckSumMappedFile(SafeMemoryMappedViewHandle BaseAddress, uint FileLength, ref uint HeaderSum, ref uint CheckSum);
        private uint UpdatePEHeaderFileCheckSum(string Path, long Size)
        {
            Logger.Instance.Log("UpdatePEHeaderFileCheckSum(): Called.");

            uint CheckSum = 0x0;
            uint HeaderSum = 0x0;
            using (MemoryMappedFile Mapping = MemoryMappedFile.CreateFromFile(Path))
            {
                using (MemoryMappedViewAccessor View = Mapping.CreateViewAccessor())
                {
                    CheckSumMappedFile(View.SafeMemoryMappedViewHandle, (uint)Size, ref HeaderSum, ref CheckSum);
                };
            };

            Logger.Instance.Log("UpdatePEHeaderFileCheckSum(): Finished successfully. New CheckSum: " + CheckSum.ToString());
            return CheckSum;
        }

        public void WritePEHeaderFileCheckSum(string Path, long Size)
        {
            uint CheckSum = UpdatePEHeaderFileCheckSum(Path, Size);
            FileStream CurrentStream = OpenFileStream(Path);
            if (CurrentStream == null)
            {
                Logger.Instance.Log(Resources.ErrorInvalidExecutable);
                MessageBox.Show(Resources.ErrorInvalidExecutable, "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            byte[] CheckSumByte = BitConverter.GetBytes(CheckSum);
            CurrentStream.Position = 0x168;
            CurrentStream.Write(CheckSumByte, 0, CheckSumByte.Length);
            
            CurrentStream.Close();
            CurrentStream.Dispose();
        }
    }
}
