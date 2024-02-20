using TMPro;
using UnityEngine;
using static OpenMonitors.Plugin;

namespace OpenMonitors.Monitors;

public class TimeMonitor : MonoBehaviour
{
    public static TimeMonitor Instance = null!;

    public TextMeshProUGUI textMesh = null!;

    private TextMeshProUGUI _timeMesh = null!;

    public void Start()
    {
        Log.LogDebug("TimeMonitor -> Start()");
        if (!Instance) Instance = this;

        _timeMesh = GameObject
            .Find("Systems/UI/Canvas/IngamePlayerHUD/ProfitQuota/Container/Box/TimeNumber")
            .GetComponent<TextMeshProUGUI>();
        textMesh = GetComponent<TextMeshProUGUI>();
        textMesh.text = "TIME:\n7:30\nAM";
        Log.LogDebug("TimeMonitor -> Start() end");
    }

    public void UpdateMonitor()
    {
        Log.LogDebug("TimeMonitor -> UpdateMonitor()");
        textMesh.text = Config.HideTime.Value ? string.Empty : $"TIME:\n{_timeMesh.text}";
    }
}
