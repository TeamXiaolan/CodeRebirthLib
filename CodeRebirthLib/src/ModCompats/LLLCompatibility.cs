using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using BepInEx.Bootstrap;
using LethalLevelLoader;
using UnityEngine;

namespace CodeRebirthLib.ModCompats;
static class LLLCompatibility
{
    public static bool Enabled => Chainloader.PluginInfos.ContainsKey(Plugin.ModGUID);

    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    public static bool TryGetCurveDictAndLevelTag(Dictionary<string, AnimationCurve> curvesByCustomLevel, SelectableLevel level, out string tagName)
    {
        tagName = string.Empty;
        ExtendedLevel? extendedLevel = PatchedContent.CustomExtendedLevels.FirstOrDefault(x => x.SelectableLevel == level) ?? PatchedContent.VanillaExtendedLevels.FirstOrDefault(x => x.SelectableLevel == level);
        if (extendedLevel == null) return false;
        foreach (var curve in curvesByCustomLevel)
        {
            foreach (ContentTag? tag in extendedLevel.ContentTags)
            {
                if (tag.contentTagName.ToLowerInvariant() == curve.Key)
                {
                    tagName = curve.Key;
                    return true;
                }
            }
        }
        return false;
    }

    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    public static void Init()
    {
        // skip LLL because for some unknown reason it chooses to just remove all scrap with 0 rarity (which is used in CRLib in some cases for dynamic weights)
        // LLL, this is not your job. If you wanted to make sure people didn't register scrap with 0 weight, please check and use your own scriptable objects, don't create behaviour that isn't defined anywhere.
        On.LethalLevelLoader.SafetyPatches.RoundManagerSpawnScrapInLevel_Prefix += orig => true;
    }
}