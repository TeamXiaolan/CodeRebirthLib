using System;

namespace CodeRebirthLib.ContentManagement.MapObjects;

[Serializable]
public class InsideMapObjectSettings
{
    public bool spawnFacingAwayFromWall;
    public bool spawnFacingWall;
    public bool spawnWithBackToWall;
    public bool spawnWithBackFlushAgainstWall;
    public bool requireDistanceBetweenSpawns;
    public bool disallowSpawningNearEntrances;
}