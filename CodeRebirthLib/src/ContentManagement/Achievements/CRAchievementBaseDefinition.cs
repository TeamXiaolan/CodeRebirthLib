using CodeRebirthLib.AssetManagement;
using UnityEngine;

namespace CodeRebirthLib.ContentManagement.Achievements;

public abstract class CRAchievementBaseDefinition : CRContentDefinition
{
    public const string REGISTRY_ID = "achievements";

    [field: SerializeField]
    public Sprite AchievementIcon { get; private set; }
    [field: SerializeField]
    public string AchievementName { get; private set; }
    [field: SerializeField]
    public string AchievementDescription { get; private set; }
    [field: SerializeField]
    public string AchievementRequirement { get; private set; }
    [field: SerializeField]
    public bool IsHidden { get; private set; }

    public bool Completed { get; protected set; } = false;
    public virtual void LoadAchievementState(ES3Settings globalSettings)
    {
        Completed = ES3.Load(Mod.Plugin.GUID + "." + AchievementName, false, globalSettings);
        CodeRebirthLibPlugin.ExtendedLogging($"Loaded Achievement: {AchievementName} with value: {Completed}");
    }

    public virtual void SaveAchievementState(ES3Settings globalSettings)
    {
        ES3.Save(Mod.Plugin.GUID + "." + AchievementName, Completed, globalSettings);
        CodeRebirthLibPlugin.ExtendedLogging($"Saving Achievement: {AchievementName} with value: {Completed}");
    }

    protected bool TryCompleteAchievement()
    {
        if (Completed)
        {
            return false;
        }

        Completed = true;
        return Completed;
    }

    public static void RegisterTo(CRMod mod)
    {
        mod.CreateRegistry(REGISTRY_ID, new CRRegistry<CRAchievementBaseDefinition>());
    }

    public override void Register(CRMod mod)
    {
        base.Register(mod);
        mod.AchievementRegistry().Register(this);
    }
}