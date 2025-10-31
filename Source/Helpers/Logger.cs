using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;

namespace S6Patcher.Source.Helpers
{
    public sealed class Logger
    {
        private readonly string GlobalFile = Path.Combine(AppContext.BaseDirectory, "S6Patcher.log");
        private readonly StreamWriter Writer = null;
        private readonly Lock Lock = new();
        public static Logger Instance {get;} = new();

        private Logger() 
        {
            try
            {
                Writer = File.CreateText(GlobalFile);
            }
            catch
            {
                Writer = null;
            }
        }

        public void Dispose()
        {
            Log("Logger: Shutting down.");
            Writer?.Close();
            Writer?.Dispose();
        }

        public void Log(string Message, [CallerMemberName] string Caller = "")
        {
            lock (Lock)
            {
                try
                {
                    Writer?.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " - " + Caller + "(): " + Message);
                    Writer?.Flush();
                }
                catch
                {
                    return;
                }
            }
        }
    }
}