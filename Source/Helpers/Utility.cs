using System.Collections.Generic;
using System.IO;
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
        public static string GetApplicationVersion() => Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyFileVersionAttribute>().Version;
        public static Stream GetEmbeddedResourceDefinition(string Name) => Assembly.GetExecutingAssembly().GetManifestResourceStream(Name);

        public static readonly Dictionary<string, string> Features = new()
        {
            {"cbHighTextures",      "HTS"},
            {"cbScalingPlacing",    "SCP"},
            {"cbEntityLimits",      "ENL"},
            {"cbMapBorder",         "BMB"},
            {"cbDevMode",           "DVM"},
            {"cbAllEntities",       "AET"},
            {"cbScriptBugFixes",    "SBF"},
            {"cbLimitedEdition",    "LME"},
        };
    }
}
