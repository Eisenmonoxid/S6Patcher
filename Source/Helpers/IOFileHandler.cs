using S6Patcher.Source.Patcher;
using System;
using System.Collections.Generic;
using System.IO;

namespace S6Patcher.Source.Helpers
{
    public sealed class IOFileHandler
    {
        private IOFileHandler() {}
        public static IOFileHandler Instance {get;} = new();

        private readonly Dictionary<Stream, uint> Streams = [];
        private uint StreamID = 0;

        public FileStream OpenFileStream(string Path, bool WithException = false)
        {
            FileStream Stream;
            try
            {
                Stream = new FileStream(Path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
            }
            catch (Exception ex)
            {
                Logger.Instance.Log(ex.ToString());
                if (WithException) {throw;};
                return null;
            }

            Streams.Add(Stream, StreamID);
            Logger.Instance.Log("Returning Stream with ID " + StreamID++.ToString() + " and Path " + Stream.Name);
            return Stream;
        }

        public void CloseStream(Stream Stream)
        {
            Stream?.Close();
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

                // Open configuration file, update Section, Key and Value
                List<string> Lines;
                try
                {
                    Lines = [.. File.ReadAllLines(CurrentPath)];
                }
                catch (Exception ex)
                {
                    Logger.Instance.Log(ex.ToString());
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
                    continue;
                }

                Logger.Instance.Log("Updated file with Section " + Section + " and Key " + Key);
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
