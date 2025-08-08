using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeRebirthLib.ContentManagement.Dungeons;
using CodeRebirthLib.ContentManagement.Enemies;
using CodeRebirthLib.ContentManagement.Items;
using CodeRebirthLib.ContentManagement.MapObjects;
using CodeRebirthLib.ContentManagement.Unlockables;
using CodeRebirthLib.ContentManagement.Weathers;
using DunGen;
using DunGen.Graph;
using Unity.Netcode;
using UnityEngine;

namespace CodeRebirthLib.ContentManagement;

public static class LethalContent
{
    public static class Levels
    {
        private static readonly List<SelectableLevel> _allLevels = [], _vanillaLevels = [];

        public static IReadOnlyList<SelectableLevel> All => _allLevels.AsReadOnly();
        public static IReadOnlyList<SelectableLevel> Vanilla => _vanillaLevels.AsReadOnly();

        public static SelectableLevel CompanyBuildingLevel { get; private set; }
        public static SelectableLevel ExperimentationLevel { get; private set; }
        public static SelectableLevel MarchLevel { get; private set; }
        public static SelectableLevel VowLevel { get; private set; }
        public static SelectableLevel AssuranceLevel { get; private set; }
        public static SelectableLevel OffenseLevel { get; private set; }
        public static SelectableLevel RendLevel { get; private set; }
        public static SelectableLevel DineLevel { get; private set; }
        public static SelectableLevel TitanLevel { get; private set; }
        public static SelectableLevel AdamanceLevel { get; private set; }
        public static SelectableLevel ArtificeLevel { get; private set; }
        public static SelectableLevel EmbrionLevel { get; private set; }
        public static SelectableLevel LiquidationLevel { get; private set; }

        internal static void Init()
        {
            List<string> unknownTypes = [];

            var levels = StartOfRound.Instance.levels;
            foreach (SelectableLevel level in levels)
            {
                _allLevels.Add(level);
                CodeRebirthLibPlugin.ExtendedLogging($"Found level: {level.name}");

                PropertyInfo? property = typeof(Levels).GetProperty(level.name);
                if (property == null)
                {
                    unknownTypes.Add(level.name);
                }
                else
                {
                    property.SetValue(null, level);
                    _vanillaLevels.Add(level);
                }
            }

            CodeRebirthLibPlugin.ExtendedLogging($"Unknown levels: {string.Join(", ", unknownTypes)}");
        }
    }

    public static class Enemies
    {
        private static readonly List<EnemyType> _allEnemies = [], _vanillaEnemies = [];

        public static IReadOnlyList<EnemyType> All => _allEnemies.AsReadOnly();
        public static IReadOnlyList<EnemyType> Vanilla => _vanillaEnemies.AsReadOnly();
        public static IEnumerable<CREnemyDefinition> CRLib => CRMod.AllMods.SelectMany(mod => mod.EnemyRegistry());

        public static EnemyType Flowerman { get; private set; }
        public static EnemyType Centipede { get; private set; }
        public static EnemyType MouthDog { get; private set; }
        public static EnemyType Crawler { get; private set; }
        public static EnemyType HoarderBug { get; private set; }
        public static EnemyType SandSpider { get; private set; }
        public static EnemyType Blob { get; private set; }
        public static EnemyType ForestGiant { get; private set; }
        public static EnemyType DressGirl { get; private set; }
        public static EnemyType SpringMan { get; private set; }
        public static EnemyType SandWorm { get; private set; }
        public static EnemyType Jester { get; private set; }
        public static EnemyType Puffer { get; private set; }
        public static EnemyType Doublewing { get; private set; }
        public static EnemyType DocileLocustBees { get; private set; }
        public static EnemyType RedLocustBees { get; private set; }
        public static EnemyType BaboonHawk { get; private set; }
        public static EnemyType Nutcracker { get; private set; }
        public static EnemyType MaskedPlayerEnemy { get; private set; }
        public static EnemyType RadMech { get; private set; }
        public static EnemyType Butler { get; private set; }
        public static EnemyType ButlerBees { get; private set; }
        public static EnemyType FlowerSnake { get; private set; }
        public static EnemyType BushWolf { get; private set; }
        public static EnemyType ClaySurgeon { get; private set; }
        public static EnemyType CaveDweller { get; private set; }
        public static EnemyType GiantKiwi { get; private set; }

