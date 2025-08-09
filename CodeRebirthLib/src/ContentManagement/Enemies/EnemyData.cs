﻿using System;

namespace CodeRebirthLib.ContentManagement.Enemies;
[Serializable]
public class EnemyData : EntityData<CREnemyReference>
{
    public string spawnWeights; // to be gotten rid of
    public string weatherMultipliers; // to be gotten rid of
    public string moonSpawnWeights;
    public string interiorSpawnWeights;
    public string weatherSpawnWeights;
    public bool generateSpawnWeightsConfig;
    public float powerLevel;
    public int maxSpawnCount;
}