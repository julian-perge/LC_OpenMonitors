using HarmonyLib;
using OpenMonitors.Monitors;
using static OpenMonitors.Plugin;

namespace OpenMonitors.Patch;

[HarmonyPatch(typeof(DepositItemsDesk))]
public class DepositItemsDeskPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(DepositItemsDesk.SellAndDisplayItemProfits))]
    private static void UpdateCreditsAfterSellingLoot()
    {
        Log.LogDebug("DepositItemsDesk.UpdateCreditsAfterSellingLoot");
        CreditsMonitor.Instance.UpdateMonitor();
    }
}
