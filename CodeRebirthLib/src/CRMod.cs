﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using BepInEx;
using BepInEx.Logging;
using CodeRebirthLib.AssetManagement;
using CodeRebirthLib.ConfigManagement;
using CodeRebirthLib.ContentManagement;
using CodeRebirthLib.ContentManagement.Achievements;
using CodeRebirthLib.ContentManagement.Enemies;
using CodeRebirthLib.ContentManagement.Items;
using CodeRebirthLib.ContentManagement.MapObjects;
using CodeRebirthLib.ContentManagement.Unlockables;
using CodeRebirthLib.ContentManagement.Weathers;
using CodeRebirthLib.Data;
using CodeRebirthLib.Exceptions;
using CodeRebirthLib.Extensions;
using CodeRebirthLib.ModCompats;
using Newtonsoft.Json;
using UnityEngine;

namespace CodeRebirthLib;

public class CRMod
{
    private static readonly List<CRMod> _allMods = new();

    private readonly string _basePath;

    private readonly Dictionary<string, CRRegistry> _registries = new();

    // todo: i dont like how many arguments are here lmao
    internal CRMod(Assembly assembly, BaseUnityPlugin plugin, AssetBundle mainBundle, string basePath, ConfigManager configManager) : this(MetadataHelper.GetMetadata(plugin.GetType()), mainBundle, basePath, configManager)
    {
        Assembly = assembly;
        ResolveCodeModeInformation(assembly);
    }

    private void ResolveCodeModeInformation(Assembly assembly)
    {
        ModInformation = ScriptableObject.CreateInstance<CRModInformation>();
        var searchDir = Path.GetFullPath(assembly.Location);
        var parent = Directory.GetParent(searchDir);

        // Ensure our search directory has a parent of `plugins` ex: `plugins/SEARCH_DIR`, this should leave us with a best match for how r2modman and it's derivatives' install mods.
        while (parent != null && !string.Equals(parent.Name, "plugins", StringComparison.OrdinalIgnoreCase))
        {
            searchDir = parent.FullName;
            parent = Directory.GetParent(searchDir); // This prevents an infinite loop, as parent becomes null if we hit the root of the drive.
        }

        if (searchDir.EndsWith(".dll")) // Return early if the searchDir is a dll file, prevents a crash from occuring below. Commonly occurs when manually installing mods.
            return;

        var iconPath = Directory.EnumerateFiles(searchDir, "icon.png", SearchOption.AllDirectories).FirstOrDefault();
        ModInformation.ModIcon = LoadIcon(iconPath);
        CodeRebirthLibPlugin.ExtendedLogging($"Mod Icon found! {ModInformation.ModIcon != null}");

        var manifestPath = Directory.EnumerateFiles(searchDir, "manifest.json", SearchOption.AllDirectories).FirstOrDefault();
        ModInformation.ModDescription = LoadDesc(manifestPath);
        ModInformation.ModName = LoadModName(manifestPath);
        ModInformation.AuthorName = LoadAuthorName(manifestPath);
        ModInformation.Version = LoadVersionNumber(manifestPath);
        ModInformation.ExtraDependencies = LoadDependencies(manifestPath);
        ModInformation.WebsiteUrl = LoadWebsiteUrl(manifestPath);
        CodeRebirthLibPlugin.ExtendedLogging($"Mod information found: {ModInformation.ModName}, {ModInformation.ModDescription}, {ModInformation.ModIcon != null}, {ModInformation.AuthorName}, {ModInformation.Version}, {ModInformation.ExtraDependencies}, {ModInformation.WebsiteUrl}");
    }

    private Sprite? LoadIcon(string iconPath)
    {
        if (iconPath == default)
            return null;

        var iconTex = new Texture2D(256, 256);
        if (!iconTex.LoadImage(File.ReadAllBytes(iconPath), true))
            return null;

        var ModIcon = Sprite.Create(iconTex, new Rect(0, 0, iconTex.width, iconTex.height), new Vector2(0.5f, 0.5f), 100);
        return ModIcon;
    }

