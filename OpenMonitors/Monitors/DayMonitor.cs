using TMPro;
using UnityEngine;
using static OpenMonitors.Plugin;

namespace OpenMonitors.Monitors;

public class DayMonitor : MonoBehaviour
{
    public static DayMonitor Instance = null!;

    public TextMeshProUGUI textMesh = null!;

    private StartOfRound _startOfRound = null!;

    private EndOfGameStats _stats = null!;

    public void Start()
    {
        Log.LogDebug("DayMonitor -> Start()");
        textMesh = GetComponent<TextMeshProUGUI>();
        if (!Instance) Instance = this;
        _startOfRound = StartOfRound.Instance;
        _stats = _startOfRound.gameStats;
        if (_startOfRound.IsHost)
        {
            Log.LogDebug("DayMonitor -> Start() -> IsHost");
            UpdateMonitor();
        }
        else
        {
            Log.LogDebug("DayMonitor -> Start() -> NOT IsHost");
            textMesh.text = "DAY:\n?";
        }
    }

    public void UpdateMonitor()
    {
        Log.LogDebug("DayMonitor -> UpdateMonitor()");
        textMesh.text = Config.HideDay.Value ? string.Empty : $"DAY:\n{_stats.daysSpent}";
    }
}
