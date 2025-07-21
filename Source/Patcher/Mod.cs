using S6Patcher.Properties;
using S6Patcher.Source.Helpers;
using System;
using System.IO.Compression;
using System.Net;
using System.Windows.Forms;

namespace S6Patcher.Source.Patcher
{
    internal class Mod
    {
        private readonly Uri GlobalDownloadURL = new Uri(Resources.ModLink);
        private readonly string GlobalDestinationDirectoryPath;
        public Mod(string DestinationDirectoryPath)
        {
            GlobalDestinationDirectoryPath = DestinationDirectoryPath ?? throw new ArgumentNullException(nameof(DestinationDirectoryPath), "Destination directory path cannot be null.");
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
                    Client.OpenRead(GlobalDownloadURL);
                    Int64 Size = Convert.ToInt64(Client.ResponseHeaders["Content-Length"]);
                    if (MessageBox.Show(Size.ToString() + " Bytes", "Size", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
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
                ExtractZipArchive(GlobalDestinationDirectoryPath + ".zip");
            }
        }
    }
}
