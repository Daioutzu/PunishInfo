using HarmonyLib;
using Multiplayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace PunishInfo.FrameSync.Patches;

[HarmonyPatch]
internal class Sync_Patches
{
    [HarmonyPatch(typeof(Sync), nameof(Sync.Init))]
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> CreateNewStateHistory(IEnumerable<CodeInstruction> instructions)
    {
        CodeMatcher cm = new CodeMatcher(instructions);
        cm.SearchForward(iL => iL.opcode == OpCodes.Stsfld && (iL.operand as FieldInfo).Name == nameof(Sync.stateHistory))
            .Advance(1);

        cm.Insert(Transpilers.EmitDelegate(delegate ()
        {
            PunishInfo_Plugin.Instance.stateHistory = new CustomStateHistory();
        }));

        return cm.InstructionEnumeration();
    }
}