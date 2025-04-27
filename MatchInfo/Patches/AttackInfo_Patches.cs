using Abilities;
using GameplayEntities;
using HarmonyLib;
using LLScreen;
using PunishInfo.MatchInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;
using static PunishInfo.MatchInfo.AttackInfo;

namespace PunishInfo.MatchInfo.Patches;

[HarmonyPatch]
internal static class AttackInfo_Patches
{
    [HarmonyPatch(typeof(GameHudPlayerInfo), nameof(GameHudPlayerInfo.SetPlayer))]
    [HarmonyPostfix]
    private static void Init_AttackInfo(GameHudPlayerInfo __instance, ALDOKEMAOMB player)
    {
        new AttackInfo(__instance);
        attackInfos[player.CJFLMDNNMIE].Init();
    }

    [HarmonyPatch(typeof(World), nameof(World.FrameUpdate))]
    [HarmonyPostfix]
    private static void UpdateAttackInfo(ScreenGameHud __instance)
    {
        for (int i = 0; i < 4; i++)
        {
            if (attackInfos[i] == null)
                continue;

            attackInfos[i].UpdateUI();
        }
    }

    [HarmonyPatch(typeof(ScreenGameHud), nameof(ScreenGameHud.DestroyPlayerInfos))]
    [HarmonyPostfix]
    private static void DestroyAttackInfos(ScreenGameHud __instance)
    {
        for (int i = 0; i < 4; i++)
        {
            if (attackInfos[i] == null)
                continue;

            attackInfos[i].Destroy();
        }
    }

    [HarmonyPatch(typeof(AbilityEntity), nameof(AbilityEntity.EndAbilityStateToNormal))]
    [HarmonyPrefix]
    private static void ResetHitCount(AbilityEntity __instance)
    {
        PlayerEntity playerEntity = __instance as PlayerEntity;

        if (playerEntity == null)
            return;

        for (int i = 0; i < 4; i++)
        {
            if (attackInfos[i] == null || (playerEntity.abilityData.bufferAbility == "bunt" && playerEntity.abilityData.abilityState == "GET_UP"))
                continue;

            attackInfos[i].ResetVictim(playerEntity.playerIndex);
        }
    }

    [HarmonyPatch(typeof(AbilityEntity), nameof(AbilityEntity.Spawn))]
    [HarmonyPostfix]
    private static void ResetVictimOnSpawn(AbilityEntity __instance)
    {
        PlayerEntity playerEntity = __instance as PlayerEntity;

        if (playerEntity == null)
            return;

        for (int i = 0; i < 4; i++)
        {
            if (attackInfos[i] == null)
                continue;

            attackInfos[i].ResetVictim(playerEntity.playerIndex);
        }
    }

    [HarmonyPatch(typeof(GetHitBallEntity), nameof(GetHitBallEntity.HitPlayer))]
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> GetHitBallEntity_HitPlayer(IEnumerable<CodeInstruction> instructions)
    {
        CodeMatcher cm = new CodeMatcher(instructions);
        cm.SearchForward(iL => iL.opcode == OpCodes.Callvirt && (iL.operand as MethodBase).Name == nameof(GetHitPlayerEntity.GetHitByBall))
            .Advance(-1);

        MethodInfo method = SymbolExtensions.GetMethodInfo(() => Invoke_HitOtherPlayer(null,0,0));

        cm.Insert(
            new CodeInstruction(OpCodes.Ldarg_1),
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(OpCodes.Ldfld, typeof(GetHitBallEntity).GetField(nameof(GetHitBallEntity.ballData))),
            new CodeInstruction(OpCodes.Ldfld, typeof(BallData).GetField(nameof(BallData.lastHitterIndex))),
            new CodeInstruction(OpCodes.Ldloc_0),
            new CodeInstruction(OpCodes.Call, method)
            );

        return cm.InstructionEnumeration();
    }

    private static void Invoke_HitOtherPlayer(PlayerEntity victim, int hitterIndex, Side side)
    {
        if (attackInfos[hitterIndex] == null)
            return;

        attackInfos[hitterIndex].HitOtherPlayer(victim, side);
    }
}