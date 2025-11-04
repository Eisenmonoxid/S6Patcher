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
        
        public static void WritePEHeaderPosition(FileStream Stream, long Offset, byte[] Bytes)
        {
            BinaryReader Reader = new(Stream);

            Reader.BaseStream.Position = 0x3C;
            Reader.BaseStream.Position = Reader.ReadInt32();

            if (Reader.ReadInt32() != 0x4550)
            {
                Logger.Instance.Log("PE Header offset not found! Skipping ...");
                return;
            }

            Reader.BaseStream.Position += Offset;
            Reader.BaseStream.Write(Bytes, 0, Bytes.Length);
        }
    }
}
