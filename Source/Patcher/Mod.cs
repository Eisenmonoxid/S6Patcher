using S6Patcher.Properties;
using S6Patcher.Source.Helpers;
using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Windows.Forms;

namespace S6Patcher.Source.Patcher
{
    internal class Mod
    {
        private readonly Uri GlobalDownloadURL = new Uri(Resources.ModLink);
        private readonly string GlobalDestinationDirectoryPath;
        private readonly execID GlobalID = execID.NONE;
        private readonly FileStream GlobalStream = null;

        public Mod(execID ID, FileStream Stream)
        {
            GlobalID = ID;
            GlobalStream = Stream;
        }

        // Initial Commit
        // Check ShadowMapSize & ReflectionQuality in Code
        // TODO: Ask user if mod package should be downloaded and display download size beforehand (e.g. 4 KB will be downloaded)
        // Download + Extraction async to not stall gui thread
        // Use max compression for zip folder
        public bool ExtractZipArchive(string ZipPath)
        {
            try
            {
                ZipFile.ExtractToDirectory(ZipPath, GlobalDestinationDirectoryPath);
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
                CreateModloaderFolder();
                ExtractZipArchive(GlobalDestinationDirectoryPath + ".zip");
            }
        }

        private void CreateModloaderFolder()
        {
            string Path = GetModloaderPath(GlobalStream, GlobalID);
            if (Directory.Exists(Path) == false)
            {
                CreateModLoader(GlobalStream, GlobalID);
            }
        }

        public string GetModloaderPath(FileStream GlobalStream, execID GlobalID)
        {
            uint Depth = (GlobalID == execID.OV || GlobalID == execID.OV_OFFSET) ? 2U : 3U;
            string ModPath = IOFileHandler.Instance.GetRootDirectory(GlobalStream.Name, Depth);
            ModPath += (Path.DirectorySeparatorChar + "modloader");
            return ModPath;
        }

        public void CreateModLoader(FileStream GlobalStream, execID GlobalID)
        {
            string ModPath = GetModloaderPath(GlobalStream, GlobalID);
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
                ModPath += (Path.DirectorySeparatorChar + "shr");
                Directory.CreateDirectory(ModPath);
                Logger.Instance.Log("SetModLoader(): Directory created " + ModPath);
            }
            else
            {
                ModPath += (Path.DirectorySeparatorChar + "bba" + Path.DirectorySeparatorChar);
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
    }
}
