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

        private async Task InstallZIPArchive(bool UseDownload)
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
                await InstallZIPArchive(UseDownload); // Files from ZipArchive
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
                byte[] FileContent = await File.ReadAllBytesAsync(CurrentFile);
                int UpdatedFileSize = FileContent.Length;

                foreach (var DataEntry in Entry.Data)
                {
                    byte Operation = DataEntry.Value[0];
                    if (Operation == 0x01) // Insert
                    {
                        UpdatedFileSize += DataEntry.Value.Length - 1;
                    }
                    else if (Operation == 0x02) // Delete
                    {
                        UpdatedFileSize -= (int)BitConverter.ToUInt32(DataEntry.Value, 1);
                    }
                }

                byte[] UpdatedFileContent = new byte[UpdatedFileSize];
                int SourceIndex = 0;
                int DestinationIndex = 0;

                foreach (var DataEntry in Entry.Data.OrderBy(E => E.Key))
                {
                    int Offset = (int)DataEntry.Key;
                    int CopyLength = Offset - SourceIndex;
                    if (CopyLength > 0)
                    {
                        Buffer.BlockCopy(FileContent, SourceIndex, UpdatedFileContent, DestinationIndex, CopyLength);
                        SourceIndex += CopyLength;
                        DestinationIndex += CopyLength;
                    }

                    byte Operation = DataEntry.Value[0];
                    switch (Operation)
                    {
                        case 0x00: // Replace (fixed size)
                        {
                            var Data = DataEntry.Value.AsSpan(1);
                            Data.CopyTo(UpdatedFileContent.AsSpan(DestinationIndex));
                            SourceIndex += Data.Length;
                            DestinationIndex += Data.Length;
                            break;
                        }
                        case 0x01: // Insert
                        {
                            var Data = DataEntry.Value.AsSpan(1);
                            Data.CopyTo(UpdatedFileContent.AsSpan(DestinationIndex));
                            DestinationIndex += Data.Length;
                            break;
                        }
                        case 0x02: // Delete
                        {
                            int Length = (int)BitConverter.ToUInt32(DataEntry.Value, 1);
                            SourceIndex += Length;
                            break;
                        }
                        default:
                        {
                            ErrorTracking.Increment();
                            Logger.Instance.Log($"Unknown op in {CurrentFile}");
                            break;
                        }
                    }
                }

                int Remaining = FileContent.Length - SourceIndex;
                if (Remaining > 0)
                {
                    Buffer.BlockCopy(FileContent, SourceIndex, UpdatedFileContent, DestinationIndex, Remaining);
                }

                string DirectoryPath = Path.Combine(ArchiveFilePath, Path.GetDirectoryName(SanitizedFilePath) ?? string.Empty);
                string Destination = Path.Combine(ArchiveFilePath, SanitizedFilePath);

                Directory.CreateDirectory(DirectoryPath);
                File.WriteAllBytes(Destination, UpdatedFileContent);

                Logger.Instance.Log("Updating file: " + Entry.FilePath);
            }
        }
    }
}