        internal static void Init()
        {
            List<string> unknownTypes = [];

            for (int i = 0; i < NetworkManager.Singleton.NetworkConfig.Prefabs.Prefabs.Count; i++)
            {
                NetworkPrefab networkPrefab = NetworkManager.Singleton.NetworkConfig.Prefabs.Prefabs[i];
                if (!networkPrefab.Prefab.TryGetComponent(out EnemyAI enemyAI) || enemyAI.enemyType == null)
                    continue;

                _allEnemies.Add(enemyAI.enemyType);
                CodeRebirthLibPlugin.ExtendedLogging($"Found enemy: {enemyAI.enemyType.name}");

                PropertyInfo? property = typeof(Enemies).GetProperty(enemyAI.enemyType.name);
                if (property == null)
                {
                    unknownTypes.Add(enemyAI.enemyType.name);
                }
                else
                {
                    property.SetValue(null, enemyAI.enemyType);
                    _vanillaEnemies.Add(enemyAI.enemyType);
                }
            }
        }
    }

    public static class Items
    {
        private static readonly List<Item> _allItems = [], _vanillaItems = [];

        public static IReadOnlyList<Item> All => _allItems.AsReadOnly();
        public static IReadOnlyList<Item> Vanilla => _vanillaItems.AsReadOnly();
        public static IEnumerable<CRItemDefinition> CRLib => CRMod.AllMods.SelectMany(mod => mod.ItemRegistry());

        public static Item SevenBall { get; private set; }
        public static Item Airhorn { get; private set; }
        public static Item BabyKiwiEgg { get; private set; }
        public static Item Bell { get; private set; }
        public static Item BeltBag { get; private set; }
        public static Item BigBolt { get; private set; }
        public static Item Binoculars { get; private set; }
        public static Item Boombox { get; private set; }
        public static Item BottleBin { get; private set; }
        public static Item Brush { get; private set; }
        public static Item Candy { get; private set; }
        public static Item CardboardBox { get; private set; }
        public static Item CashRegister { get; private set; }
        public static Item CaveDwellerBaby { get; private set; }
        public static Item ChemicalJug { get; private set; }
        public static Item Clipboard { get; private set; }
        public static Item Clock { get; private set; }
        public static Item ClownHorn { get; private set; }
        public static Item Cog1 { get; private set; }
        public static Item ComedyMask { get; private set; }
        public static Item ControlPad { get; private set; }
        public static Item Dentures { get; private set; }
        public static Item DiyFlashBang { get; private set; }
        public static Item DustPan { get; private set; }
        public static Item EasterEgg { get; private set; }
        public static Item EggBeater { get; private set; }
        public static Item EnginePart1 { get; private set; }
        public static Item ExtensionLadder { get; private set; }
        public static Item FancyLamp { get; private set; }
        public static Item FancyPainting { get; private set; }
        public static Item FishTestProp { get; private set; }
        public static Item FlashLaserPointer { get; private set; }
        public static Item Flashlight { get; private set; }
        public static Item Flask { get; private set; }
        public static Item GarbageLid { get; private set; }
        public static Item GiftBox { get; private set; }
        public static Item GoldBar { get; private set; }
        public static Item GunAmmo { get; private set; }
        public static Item Hairdryer { get; private set; }
        public static Item Jetpack { get; private set; }
        public static Item Key { get; private set; }
        public static Item Knife { get; private set; }
        public static Item LockPicker { get; private set; }
        public static Item LungApparatus { get; private set; }
        public static Item MagnifyGlass { get; private set; }
        public static Item MapDevice { get; private set; }
        public static Item MetalSheet { get; private set; }
        public static Item MoldPan { get; private set; }
        public static Item Mug { get; private set; }
        public static Item PerfumeBottle { get; private set; }
        public static Item Phone { get; private set; }
        public static Item PickleJar { get; private set; }
        public static Item PillBottle { get; private set; }
        public static Item PlasticCup { get; private set; }
        public static Item ProFlashlight { get; private set; }
        public static Item RadarBooster { get; private set; }
        public static Item Ragdoll { get; private set; }
        public static Item RedLocustHive { get; private set; }
        public static Item Remote { get; private set; }
        public static Item Ring { get; private set; }
        public static Item RobotToy { get; private set; }
        public static Item RubberDuck { get; private set; }
        public static Item Shotgun { get; private set; }
        public static Item Shovel { get; private set; }
        public static Item SoccerBall { get; private set; }
        public static Item SodaCanRed { get; private set; }
        public static Item SprayPaint { get; private set; }
        public static Item SteeringWheel { get; private set; }
        public static Item StickyNote { get; private set; }
        public static Item StopSign { get; private set; }
        public static Item StunGrenade { get; private set; }
        public static Item TeaKettle { get; private set; }
        public static Item ToiletPaperRolls { get; private set; }
        public static Item Toothpaste { get; private set; }
        public static Item ToyCube { get; private set; }
        public static Item ToyTrain { get; private set; }
        public static Item TragedyMask { get; private set; }
        public static Item TZPInhalant { get; private set; }
        public static Item WalkieTalkie { get; private set; }
        public static Item WeedKillerBottle { get; private set; }
        public static Item WhoopieCushion { get; private set; }
        public static Item YieldSign { get; private set; }
        public static Item ZapGun { get; private set; }
        public static Item Zeddog { get; private set; }

