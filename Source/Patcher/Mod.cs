using S6Patcher.Properties;
using S6Patcher.Source.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using System.IO.Hashing;
using S6Patcher.Source.Archive;

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

        private struct ModLoaderFile
        {
            public string Name;
            public byte[] Data;
            public FileDataEntry Entry;
        }

        private readonly string ArchiveFilePath = Path.Combine(GlobalDestinationDirectoryPath, "shr");
        private readonly string ArchiveFilePathBase = Path.Combine(GlobalDestinationDirectoryPath, "base");
        private readonly string ArchiveFilePathExtra1 = Path.Combine(GlobalDestinationDirectoryPath, "extra1");
        private const string ArchiveFileName = "mod.bba";
        private const string ArchiveFileNameBase = "base.bba";
        private const string ArchiveFileNameExtra1 = "extra1.bba";
        public event Action<string> ShowMessage;

        private async Task ExtractHistoryEditionArchiveFiles(ZipArchive Archive)
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
                    await Entry.ExtractToFileAsync(Destination, true);
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

        private async Task ExtractZipArchive(MemoryStream ZipArchiveStream)
        {
            try
            {
                using ZipArchive Archive = new(ZipArchiveStream, ZipArchiveMode.Read, true);
                if (GlobalID == execID.HE_UBISOFT || GlobalID == execID.HE_STEAM)
                {
                    await ExtractHistoryEditionArchiveFiles(Archive);
                }
                else
                {
                    ZipArchiveEntry Entry = Archive.GetEntry(ArchiveFileNameBase);
                    await Entry.ExtractToFileAsync(Path.Combine(ArchiveFilePathBase, ArchiveFileName), true);
                    Entry = Archive.GetEntry(ArchiveFileNameExtra1);
                    await Entry.ExtractToFileAsync(Path.Combine(ArchiveFilePathExtra1, ArchiveFileName), true);
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

            await ExtractZipArchive(Result);
            Result.Dispose();
        }

        public async Task Create(bool InstallBugfixMod, bool UseDownload)
        {
            try
            {
                await WriteModLoaderFiles();
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

                if (GlobalID == execID.OV)
                {
                    await GetModLoaderFilesFromArchives(); // Updated Files from game archive files 
                }
                else
                {
                   await UpdateModLoaderFilesByFileDataMapping(); // Updated Files from game directory 
                }
            }
        }

        private async Task WriteModLoaderFiles()
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
                await File.WriteAllBytesAsync(Path.Combine(ArchiveFilePathBase, ArchiveFileName), Resources.mod);
                await File.WriteAllBytesAsync(Path.Combine(ArchiveFilePathExtra1, ArchiveFileName), Resources.mod);
                Logger.Instance.Log("Written " + ArchiveFileName + " to Paths " + ArchiveFilePathBase + " and " + ArchiveFilePathExtra1);
            }
        }

        private void UpdateFileContent(FileDataEntry Entry, List<byte> FileContent)
        {
            foreach (var DataEntry in Entry.Data.OrderByDescending(E => E.Key))
            {
                FileOperation Operation = (FileOperation)DataEntry.Value[0];
                switch (Operation)
                {
                    case FileOperation.Replace:
                    {
                        var Data = DataEntry.Value.Skip(1);
                        Utility.ReplaceRange(FileContent, (int)DataEntry.Key, DataEntry.Value.Length - 1, Data);
                        break;
                    }
                    case FileOperation.Insert:
                    {
                        var Data = DataEntry.Value.Skip(1);
                        FileContent.InsertRange((int)DataEntry.Key, Data);
                        break;
                    }
                    case FileOperation.Delete:
                    {
                        int Length = (int)BitConverter.ToUInt32(DataEntry.Value, 1);
                        FileContent.RemoveRange((int)DataEntry.Key, Length);
                        break;
                    }
                }
            }
        }

        private async Task GetModLoaderFilesFromArchives()
        {
            Dictionary<string, List<ModLoaderFile>> ArchiveFilesToParse = [];
            foreach (var Element in GlobalFileDataMappings)
            {
                ModLoaderFile CurrentFile = new()
                {
                    Name = Element.FilePath,
                    Entry = Element
                };

                if (ArchiveFilesToParse.TryGetValue(Element.BBArchiveName, out List<ModLoaderFile> Value))
                {
                    Value.Add(CurrentFile);
                }
                else
                {
                    List<ModLoaderFile> Files = [CurrentFile];
                    ArchiveFilesToParse.Add(Element.BBArchiveName, Files);
                }
            }

            foreach (var Element in ArchiveFilesToParse)
            {
                FileStream ArchiveFileStream;
                try
                {
                    ArchiveFileStream = new(Path.Combine(GlobalGameDataDirectoryPath, Element.Key), FileMode.Open, FileAccess.Read, FileShare.None);
                }
                catch (Exception ex)
                {
                    ErrorTracking.Increment();
                    Logger.Instance.Log(ex.ToString());
                    continue;
                }

                BBAArchiveFile ArchiveFile = new(ArchiveFileStream, true);
                for (int i = 0; i < Element.Value.Count; i++)
                {
                    ModLoaderFile MLF = Element.Value[i];  
                    MLF.Data = ArchiveFile.GetFileDataByDataEntryName(MLF.Name);
                    Element.Value[i] = MLF;
                }

                ArchiveFileStream.Dispose();
            }

            foreach (var Element in ArchiveFilesToParse)
            {
                foreach (var CurrentFile in Element.Value)
                {
                    string SanitizedFilePath = Utility.SanitizeFilePath(CurrentFile.Entry.FilePath);

                    Crc32 CRC = new();
                    CRC.Append(CurrentFile.Data);
                    
                    if (CRC.GetCurrentHashAsUInt32() != CurrentFile.Entry.OriginalFileCRC)
                    {
                        ErrorTracking.Increment();
                        Logger.Instance.Log($"File Hash for file {CurrentFile.Name} not equivalent! Skipping ...");
                        continue;
                    }

                    List<byte> FileContentAsList = [.. CurrentFile.Data];
                    UpdateFileContent(CurrentFile.Entry, FileContentAsList);

                    string DirectoryPath = Path.Combine(ArchiveFilePath, Path.GetDirectoryName(SanitizedFilePath) ?? string.Empty);
                    string Destination = Path.Combine(ArchiveFilePath, SanitizedFilePath);

                    Directory.CreateDirectory(DirectoryPath);
                    await File.WriteAllBytesAsync(Destination, [.. FileContentAsList]);
                }
            }

            // Create archive file from previously extracted files
            // TODO: Maybe do all of this in memory instead of writing the files to the disk inbetween?

            string FilePath = Path.Combine(ArchiveFilePathBase, ArchiveFileName);
            FileStream WrittenArchive;
            try
            {
                WrittenArchive = File.Create(FilePath);
            }
            catch (Exception ex)
            {
                ErrorTracking.Increment();
                Logger.Instance.Log(ex.ToString());
                return;
            }

            new BBAArchiveFile(WrittenArchive, ArchiveFilePath, true);
            await WrittenArchive.DisposeAsync();

            Directory.Delete(ArchiveFilePath);
            Logger.Instance.Log($"Finished creating archive file {FilePath}!");
        }

        private async Task UpdateModLoaderFilesByFileDataMapping()
        {
            await Parallel.ForEachAsync(GlobalFileDataMappings, async (Entry, CT) =>
            {
                string SanitizedFilePath = Utility.SanitizeFilePath(Entry.FilePath);
                string CurrentFile = Utility.ResolveCaseInsensitivePath(Path.Combine(GlobalGameDataDirectoryPath, SanitizedFilePath));

                byte[] FileContent = await File.ReadAllBytesAsync(CurrentFile, CT);

                Crc32 CRC = new();
                CRC.Append(FileContent);
                
                if (CRC.GetCurrentHashAsUInt32() != Entry.OriginalFileCRC)
                {
                    ErrorTracking.Increment();
                    Logger.Instance.Log($"File Hash for file {Path.GetFileName(CurrentFile)} not equivalent! Skipping ...");
                    return;
                }

                List<byte> FileContentAsList = [.. FileContent];
                UpdateFileContent(Entry, FileContentAsList);

                string DirectoryPath = Path.Combine(ArchiveFilePath, Path.GetDirectoryName(SanitizedFilePath) ?? string.Empty);
                string Destination = Path.Combine(ArchiveFilePath, SanitizedFilePath);

                Directory.CreateDirectory(DirectoryPath);
                await File.WriteAllBytesAsync(Destination, [.. FileContentAsList], CT);

                Logger.Instance.Log($"Updated file: {Path.GetFileName(CurrentFile)}");
            });
        }
    }
}
