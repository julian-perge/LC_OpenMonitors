using System.Collections;
using System.Linq;
using System.Text;
using DunGen;
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
        ModLogger.LogDebug($"{name} -> Start()");
        if (!Instance) Instance = this;
        textMesh = GetComponent<TextMeshProUGUI>();
        textMesh.fontSize = 42;
        textMesh.enableWordWrapping = false;
        textMesh.text = Config.HidePlayersLifeSupport.Value ? string.Empty : "LIFE SUPPORT:\n?";
        ModLogger.LogDebug($"{name} -> Start() end");
    }

    public void UpdateMonitor()
    {
        ModLogger.LogDebug($"{name} -> UpdateMonitor()");
        if (Config.HidePlayersLifeSupport.Value) return;
        CoroutineHelper.Instance.StartCoroutine(UpdateMonitorCoroutine());
    }

    private IEnumerator UpdateMonitorCoroutine()
    {
        ModLogger.LogDebug(
            $"{name} -> UpdateCoroutine(), waiting 2 seconds before updating due to slow player loading"
        );
        yield return new WaitForSeconds(2);
        var builder = new StringBuilder().AppendLine("LIFE SUPPORT:").AppendLine();
        foreach (
            var player in StartOfRound.Instance.ClientPlayerList.Keys.Select(
                playerId => StartOfRound.Instance.allPlayerScripts[playerId]
            )
        )
        {
            builder.Append(
                player.playerUsername.Length > 15
                    ? $"- {player.playerUsername[..15]}... "
                    : $"- {player.playerUsername} "
            );
            if (player.isPlayerDead)
            {
                ModLogger.LogDebug($"-> {player.playerUsername} is dead");
                builder.AppendLine("<color=#FF0000>(DEAD)</color>");
            }
            else if (player.health <= 50)
            {
                ModLogger.LogDebug($"-> {player.playerUsername} is injured! {player.health}");
                builder.AppendLine("<color=#FFF01C>(HURT)</color>");
            }
            else
            {
                ModLogger.LogDebug($"-> {player.playerUsername} is still alive!");
                builder.AppendLine();
            }
        }

        textMesh.text = builder.ToString();
    }
}
