using System.Reflection;

namespace S6Patcher.Source.Helpers
{
    public enum execID
    {
        NONE = 0,
        OV = 1,
        HE_UBISOFT = 2,
        HE_STEAM = 3,
        ED = 4
    };

    public static class Utility
    {
        public static string GetApplicationVersion() => Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyFileVersionAttribute>().Version;
    }
}
