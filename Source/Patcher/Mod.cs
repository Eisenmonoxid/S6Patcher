using S6Patcher.Properties;
using S6Patcher.Source.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.IO.Hashing;
using Utility = S6Patcher.Source.Utilities.Utility;
using S6Packer.Source;

namespace S6Patcher.Source.Patcher
{
    internal class Mod(execID GlobalID, string GlobalStreamName)
    {
        private enum FileOperation : byte
        {
            Replace = 0x00,
            Insert = 0x01,
            Delete = 0x02
        }

        private struct ModLoaderFile
        {
            public bool IsExtra1File;
            public string Name;
            public byte[] Data;
            public FileDataEntry Entry;
        }

        private readonly List<FileDataEntry> GlobalFileDataMappings = [];
        private readonly string GlobalDestinationDirectoryPath = IOFileHandler.Instance.GetModLoaderDirectory(GlobalID, GlobalStreamName);
        private readonly string GlobalBaseGameDataDirectoryPath = IOFileHandler.Instance.GetGameDataDirectory(GlobalID, GlobalStreamName, false);
        private readonly string GlobalExtra1GameDataDirectoryPath = IOFileHandler.Instance.GetGameDataDirectory(GlobalID, GlobalStreamName, true);
        private readonly string ArchiveFilePath = Path.Combine(IOFileHandler.Instance.GetModLoaderDirectory(GlobalID, GlobalStreamName), "shr");
        private readonly string ArchiveFilePathModloader = Path.Combine(IOFileHandler.Instance.GetModLoaderDirectory(GlobalID, GlobalStreamName), "shr");
        private readonly string ArchiveFilePathBase = Path.Combine(IOFileHandler.Instance.GetModLoaderDirectory(GlobalID, GlobalStreamName), "base");
        private readonly string ArchiveFilePathExtra1 = Path.Combine(IOFileHandler.Instance.GetModLoaderDirectory(GlobalID, GlobalStreamName), "extra1");
        private const string ArchiveFileName = "mod.bba";
        public event Action<string> ShowMessage;
        private static async Task<List<MemoryStream>> DownloadDefinitionFile(string FilePath) => await WebHandler.Instance.DownloadFilesAsync([new Uri(FilePath)]);

        public async Task Create(bool InstallMod, bool DownloadDefinition)
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

            if (!InstallMod)
            {
                return;
            }

            BinaryParser FileDataParser = null;
            if (DownloadDefinition)
            {
                List<MemoryStream> Definition = await DownloadDefinitionFile(Resources.RepoDefinitionBasePath + "FileData.bin");
                if (Definition.Count == 1)
                {
                    try
                    {
                        FileDataParser = new BinaryParser(Definition[0]);
                    }
                    catch
                    {
                        Definition.ForEach(async Element => await Element.DisposeAsync());
                        FileDataParser = null;

                        Logger.Instance.Log("Downloaded Definition file invalid, falling back to embedded resource ...");
                    }
                }
            }

            FileDataParser ??= new BinaryParser("S6Patcher.Definitions.FileData.bin");
            List<FileDataEntry> Entries = FileDataParser.ParseFileData();
            GlobalFileDataMappings.AddRange(Entries);
            FileDataParser.Dispose();
            
            if (GlobalID == execID.OV)
            {
                await GetModLoaderFilesFromArchives(); // Updated Files from game archive files 
            }
            else
            {
                await UpdateModLoaderFilesByFileDataMapping(); // Updated Files from game directory 
            }
        }

