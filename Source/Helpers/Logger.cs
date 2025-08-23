using System;
using System.IO;
using System.Reflection;

namespace S6Patcher.Source.Helpers
{
    public sealed class Logger
    {
        private static readonly string GlobalFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), 
            "S6Patcher.log");

        private static StreamWriter Writer = null;
        private static readonly object Lock = new();

        private Logger() {}
        public static Logger Instance {get;} = new();

        ~Logger()
        {
            IOFileHandler.Instance.CloseStream(Writer.BaseStream);
            Writer = null;
        }

        private void EnsureLogWriter()
        {
            lock (Lock)
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
                    Writer.WriteLine(DateTime.Now.ToString() + ": " + Message);
                    Writer.Flush();
                }
            }
            catch {};
        }
    }
}