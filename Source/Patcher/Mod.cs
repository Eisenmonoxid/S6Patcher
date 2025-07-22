using S6Patcher.Properties;
using S6Patcher.Source.Helpers;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
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

        public bool ExtractZipArchive(string ZipPath)
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
                        }
                    }
                    else
                    {
                        Archive.GetEntry("mod.bba").ExtractToFile(Path.Combine(ArchiveFilePath, ArchiveFileName), true);
                    }
                }

                File.Delete(ZipPath);
            }
            catch (Exception ex)
            {
                Logger.Instance.Log(ex.ToString());
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            Logger.Instance.Log("ExtractZipArchive(): Successfully extracted " + ZipPath + " to " + GlobalDestinationDirectoryPath);
            return true;
        }

        public void DownloadZipArchive()
        {
            try
            {
                using (WebClient Client = new WebClient())
                {
                    // SecurityPoint TLS
                    Client.OpenRead(GlobalDownloadURL);
                    Int64 Size = Convert.ToInt64(Client.ResponseHeaders["Content-Length"]);
                    if (MessageBox.Show((Size / 1024).ToString() + " KB", "Size", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    {
                        return;
                    }

                    Client.DownloadFileCompleted += DownloadZipFileCompleted;
                    Client.DownloadFileAsync(GlobalDownloadURL, GlobalDestinationDirectoryPath + ".zip");
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.Log(ex.ToString());
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DownloadZipFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                Logger.Instance.Log(e.Error.ToString());
                MessageBox.Show(e.Error.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                Logger.Instance.Log("Download completed successfully.");
                CreateModLoader();
                ExtractZipArchive(GlobalDestinationDirectoryPath + ".zip");
            }
        }

        public string GetModloaderPath()
        {
            uint Depth = (GlobalID == execID.OV || GlobalID == execID.OV_OFFSET) ? 2U : 3U;
            return IOFileHandler.Instance.GetRootDirectory(GlobalStream.Name, Depth) + Path.DirectorySeparatorChar + "modloader";
        }

        public void CreateModLoader()
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
                    File.WriteAllBytes(Path.Combine(ArchiveFilePath, ArchiveFileName), Resources.mod);
                    Logger.Instance.Log("SetModLoader(): Written " + ArchiveFileName + " to Path " + ArchiveFilePath);
                }
                catch (Exception ex)
                {
                    Logger.Instance.Log(ex.ToString());
                    MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