        private async Task WriteModLoaderFiles()
        {
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
                Directory.CreateDirectory(ArchiveFilePathModloader);

                await WriteModLoaderArchiveFileAsync(ArchiveFilePathBase);
                await WriteModLoaderArchiveFileAsync(ArchiveFilePathExtra1);
                await WriteModLoaderArchiveFileAsync(ArchiveFilePathModloader);
            }
        }

        private async Task WriteModLoaderArchiveFileAsync(string BasePath)
        {
            string CurrentPath = Path.Combine(BasePath, ArchiveFileName);
            if (!File.Exists(CurrentPath))
            {
                await File.WriteAllBytesAsync(CurrentPath, Resources.mod);
                Logger.Instance.Log("Written " + ArchiveFileName + " to " + CurrentPath);
            }
            else
            {
                Logger.Instance.Log("Archive file already existing at " + CurrentPath);
            }
        }

        private async Task GetModLoaderFilesFromArchives()
        {
            var ArchiveFilesToParse = GetArchiveFilesToParse();
            await LoadDataFromArchiveFiles(ArchiveFilesToParse);
            await WriteAllModFilesToCorrespondingFolder(ArchiveFilesToParse);
            await CreateArchiveFileInPath(ArchiveFilePathModloader);
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

        private FileStream OpenArchiveFileStream(string ArchiveFile, out bool IsExtra1ArchiveFile)
        {
            IsExtra1ArchiveFile = false;

            string ArchiveFilePath = Path.Combine(GlobalBaseGameDataDirectoryPath, ArchiveFile);
            if (!File.Exists(ArchiveFilePath))
            {
                // Check if is extra1
                ArchiveFilePath = Path.Combine(GlobalExtra1GameDataDirectoryPath, ArchiveFile);

                if (!File.Exists(ArchiveFilePath))
                {
                    Logger.Instance.Log($"Could NOT find archive file {ArchiveFile}. Skipping ...");
                    return null;
                }

                IsExtra1ArchiveFile = true;
            }

            return Archives.OpenArchiveFileStream(ArchiveFilePath);
        }

        private async Task LoadDataFromArchiveFiles(Dictionary<string, List<ModLoaderFile>> ArchiveFilesToParse)
        {
            foreach (var Element in ArchiveFilesToParse)
            {
                FileStream ArchiveFileStream = OpenArchiveFileStream(Element.Key, out bool IsExtra1ArchiveFile);
                if (ArchiveFileStream == null)
                {
                    ErrorTracking.Increment();
                    continue;
                }

                for (int i = 0; i < Element.Value.Count; i++)
                {
                    ModLoaderFile MLF = Element.Value[i];  
                    MLF.IsExtra1File = IsExtra1ArchiveFile;
                    Element.Value[i] = MLF;
                }

                BBAArchiveFile ArchiveFile;
                try
                {
                    ArchiveFile = new(ArchiveFileStream, true);
                }
                catch
                {
                    IOFileHandler.Instance.CloseStream(ArchiveFileStream);
                    ErrorTracking.Increment();
                    Logger.Instance.Log($"Could NOT parse archive file {Element.Key}. Skipping ...");
                    continue;
                }
                
                for (int i = 0; i < Element.Value.Count; i++)
                {
                    ModLoaderFile MLF = Element.Value[i];  
                    MLF.Data = ArchiveFile.GetFileDataByDataEntryName(MLF.Name);
                    if (MLF.Data == null)
                    {
                        ErrorTracking.Increment();
                        Logger.Instance.Log($"Could NOT parse data from archive file {Element.Key}.");
                    }

                    Element.Value[i] = MLF;
                }

                IOFileHandler.Instance.CloseStream(ArchiveFileStream);
            }
        }

        private Dictionary<string, List<ModLoaderFile>> GetArchiveFilesToParse()
        {
            Dictionary<string, List<ModLoaderFile>> ArchiveFilesToParse = [];
            foreach (var Element in GlobalFileDataMappings)
            {
                ModLoaderFile CurrentFile = new()
                {
                    Name = Element.FilePath,
                    Entry = Element,
                    Data = null,
                    IsExtra1File = false
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

            return ArchiveFilesToParse;
        }

        private async Task WriteAllModFilesToCorrespondingFolder(Dictionary<string, List<ModLoaderFile>> ArchiveFilesToParse)
        {
            foreach (var Element in ArchiveFilesToParse)
            {
                foreach (var CurrentFile in Element.Value)
                {
                    if (CurrentFile.Data == null)
                    {
                        Logger.Instance.Log($"Skipping file {CurrentFile.Name} cause Data field is null.");
                        continue;
                    }

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
                    if (FileContentAsList.Count == 0)
                    {
                        Logger.Instance.Log($"Skipping Empty file: {CurrentFile.Name}");
                        continue;
                    }

                    UpdateFileContent(CurrentFile.Entry, FileContentAsList);

                    string DirectoryPath = Path.Combine(ArchiveFilePathModloader, Path.GetDirectoryName(SanitizedFilePath) ?? string.Empty);
                    string Destination = Path.Combine(ArchiveFilePathModloader, SanitizedFilePath);

                    try
                    {
                        Directory.CreateDirectory(DirectoryPath);
                        await File.WriteAllBytesAsync(Destination, [.. FileContentAsList]);
                    }
                    catch (Exception ex)
                    { 
                        ErrorTracking.Increment();
                        Logger.Instance.Log(ex.ToString());
                        continue;
                    }
                }
            }
        }

        private async Task CreateArchiveFileInPath(string RootDirectoryFilePath)
        {
            // Create archive file from previously extracted files
            // TODO: Maybe do all of this in memory instead of writing the files to the disk inbetween?
            string TemporaryFile = Path.Combine(GlobalDestinationDirectoryPath, ArchiveFileName);
            FileStream WrittenArchive;
            try
            {
                WrittenArchive = File.Create(TemporaryFile);
            }
            catch (Exception ex)
            {
                ErrorTracking.Increment();
                Logger.Instance.Log(ex.ToString());
                return;
            }

            string FilePath = Path.Combine(RootDirectoryFilePath, ArchiveFileName);
            if (File.Exists(FilePath))
            {
                try
                {
                    File.Delete(FilePath);
                }
                catch (Exception ex)
                {
                    ErrorTracking.Increment();
                    Logger.Instance.Log(ex.ToString());
                    return;
                }
            }

            try
            {
                new BBAArchiveFile(WrittenArchive, RootDirectoryFilePath, ".bba");
            }
            catch (Exception ex)
            {
                ErrorTracking.Increment();
                Logger.Instance.Log(ex.ToString());

                await WriteModLoaderArchiveFileAsync(ArchiveFilePathModloader);
                return;
            }
            finally
            {
                await WrittenArchive.DisposeAsync();
            }
            
            try
            {
                Directory.Delete(RootDirectoryFilePath, true); // Clear all files
                Directory.CreateDirectory(RootDirectoryFilePath);
                File.Move(TemporaryFile, Path.Combine(RootDirectoryFilePath, ArchiveFileName), true);
            }
            catch (Exception ex)
            {
                ErrorTracking.Increment();
                Logger.Instance.Log(ex.ToString());

                await WriteModLoaderArchiveFileAsync(ArchiveFilePathModloader);
                return;
            }

            Logger.Instance.Log($"Finished creating archive file {FilePath}!");
        }

        private async Task UpdateModLoaderFilesByFileDataMapping()
        {
            await Parallel.ForEachAsync(GlobalFileDataMappings, async (Entry, CT) =>
            {
                string SanitizedFilePath = Utility.SanitizeFilePath(Entry.FilePath);
                string CurrentFile = Utility.ResolveCaseInsensitivePath(Path.Combine(GlobalBaseGameDataDirectoryPath, SanitizedFilePath));

                if (!File.Exists(CurrentFile))
                {
                    CurrentFile = Utility.ResolveCaseInsensitivePath(Path.Combine(GlobalExtra1GameDataDirectoryPath, SanitizedFilePath));
                    if (!File.Exists(CurrentFile))
                    {
                        ErrorTracking.Increment();
                        Logger.Instance.Log($"Could NOT find file {CurrentFile}. Skipping ...");
                        return;
                    }
                }

                byte[] FileContent;
                try
                {
                    FileContent = await File.ReadAllBytesAsync(CurrentFile, CT);
                }
                catch (Exception ex)
                {
                    ErrorTracking.Increment();
                    Logger.Instance.Log(ex.ToString());
                    return;
                }

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

                try
                {
                    Directory.CreateDirectory(DirectoryPath);
                    await File.WriteAllBytesAsync(Destination, [.. FileContentAsList], CT);
                }
                catch (Exception ex)
                { 
                    ErrorTracking.Increment();
                    Logger.Instance.Log(ex.ToString());
                    return;
                }

                Logger.Instance.Log($"Updated file: {Path.GetFileName(CurrentFile)}");
            });
        }
    }
}
