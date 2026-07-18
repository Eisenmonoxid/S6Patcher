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
using Avalonia.Controls.Shapes;

namespace S6Patcher.Source.Patcher
{
    internal static class Archives
    {
        public static async Task<bool> PackFolderFilesIntoArchiveFileAsync(string BaseFolderPath)
        {
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
                await ArchiveFileStream.DisposeAsync();
                Logger.Instance.Log($"Could NOT parse archive file {FilePath}. Returning ...");
                return false;
            }

            ArchiveFile.UnpackAllDataEntriesFromArchive(OutputDirectoryPath);
            return true;
        }

        public static FileStream OpenArchiveFileStream(string ArchiveFilePath) => IOFileHandler.Instance.OpenFileStream(ArchiveFilePath, false, FileMode.Open, FileAccess.Read);
    }
}
