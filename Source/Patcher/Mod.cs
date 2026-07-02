using S6Patcher.Properties;
using S6Patcher.Source.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S6Patcher.Source.Patcher
{
    internal class Mod(execID GlobalID, string GlobalDestinationDirectoryPath, string GlobalGameDataDirectoryPath, List<FileDataEntry> GlobalFileDataMappings)
    {
        private readonly string ArchiveFilePath = Path.Combine(GlobalDestinationDirectoryPath, "shr");
        private readonly string ArchiveFilePathBase = Path.Combine(GlobalDestinationDirectoryPath, "base");
        private readonly string ArchiveFilePathExtra1 = Path.Combine(GlobalDestinationDirectoryPath, "extra1");
        private const string ArchiveFileName = "mod.bba";
        private const string ArchiveFileNameBase = "base.bba";
        private const string ArchiveFileNameExtra1 = "extra1.bba";
        public event Action<string> ShowMessage;

        private void ExtractHistoryEditionArchiveFiles(ZipArchive Archive)
        {
            var Entries = Archive.Entries
                .Where(Element => !Element.FullName.Contains(ArchiveFileNameBase) && !Element.FullName.Contains(ArchiveFileNameExtra1))
                .Where(Element => !string.IsNullOrEmpty(Element.Name));

            foreach (var Entry in Entries)
            {
                string DirectoryPath = Path.Combine(GlobalDestinationDirectoryPath, Path.GetDirectoryName(Entry.FullName) ?? string.Empty);
                string Destination = Path.Combine(GlobalDestinationDirectoryPath, Entry.FullName);

                try
                {
                    Directory.CreateDirectory(DirectoryPath);
                    Entry.ExtractToFile(Destination, true);
                }
                catch (Exception ex)
                {
                    ErrorTracking.Increment();
                    Logger.Instance.Log(ex.ToString());
                    continue;
                }

                Logger.Instance.Log("Successfully extracted " + Destination);
            };
        }

        private void ExtractZipArchive(MemoryStream ZipArchiveStream)
        {
            try
            {
                using ZipArchive Archive = new(ZipArchiveStream, ZipArchiveMode.Read, true);
                if (GlobalID == execID.HE_UBISOFT || GlobalID == execID.HE_STEAM)
                {
                    ExtractHistoryEditionArchiveFiles(Archive);
                }
                else
                {
                    ZipArchiveEntry Entry = Archive.GetEntry(ArchiveFileNameBase);
                    Entry.ExtractToFile(Path.Combine(ArchiveFilePathBase, ArchiveFileName), true);
                    Entry = Archive.GetEntry(ArchiveFileNameExtra1);
                    Entry.ExtractToFile(Path.Combine(ArchiveFilePathExtra1, ArchiveFileName), true);
                }
            }
            catch (Exception ex)
            {
                ErrorTracking.Increment();
                Logger.Instance.Log(ex.ToString());
                ShowMessage.Invoke(ex.Message);
                return;
            }

            Logger.Instance.Log("Successfully extracted " + ZipArchiveStream.Length + " bytes to " + GlobalDestinationDirectoryPath);
        }

        private async Task DownloadZipArchive(bool UseDownload)
        {
            MemoryStream Result;
            if (UseDownload)
            {
                Result = await WebHandler.Instance.DownloadZipArchiveAsync() ?? new(Resources.Modfiles);
            }
            else
            {
                Result = new(Resources.Modfiles);
            }

            ExtractZipArchive(Result);
            Result.Dispose();
        }

        public async Task CreateModLoader(bool InstallBugfixMod, bool UseDownload)
        {
            try
            {
                WriteModLoaderFiles();
            }
            catch (Exception ex)
            {
                ErrorTracking.Increment();
                Logger.Instance.Log(ex.ToString());
                ShowMessage.Invoke(ex.Message);
                return;
            }

            if (InstallBugfixMod)
            {
                await DownloadZipArchive(UseDownload); // Files from ZipArchive
                await UpdateModLoaderFilesByFileDataMapping(); // Updated Files from game directory
            }
        }

        private void WriteModLoaderFiles()
        {
            if (Directory.Exists(GlobalDestinationDirectoryPath))
            {
                Directory.Delete(GlobalDestinationDirectoryPath, true);
            }

            Directory.CreateDirectory(GlobalDestinationDirectoryPath);

            if (GlobalID == execID.HE_UBISOFT || GlobalID == execID.HE_STEAM)
            {
                Directory.CreateDirectory(ArchiveFilePath);
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
                Logger.Instance.Log("Written " + ArchiveFileName + " to Paths " + ArchiveFilePathBase + " and " + ArchiveFilePathExtra1);
            }
        }

        public async Task UpdateModLoaderFilesByFileDataMapping()
        {
            foreach (var Entry in GlobalFileDataMappings)
            {
                string SanitizedFilePath = Utility.SanitizeFilePath(Entry.FilePath);
                string CurrentFile = Path.Combine(GlobalGameDataDirectoryPath, SanitizedFilePath);

                if (Entry.IsLineNumber)
                {
                    string[] Lines = File.ReadAllLines(CurrentFile);
                    foreach (var DataEntry in Entry.Data)
                    {
                        Lines[DataEntry.Key] = Encoding.UTF8.GetString(DataEntry.Value);
                    }

                    string DirectoryPath = Path.Combine(ArchiveFilePath, Path.GetDirectoryName(SanitizedFilePath) ?? string.Empty);
                    string Destination = Path.Combine(ArchiveFilePath, SanitizedFilePath);

                    try
                    {
                        Directory.CreateDirectory(DirectoryPath);
                        File.WriteAllLines(Destination, Lines);
                    }
                    catch (Exception ex)
                    {
                        ErrorTracking.Increment();
                        Logger.Instance.Log(ex.ToString());
                        continue;
                    }
                }        

                // TODO: Read file entry from base game, update with data, write to mod loader directory
                Logger.Instance.Log("Updating file: " + Entry.FilePath + " with data from mapping.");
            }
        }
    }
}
