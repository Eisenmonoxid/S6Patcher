using S6Patcher.Properties;
using S6Patcher.Source.Utilities;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
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
            var Entries = Archive.Entries
                .Where(Element => !Element.FullName.Contains(ArchiveFileName))
                .Where(Element => !string.IsNullOrEmpty(Element.Name));

            foreach (var Entry in Entries)
            {
                string DirectoryPath = Path.Combine(BaseDirectoryPath, Path.GetDirectoryName(Entry.FullName) ?? string.Empty);
                string Destination = Path.Combine(BaseDirectoryPath, Entry.FullName);

                try
                {
                    Directory.CreateDirectory(DirectoryPath);
                    Entry.ExtractToFile(Destination, true);
                }
                catch (Exception ex)
                {
                    Interlocked.Increment(ref Utility.ErrorCount);
                    Logger.Instance.Log(ex.ToString());
                    continue;
                }

                Logger.Instance.Log("Successfully extracted " + Destination);
            };
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
                Interlocked.Increment(ref Utility.ErrorCount);
                Logger.Instance.Log(ex.ToString());
                ShowMessage.Invoke(ex.Message);
                return;
            }

            Logger.Instance.Log("Successfully extracted " + ZipPath + " to " + GlobalDestinationDirectoryPath);
        }

        private void WriteEmbeddedFiles()
        {
            try
            {
                File.WriteAllBytes(GlobalDestinationDirectoryPath + ".zip", Resources.Modfiles);
            }
            catch (Exception ex)
            {
                Interlocked.Increment(ref Utility.ErrorCount);
                Logger.Instance.Log(ex.ToString());
                ShowMessage.Invoke(ex.Message);
                return;
            }

            Logger.Instance.Log("Written fallback ModFiles to " + GlobalDestinationDirectoryPath + ".zip");
            ExtractZipArchive(GlobalDestinationDirectoryPath + ".zip");
        }

        private async Task DownloadZipArchive(bool UseDownload)
        {
            bool Result = UseDownload && await WebHandler.Instance.DownloadZipArchiveAsync(GlobalDestinationDirectoryPath);
            if (Result)
            {
                ExtractZipArchive(GlobalDestinationDirectoryPath + ".zip");
            }
            else
            {
                WriteEmbeddedFiles();
            }
        }

        public async Task CreateModLoader(bool InstallBugfixMod, bool UseDownload)
        {
            try
            {
                WriteModLoaderFiles();
            }
            catch (Exception ex)
            {
                Interlocked.Increment(ref Utility.ErrorCount);
                Logger.Instance.Log(ex.ToString());
                ShowMessage.Invoke(ex.Message);
                return;
            }

            if (InstallBugfixMod)
            {
                await DownloadZipArchive(UseDownload);
            }
        }

        private void WriteModLoaderFiles()
        {
            if (GlobalID == execID.HE_UBISOFT || GlobalID == execID.HE_STEAM)
            {
                Directory.CreateDirectory(BaseDirectoryPath);
                Directory.CreateDirectory(Path.Combine(ArchiveFilePathBase, "shr"));
                Directory.CreateDirectory(Path.Combine(ArchiveFilePathExtra1, "shr"));
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
