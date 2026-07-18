using S6Patcher.Source.Utilities;
using System;
using System.IO;
using System.Threading.Tasks;
using S6Packer.Source;

namespace S6Patcher.Source.Patcher
{
    internal static class Archives
    {
        public static async Task<bool> PackFolderFilesIntoArchiveFileAsync(string BaseFolderPath, string ArchiveFilePath)
        {
            FileStream Archive;
            try
            {
                Archive = File.Create(ArchiveFilePath);
            }
            catch (Exception ex)
            {
                Logger.Instance.Log(ex.ToString());
                return false;
            }

            try
            {
                new BBAArchiveFile(Archive, BaseFolderPath, Path.GetExtension(ArchiveFilePath));
            }
            catch (Exception ex)
            {
                Logger.Instance.Log(ex.ToString());
                return false;
            }
            finally
            {
                await Archive.DisposeAsync();
            }

            return true;
        }

        public static async Task<bool> ExtractArchiveFileToFolderAsync(string FilePath, string OutputDirectoryPath, bool IsMap)
        {
            FileStream ArchiveFileStream = OpenArchiveFileStream(FilePath);
            if (ArchiveFileStream == null)
            {
                return false;
            }

            BBAArchiveFile ArchiveFile;
            try
            {
                ArchiveFile = new(ArchiveFileStream, !IsMap);
            }
            catch
            {
                IOFileHandler.Instance.CloseStream(ArchiveFileStream);
                Logger.Instance.Log($"Could NOT parse archive file {FilePath}. Returning ...");
                return false;
            }

            ArchiveFile.UnpackAllDataEntriesFromArchive(OutputDirectoryPath);
            IOFileHandler.Instance.CloseStream(ArchiveFileStream);
            return true;
        }

        public static FileStream OpenArchiveFileStream(string ArchiveFilePath) => IOFileHandler.Instance.OpenFileStream(ArchiveFilePath, false, FileMode.Open, FileAccess.Read);
    }
}
