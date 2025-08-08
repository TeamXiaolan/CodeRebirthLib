using System.Collections.Generic;
using System.Linq;
using CodeRebirthLib.ContentManagement.Unlockables.Progressive;
using CodeRebirthLib.Data;
using UnityEngine;

namespace CodeRebirthLib.Patches.ContentRegistration;

static class UnlockableRegistrationHandler
{
    static readonly List<UnlockableItemRegistrationSettings> _unlockablesToInject = [];

    internal static void Init()
    {
        On.Terminal.LoadNewNodeIfAffordable += Terminal_LoadNewNodeIfAffordable;
        On.StartOfRound.Awake += StartOfRound_Awake; // maybe patch terminal instead and await for startofround's instance to be not null?
    }

    public static void AddUnlockableToShop(UnlockableItemRegistrationSettings unlockableItemRegistrationSettings)
    {
        _unlockablesToInject.Add(unlockableItemRegistrationSettings);
    }

    private static void StartOfRound_Awake(On.StartOfRound.orig_Awake orig, StartOfRound self)
    {
        orig(self);

        Terminal terminal = GameObject.FindObjectOfType<Terminal>(); // TODO not do this lol, should maybe store terminal's reference somewhere
        TerminalKeyword buyKeyword = terminal.terminalNodes.allKeywords.First(keyword => keyword.word == "buy");
        TerminalKeyword confirmPurchaseKeyword = terminal.terminalNodes.allKeywords.First(keyword2 => keyword2.word == "confirm");
        TerminalKeyword denyPurchaseKeyword = terminal.terminalNodes.allKeywords.First(keyword2 => keyword2.word == "deny");
        TerminalNode cancelPurchaseNode = buyKeyword.compatibleNouns[0].result.terminalOptions[1].result; // TODO, I use these a couple times, maybe i should have em stored somewhere in LethalContent?

        UnlockableItem latestValidUnlockable = StartOfRound.Instance.unlockablesList.unlockables.Where(unlockable => unlockable.shopSelectionNode != null).OrderBy(x => x.shopSelectionNode.shipUnlockableID).FirstOrDefault();
        int latestUnlockableID = latestValidUnlockable.shopSelectionNode.shipUnlockableID;
        CodeRebirthLibPlugin.ExtendedLogging($"latestUnlockableID = {latestUnlockableID}");

        foreach (var unlockableRegistrationSetting in _unlockablesToInject)
        {
            StartOfRound.Instance.unlockablesList.unlockables.Add(unlockableRegistrationSetting.UnlockableItem);
            TerminalNode shopSelectionNode = ScriptableObject.CreateInstance<TerminalNode>(); // unsure if its relevant but for some reason some ship upgrades dont have this, might not be needed and game might auto generate them.
            shopSelectionNode.displayText = $"You have requested to order a {unlockableRegistrationSetting.UnlockableItem.unlockableName.ToLowerInvariant()}.\nTotal cost of item: [totalCost].\n\nPlease CONFIRM or DENY.";
            shopSelectionNode.clearPreviousText = true;
            shopSelectionNode.maxCharactersToType = 25;
            shopSelectionNode.shipUnlockableID = latestUnlockableID;
            shopSelectionNode.itemCost = unlockableRegistrationSetting.Cost;
            shopSelectionNode.creatureName = unlockableRegistrationSetting.UnlockableItem.unlockableName;
            shopSelectionNode.overrideOptions = true;

            CompatibleNoun confirmBuyCompatibleNoun = new();
            confirmBuyCompatibleNoun.noun = confirmPurchaseKeyword;
            confirmBuyCompatibleNoun.result = CreateUnlockableConfirmNode(unlockableRegistrationSetting.UnlockableItem, latestUnlockableID, unlockableRegistrationSetting.Cost);

            CompatibleNoun cancelDenyCompatibleNoun = new();
            cancelDenyCompatibleNoun.noun = denyPurchaseKeyword;
            cancelDenyCompatibleNoun.result = cancelPurchaseNode;

            shopSelectionNode.terminalOptions = [confirmBuyCompatibleNoun, cancelDenyCompatibleNoun];
            unlockableRegistrationSetting.UnlockableItem.shopSelectionNode = shopSelectionNode;
            latestUnlockableID++;
            // TODO: bruh while doing this i just realised decors are literally just ship upgrades without "Always In Stock" ticked, like i thought it'd be more than that lol but fair enough ig, means i need to clone this basically.
        }
        // TODO: handle adding stuff like suits etc and not just placeable gameobjects maybe?
    }

    private static TerminalNode CreateUnlockableConfirmNode(UnlockableItem unlockableItem, int latestUnlockableID, int cost)
    {
        TerminalNode terminalNode = ScriptableObject.CreateInstance<TerminalNode>();
        terminalNode.displayText = $"Ordered the {unlockableItem.unlockableName.ToLowerInvariant()}! Your new balance is [playerCredits].\nPress [B] to rearrange objects in your ship and [V] to confirm.";
        terminalNode.clearPreviousText = true;
        terminalNode.maxCharactersToType = 35;
        terminalNode.shipUnlockableID = latestUnlockableID;
        terminalNode.buyUnlockable = true;
        terminalNode.itemCost = cost;
        terminalNode.playSyncedClip = 0;
        return terminalNode;
    }

    private static void Terminal_LoadNewNodeIfAffordable(On.Terminal.orig_LoadNewNodeIfAffordable orig, Terminal self, TerminalNode node)
    {
        if (node.shipUnlockableID != -1)
        {
            UnlockableItem unlockableItem = StartOfRound.Instance.unlockablesList.unlockables[node.shipUnlockableID];
            ProgressiveUnlockData? unlockData = ProgressiveUnlockableHandler.AllProgressiveUnlockables
                .FirstOrDefault(it => it.Definition.UnlockableItem == unlockableItem);

            if (unlockData != null && !unlockData.IsUnlocked)
            {
                orig(self, unlockData.Definition.ProgressiveDenyNode);
                return;
            }
        }
        orig(self, node);
    }
}