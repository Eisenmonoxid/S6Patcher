using S6Patcher.Source.Patcher;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace S6Patcher.Source.Helpers
{
    public sealed class IOFileHandler
    {
        private IOFileHandler() {}
        public static IOFileHandler Instance {get;} = new();

        private readonly Dictionary<Stream, uint> Streams = [];
        private readonly Dictionary<string, List<string>> OpenOptionFiles = [];
        private uint StreamID = 0;

        public FileStream OpenFileStream(string FilePath, bool WithException = false)
        {
            FileStream Stream;
            try
            {
                Stream = new FileStream(FilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
            }
            catch (Exception ex)
            {
                Logger.Instance.Log(ex.ToString());
                if (WithException) {throw;};
                return null;
            }

            Streams.Add(Stream, StreamID);
            Logger.Instance.Log("Returning Stream with ID " + StreamID++.ToString() + " and Name " + Path.GetFileName(Stream.Name));
            return Stream;
        }

        public void CloseStream(Stream Stream)
        {
            Stream?.Dispose();
            if (Streams.TryGetValue(Stream, out uint Key))
            {
                Streams.Remove(Stream);
                Logger.Instance.Log("Stream with ID " + Key.ToString() + " has been closed.");
            }
        }

        public string IsPlayLauncherExecutable(string Filepath)
        {
            string CurrentPath = Path.GetFileNameWithoutExtension(Filepath);
            if (!CurrentPath.StartsWith("Play")) {return Filepath;};

            Logger.Instance.Log("Found Launcher!");

            string Version = CurrentPath.Contains("Eastern") ? "extra1" : "base";
            string NewPath = Path.Combine(Path.GetDirectoryName(Filepath), Version, "bin", "Settlers6.exe");

            Logger.Instance.Log("Trying to redirect Filepath to " + NewPath);
            return File.Exists(NewPath) ? NewPath : Filepath;
        }

        public void UpdateEntryInOptionsFile(string Section, string Key, uint Entry)
        {
            string Name = "Options.ini";
            foreach (string Element in UserScriptHandler.Instance.GetUserScriptDirectories())
            {
                string CurrentPath = Path.Combine(Element, "Config", Name);
                if (File.Exists(CurrentPath) == false)
                {
                    Logger.Instance.Log("Skipping! Did not find Options file " + CurrentPath);
                    continue;
                }

                List<string> Lines = OpenOptionFiles.GetValueOrDefault(CurrentPath);
                // Do not open file everytime, read once, modify lines, write once
                if (Lines == default)
                {
                    // Open configuration file, update Section, Key and Value
                    Logger.Instance.Log("No entry found for Options.ini file " + CurrentPath + "! Trying to open ...");
                    try
                    {
                        Lines = [.. File.ReadAllLines(CurrentPath)];
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.Log(ex.ToString());
                        continue;
                    }

                    OpenOptionFiles.Add(CurrentPath, Lines);
                }

                Lines.RemoveAll(Line => Line.Contains(Key));
                if (!Lines.Contains(Section))
                {
                    Lines.Add(Section);
                }
                Lines.Insert(Lines.IndexOf(Section) + 1, Key + "=" + Entry.ToString());

                Logger.Instance.Log("Updated Section " + Section + " - Key " + Key);
            }
        }

        public void WriteBackToOptionsFiles()
        {
            Parallel.ForEach(OpenOptionFiles, Element =>
            {
                try
                {
                    File.WriteAllLines(Element.Key, Element.Value);
                    OpenOptionFiles.Remove(Element.Key);
                    Logger.Instance.Log("Updated Options.ini file " + Element.Key);
                }
                catch (Exception ex)
                {
                    Logger.Instance.Log(ex.ToString());
                }
            });

            if (OpenOptionFiles.Count != 0)
            {
                Logger.Instance.Log("Could not write back to all Options.ini files!");
            }
        }

        public string GetModLoaderDirectory(execID ID, string Filepath)
        {
            uint Depth = ID == execID.OV ? 2U : 3U;
            return GetRootDirectory(Filepath, Depth) + Path.DirectorySeparatorChar + "modloader";
        }

        private string GetRootDirectory(string Filepath, uint Depth)
        {
            Logger.Instance.Log("Called with Depth: " + Depth.ToString() + " and Input: " + Filepath);

            DirectoryInfo Info = new(Path.GetDirectoryName(Filepath));
            for (; Depth > 0; Depth--)
            {
                Info = Info.Parent;
            }

            Logger.Instance.Log("Returning Path: " + Info.FullName);
            return Info.FullName;
        }
    }
}
