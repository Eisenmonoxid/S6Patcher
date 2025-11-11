using S6Patcher.Properties;
using S6Patcher.Source.Helpers;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace S6Patcher.Source.Patcher
{
    internal class Mod(execID GlobalID, string GlobalDestinationDirectoryPath)
    {
        private readonly string ArchiveFilePathBase = Path.Combine(GlobalDestinationDirectoryPath, "base");
        private readonly string ArchiveFilePathExtra1 = Path.Combine(GlobalDestinationDirectoryPath, "extra1");
        private readonly string BaseDirectoryPath = Path.Combine(GlobalDestinationDirectoryPath, "shr");
        private const string ArchiveFileName = "mod.bba";
        public event Action<string> ShowMessage;

        private void ExtractHistoryEditionArchiveFiles(ZipArchive Archive)
        {
            var Entries = from Entry in Archive.Entries
                          where !Entry.FullName.Contains(ArchiveFileName)
                          where !string.IsNullOrEmpty(Entry.Name)
                          select Entry;

            Parallel.ForEach(Entries, Entry =>
            {
                string FullPath = Path.Combine(BaseDirectoryPath, Path.GetDirectoryName(Entry.FullName));
                if (!Directory.Exists(FullPath))
                {
                    lock (FullPath)
                    {
                        if (!Directory.Exists(FullPath))
                        {
                            Directory.CreateDirectory(FullPath);
                        }
                    }
                }

                Entry.ExtractToFile(Path.Combine(BaseDirectoryPath, Entry.FullName), true);
                Logger.Instance.Log("Extracted " + Path.Combine(BaseDirectoryPath, Entry.FullName));
            });
        }

        private void ExtractZipArchive(string ZipPath)
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
                ShowMessage.Invoke(ex.ToString());
                return;
            }

            Logger.Instance.Log("Successfully extracted " + ZipPath + " to " + GlobalDestinationDirectoryPath);
        }

        private async Task DownloadZipArchive()
        {
            bool Result = await WebHandler.Instance.DownloadZipArchiveAsync(GlobalDestinationDirectoryPath);
            if (Result)
            {
                ExtractZipArchive(GlobalDestinationDirectoryPath + ".zip");
            }
            else
            {
                // Write File from Resources as fallback
                try
                {
                    File.WriteAllBytes(GlobalDestinationDirectoryPath + ".zip", Resources.Modfiles);
                    Logger.Instance.Log("Written fallback ModFiles to " + GlobalDestinationDirectoryPath + ".zip");
                    ExtractZipArchive(GlobalDestinationDirectoryPath + ".zip");
                }
                catch (Exception ex)
                {
                    Logger.Instance.Log(ex.ToString());
                    ShowMessage.Invoke(ex.ToString());
                }
            }
        }

        public async Task CreateModLoader(bool DownloadModfiles)
        {
            try
            {
                WriteModLoaderFiles();
            }
            catch (Exception ex)
            {
                Logger.Instance.Log(ex.ToString());
                ShowMessage.Invoke(ex.ToString());
                return;
            }

            if (DownloadModfiles)
            {
                await DownloadZipArchive();
            }
        }

        private void WriteModLoaderFiles()
        {
            if (GlobalID == execID.HE_UBISOFT || GlobalID == execID.HE_STEAM)
            {
                Directory.CreateDirectory(BaseDirectoryPath);
                Directory.CreateDirectory(Path.Combine(GlobalDestinationDirectoryPath, "base", "shr"));
                Directory.CreateDirectory(Path.Combine(GlobalDestinationDirectoryPath, "extra1", "shr"));
                Logger.Instance.Log("Directories created.");
            }
            else
            {
                Directory.CreateDirectory(ArchiveFilePathBase);
                Directory.CreateDirectory(ArchiveFilePathExtra1);
                // Always do this in case the download fails or user cancels
                File.WriteAllBytes(Path.Combine(ArchiveFilePathBase, ArchiveFileName), Resources.mod);
                File.WriteAllBytes(Path.Combine(ArchiveFilePathExtra1, ArchiveFileName), Resources.mod);
                Logger.Instance.Log("Written " + ArchiveFileName + " to Paths "
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
