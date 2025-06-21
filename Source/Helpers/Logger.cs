using System;
using System.IO;
using System.Reflection;

namespace S6Patcher.Source.Helpers
{
    public sealed class Logger
    {
        private static readonly string CurrentFilepath = Assembly.GetExecutingAssembly().Location;
        private static readonly string CurrentLogFile = CurrentFilepath.Replace(Path.GetFileName(CurrentFilepath), "") + Path.DirectorySeparatorChar + "S6Patcher_Output.log";
        private static bool FileCreated = false;
        private static StreamWriter LogWriter = null;

        private static readonly Logger _instance = new Logger();
        private Logger() {}
        public static Logger Instance => _instance;
        ~Logger() 
        {
            try
            {
                LogWriter.Close();
                LogWriter.Dispose();
                LogWriter = null;
            }
            catch
            {
                return;
            }
        }

        public bool CreateOutputLogFile()
        {
            try
            {
                LogWriter = File.CreateText(CurrentLogFile);
                FileCreated = true;
            }
            catch
            {
                return false;
            }

            return true;
        }

        public void Log(string Message)
        {
            if (FileCreated == false || LogWriter == null)
            {
                if (CreateOutputLogFile() == false)
                {
                    return;
                }
            }

            string Line = DateTime.Now.ToString() + ": " + Message;
            try
            {
                LogWriter.WriteLine(Line);
                LogWriter.Flush();
            }
            catch
            {
                return;
            }
        }
    }
}
