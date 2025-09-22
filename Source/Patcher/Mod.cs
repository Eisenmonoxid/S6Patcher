using S6Patcher.Properties;
using S6Patcher.Source.Helpers;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;

namespace S6Patcher.Source.Patcher
{
    internal class Mod(execID GlobalID, string GlobalDestinationDirectoryPath)
    {
        private readonly string ArchiveFilePathBase = Path.Combine(GlobalDestinationDirectoryPath, "base");
        private readonly string ArchiveFilePathExtra1 = Path.Combine(GlobalDestinationDirectoryPath, "extra1");
        private readonly string BaseDirectoryPath = Path.Combine(GlobalDestinationDirectoryPath, "shr");
        private const string ArchiveFileName = "mod.bba";

        public event Action<bool> Finished;
        public event Action<string> ShowErrorMessage;

        private void ExtractHistoryEditionArchiveFiles(ZipArchive Archive)
        {
            var Entries = from Entry in Archive.Entries
                          where !Entry.FullName.Contains(ArchiveFileName)
                          where !string.IsNullOrEmpty(Entry.Name)
                          select Entry;

            foreach (ZipArchiveEntry Entry in Entries)
            {
                string FullPath = Path.Combine(BaseDirectoryPath, Path.GetDirectoryName(Entry.FullName));
                if (Directory.Exists(FullPath) == false)
                {
                    Directory.CreateDirectory(FullPath);
                }

                Entry.ExtractToFile(Path.Combine(BaseDirectoryPath, Entry.FullName), true);
                Logger.Instance.Log("ExtractHistoryEditionArchiveFiles(): Extracted " + Path.Combine(BaseDirectoryPath, Entry.FullName));
            }
        }

        private bool ExtractZipArchive(string ZipPath)
        {
            try
            {
                using ZipArchive Archive = ZipFile.OpenRead(ZipPath);
                if (GlobalID == execID.HE_UBISOFT || GlobalID == execID.HE_STEAM)
                {
                    ExtractHistoryEditionArchiveFiles(Archive);
                }
                else
                {
                    ZipArchiveEntry Entry = Archive.GetEntry(ArchiveFileName);
                    Entry.ExtractToFile(Path.Combine(ArchiveFilePathBase, ArchiveFileName), true);
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.Log(ex.ToString());
                ShowErrorMessage?.Invoke(ex.ToString());
                return false;
            }

            Logger.Instance.Log("ExtractZipArchive(): Successfully extracted " + ZipPath + " to " + GlobalDestinationDirectoryPath);
            return true;
        }

        private async void DownloadZipArchive()
        {
            bool Result = await WebHandler.Instance.DownloadZipArchiveAsync(GlobalDestinationDirectoryPath);
            if (Result)
            {
                Result = ExtractZipArchive(GlobalDestinationDirectoryPath + ".zip");
            }
            else
            {
                // Write File from Resources as fallback
                try
                {
                    File.WriteAllBytes(GlobalDestinationDirectoryPath + ".zip", Resources.Modfiles);
                    Logger.Instance.Log("DownloadZipArchive(): Written fallback Modfiles.zip to " + GlobalDestinationDirectoryPath + ".zip");
                    Result = ExtractZipArchive(GlobalDestinationDirectoryPath + ".zip");
                }
                catch (Exception ex)
                {
                    Logger.Instance.Log(ex.ToString());
                    ShowErrorMessage?.Invoke(ex.ToString());
                    Result = false;
                }
            }

            Finished.Invoke(Result);
        }

        public void CreateModLoader(bool DownloadModfiles)
        {
            try
            {
                WriteModLoaderFiles();
            }
            catch (Exception ex)
            {
                Logger.Instance.Log(ex.ToString());
                ShowErrorMessage?.Invoke(ex.ToString());
                Finished.Invoke(false);
                return;
            }

            if (DownloadModfiles)
            {
                Thread Context = new(DownloadZipArchive)
                {
                    IsBackground = true
                };
                Context.Start();
            }
            else
            {
                Finished.Invoke(true);
            }
        }

        private void WriteModLoaderFiles()
        {
            if (GlobalID == execID.HE_UBISOFT || GlobalID == execID.HE_STEAM)
            {
                Directory.CreateDirectory(BaseDirectoryPath);
                Directory.CreateDirectory(Path.Combine(GlobalDestinationDirectoryPath, "base", "shr"));
                Directory.CreateDirectory(Path.Combine(GlobalDestinationDirectoryPath, "extra1", "shr"));
                Logger.Instance.Log("WriteModLoaderFiles(): Directories created.");
            }
            else
            {
                Directory.CreateDirectory(ArchiveFilePathBase);
                Directory.CreateDirectory(ArchiveFilePathExtra1);
                // Always do this in case the download fails or user cancels
                File.WriteAllBytes(Path.Combine(ArchiveFilePathBase, ArchiveFileName), Resources.mod);
                File.WriteAllBytes(Path.Combine(ArchiveFilePathExtra1, ArchiveFileName), Resources.mod);
                Logger.Instance.Log("WriteModLoaderFiles(): Written " + ArchiveFileName + " to Paths "
                    + ArchiveFilePathBase + " and " + ArchiveFilePathExtra1);

                string OldModLoaderPath = Path.Combine(GlobalDestinationDirectoryPath, "bba");
                if (Directory.Exists(OldModLoaderPath))
                {
                    Directory.Delete(OldModLoaderPath, true);
                }
            }
        }
    }
}
