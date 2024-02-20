using System.Collections;
using DunGen;
using GameNetcodeStuff;
using TMPro;
using UnityEngine;
using static OpenMonitors.Plugin;

namespace OpenMonitors.Monitors;

public class PlayersLifeSupportMonitor : MonoBehaviour
{
    public static PlayersLifeSupportMonitor Instance = null!;


    public TextMeshProUGUI textMesh = null!;

    public void Start()
    {
        Log.LogDebug($"{name} -> Start()");
        if (!Instance) Instance = this;
        textMesh = GetComponent<TextMeshProUGUI>();
        textMesh.fontSize = 42;
        textMesh.text = Config.HidePlayersLifeSupport.Value ? string.Empty: "LIFE SUPPORT:\n?";
        Log.LogDebug($"{name} -> Start() end");
    }

    public void UpdateMonitor()
    {
        Log.LogDebug($"{name} -> UpdateMonitor()");
        if (Config.HidePlayersLifeSupport.Value) return;
        CoroutineHelper.Instance.StartCoroutine(UpdateMonitorCoroutine());
    }
    
    private IEnumerator UpdateMonitorCoroutine()
    {
        Log.LogDebug($"{name} -> UpdateCoroutine(), waiting 2 seconds before updating due to slow player loading");
        yield return new WaitForSeconds(2);
        var builder = new System.Text.StringBuilder().AppendLine("LIFE SUPPORT: ");
        foreach (var playerId in StartOfRound.Instance.ClientPlayerList.Keys)
        {
            PlayerControllerB player = StartOfRound.Instance.allPlayerScripts[playerId];
            builder.Append($"- {player.playerUsername} ");
            if (player.isPlayerDead)
            {
                Log.LogDebug($"-> {player.playerUsername} is dead");
                builder.AppendLine("<color=#FF0000>(DEAD)</color>");
            } 
            else if (player.health <= 50)
            {
                Log.LogDebug($"-> {player.playerUsername} is injured! {player.health}");
                builder.AppendLine("<color=#FFF01C>(INJURED)</color>");
            }
            else
            {
                Log.LogDebug($"-> {player.playerUsername} is still alive!");
                builder.AppendLine();
            }

        }

        textMesh.text = builder.ToString();
    }
}
