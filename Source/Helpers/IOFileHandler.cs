using S6Patcher.Source.Patcher;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace S6Patcher.Source.Helpers
{
    public sealed class IOFileHandler
    {
        private IOFileHandler() {}
        public static IOFileHandler Instance {get;} = new();
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
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return null;
            }

            Logger.Instance.Log("OpenFileStream(): Returning Stream: " + Stream.Name);
            return Stream;
        }

        public void CloseStream(Stream Stream)
        {
            if (Stream != null)
            {
                Stream.Close();
                Stream.Dispose();
                Logger.Instance.Log("CloseStream(): Stream has been closed.");
            }
        }

        public OpenFileDialog CreateOFDialog(string Filter, Environment.SpecialFolder Folder) => new()
        {
            CheckFileExists = true,
            ShowHelp = false,
            CheckPathExists = true,
            DereferenceLinks = true,
            InitialDirectory = (InitialDirectory == String.Empty || Folder == Environment.SpecialFolder.MyDocuments) ? 
                Environment.GetFolderPath(Folder) : InitialDirectory,
            Multiselect = false,
            ShowReadOnly = false,
            Filter = Filter
        };

        public string IsPlayLauncherExecutable(string Filepath)
        {
            string CurrentPath = Path.GetFileNameWithoutExtension(Filepath);
            if (!CurrentPath.StartsWith("Play")) {return Filepath;};

            Logger.Instance.Log("IsPlayLauncherExecutable(): Found Launcher!");

            string Version = CurrentPath.Contains("Eastern") ? "extra1" : "base";
            string NewPath = Path.Combine(Path.GetDirectoryName(Filepath), Version, "bin", "Settlers6.exe");

            Logger.Instance.Log("IsPlayLauncherExecutable(): Trying to redirect Filepath to " + NewPath);
            return File.Exists(NewPath) ? NewPath : Filepath;
        }

        public void UpdateEntryInOptionsFile(string Section, string Key, uint Entry)
        {
            Logger.Instance.Log("UpdateEntryInOptionsFile(): Called.");

            string Name = "Options.ini";
            foreach (string Element in UserScriptHandler.GetUserScriptDirectories())
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
                    Lines = [.. File.ReadAllLines(CurrentPath)];
                }
                catch (Exception ex)
                {
                    Logger.Instance.Log(ex.ToString());
                    MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    continue;
                }

                Lines.RemoveAll(Line => Line.Contains(Key));
                if (Lines.Contains(Section) == false)
                {
                    Lines.Add(Section);
                }
                Lines.Insert(Lines.IndexOf(Section) + 1, Key + "=" + Entry.ToString());

                try
                {
                    File.WriteAllLines(CurrentPath, Lines);
                }
                catch (Exception ex)
                {
                    Logger.Instance.Log(ex.ToString());
                    MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    continue;
                }

                Logger.Instance.Log("UpdateEntryInOptionsFile(): Updated file with Section " + Section + " and Key " + Key);
            }
        }

        public void CreateUserScriptFiles()
        {
            foreach (string Element in UserScriptHandler.GetUserScriptDirectories())
            {
                string CurrentPath = Path.Combine(Element, "Script");
                try
                {
                    Directory.CreateDirectory(CurrentPath);
                    for (uint i = 0; i < UserScriptHandler.ScriptFiles.Length; i++)
                    {
                        File.WriteAllBytes(Path.Combine(CurrentPath, UserScriptHandler.ScriptFiles[i]), 
                            UserScriptHandler.ScriptResources[i]);
                        Logger.Instance.Log("SetLuaScriptBugFixes(): Finished writing ScriptFile named " + 
                            UserScriptHandler.ScriptFiles[i] + " to " + CurrentPath);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Instance.Log(ex.ToString());
                    MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public string GetRootDirectory(string Filepath, uint Depth)
        {
            Logger.Instance.Log("GetRootPathFromFile(): Called with Depth: " + Depth.ToString() + " and Input: " + Filepath);

            DirectoryInfo Info = new(Path.GetDirectoryName(Filepath));
            for (; Depth > 0; Depth--)
            {
                Info = Info.Parent;
            }

            Logger.Instance.Log("GetRootPathFromFile(): Returning Path: " + Info.FullName);
            return Info.FullName;
        }
    }
}
