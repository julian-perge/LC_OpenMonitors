using System.Collections;
using DunGen;
using GameNetcodeStuff;
using HarmonyLib;
using OpenMonitors.Monitors;
using Unity.Netcode;
using UnityEngine;
using static OpenMonitors.Plugin;

namespace OpenMonitors.Patch;

[HarmonyPatch(typeof(GameNetcodeStuff.PlayerControllerB))]
public class PlayerControllerB
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(GameNetcodeStuff.PlayerControllerB.ConnectClientToPlayerObject))]
    private static void OnPlayerConnect()
    {
        CoroutineHelper.Instance.StartCoroutine(WaitOnPlayerConnectForMonitorsToBeCreated());
    }

    private static IEnumerator WaitOnPlayerConnectForMonitorsToBeCreated()
    {
        Log.LogDebug("WaitOnPlayerConnectForMonitorsToBeCreated");
        yield return new WaitUntil(
            () => Object.FindObjectOfType<CreditsMonitor>() && Object.FindObjectOfType<LifeSupportMonitor>()
        );
        CreditsMonitor.Instance.UpdateMonitor();
        LifeSupportMonitor.Instance.UpdateMonitor();
        PlayersLifeSupportMonitor.Instance.UpdateMonitor();
    }


    [HarmonyPostfix]
    [HarmonyPatch(nameof(GameNetcodeStuff.PlayerControllerB.GrabObjectClientRpc))]
    private static void RefreshLootOnPickupClient(bool grabValidated, ref NetworkObjectReference grabbedObject)
    {
        if (grabbedObject.TryGet(out var networkObject))
        {
            var componentInChildren = networkObject.gameObject.GetComponentInChildren<GrabbableObject>();
            if (componentInChildren.isInShipRoom || componentInChildren.isInElevator)
            {
                Log.LogDebug("PlayerControllerB.RefreshLootOnPickupClient");
                LootMonitor.Instance.UpdateMonitor();
            }
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(GameNetcodeStuff.PlayerControllerB.ThrowObjectClientRpc))]
    private static void RefreshLootOnThrowClient(
        bool droppedInElevator,
        bool droppedInShipRoom,
        Vector3 targetFloorPosition,
        NetworkObjectReference grabbedObject
    )
    {
        if (droppedInShipRoom || droppedInElevator)
        {
            Log.LogDebug("PlayerControllerB.RefreshLootOnThrowClient");
            LootMonitor.Instance.UpdateMonitor();
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(GameNetcodeStuff.PlayerControllerB.KillPlayer))]
    private static void UpdateLifeSupportMonitorOnPlayerDeath(
        Vector3 bodyVelocity,
        bool spawnBody,
        CauseOfDeath causeOfDeath,
        int deathAnimation
    )
    {
        Log.LogDebug("PlayerControllerB.UpdateLifeSupportMonitorOnPlayerDeath");
        LifeSupportMonitor.Instance.UpdateMonitor();
        PlayersLifeSupportMonitor.Instance.UpdateMonitor();
    }
    
    [HarmonyPostfix]
    [HarmonyPatch(nameof(GameNetcodeStuff.PlayerControllerB.KillPlayerClientRpc))]
    private static void UpdateLifeSupportMonitorOnPlayerDeathClientRpc(
        int playerId,
        bool spawnBody,
        Vector3 bodyVelocity,
        int causeOfDeath,
        int deathAnimation
    )
    {
        Log.LogDebug("PlayerControllerB.UpdateLifeSupportMonitorOnPlayerDeathClientRpc");
        LifeSupportMonitor.Instance.UpdateMonitor();
        PlayersLifeSupportMonitor.Instance.UpdateMonitor();
    }
}
