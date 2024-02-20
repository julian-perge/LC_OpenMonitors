using TMPro;
using UnityEngine;
using static OpenMonitors.Plugin;

namespace OpenMonitors.Monitors;

public class LifeSupportMonitor : MonoBehaviour
{
    public static LifeSupportMonitor Instance = null!;

    private TextMeshProUGUI _textMesh = null!;

    public void Start()
    {
        Log.LogDebug($"{name} -> Start()");
        if (!Instance) Instance = this;
        _textMesh = GetComponent<TextMeshProUGUI>();
        Log.LogDebug($"{name} -> Start() -> UpdateMonitor()");
        UpdateMonitor();
    }

    public void UpdateMonitor()
    {
        Log.LogDebug($"{name} -> UpdateMonitor()");
        _textMesh.text = Config.HideLifeSupport.Value ? string.Empty : $"ALIVE:\n${StartOfRound.Instance.livingPlayers}";
    }
}
