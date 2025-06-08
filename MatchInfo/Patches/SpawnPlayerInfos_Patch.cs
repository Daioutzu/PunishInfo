using GameplayEntities;
using HarmonyLib;
using LLScreen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;

namespace PunishInfo.MatchInfo.Patches;

[HarmonyPatch]
internal class SpawnPlayerInfos_Patch
{
    [HarmonyTargetMethod]
    private static MethodInfo TargetMethod()
    {
        Type firstInner = typeof(ScreenGameHud).GetNestedType("<SpawnPlayerInfos>c__AnonStoreyC", BindingFlags.NonPublic);
        return firstInner.GetMethod("<>m__0", BindingFlags.Instance | BindingFlags.NonPublic);
    }

    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        CodeMatcher cm = new CodeMatcher(instructions);
        cm.SearchForward(iL => iL.opcode == OpCodes.Callvirt && (iL.operand as MethodBase).Name == nameof(GameHudPlayerInfo.SetPlayer))
            .ThrowIfNotMatch($"'{nameof(GameHudPlayerInfo.SetPlayer)}' was not found")
            .Advance(1);

        cm.Insert(
            new CodeInstruction(OpCodes.Ldloc_2),
            Transpilers.EmitDelegate(delegate (GameHudPlayerInfo playerInfo)
            {
                var info = new AttackInfo(playerInfo);
                info.Init();
            })
            );

        return cm.InstructionEnumeration();
    }
}