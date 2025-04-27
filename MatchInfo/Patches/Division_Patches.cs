using BepInEx.Logging;
using CodeStage.AntiCheat.ObscuredTypes;
using GameplayEntities;
using HarmonyLib;
using LLScreen;
using PunishInfo.MatchInfo;
using Multiplayer;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;
using UnityEngine.UI;

namespace PunishInfo.MatchInfo.Patches;

[HarmonyPatch]
internal class Division_Patches
{
    [HarmonyPatch(typeof(GameHudPlayerInfo), nameof(GameHudPlayerInfo.SetPlayer))]
    [HarmonyPostfix]
    private static void AddDivisionIcon(GameHudPlayerInfo __instance, ALDOKEMAOMB player)
    {
        if (JOMBNFKIHIC.EAENFOJNNGP != OnlineMode.RANKED || PunishInfo_Plugin.Instance.showRankInGame.Value == false)
            return;

        GameObject gameObject = new GameObject("divIcon",new Type[]{typeof(Image) });
        gameObject.transform.SetParent(__instance.transform);
        Image image = gameObject.GetComponent<Image>();
        ObscuredInt obscuredInt = P2P.localPeer.playerNr != player.CJFLMDNNMIE ?
                CGLLJHHAJAK.BBHCKEPIDFG.JHPMKHLAGPH : CGLLJHHAJAK.BBHCKEPIDFG.BCABOGEMIDC;

        NJLEDGLLDDJ.HLDHGADFCGH hldhgadfcgh = NJLEDGLLDDJ.LEJPKPFFIIF(obscuredInt);
        if (hldhgadfcgh.PACBDHOJMBE == NJLEDGLLDDJ.BGOPHAMGDGC.APALPGDPKEH)
        {
            hldhgadfcgh = NJLEDGLLDDJ.HCLNIEKIFDG(NJLEDGLLDDJ.BGOPHAMGDGC.APALPGDPKEH);
        }
        image.sprite = JPLELOFJOOH.EGHEJAKEOFC(hldhgadfcgh.PACBDHOJMBE);
        image.SetNativeSize();
        image.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
        image.transform.localPosition = new Vector3(120, -44, -132);
    }
}