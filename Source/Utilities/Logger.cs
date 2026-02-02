using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;

namespace S6Patcher.Source.Helpers
{
    public sealed class Logger
    {
        private Logger()
        {
            FileStream Stream;
            string LogFilePath = Path.Combine(AppContext.BaseDirectory, "S6Patcher.log");

            try
            {
                Stream = new FileStream(LogFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
            }
            catch
            {
                return;
            }

            Writer = new StreamWriter(Stream);
        }

        private readonly StreamWriter Writer = null;
        private readonly Lock Lock = new();
        public static Logger Instance {get;} = new();

        public void Dispose()
        {
            Log("Logger: Shutting down.");
            Writer?.Dispose();
        }

        public void Log(string Message, [CallerMemberName] string Caller = "")
        {
            lock (Lock)
            {
                Writer?.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " - " + Caller + "(): " + Message);
                Writer?.Flush();
            }
        }
    }
}
