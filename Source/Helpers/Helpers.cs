using System.Reflection;

namespace S6Patcher.Source.Helpers
{
    public enum execID
    {
        NONE = -1,
        OV = 0,
        HE_STEAM = 1,
        HE_UBISOFT = 2,
        ED = 3
    };

    public static class Utility
    {
        public static string GetApplicationVersion() => Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyFileVersionAttribute>().Version;
    }
}
