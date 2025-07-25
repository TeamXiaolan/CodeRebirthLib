﻿using System.Linq;
using CodeRebirthLib.Util.INetworkSerializables;
using GameNetcodeStuff;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

namespace CodeRebirthLib.ContentManagement.Unlockables.Progressive;
public class UnlockShipUnlockable : NetworkBehaviour
{
    [FormerlySerializedAs("interactTrigger")] [SerializeField]
    private InteractTrigger _interactTrigger = null!;

    private void Start()
    {
        _interactTrigger.onInteract.AddListener(OnInteract);
    }

    private void OnInteract(PlayerControllerB player)
    {
        if (player != GameNetworkManager.Instance.localPlayerController || player.currentlyHeldObjectServer is not UnlockableUpgradeScrap) return;
        UnlockShipUpgradeServerRpc(player);
    }

    [ServerRpc(RequireOwnership = false)]
    private void UnlockShipUpgradeServerRpc(PlayerControllerReference reference)
    {
        UnlockShipUpgradeClientRpc(reference);
    }

    [ClientRpc]
    private void UnlockShipUpgradeClientRpc(PlayerControllerReference reference)
    {
        PlayerControllerB player = reference; // implict cast
        if (player.currentlyHeldObjectServer is UnlockableUpgradeScrap unlockableUpgradeScrap)
        {
            ProgressiveUnlockData unlockData = ProgressiveUnlockableHandler.AllProgressiveUnlockables
                .First(it => it.Definition.UnlockableItemDef == unlockableUpgradeScrap.UnlockableItemDef);
            unlockData.Unlock(
                new HUDDisplayTip(
                    "Assembled Parts",
                    $"Congratulations on finding the parts, Unlocked {unlockData.OriginalName}."
                )
            );
            if (unlockableUpgradeScrap.IsOwner) player.DespawnHeldObject();
        }
        else
        {
            CodeRebirthLibPlugin.Logger.LogError("UnlockableUpgradeScrap is null, how did you even get here????");
        }
    }
}