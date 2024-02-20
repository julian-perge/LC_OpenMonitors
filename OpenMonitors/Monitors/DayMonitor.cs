using TMPro;
using UnityEngine;
using static OpenMonitors.Plugin;

namespace OpenMonitors.Monitors;

public class DayMonitor : MonoBehaviour
{
    public static DayMonitor Instance = null!;

    public TextMeshProUGUI textMesh = null!;

    public void Start()
    {
        Log.LogDebug($"{name} -> Start()");
        if (!Instance) Instance = this;
        textMesh = GetComponent<TextMeshProUGUI>();
        textMesh.text = Config.HideDay.Value ? string.Empty : "DAY:\n?";
        if (StartOfRound.Instance.IsHost)
        {
            Log.LogDebug($"{name} -> Start() -> IsHost");
            UpdateMonitor();
        }
        else
        {
            Log.LogDebug($"{name} -> Start() -> NOT IsHost");
        }
    }

    public void UpdateMonitor()
    {
        Log.LogDebug($"{name} -> UpdateMonitor()");
        textMesh.text = Config.HideDay.Value ? string.Empty : $"DAY:\n{StartOfRound.Instance.gameStats.daysSpent}";
    }
}
