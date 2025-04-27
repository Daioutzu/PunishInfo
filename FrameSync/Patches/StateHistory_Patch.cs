using HarmonyLib;
using Multiplayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PunishInfo.FrameSync.Patches;

[HarmonyPatch(typeof(StateHistory))]
internal class StateHistory_Patch
{
    [HarmonyPatch(nameof(StateHistory.LoadFrame))]
    [HarmonyPostfix]
    private static void LoadFrame(StateHistory __instance, int frame)
    {
        PunishInfo_Plugin.Instance.stateHistory.LoadFrame(frame);
    }

    [HarmonyPatch(nameof(StateHistory.SaveFrame))]
    [HarmonyPostfix]
    private static void SaveFrame(StateHistory __instance, int frame)
    {
        PunishInfo_Plugin.Instance.stateHistory.SaveFrame(frame);
    }

    [HarmonyPatch(nameof(StateHistory.DeleteFramesBefore))]
    [HarmonyPostfix]
    private static void DeleteFramesBefore(StateHistory __instance, int frame)
    {
        PunishInfo_Plugin.Instance.stateHistory.DeleteFramesBefore(frame);
    }

    [HarmonyPatch(nameof(StateHistory.DeleteFramesAfter))]
    [HarmonyPostfix]
    private static void DeleteFramesAfter(StateHistory __instance, int frame)
    {
        PunishInfo_Plugin.Instance.stateHistory.DeleteFramesAfter(frame);
    }
}