    private string LoadDesc(string manifestPath)
    {
        try
        {
            if (manifestPath == default)
                return string.Empty;

            string manifestContents = File.ReadAllText(manifestPath);

            var manifest = JsonConvert.DeserializeObject<ThunderstoreManifest>(manifestContents);
            if (manifest is null)
                return string.Empty;

            return manifest.description;
        }
        catch (JsonReaderException e)
        {
            CodeRebirthLibPlugin.Logger.LogWarning($"Failed to load author name from manifest.json: {e.Message} with filepath: {manifestPath}");
            return string.Empty;
            // Catch-all just to make sure we don't crash if we can't load the manifest.
        }
    }

    private string LoadAuthorName(string manifestPath)
    {
        try
        {
            if (manifestPath == default)
                return string.Empty;

            var manifestContents = File.ReadAllText(manifestPath);

            var manifest = JsonConvert.DeserializeObject<ThunderstoreManifest>(manifestContents);
            if (manifest is null)
                return string.Empty;

            return manifest.author_name;
        }
        catch (JsonReaderException e)
        {
            CodeRebirthLibPlugin.Logger.LogWarning($"Failed to load author name from manifest.json: {e.Message} with filepath: {manifestPath}");
            return string.Empty;
            // Catch-all just to make sure we don't crash if we can't load the manifest.
        }
    }

    private string LoadModName(string manifestPath)
    {
        try
        {
            if (manifestPath == default)
                return string.Empty;

            var manifestContents = File.ReadAllText(manifestPath);

            var manifest = JsonConvert.DeserializeObject<ThunderstoreManifest>(manifestContents);
            if (manifest is null)
                return string.Empty;

            return manifest.name;
        }
        catch (JsonReaderException e)
        {
            CodeRebirthLibPlugin.Logger.LogWarning($"Failed to load author name from manifest.json: {e.Message} with filepath: {manifestPath}");
            return string.Empty;
            // Catch-all just to make sure we don't crash if we can't load the manifest.
        }
    }

    private string LoadVersionNumber(string manifestPath)
    {
        try
        {
            if (manifestPath == default)
                return string.Empty;

            var manifestContents = File.ReadAllText(manifestPath);

            var manifest = JsonConvert.DeserializeObject<ThunderstoreManifest>(manifestContents);
            if (manifest is null)
                return string.Empty;

            return manifest.version_number;
        }
        catch (JsonReaderException e)
        {
            CodeRebirthLibPlugin.Logger.LogWarning($"Failed to load author name from manifest.json: {e.Message} with filepath: {manifestPath}");
            return string.Empty;
            // Catch-all just to make sure we don't crash if we can't load the manifest.
        }
    }

    private string LoadWebsiteUrl(string manifestPath)
    {
        try
        {
            if (manifestPath == default)
                return string.Empty;

            var manifestContents = File.ReadAllText(manifestPath);

            var manifest = JsonConvert.DeserializeObject<ThunderstoreManifest>(manifestContents);
            if (manifest is null)
                return string.Empty;

            return manifest.website_url;
        }
        catch (JsonReaderException e)
        {
            CodeRebirthLibPlugin.Logger.LogWarning($"Failed to load author name from manifest.json: {e.Message} with filepath: {manifestPath}");
            return string.Empty;
            // Catch-all just to make sure we don't crash if we can't load the manifest.
        }
    }

    private List<string> LoadDependencies(string manifestPath)
    {
        try
        {
            if (manifestPath == default)
                return new();

            var manifestContents = File.ReadAllText(manifestPath);

            var manifest = JsonConvert.DeserializeObject<ThunderstoreManifest>(manifestContents);
            if (manifest is null)
                return new();

            return manifest.dependencies;
        }
        catch (JsonReaderException e)
        {
            CodeRebirthLibPlugin.Logger.LogWarning($"Failed to load author name from manifest.json: {e.Message} with filepath: {manifestPath}");
            return new();
            // Catch-all just to make sure we don't crash if we can't load the manifest.
        }
    }

    internal CRMod(BepInPlugin plugin, AssetBundle mainBundle, string basePath, ConfigManager configManager)
    {
        ConfigManager = configManager;
        _basePath = basePath;
        Plugin = plugin;

        ContentContainer[] containers = mainBundle.LoadAllAssets<ContentContainer>();
        if (containers.Length == 0)
        {
            throw new NoContentDefinitionInBundle(mainBundle);
        }
        if (containers.Length >= 2)
        {
            throw new MultipleContentDefinitionsInBundle(mainBundle);
        }

        Content = containers[0];

        AddDefaultRegistries();
        if (WeatherRegistryCompatibility.Enabled)
        {
            AddWeatherRegistry();
        }

        _allMods.Add(this);
    }
    public static IReadOnlyList<CRMod> AllMods => _allMods.AsReadOnly();

