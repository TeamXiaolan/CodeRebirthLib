﻿using System;
using System.Text;
using CodeRebirthLib.Extensions;
using CodeRebirthLib.Util.INetworkSerializables;

namespace CodeRebirthLib.ContentManagement.Unlockables.Progressive;
public class ProgressiveUnlockData
{

    public const string LOCKED_NAME = "???";

    public ProgressiveUnlockData(CRUnlockableDefinition definition)
    {
        Definition = definition;
        OriginalName = Unlockable.unlockableName;
        ProgressiveUnlockableHandler.AllProgressiveUnlockables.Add(this);
    }
    public CRUnlockableDefinition Definition { get; }
    public bool IsUnlocked { get; private set; }
    public string OriginalName { get; }

    private UnlockableItem Unlockable => Definition.UnlockableItemDef.unlockable;

    private string SaveID => OriginalName;
    internal uint NetworkID => BitConverter.ToUInt32(Encoding.UTF8.GetBytes(SaveID), 0);

    public void Unlock(HUDDisplayTip? displayTip = null)
    {
        if (IsUnlocked)
            return;

        IsUnlocked = true;
        UpdateName();

        if (displayTip != null)
        {
            HUDManager.Instance.DisplayTip(displayTip);
        }
    }

    public void Load(ES3Settings settings)
    {
        IsUnlocked = ES3.Load(SaveID, false, settings);
        UpdateName();
        CodeRebirthLibPlugin.ExtendedLogging($"IsUnlocked: {IsUnlocked}, Loaded unlockable: {Unlockable.unlockableName} with saveID: {SaveID}");
    }

    public void Save(ES3Settings settings)
    {
        CodeRebirthLibPlugin.ExtendedLogging($"Saving unlockable: {Unlockable.unlockableName} with original name: {OriginalName} that is unlocked: {IsUnlocked} with saveID: {SaveID}");
        ES3.Save(SaveID, IsUnlocked, settings);
    }

    internal void SetFromServer(bool isUnlocked)
    {
        CodeRebirthLibPlugin.ExtendedLogging($"{OriginalName} is being set from the server; unlocked = {isUnlocked}");
        IsUnlocked = isUnlocked;
        UpdateName();
    }

    private void UpdateName()
    {
        Unlockable.unlockableName = IsUnlocked ? OriginalName : LOCKED_NAME;
    }
}