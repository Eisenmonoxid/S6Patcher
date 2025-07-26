using S6Patcher.Properties;
using S6Patcher.Source.Helpers;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Windows.Forms;

namespace S6Patcher.Source.Patcher
{
    internal class Mod
    {
        private readonly Uri GlobalDownloadURL = new Uri(Resources.ModLink);
        private readonly string GlobalDestinationDirectoryPath;
        private readonly string ArchiveFilePath;
        private readonly string BaseDirectoryPath;
        private const string ArchiveFileName = "mod.bba";

        private readonly execID GlobalID = execID.NONE;
        private readonly FileStream GlobalStream = null;

        public Mod(execID ID, FileStream Stream)
        {
            GlobalID = ID;
            GlobalStream = Stream;
            GlobalDestinationDirectoryPath = GetModloaderPath();
            ArchiveFilePath = Path.Combine(GlobalDestinationDirectoryPath, "bba");
            BaseDirectoryPath = Path.Combine(GlobalDestinationDirectoryPath, "shr");
        }

        private void ExtractZipArchive(string ZipPath)
        {
            try
            {
                using (ZipArchive Archive = ZipFile.OpenRead(ZipPath))
                {
                    if (GlobalID == execID.HE_UBISOFT || GlobalID == execID.HE_STEAM)
                    {
                        var Entries = (from Entry in Archive.Entries
                                      where !Entry.FullName.Contains(ArchiveFileName)
                                      where !String.IsNullOrEmpty(Entry.Name)
                                      select Entry);

                        foreach (ZipArchiveEntry Entry in Entries)
                        {
                            string FullPath = Path.Combine(BaseDirectoryPath, Path.GetDirectoryName(Entry.FullName));
                            if (Directory.Exists(FullPath) == false)
                            {
                                Directory.CreateDirectory(FullPath);
                            }
                            Entry.ExtractToFile(Path.Combine(BaseDirectoryPath, Entry.FullName), true);
                            Logger.Instance.Log("ExtractZipArchive(): Extracted " + Path.Combine(BaseDirectoryPath, Entry.FullName));
                        }
                    }
                    else
                    {
                        Archive.GetEntry("mod.bba").ExtractToFile(Path.Combine(ArchiveFilePath, ArchiveFileName), true);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.Log(ex.ToString());
                if (!Program.IsMono)
                {
                    MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return;
            }

            Logger.Instance.Log("ExtractZipArchive(): Successfully extracted " + ZipPath + " to " + GlobalDestinationDirectoryPath);
        }

        private void DownloadZipArchive()
        {
            bool Result = WebHandler.Instance.DownloadZipArchive(GlobalDownloadURL, GlobalDestinationDirectoryPath);
            if (Result)
            {
                ExtractZipArchive(GlobalDestinationDirectoryPath + ".zip");
            }
        }

        public string GetModloaderPath()
        {
            uint Depth = (GlobalID == execID.OV || GlobalID == execID.OV_OFFSET) ? 2U : 3U;
            return IOFileHandler.Instance.GetRootDirectory(GlobalStream.Name, Depth) + Path.DirectorySeparatorChar + "modloader";
        }

        public void CreateModLoader(bool UseBugfixMod)
        {
            if (GlobalID == execID.HE_UBISOFT || GlobalID == execID.HE_STEAM)
            {
                Directory.CreateDirectory(BaseDirectoryPath);
                Logger.Instance.Log("SetModLoader(): Directory created " + BaseDirectoryPath);
            }
            else
            {
                Directory.CreateDirectory(ArchiveFilePath);
                try
                {
                    File.WriteAllBytes(Path.Combine(ArchiveFilePath, ArchiveFileName), Resources.mod); // Always do this in case the download fails or user cancels
                    Logger.Instance.Log("SetModLoader(): Written " + ArchiveFileName + " to Path " + ArchiveFilePath);
                }
                catch (Exception ex)
                {
                    Logger.Instance.Log(ex.ToString());
                    if (!Program.IsMono)
                    {
                        MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            if (UseBugfixMod)
            {
                DownloadZipArchive();
            }
        }
    }
}