    public ConfigManager ConfigManager { get; }
    public ContentContainer Content { get; }

    public Assembly? Assembly { get; }
    public CRModInformation ModInformation { get; set; }
    public ManualLogSource? Logger { get; set; }

    public BepInPlugin Plugin { get; }

    public string GetRelativePath(params string[] path)
    {
        return Path.Combine(_basePath, Path.Combine(path));
    }

    public bool TryGetRelativeFile(out string fullPath, params string[] path)
    {
        fullPath = GetRelativePath(path);
        return File.Exists(fullPath);
    }

    public void CreateRegistry<T>(string name, CRRegistry<T> registry) where T : CRContentDefinition
    {
        _registries[name] = registry;
        CodeRebirthLibPlugin.ExtendedLogging($"Created Registry: {name}");
    }

    public CRRegistry<T> GetRegistryByName<T>(string name) where T : CRContentDefinition
    {
        return (CRRegistry<T>)_registries[name];
    }

    public CRRegistry<CREnemyDefinition> EnemyRegistry()
    {
        return GetRegistryByName<CREnemyDefinition>(CREnemyDefinition.REGISTRY_ID);
    }
    public static IEnumerable<CREnemyDefinition> AllEnemies()
    {
        return AllMods.SelectMany(mod => mod.EnemyRegistry());
    }

    public CRRegistry<CRItemDefinition> ItemRegistry()
    {
        return GetRegistryByName<CRItemDefinition>(CRItemDefinition.REGISTRY_ID);
    }
    public static IEnumerable<CRItemDefinition> AllItems()
    {
        return AllMods.SelectMany(mod => mod.ItemRegistry());
    }

    public CRRegistry<CRMapObjectDefinition> MapObjectRegistry()
    {
        return GetRegistryByName<CRMapObjectDefinition>(CRMapObjectDefinition.REGISTRY_ID);
    }
    public static IEnumerable<CRMapObjectDefinition> AllMapObjects()
    {
        return AllMods.SelectMany(mod => mod.MapObjectRegistry());
    }

    public CRRegistry<CRUnlockableDefinition> UnlockableRegistry()
    {
        return GetRegistryByName<CRUnlockableDefinition>(CRUnlockableDefinition.REGISTRY_ID);
    }
    public static IEnumerable<CRUnlockableDefinition> AllUnlockables()
    {
        return AllMods.SelectMany(mod => mod.UnlockableRegistry());
    }

    public CRRegistry<CRAchievementBaseDefinition> AchievementRegistry()
    {
        return GetRegistryByName<CRAchievementBaseDefinition>(CRAchievementBaseDefinition.REGISTRY_ID);
    }
    public static IEnumerable<CRAchievementBaseDefinition> AllAchievements()
    {
        return AllMods.SelectMany(mod => mod.AchievementRegistry());
    }

    public bool TryGetBundleDataFromName(string bundleName, [NotNullWhen(true)] out AssetBundleData? data)
    {
        data = Content.assetBundles.FirstOrDefault(it => it.assetBundleName == bundleName);
        return data != null;
    }

    private void AddDefaultRegistries()
    {
        CREnemyDefinition.RegisterTo(this);
        CRMapObjectDefinition.RegisterTo(this);
        CRItemDefinition.RegisterTo(this);
        CRUnlockableDefinition.RegisterTo(this);
        CRAchievementBaseDefinition.RegisterTo(this);
    }

    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    private void AddWeatherRegistry()
    {
        CRWeatherDefinition.RegisterTo(this);
    }

    public void RegisterContentHandlers()
    {
        if (Assembly == null)
        {
            CodeRebirthLibPlugin.Logger.LogWarning($"Tried to Register Content Handlers for {Plugin.Name} but it is a no-code CRMod!");
            return;
        }

        IEnumerable<Type> contentHandlers = Assembly.GetLoadableTypes().Where(x =>
            !x.IsNested && x.BaseType != null
            && x.BaseType.IsGenericType
            && x.BaseType.GetGenericTypeDefinition() == typeof(ContentHandler<>)
        );

        foreach (Type type in contentHandlers)
        {
            type.GetConstructor([typeof(CRMod)]).Invoke([this]);
        }
    }
}