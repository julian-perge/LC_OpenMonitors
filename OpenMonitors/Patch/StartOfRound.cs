using System.Text;
using System.Text.RegularExpressions;
using BepInEx.Configuration;
using HarmonyLib;
using OpenMonitors.Monitors;
using TMPro;
using static OpenMonitors.Plugin;

namespace OpenMonitors.Patch;

[HarmonyPatch(typeof(global::StartOfRound))]
public class StartOfRound
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(global::StartOfRound.Start))]
    private static void Initialize()
    {
        ModLogger.LogDebug("StartOfRound.Initialize");
        Setup.Initialize();
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(global::StartOfRound.ReviveDeadPlayers))]
    private static void RefreshMonitorsWhenPlayerRevives()
    {
        ModLogger.LogDebug("StartOfRound.RefreshMonitorsWhenPlayerRevives");
        CreditsMonitor.Instance.UpdateMonitor();
        DayMonitor.Instance.UpdateMonitor();
        LifeSupportMonitor.Instance.UpdateMonitor();
        LootMonitor.Instance.UpdateMonitor();
        PlayersLifeSupportMonitor.Instance.UpdateMonitor();
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(global::StartOfRound.SyncShipUnlockablesClientRpc))]
    private static void RefreshLootForClientOnStart()
    {
        ModLogger.LogDebug("StartOfRound.RefreshLootForClientOnStart");
        LootMonitor.Instance.UpdateMonitor();
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(global::StartOfRound.ChangeLevelClientRpc))]
    private static void UpdateCreditsWhenSwitchingMoons()
    {
        ModLogger.LogDebug("StartOfRound.UpdateCreditsWhenSwitchingMoons");
        CreditsMonitor.Instance.UpdateMonitor();
    }


    [HarmonyPostfix]
    [HarmonyPatch(nameof(global::StartOfRound.EndOfGameClientRpc))]
    private static void RefreshDayWhenShipHasLeft()
    {
        ModLogger.LogDebug("StartOfRound.RefreshDayWhenShipHasLeft");
        DayMonitor.Instance.UpdateMonitor();
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(global::StartOfRound.StartGame))]
    private static void UpdateDayAtStartOfGame()
    {
        ModLogger.LogDebug("StartOfRound.UpdateDayAtStartOfGame");
        DayMonitor.Instance.UpdateMonitor();
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(global::StartOfRound.OnClientConnect))]
    private static void UpdateMonitorsWhenPlayerConnectsClient(ulong clientId)
    {
        ModLogger.LogDebug("StartOfRound.UpdateMonitorsWhenPlayerConnectsClient");
        CreditsMonitor.Instance.UpdateMonitor();
        LifeSupportMonitor.Instance.UpdateMonitor();
        PlayersLifeSupportMonitor.Instance.UpdateMonitor();
        LootMonitor.Instance.UpdateMonitor();
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(global::StartOfRound.OnPlayerConnectedClientRpc))]
    private static void UpdateMonitorsWhenPlayerConnectsClientRpc(
        ulong clientId,
        int connectedPlayers,
        ulong[] connectedPlayerIdsOrdered,
        int assignedPlayerObjectId,
        int serverMoneyAmount,
        int levelID,
        int profitQuota,
        int timeUntilDeadline,
        int quotaFulfilled,
        int randomSeed
    )
    {
        ModLogger.LogDebug("StartOfRound.UpdateMonitorsWhenPlayerConnectsClientRpc");
        CreditsMonitor.Instance.UpdateMonitor();
        LifeSupportMonitor.Instance.UpdateMonitor();
        LootMonitor.Instance.UpdateMonitor();
        PlayersLifeSupportMonitor.Instance.UpdateMonitor();
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(global::StartOfRound.OnPlayerDC))]
    private static void UpdateMonitorsWhenPlayerDisconnects(int playerObjectNumber, ulong clientId)
    {
        ModLogger.LogDebug("StartOfRound.UpdateMonitorsWhenPlayerDisconnects");
        CreditsMonitor.Instance.UpdateMonitor();
        LifeSupportMonitor.Instance.UpdateMonitor();
        LootMonitor.Instance.UpdateMonitor();
        PlayersLifeSupportMonitor.Instance.UpdateMonitor();
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(global::StartOfRound.SetMapScreenInfoToCurrentLevel))]
    // ReSharper disable twice InconsistentNaming
    private static void ColorWeather(ref TextMeshProUGUI ___screenLevelDescription, ref SelectableLevel ___currentLevel)
    {
        var builder = new StringBuilder("Orbiting: ").AppendLine(___currentLevel.PlanetName);

        if (!Config.HideWeather.Value)
        {
            builder
                .Append("Weather: ")
                .AppendLine(FormatWeather(___currentLevel.currentWeather));
        }

        builder.Append(___currentLevel.LevelDescription ?? string.Empty);

        ___screenLevelDescription.text = builder.ToString();
    }

    private static string FormatWeather(LevelWeatherType currentWeather)
    {
        ModLogger.LogDebug($"Weather: {currentWeather}");

        // Verifies the config is a valid hex color code, or defaults to `LimeGreen`
        string text = currentWeather switch
        {
            LevelWeatherType.DustClouds => ParseColorInput(Config.DustCloudsWeatherColor),
            LevelWeatherType.Eclipsed => ParseColorInput(Config.EclipsedWeatherColor),
            LevelWeatherType.Flooded => ParseColorInput(Config.FloodedWeatherColor),
            LevelWeatherType.Foggy => ParseColorInput(Config.FoggyWeatherColor),
            LevelWeatherType.None => ParseColorInput(Config.NoneWeatherColor),
            LevelWeatherType.Rainy => ParseColorInput(Config.RainyWeatherColor),
            LevelWeatherType.Stormy => ParseColorInput(Config.StormyWeatherColor),
            _ => Config.NoneWeatherColor.DefaultValue.ToString()
        };
        return $"<color=#{text}>{currentWeather}</color>";
    }

    private static string ParseColorInput(ConfigEntry<string> input)
    {
        // Matches any 6 character combination of digits and case-insensitive letters
        Regex reg = new Regex(@"(?i)[0-9a-f]{6}");
        return reg.IsMatch(input.Value.Replace("#", "")) ? input.Value : input.DefaultValue.ToString();
    }
}
