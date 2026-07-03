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
        private enum FileOperation : byte
        {
            Replace = 0x00,
            Insert = 0x01,
            Delete = 0x02
        }

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
                UpdateModLoaderFilesByFileDataMapping(); // Updated Files from game directory
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

        private int GetUpdatedFileSize(FileDataEntry Entry, int Length)
        {
            int UpdatedFileSize = Length;
            foreach (var DataEntry in Entry.Data)
            {
                FileOperation Operation = (FileOperation)DataEntry.Value[0];
                if (Operation == FileOperation.Insert)
                {
                    UpdatedFileSize += DataEntry.Value.Length - 1;
                }
                else if (Operation == FileOperation.Delete)
                {
                    UpdatedFileSize -= (int)BitConverter.ToUInt32(DataEntry.Value, 1);
                }
            }

            return UpdatedFileSize;
        }

        private byte[] UpdateFileContent(int UpdatedFileSize, FileDataEntry Entry, byte[] FileContent)
        {
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

                FileOperation Operation = (FileOperation)DataEntry.Value[0];
                switch (Operation)
                {
                    case FileOperation.Replace: // Replace (fixed size)
                    {
                        var Data = DataEntry.Value.AsSpan(1);
                        Data.CopyTo(UpdatedFileContent.AsSpan(DestinationIndex));
                        SourceIndex += Data.Length;
                        DestinationIndex += Data.Length;
                        break;
                    }
                    case FileOperation.Insert: // Insert
                    {
                        var Data = DataEntry.Value.AsSpan(1);
                        Data.CopyTo(UpdatedFileContent.AsSpan(DestinationIndex));
                        DestinationIndex += Data.Length;
                        break;
                    }
                    case FileOperation.Delete: // Delete
                    {
                        int Length = (int)BitConverter.ToUInt32(DataEntry.Value, 1);
                        SourceIndex += Length;
                        break;
                    }
                    default:
                    {
                        break;
                    }
                }
            }

            int Remaining = FileContent.Length - SourceIndex;
            if (Remaining > 0)
            {
                Buffer.BlockCopy(FileContent, SourceIndex, UpdatedFileContent, DestinationIndex, Remaining);
            }

            return UpdatedFileContent;
        }

        private void UpdateModLoaderFilesByFileDataMapping()
        {
            Parallel.ForEach(GlobalFileDataMappings, Entry =>
            {
                string SanitizedFilePath = Utility.SanitizeFilePath(Entry.FilePath);
                string CurrentFile = Path.Combine(GlobalGameDataDirectoryPath, SanitizedFilePath);
                byte[] FileContent = File.ReadAllBytes(CurrentFile);
                int UpdatedFileSize = GetUpdatedFileSize(Entry, FileContent.Length);

                string DirectoryPath = Path.Combine(ArchiveFilePath, Path.GetDirectoryName(SanitizedFilePath) ?? string.Empty);
                string Destination = Path.Combine(ArchiveFilePath, SanitizedFilePath);

                Directory.CreateDirectory(DirectoryPath);
                File.WriteAllBytes(Destination, UpdateFileContent(UpdatedFileSize, Entry, FileContent));

                Logger.Instance.Log("Updating file: " + Entry.FilePath);
            });
        }
    }
}