        // todo: add all items or look at automatically generating this somehow so xu doesn't go insasne
        // i been insane

        internal static void Init()
        {
            List<string> unknownTypes = [];

            for (int i = 0; i < NetworkManager.Singleton.NetworkConfig.Prefabs.Prefabs.Count; i++)
            {
                NetworkPrefab networkPrefab = NetworkManager.Singleton.NetworkConfig.Prefabs.Prefabs[i];
                if (!networkPrefab.Prefab.TryGetComponent(out GrabbableObject grabbable) || grabbable.itemProperties == null)
                    continue;

                _allItems.Add(grabbable.itemProperties);
                CodeRebirthLibPlugin.ExtendedLogging($"Found item: {grabbable.itemProperties.name}");

                PropertyInfo? property = typeof(Items).GetProperty(grabbable.itemProperties.name.Replace("7", "Seven")); // todo: make this better
                if (property == null)
                {
                    unknownTypes.Add(grabbable.itemProperties.name);
                }
                else
                {
                    property.SetValue(null, grabbable.itemProperties);
                    _vanillaItems.Add(grabbable.itemProperties);
                }
            }

            CodeRebirthLibPlugin.ExtendedLogging($"Unknown item types: {string.Join(", ", unknownTypes)}");
        }
    }

    public static class MapObjects // unsure how to do this, vanilla doesnt use scriptable objects, im just gonna grab them from the levels but i could do this much earlier if maybe i did it with layers? (fuck you spikerooftrap) (also cant do it earlier because outside map objects arent networked)
    {
        private static readonly List<GameObject> _allMapObjects = [], _vanillaMapObjects = [];

        public static IReadOnlyList<GameObject> All => _allMapObjects.AsReadOnly();
        public static IReadOnlyList<GameObject> Vanilla => _vanillaMapObjects.AsReadOnly();
        public static IEnumerable<CRMapObjectDefinition> CRLib => CRMod.AllMods.SelectMany(mod => mod.MapObjectRegistry());

        public static GameObject Landmine { get; private set; }
        public static GameObject SpikeRoofTrapHazard { get; private set; } // this shit aint even in the right layer breh
        public static GameObject TurretContainer { get; private set; } // TODO imma be honest i have no idea what the vanilla outside map objects are, ill have to log and check later

        public static GameObject LargeRock1 { get; private set; }
        public static GameObject LargeRock2 { get; private set; }
        public static GameObject LargeRock3 { get; private set; }
        public static GameObject LargeRock4 { get; private set; }
        // public static GameObject treeLeaflessBrown.001 Variant { get; private set; } TODO
        public static GameObject GreyRockGrouping2 { get; private set; }
        public static GameObject GiantPumpkin { get; private set; }
        public static GameObject tree { get; private set; }
        public static GameObject GreyRockGrouping4 { get; private set; }
        // public static GameObject treeLeafless.002_LOD0 { get; private set; } TODO
        // public static GameObject treeLeafless.003_LOD0 { get; private set; } TODO


