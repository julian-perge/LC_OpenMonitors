using HarmonyLib;
using OpenMonitors.Monitors;
using static OpenMonitors.Plugin;

namespace OpenMonitors.Patch;

[HarmonyPatch(typeof(HUDManager))]
public class HudManagerPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(HUDManager.ApplyPenalty))]
    private static void UpdateCreditsAfterDeadPlayersPenalty()
    {
        Log.LogDebug("HUDManager.UpdateCreditsAfterDeadPlayersPenalty");
        CreditsMonitor.Instance.UpdateMonitor();
    }
}
