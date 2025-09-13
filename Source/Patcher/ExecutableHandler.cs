using S6Patcher.Source.Helpers;
using System;
using System.IO;

namespace S6Patcher.Source.Patcher
{
    internal class ExecutableHandler(string ExecutablePath)
    {
        public FileStream OpenExecutableFile()
        {
            if (Backup.Create(ExecutablePath) == false)
            {
                throw new Exception("Could not create backup file!\nAborting ...");
            }

            try
            {
                return IOFileHandler.Instance.OpenFileStream(ExecutablePath, true);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
