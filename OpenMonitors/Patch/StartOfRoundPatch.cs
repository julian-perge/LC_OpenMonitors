using System.Text;
using HarmonyLib;
using OpenMonitors.Monitors;
using TMPro;
using static OpenMonitors.Plugin;

namespace OpenMonitors.Patch;

[HarmonyPatch(typeof(StartOfRound))]
public class StartOfRoundPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(StartOfRound.Start))]
    private static void Initialize()
    {
        Log.LogDebug("StartOfRound.Initialize");
        Setup.Initialize();
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(StartOfRound.ReviveDeadPlayers))]
    private static void RefreshMonitorsWhenPlayerRevives()
    {
        Log.LogDebug("StartOfRound.RefreshMonitorsWhenPlayerRevives");
        CreditsMonitor.Instance.UpdateMonitor();
        DayMonitor.Instance.UpdateMonitor();
        LifeSupportMonitor.Instance.UpdateMonitor();
        LootMonitor.Instance.UpdateMonitor();
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(StartOfRound.SyncShipUnlockablesClientRpc))]
    private static void RefreshLootForClientOnStart()
    {
        Log.LogDebug("StartOfRound.RefreshLootForClientOnStart");
        LootMonitor.Instance.UpdateMonitor();
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(StartOfRound.ChangeLevelClientRpc))]
    private static void UpdateCreditsWhenSwitchingMoons()
    {
        Log.LogDebug("StartOfRound.UpdateCreditsWhenSwitchingMoons");
        CreditsMonitor.Instance.UpdateMonitor();
    }


    [HarmonyPostfix]
    [HarmonyPatch(nameof(StartOfRound.EndOfGameClientRpc))]
    private static void RefreshDayWhenShipHasLeft()
    {
        Log.LogDebug("StartOfRound.RefreshDayWhenShipHasLeft");
        DayMonitor.Instance.UpdateMonitor();
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(StartOfRound.StartGame))]
    private static void UpdateDayAtStartOfGame()
    {
        Log.LogDebug("StartOfRound.UpdateDayAtStartOfGame");
        DayMonitor.Instance.UpdateMonitor();
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(StartOfRound.SetMapScreenInfoToCurrentLevel))]
    // ReSharper disable twice InconsistentNaming
    private static void ColorWeather(ref TextMeshProUGUI ___screenLevelDescription, ref SelectableLevel ___currentLevel)
    {
        ___screenLevelDescription.text = new StringBuilder()
            .Append("Orbiting: ")
            .Append(___currentLevel.PlanetName)
            .AppendLine()
            .Append("Weather: ")
            .Append(FormatWeather(___currentLevel.currentWeather))
            .AppendLine()
            .Append(___currentLevel.LevelDescription ?? string.Empty)
            .ToString();
    }

    private static string FormatWeather(LevelWeatherType currentWeather)
    {
        Log.LogDebug($"Weather: {currentWeather}");
        string text;
        switch (currentWeather)
        {
            case LevelWeatherType.Rainy:
            case LevelWeatherType.Foggy:
                // yellow
                text = "FFF01C";
                break;
            case LevelWeatherType.Stormy:
            case LevelWeatherType.Flooded:
                // orange
                text = "FF9B00";
                break;
            case LevelWeatherType.Eclipsed:
                // red
                text = "FF0000";
                break;
            case LevelWeatherType.None:
            case LevelWeatherType.DustClouds:
            default:
                // lime green
                text = "69FF69";
                break;
        }

        return $"<color=#{text}>{currentWeather}</color>";
    }
}