        internal static void Init()
        {
            List<string> unknownTypes = [];

            foreach (var level in StartOfRound.Instance.levels)
            {
                foreach (var insideMapObject in level.spawnableMapObjects)
                {
                    if (insideMapObject.prefabToSpawn == null || All.Contains(insideMapObject.prefabToSpawn))
                        continue;

                    _allMapObjects.Add(insideMapObject.prefabToSpawn);
                    CodeRebirthLibPlugin.ExtendedLogging($"Found Inside Map Object: {insideMapObject.prefabToSpawn.name}");

                    PropertyInfo? property = typeof(GameObject).GetProperty(insideMapObject.prefabToSpawn.name); // todo: make this better
                    if (property == null)
                    {
                        unknownTypes.Add(insideMapObject.prefabToSpawn.name);
                    }
                    else
                    {
                        property.SetValue(null, insideMapObject.prefabToSpawn);
                        _vanillaMapObjects.Add(insideMapObject.prefabToSpawn);
                    }
                }

                foreach (var outsideMapObject in level.spawnableOutsideObjects)
                {
                    if (outsideMapObject.spawnableObject.prefabToSpawn == null || All.Contains(outsideMapObject.spawnableObject.prefabToSpawn))
                        continue;

                    _allMapObjects.Add(outsideMapObject.spawnableObject.prefabToSpawn);
                    CodeRebirthLibPlugin.ExtendedLogging($"Found Outside Map Object: {outsideMapObject.spawnableObject.prefabToSpawn.name}");

                    PropertyInfo? property = typeof(GameObject).GetProperty(outsideMapObject.spawnableObject.prefabToSpawn.name); // todo: make this better
                    if (property == null)
                    {
                        unknownTypes.Add(outsideMapObject.spawnableObject.prefabToSpawn.name);
                    }
                    else
                    {
                        property.SetValue(null, outsideMapObject.spawnableObject.prefabToSpawn);
                        _vanillaMapObjects.Add(outsideMapObject.spawnableObject.prefabToSpawn);
                    }
                }
            }

            CodeRebirthLibPlugin.ExtendedLogging($"Unknown map object types: {string.Join(", ", unknownTypes)}");
        }
    }

    public static class Dungeons
    {
        private static readonly List<DungeonFlow> _allFlows = [], _vanillaFlows = [];

        public static IReadOnlyList<DungeonFlow> All = _allFlows.AsReadOnly();
        public static IReadOnlyList<DungeonFlow> Vanilla = _vanillaFlows.AsReadOnly();
        public static IEnumerable<CRAdditionalTilesDefinition> CRLib => CRMod.AllMods.SelectMany(mod => mod.AdditionalTilesRegistry());

        public static DungeonFlow Level1Flow { get; private set; }
        public static DungeonFlow Level1Flow3Exits { get; private set; } // todo: does this get replaced by LLL?
        public static DungeonFlow Level1FlowExtraLarge { get; private set; }
        public static DungeonFlow Level2Flow { get; private set; }
        public static DungeonFlow Level3Flow { get; private set; }

        private static readonly List<DoorwaySocket> _vanillaSockets = [];
        public static IReadOnlyList<DoorwaySocket> VanillaDoorSockets = _vanillaSockets.AsReadOnly();

        internal static void Init()
        {
            List<string> unknownTypes = [];

            foreach (DungeonFlow flow in RoundManager.Instance.dungeonFlowTypes.Select(i => i.dungeonFlow))
            {
                _allFlows.Add(flow);
                CodeRebirthLibPlugin.ExtendedLogging($"Found dungeon flow: {flow.name}");

                PropertyInfo property = typeof(Dungeons).GetProperty(flow.name);
                if (property == null)
                {
                    unknownTypes.Add(flow.name);
                }
                else
                {
                    property.SetValue(null, flow);
                    _vanillaFlows.Add(flow);
                }
            }

            CodeRebirthLibPlugin.ExtendedLogging($"Unknown dungeon flows: {string.Join(", ", unknownTypes)}");

            foreach (DungeonFlow flow in Vanilla)
            {
                foreach (GameObjectChance chance in flow.GetUsedTileSets().Select(it => it.TileWeights.Weights).SelectMany(it => it))
                {
                    Doorway[] doorways = chance.Value.GetComponentsInChildren<Doorway>();

                    foreach (Doorway doorway in doorways)
                    {
                        if (_vanillaSockets.Contains(doorway.socket))
                            continue;

                        _vanillaSockets.Add(doorway.socket);
                    }
                }
            }
        }
    }

    public static class Unlockables
    {
        private static readonly List<UnlockableItem> _allUnlockables = [], _vanillaUnlockables = [];

        public static IReadOnlyList<UnlockableItem> All = _allUnlockables.AsReadOnly();
        public static IReadOnlyList<UnlockableItem> Vanilla = _vanillaUnlockables.AsReadOnly();
        public static IEnumerable<CRUnlockableDefinition> CRLib => CRMod.AllMods.SelectMany(mod => mod.UnlockableRegistry());

