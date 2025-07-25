using System.IO;
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
        public static string IsPlayLauncherExecutable(string Filepath)
        {
            string CurrentPath = Path.GetFileNameWithoutExtension(Filepath);
            if (CurrentPath.StartsWith("Play"))
            {
                Logger.Instance.Log("IsPlayLauncherExecutable(): Found Launcher!");

                string Version = CurrentPath.Contains("Eastern") ? "extra1" : "base";
                CurrentPath = Path.GetDirectoryName(Filepath);
                CurrentPath = Path.Combine(CurrentPath, Version, "bin", "Settlers6.exe");

                if (File.Exists(CurrentPath) == true)
                {
                    Logger.Instance.Log("IsPlayLauncherExecutable(): Redirecting Path to " + CurrentPath);
                    Filepath = CurrentPath;
                }
            }

            return Filepath;
        }
    }
}
