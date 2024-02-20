using HarmonyLib;
using OpenMonitors.Monitors;
using static OpenMonitors.Plugin;

namespace OpenMonitors.Patch;

[HarmonyPatch(typeof(global::DepositItemsDesk))]
public class DepositItemsDesk
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(global::DepositItemsDesk.SellAndDisplayItemProfits))]
    private static void UpdateCreditsAfterSellingLoot()
    {
        Log.LogDebug("DepositItemsDesk.UpdateCreditsAfterSellingLoot");
        CreditsMonitor.Instance.UpdateMonitor();
    }
}
