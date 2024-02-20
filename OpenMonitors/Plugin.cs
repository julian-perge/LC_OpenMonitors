using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace OpenMonitors;

public static class ModInfo
{
    public const string Name = "OpenMonitors";
    public const string Guid = "xxxstoner420bongmasterxxx.open_monitors";
    public const string Version = "1.0.0";
}

[BepInPlugin(ModInfo.Guid, ModInfo.Name, ModInfo.Version)]
public class Plugin : BaseUnityPlugin
{
    public static ConfigFile ModConfig = null!;

    private static Plugin? _instance;

    public static ManualLogSource Log = null!;
    private readonly Harmony _harmony = new(ModInfo.Guid);

    private void Awake()
    {
        Log = Logger;

        if (!_instance)
        {
            Log.LogInfo($"{ModInfo.Name} -> loading");
            _instance = this;
            ModConfig = Config;
            Monitors.Config.Initialize();
            _harmony.PatchAll(Assembly.GetExecutingAssembly());
            Log.LogInfo($"{ModInfo.Name} -> complete");
        }
    }
}
