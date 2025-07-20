using S6Patcher.Source.Helpers;
using System;
using System.IO.Compression;
using System.Windows.Forms;

namespace S6Patcher.Source.Patcher
{
    internal class Mod
    {
        private readonly string GlobalDownloadURL;
        private readonly string GlobalDestinationDirectoryPath;
        public Mod(string DownloadURL, string DestinationDirectoryPath) 
        {
            GlobalDownloadURL = DownloadURL ?? throw new ArgumentNullException(nameof(DownloadURL), "Download URL cannot be null.");
            GlobalDestinationDirectoryPath = DestinationDirectoryPath ?? throw new ArgumentNullException(nameof(DestinationDirectoryPath), "Destination directory path cannot be null.");
        }

        // Initial Commit
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
    }
}
