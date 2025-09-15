using System.Collections.Generic;

namespace S6Patcher.Source.Patcher
{
    static class ScriptFeatures
    {
        public static readonly Dictionary<string, bool> Features = new()
        {
            {"UseAlternateBackground", true},
            {"UseSingleStop", true},
            {"UseDowngrade", true},
            {"UseMilitaryRelease", true},
            {"DayNightCycle", true},
            {"ExtendedKnightSelection", true},
            {"SpecialKnightsAvailable", false},
            {"FeaturesInUsermaps", false},
            {"IsModAvailable", false},
        };
    }
}
