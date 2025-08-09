using System;
using System.IO;
using System.Reflection;

namespace S6Patcher.Source.Helpers
{
    public sealed class Logger
    {
        private static readonly string GlobalFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "S6Patcher.log");

        private static StreamWriter Writer = null;
        private static readonly object Lock = new object();
        private static readonly Logger _instance = new Logger();
        private Logger() {}
        public static Logger Instance => _instance;

        ~Logger()
        {
            if (Writer != null)
            {
                Writer.Close();
                Writer.Dispose();
            }
        }

        private void EnsureLogWriter()
        {
            lock (Lock)
            {
                try
                {
                    Writer = File.CreateText(GlobalFile);
                }
                catch (Exception ex)
                {
                    Writer = null;
                }
            }
        }

        public void Log(string Message)
        {
            if (Writer == null)
            {
                EnsureLogWriter();
                if (Writer == null) {return;};
            }

            try
            {
                lock (Lock)
                {
                    Writer.WriteLine($"{DateTime.Now}: {Message}");
                    Writer.Flush();
                }
            }
            catch {};
        }
    }
}