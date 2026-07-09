using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace S6Patcher.Source.Utilities
{
    public enum execID
    {
        NONE = -1,
        OV = 0,
        HE_STEAM = 1,
        HE_UBISOFT = 2,
        ED = 3
    };

    public static class ErrorTracking
    {
        private static uint ErrorCount = 0;

        public static void Increment() => Interlocked.Increment(ref ErrorCount);
        public static void Reset() => Interlocked.Exchange(ref ErrorCount, 0);
        public static uint Count => ErrorCount;
    }

    public static class Utility
    {
        public static string GetApplicationVersion() => Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version;
        public static Stream GetEmbeddedResourceDefinition(string Name) => Assembly.GetExecutingAssembly().GetManifestResourceStream(Name);
        public static string SanitizeFilePath(string FilePath) => FilePath.Replace('\\', Path.DirectorySeparatorChar);

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
        public static readonly Dictionary<string, bool> ScriptFeatures = new()
        {
            {"UseAlternateBackground",      true},
            {"UseSingleStop",               true},
            {"UseDowngrade",                true},
            {"UseMilitaryRelease",          true},
            {"DayNightCycle",               true},
            {"ExtendedKnightSelection",     true},
            {"SpecialKnightsAvailable",     false},
            {"FeaturesInUsermaps",          false},
        };

        public static void WritePEHeaderPosition(FileStream Stream, long Offset, byte[] Bytes)
        {
            BinaryReader Reader = new(Stream);

            Reader.BaseStream.Position = 0x3C;
            Reader.BaseStream.Position = Reader.ReadInt32();

            if (Reader.ReadInt32() != 0x4550)
            {
                ErrorTracking.Increment();
                Logger.Instance.Log("PE Header offset not found! Skipping ...");
                return;
            }

            Reader.BaseStream.Position += Offset;
            Reader.BaseStream.Write(Bytes, 0, Bytes.Length);
        }

        public static string ResolveCaseInsensitivePath(string InputPath)
        {
            if (File.Exists(InputPath) || Directory.Exists(InputPath))
            {
               return InputPath; 
            }

            string RootPath = Path.GetPathRoot(InputPath)!;
            string CurrentPath = RootPath;

            foreach (string Part in InputPath[RootPath.Length..]
                    .Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                    .Where(P => !string.IsNullOrEmpty(P)))
            {
                var Match = Directory.EnumerateFileSystemEntries(CurrentPath)
                    .FirstOrDefault(X => string.Equals(Path.GetFileName(X), Part, StringComparison.OrdinalIgnoreCase));

                if (Match == null)
                {
                    return InputPath;
                }

                CurrentPath = Match;
            }

            return CurrentPath;
        }

        public static void ReplaceRange<T>(this List<T> List, int Index, int Length, IEnumerable<T> Replacement)
        {
            if (Index + Length >= List.Count)
            {
                Length = List.Count - Index;
            }

            List.RemoveRange(Index, Length);
            List.InsertRange(Index, Replacement);
        }
    }
}
