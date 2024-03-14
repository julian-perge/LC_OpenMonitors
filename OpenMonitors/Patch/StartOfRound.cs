using System.Text;
using HarmonyLib;
using OpenMonitors.Monitors;
using TMPro;
using static OpenMonitors.Plugin;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System;
using UnityEngine;

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
        ___screenLevelDescription.text = new StringBuilder()
            .Append("Orbiting: ")
            .AppendLine(___currentLevel.PlanetName)
            .Append("Weather: ")
            .AppendLine(FormatWeather(___currentLevel.currentWeather))
            .Append(___currentLevel.LevelDescription ?? string.Empty)
            .ToString();
    }

    private static string FormatWeather(LevelWeatherType currentWeather)
    {
        ModLogger.LogDebug($"Weather: {currentWeather}");
        string text = "";           

        //if current weather has 6-digit hex color in config, calls regex parser to make sure it is correct
        //then sets that value to text var
        switch (currentWeather)
        {
            case LevelWeatherType.Rainy:
                text = ParseColorInput(Config.RainyWeatherColor.Value.ToString());
                break;
            case LevelWeatherType.Foggy:
                text = ParseColorInput(Config.FoggyWeatherColor.Value.ToString());
                break;
            case LevelWeatherType.Stormy:
                text = ParseColorInput(Config.StormyWeatherColor.Value.ToString());
                break;
            case LevelWeatherType.Flooded:
                text = ParseColorInput(Config.FloodedWeatherColor.Value.ToString());
                break;
            case LevelWeatherType.Eclipsed:
                text = ParseColorInput(Config.EclipsedWeatherColor.Value.ToString());
                break;
            case LevelWeatherType.None:
                text = ParseColorInput(Config.NoneWeatherColor.Value.ToString());
                break;
            case LevelWeatherType.DustClouds:
                text = ParseColorInput(Config.DustCloudsWeatherColor.Value.ToString());
                break;
            default:
                break;
        }

        if (text == string.Empty)                                           //uses default text formatting if config input is empty or invalid
        {
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
        }
        return $"<color=#{text}>{currentWeather}</color>";
    }
    private static string ParseColorInput(string input)
    {
        Regex reg = new Regex(@"(?i)[0-9a-f]{6}");                                                          //matches any 6 character combination of digits and case-insensitive letters
        if (reg.IsMatch(input)) { return input; }                               //if match return
        else { return ""; }                                                  //no match returns empty string
    }
}
