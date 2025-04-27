using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PunishInfo.FrameSync.Patches;

[HarmonyPatch(typeof(World))]
internal class World_Patches
{
    [HarmonyPatch(nameof(World.Init1))]
    [HarmonyPostfix]
    private static void Init1_Patch(World __instance)
    {
        PunishInfo_Plugin.Instance.pluginData = new PluginSync.MatchInfoData();
    }

    [HarmonyPatch(nameof(World.SaveState))]
    [HarmonyPostfix]
    private static void SaveState(World __instance)
    {
        PunishInfo_Plugin plugin = PunishInfo_Plugin.Instance;
        if (plugin.pluginDataSave == null)
        {
            plugin.pluginDataSave = new PluginSync.MatchInfoData();
        }
        plugin.pluginDataSave.Load(plugin.pluginData, true);
    }

    [HarmonyPatch(nameof(World.LoadState))]
    [HarmonyPostfix]
    private static void LoadState(World __instance)
    {
        PunishInfo_Plugin plugin = PunishInfo_Plugin.Instance;
        if (plugin.pluginDataSave != null)
        {
            plugin.pluginData.Load(plugin.pluginDataSave, false);
        }
        plugin.LoadedState();
    }
}