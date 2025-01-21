using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

namespace S6Patcher.Source.Helpers
{
    public sealed class Logger
    {
        private static readonly Logger _instance = new Logger();
        private Logger() {}
        ~Logger() 
        {
            if (LogWriter != null && LogWriter.BaseStream.CanRead == true)
            {
                LogWriter.Close();
                LogWriter.Dispose();
                LogWriter = null;
            }
        }
        public static Logger Instance
        {
            get
            {
                return _instance;
            }
        }

        private static readonly string CurrentFilepath = Assembly.GetExecutingAssembly().Location;
        private static readonly string CurrentLogFile = CurrentFilepath + Path.DirectorySeparatorChar + "S6Patcher_Log.log";
        private static bool FileCreated = false;
        private static StreamWriter LogWriter = null;
        
        public void Log(string Message)
        {
            if (FileCreated == false || LogWriter == null)
            {
                try
                {
                    LogWriter = File.CreateText(CurrentLogFile);
                }
                catch
                {
                    return;
                }
            }

            string Line = DateTime.Now.ToString() + ": " + Message + "\n";
            LogWriter.WriteLine(Line);
        }
    }
}