        public static UnlockableItem Orangesuit { get; private set; }
        public static UnlockableItem Greensuit { get; private set; }
        public static UnlockableItem Hazardsuit { get; private set; }
        public static UnlockableItem Pajamasuit { get; private set; }
        public static UnlockableItem Cozylights { get; private set; }
        public static UnlockableItem Teleporter { get; private set; }
        public static UnlockableItem Television { get; private set; }
        public static UnlockableItem Cupboard { get; private set; }
        public static UnlockableItem FileCabinet { get; private set; }
        public static UnlockableItem Toilet { get; private set; }
        public static UnlockableItem Shower { get; private set; }
        public static UnlockableItem Lightswitch { get; private set; }
        public static UnlockableItem Recordplayer { get; private set; }
        public static UnlockableItem Table { get; private set; }
        public static UnlockableItem Romantictable { get; private set; }
        public static UnlockableItem Bunkbeds { get; private set; }
        public static UnlockableItem Terminal { get; private set; }
        public static UnlockableItem Signaltranslator { get; private set; }
        public static UnlockableItem Loudhorn { get; private set; }
        public static UnlockableItem InverseTeleporter { get; private set; }
        public static UnlockableItem JackOLantern { get; private set; }
        public static UnlockableItem Welcomemat { get; private set; }
        public static UnlockableItem Goldfish { get; private set; }
        public static UnlockableItem Plushiepajamaman { get; private set; }
        public static UnlockableItem PurpleSuit { get; private set; }
        public static UnlockableItem BeeSuit { get; private set; }
        public static UnlockableItem BunnySuit { get; private set; }
        public static UnlockableItem DiscoBall { get; private set; }
        public static UnlockableItem Microwave { get; private set; }
        public static UnlockableItem Sofachair { get; private set; }
        public static UnlockableItem Fridge { get; private set; }
        public static UnlockableItem Classicpainting { get; private set; }
        public static UnlockableItem Electricchair { get; private set; }
        public static UnlockableItem Doghouse { get; private set; }

        internal static void Init()
        {
            List<string> unknownTypes = [];
            foreach (var unlockableItem in StartOfRound.Instance.unlockablesList.unlockables)
            {
                _allUnlockables.Add(unlockableItem);
                CodeRebirthLibPlugin.ExtendedLogging($"Found UnlockableItem: {unlockableItem.unlockableName}");

                PropertyInfo property = typeof(UnlockableItem).GetProperty(unlockableItem.unlockableName.Replace(" ", "")); // TODO: this doesnt work for some reason
                if (property == null)
                {
                    unknownTypes.Add(unlockableItem.unlockableName);
                }
                else
                {
                    property.SetValue(null, unlockableItem);
                    _vanillaUnlockables.Add(unlockableItem);
                }
            }

            CodeRebirthLibPlugin.ExtendedLogging($"Unknown UnlockableItems: {string.Join(", ", unknownTypes)}");
        }
    }

    public static class Weathers
    {
        private static readonly List<WeatherEffect> _allWeathers = [], _vanillaWeathers = [];

        public static IReadOnlyList<WeatherEffect> All = _allWeathers.AsReadOnly();
        public static IReadOnlyList<WeatherEffect> Vanilla = _vanillaWeathers.AsReadOnly(); // huh so this is a thing that mrov just copy pasted from vanilla, i cant tell the differences?
        public static IEnumerable<CRWeatherDefinition> CRLib => CRMod.AllMods.SelectMany(mod => mod.WeatherRegistry());

        public static WeatherEffect rollinggroundfog { get; private set; }
        public static WeatherEffect rainy { get; private set; }
        public static WeatherEffect stormy { get; private set; }
        public static WeatherEffect foggy { get; private set; }
        public static WeatherEffect flooded { get; private set; }
        public static WeatherEffect eclipsed { get; private set; }

        internal static void Init()
        {
            List<string> unknownTypes = [];
            foreach (var weatherEffect in TimeOfDay.Instance.effects)
            {
                _allWeathers.Add(weatherEffect);
                CodeRebirthLibPlugin.ExtendedLogging($"Found WeatherEffect: {weatherEffect.name}");

                PropertyInfo property = typeof(WeatherEffect).GetProperty(weatherEffect.name.Replace(" ", "")); // TODO: this doesnt work for some reason
                if (property == null)
                {
                    unknownTypes.Add(weatherEffect.name);
                }
                else
                {
                    property.SetValue(null, weatherEffect);
                    _vanillaWeathers.Add(weatherEffect);
                }
            }

            CodeRebirthLibPlugin.ExtendedLogging($"Unknown WeatherEffects: {string.Join(", ", unknownTypes)}");
        }
    }
}