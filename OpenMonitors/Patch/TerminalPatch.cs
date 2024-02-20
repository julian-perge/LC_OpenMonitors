using System;
using System.Linq;
using HarmonyLib;
using OpenMonitors.Monitors;
using static OpenMonitors.Plugin;

namespace OpenMonitors.Patch;

[HarmonyPatch(typeof(Terminal))]
public class TerminalPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(Terminal.SyncGroupCreditsClientRpc))]
    private static void RefreshMoney()
    {
        Log.LogDebug("Terminal.RefreshMoney");
        CreditsMonitor.Instance.UpdateMonitor();
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(Terminal.TextPostProcess))]
    // ReSharper disable once InconsistentNaming
    private static string HideWeatherConditions(string __result)
    {
        if (Config.HideWeather.Value)
        {
#pragma warning disable Harmony003
            __result = Enum
                .GetValues(typeof(LevelWeatherType))
                .Cast<LevelWeatherType>()
                .Aggregate(__result, (current, value) => current.Replace($"({value})", string.Empty));
#pragma warning restore Harmony003
        }

        return __result;
    }
}
