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
        ~Logger() 
        {
            if (LogWriter != null)
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
        }
        public static Logger Instance
        {
            get
            {
                return _instance;
            }
        }

        public void Log(string Message)
        {
            if (FileCreated == false || LogWriter == null)
            {
                try
                {
                    LogWriter = File.CreateText(CurrentLogFile);
                    FileCreated = true;
                }
                catch
                {
                    return;
                }
            }

            string Line = DateTime.Now.ToString() + ": " + Message + "\n";
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
