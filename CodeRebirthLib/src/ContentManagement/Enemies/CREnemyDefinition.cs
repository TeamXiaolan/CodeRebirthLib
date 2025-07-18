﻿using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CodeRebirthLib.AssetManagement;
using CodeRebirthLib.ConfigManagement;
using CodeRebirthLib.Util.Attributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace CodeRebirthLib.ContentManagement.Enemies;
[CreateAssetMenu(fileName = "New Enemy Definition", menuName = "CodeRebirthLib/Definitions/Enemy Definition")]
public class CREnemyDefinition : CRContentDefinition<EnemyData>
{
    public const string REGISTRY_ID = "enemies";

    [field: FormerlySerializedAs("enemyType")] [field: SerializeField]
    public EnemyType EnemyType { get; private set; }

    [field: FormerlySerializedAs("terminalNode")] [field: SerializeField]
    public TerminalNode? TerminalNode { get; private set; }

    [field: FormerlySerializedAs("terminalKeyword")] [field: SerializeField]
    public TerminalKeyword? TerminalKeyword { get; private set; }

    private readonly Dictionary<SelectableLevel, AttributeStack<int>> _moonWeights = new();

    [HideInInspector]
    public Dictionary<string, float> WeatherMultipliers = new();

    public EnemyConfig Config { get; private set; }

    public override void Register(CRMod mod, EnemyData data)
    {
        if (string.IsNullOrEmpty(data.weatherMultipliers))
        {
            data.weatherMultipliers = "None:1";
        }

        using ConfigContext section = mod.ConfigManager.CreateConfigSectionForBundleData(AssetBundleData);
        Config = CreateEnemyConfig(section, data, EnemyType.enemyName);

        List<string> weatherMultipliersList = Config.WeatherMultipliers.Value.Split(',').ToList();
        foreach (string[]? weatherMultiplierInList in weatherMultipliersList.Select(s => s.Split(':')))
        {
            string weatherName = weatherMultiplierInList[0].Trim();
            if (weatherMultiplierInList.Count() == 2 && float.TryParse(weatherMultiplierInList[1], NumberStyles.Float, CultureInfo.InvariantCulture, out float multiplier))
            {
                WeatherMultipliers.Add(weatherName, multiplier);
            }
            else
            {
                CodeRebirthLibPlugin.Logger.LogError($"Weather: {weatherName} given invalid or empty multiplier");
            }
        }

        EnemyType enemy = EnemyType;
        enemy.MaxCount = Config.MaxSpawnCount.Value;
        enemy.PowerLevel = Config.PowerLevel.Value;
        (var spawnRateByLevelType, Dictionary<string, int> spawnRateByCustomLevelType) = ConfigManager.ParseMoonsWithRarity(Config.SpawnWeights.Value);
        LethalLib.Modules.Enemies.RegisterEnemy(enemy, spawnRateByLevelType, spawnRateByCustomLevelType, TerminalNode, TerminalKeyword);
        mod.EnemyRegistry().Register(this);
    }

    internal static void CreateMoonAttributeStacks()
    {
        foreach (SelectableLevel moon in StartOfRound.Instance.levels)
        {
            CreateMoonAttributesStackForSetEnemiesInLevel(moon.Enemies, moon);
            CreateMoonAttributesStackForSetEnemiesInLevel(moon.OutsideEnemies, moon);
            CreateMoonAttributesStackForSetEnemiesInLevel(moon.DaytimeEnemies, moon);
        }
    }

    private static void CreateMoonAttributesStackForSetEnemiesInLevel(List<SpawnableEnemyWithRarity> enemies, SelectableLevel level)
    {
        foreach (SpawnableEnemyWithRarity enemyWithRarity in enemies)
        {
            EnemyType enemy = enemyWithRarity.enemyType;
            if (!enemy.TryGetDefinition(out CREnemyDefinition? definition) || definition._moonWeights.ContainsKey(level))
                continue;

            AttributeStack<int> stack = new(enemyWithRarity.rarity);
            stack.Add(input =>
            { // Handle Weather Multipliers, note that this keeps the reference of 'definition' and 'moon' from the foreach loops
                string weatherName = level.currentWeather.ToString();
                if (!definition.WeatherMultipliers.TryGetValue(weatherName, out float multiplier))
                    return input;

                return Mathf.FloorToInt(multiplier * input);
            });
            definition._moonWeights[level] = stack;
        }
    }


    internal static void UpdateAllWeights()
    {
        if (StartOfRound.Instance == null || StartOfRound.Instance.levels == null)
            return;

        foreach (SelectableLevel moon in StartOfRound.Instance.levels)
        {
            UpdateAllEnemyWeightsForLevel(moon.Enemies, moon);
            UpdateAllEnemyWeightsForLevel(moon.OutsideEnemies, moon);
            UpdateAllEnemyWeightsForLevel(moon.DaytimeEnemies, moon);
        }
    }

    internal static void UpdateAllEnemyWeightsForLevel(List<SpawnableEnemyWithRarity> enemies, SelectableLevel level)
    {
        foreach (SpawnableEnemyWithRarity enemyWithRarity in enemies)
        {
            EnemyType enemy = enemyWithRarity.enemyType;
            if (!enemy.TryGetDefinition(out CREnemyDefinition? definition) || !definition._moonWeights.ContainsKey(level))
                continue;

            enemyWithRarity.rarity = definition._moonWeights[level].Calculate(forceRecalculate: true);
        }
    }

    public static EnemyConfig CreateEnemyConfig(ConfigContext section, EnemyData data, string enemyName)
    {
        return new EnemyConfig
        {
            SpawnWeights = section.Bind($"{enemyName} | Spawn Weights", $"Spawn weights for {enemyName}.", data.spawnWeights),
            WeatherMultipliers = section.Bind($"{enemyName} | Weather Multipliers", $"Weather * SpawnWeight multipliers for {enemyName}.", data.weatherMultipliers),
            PowerLevel = section.Bind($"{enemyName} | Power Level", $"Power level for {enemyName}.", data.powerLevel),
            MaxSpawnCount = section.Bind($"{enemyName} | Max Spawn Count", $"Max spawn count for {enemyName}.", data.maxSpawnCount),
        };
    }

    public static void RegisterTo(CRMod mod)
    {
        mod.CreateRegistry(REGISTRY_ID, new CRRegistry<CREnemyDefinition>());
    }

    public override List<EnemyData> GetEntities(CRMod mod)
    {
        return mod.Content.assetBundles.SelectMany(it => it.enemies).ToList();
        // probably should be cached but i dont care anymore.
    }
}