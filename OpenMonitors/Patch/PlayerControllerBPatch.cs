using System.Collections;
using DunGen;
using GameNetcodeStuff;
using HarmonyLib;
using OpenMonitors.Monitors;
using Unity.Netcode;
using UnityEngine;
using static OpenMonitors.Plugin;

namespace OpenMonitors.Patch;

[HarmonyPatch(typeof(PlayerControllerB))]
public class PlayerControllerBPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(PlayerControllerB.ConnectClientToPlayerObject))]
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
    }


    [HarmonyPostfix]
    [HarmonyPatch(nameof(PlayerControllerB.GrabObjectClientRpc))]
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
    [HarmonyPatch(nameof(PlayerControllerB.ThrowObjectClientRpc))]
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
    [HarmonyPatch(nameof(PlayerControllerB.KillPlayer))]
    private static void UpdateLifeSupportMonitorOnPlayerDeath(
        PlayerControllerB __instance,
        Vector3 bodyVelocity,
        bool spawnBody,
        CauseOfDeath causeOfDeath,
        int deathAnimation
    )
    {
        Log.LogDebug("PlayerControllerB.UpdateLifeSupportMonitorOnPlayerDeath");
        LifeSupportMonitor.Instance.UpdateMonitor();
    }
}
