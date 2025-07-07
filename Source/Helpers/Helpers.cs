using IWshRuntimeLibrary;
using System;
using System.IO;
using System.Windows.Forms;

namespace S6Patcher.Source.Helpers
{
    public enum execID
    {
        NONE = 0,
        OV = 1,
        OV_OFFSET = 2,
        HE_UBISOFT = 3,
        HE_STEAM = 4,
        ED = 5
    };

    public static class Helpers
    {
        public static void CreateDesktopShortcut(string Filepath, string LinkName, string Arguments)
        {
            string Link = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            Link += @"\" + LinkName + " - Patched.lnk";

            try
            {
                WshShell Shell = new WshShell();
                IWshShortcut PatchedShortcut = (IWshShortcut)Shell.CreateShortcut(Link);
                PatchedShortcut.Description = "Launches patched " + LinkName;
                PatchedShortcut.TargetPath = Filepath;
                PatchedShortcut.Arguments = Arguments;
                PatchedShortcut.Save();
            }
            catch (Exception ex)
            {
                Logger.Instance.Log(ex.ToString());
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Logger.Instance.Log("CreateDesktopShortcut(): Creating shortcut successful! Link: " + Link);
        }

        public static string GetRootDirectory(string Filepath, uint Depth)
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

        public static string IsPlayLauncherExecutable(string Filepath)
        {
            string CurrentPath = Path.GetFileNameWithoutExtension(Filepath);
            if (CurrentPath.StartsWith("Play"))
            {
                Logger.Instance.Log("IsPlayLauncherExecutable(): Found Launcher!");

                CurrentPath = Path.GetDirectoryName(Filepath);
                CurrentPath = Path.Combine(CurrentPath, CurrentPath.Contains("Eastern") ? "extra1" : "base", "bin", "Settlers6.exe");

                if (System.IO.File.Exists(CurrentPath) == true)
                {
                    Logger.Instance.Log("IsPlayLauncherExecutable(): Redirecting Path to " + CurrentPath);
                    Filepath = CurrentPath;
                }
            }

            return Filepath;
        }
    }
}
