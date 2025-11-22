using System;
using Avalonia;

namespace S6Patcher.Source
{
    internal class Program
    {
        public static string[] CommandLineArguments = [];

        [STAThread]
        public static void Main(string[] args)
        {
            CommandLineArguments = args;
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .WithInterFont()
                .LogToTrace();
    }
}
