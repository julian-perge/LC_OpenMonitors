using HarmonyLib;
using OpenMonitors.Monitors;
using static OpenMonitors.Plugin;

namespace OpenMonitors.Patch;

[HarmonyPatch(typeof(TimeOfDay))]
public class TimeOfDayPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(TimeOfDay.SyncNewProfitQuotaClientRpc))]
    private static void UpdateCreditsAfterReachingQuota()
    {
        Log.LogDebug("TimeOfDay.UpdateCreditsAfterReachingQuota");
        CreditsMonitor.Instance.UpdateMonitor();
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(TimeOfDay.MoveTimeOfDay))]
    private static void UpdateClockTime()
    {
        Log.LogDebug("TimeOfDay.UpdateClockTime");
        TimeMonitor.Instance.UpdateMonitor();
    }
}
