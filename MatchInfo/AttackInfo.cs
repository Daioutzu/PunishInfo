using Abilities;
using GameplayEntities;
using LLScreen;
using PunishInfo.Setup;
using Multiplayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace PunishInfo.MatchInfo;

internal class AttackInfo
{
    private const int HIT_TYPE_DURATION = 90;
    internal GameHudPlayerInfo GameHudPlayerInfo { get; private set; }
    private int playerIndex = -1;
    private int hideTime = int.MaxValue;
    private int prevOnHitFrame = 0;
    private PlayerStatistics playerStatistics;
    private int prevHitCount;
    private PunishType prevPunishType;

    internal static AttackInfo[] attackInfos = new AttackInfo[4];
    private GameObject attackInfoObjects;
    private GameObject counterObj;
    private GameObject punishObj;
    private GameObject hitCountObj;
    private Image imhitCount;

    internal AttackInfo(GameHudPlayerInfo hudPlayerInfo)
    {
        GameHudPlayerInfo = hudPlayerInfo;
        playerIndex = GameHudPlayerInfo.shownPlayer.CJFLMDNNMIE;
        attackInfos[playerIndex] = this;
    }

    internal void Init()
    {
        attackInfoObjects = UnityEngine.Object.Instantiate(PluginBundle.prefabs["AttackInfo"], GameHudPlayerInfo.transform);
        attackInfoObjects.name = $"AttackInfo_{playerIndex}";

        GameObject counterObj = attackInfoObjects.transform.GetChild(0).gameObject;

        this.counterObj = counterObj;
        this.counterObj.SetActive(false);

        GameObject punishObj = attackInfoObjects.transform.GetChild(1).gameObject;

        this.punishObj = punishObj;
        this.punishObj.SetActive(false);

        hitCountObj = attackInfoObjects.transform.GetChild(2).gameObject;
        imhitCount = hitCountObj.GetComponent<Image>();
        hitCountObj.SetActive(false);

        playerStatistics = PunishInfo_Plugin.Instance.pluginData.playerStatistics[playerIndex];
    }

    private static bool GetSideHit(PlayerEntity playerEntity, Side side)
    {
        Side facingDirection = playerEntity.moveableData.heading;
        if (playerEntity.abilityData.abilityState.StartsWith("CROC_CLIMB"))
        {
            facingDirection ^= Side.LEFT;
        }

        return facingDirection != side;
    }

    internal void HitOtherPlayer(PlayerEntity victim, Side side)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendFormat("\n[{4}] P{0} Hit P{1} | {2} | {3}", playerIndex, victim.playerIndex, GetSideHit(victim, side) ? "Behind" : "Front", victim.playerData.playerState, Math.SecondsToString(World.GetTimeLeftSecs() + 1));
        if (victim.abilityData.abilityState != "")
        {
            sb.AppendFormat(" | {0}", victim.abilityData.abilityState);
        }

        //Determine what type of hit i.e counter, punish or parry punish
        if (IsCounter(victim, victim.GetCurrentAbilityState()))
        {
            playerStatistics.punishType = PunishType.COUNTER;
        }
        else if (IsPunish(victim, victim.GetCurrentAbilityState()))
        {
            playerStatistics.punishType = PunishType.PUNISH;
        }
        else
        {
            playerStatistics.punishType = PunishType.NONE;
            HideUINow();
            return;
        }

        sb.AppendFormat(" | {0}!!", playerStatistics.punishType.ToString());
        if (playerStatistics.victimIndex == victim.playerIndex)
        {
            prevHitCount = playerStatistics.additionalHitCount++;
            sb.AppendFormat(" | Combo: {0}!!", playerStatistics.additionalHitCount);
        }
        else
        {
            prevHitCount = playerStatistics.additionalHitCount = 0;
        }

        SetVictim(victim);
        prevOnHitFrame = Sync.curFrame;
        PunishInfo_Plugin.Logger.LogWarning(sb.ToString());
    }

    internal void UpdateUI()
    {
        HideUI();
        ShowPunishText();
        ShowHitCount();
    }

    internal void LoadedState()
    {
        UpdateUI();
    }

    private void ShowHitCount()
    {
        if (hitCountObj == null)
            return;

        if (playerStatistics.additionalHitCount == prevHitCount)
            return;

        prevHitCount = playerStatistics.additionalHitCount;

        if (playerStatistics.additionalHitCount != 0)
        {
            imhitCount.sprite = PluginBundle.sprites[$"HitCount_{playerStatistics.additionalHitCount - 1}"];
            hitCountObj.SetActive(true);
        }
        else
        {
            hitCountObj.SetActive(false);
        }
    }

    private void HideUI()
    {
        if (Sync.curFrame > hideTime)
        {
            HideUINow();
        }
    }

    private void HideUINow()
    {
        punishObj.SetActive(false);
        playerStatistics.punishType = PunishType.NONE;
        counterObj.SetActive(false);
        hitCountObj.SetActive(false);
        hideTime = int.MaxValue;
    }

    private void ShowPunishText()
    {
        if (playerStatistics.punishType == prevPunishType)
            return;

        hideTime = Sync.curFrame + HIT_TYPE_DURATION;
        prevPunishType = playerStatistics.punishType;

        switch (playerStatistics.punishType)
        {
            case PunishType.COUNTER:
            counterObj.SetActive(true);
            punishObj.SetActive(false);
            break;

            case PunishType.PUNISH:
            case PunishType.PARRY_PUNISH:
            counterObj.SetActive(false);
            punishObj.SetActive(true);
            break;

            default:
            counterObj.SetActive(false);
            punishObj.SetActive(false);
            return;
        }
    }

    internal void SetVictim(PlayerEntity playerEntity)
    {
        playerStatistics.victimIndex = playerEntity.playerIndex;
    }

    internal void ResetVictim(int playerIndex)
    {
        if (playerStatistics.victimIndex == playerIndex)
        {
            playerStatistics.victimIndex = -1;
            playerStatistics.additionalHitCount = 0;
        }
    }

    private static bool IsCounter(PlayerEntity player, AbilityState currentAbilityState)
    {
        if (currentAbilityState == null)
            return false;

        for (int i = 0; currentAbilityState.hitboxes?.Count > 0; i++)
        {
            string hitboxName = currentAbilityState.hitboxes[i];
            if (player.hitboxes[hitboxName].active)
            {
                return true;
            }
        }

        if (string.IsNullOrEmpty(currentAbilityState.nextAbilityState) == false)
        {
            AbilityState nextState = player.abilityStates[currentAbilityState.nextAbilityState];
            return nextState.hitboxes.Count > 0;
        }

        string bufferAbility = player.abilityData.bufferAbility;

        if (currentAbilityState.name == "POST_CROUCH" && bufferAbility != "jump" && bufferAbility != string.Empty)
        {
            return true;
        }

        return false;
    }

    private static bool IsPunish(PlayerEntity player, AbilityState abilityState)
    {
        string abilityName = player.GetCurrentAbility()?.name;
        bool ignoredAbility = string.IsNullOrEmpty(abilityName) ? false : abilityName == "crouch" || abilityName == "knockedOut";

        if (player.playerData.playerState == PlayerState.SPECIAL || ignoredAbility)
        {
            return false;
        }

        if (!player.playerData.checkedActionsThisFrame && (player.playerData.playerState == PlayerState.NORMAL || player.playerData.playerState == PlayerState.ACTION && abilityState.canBeCancelledByAnyAction))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    internal void Destroy()
    {
        UnityEngine.Object.Destroy(attackInfoObjects.gameObject);
        UnityEngine.Object.Destroy(counterObj);
        UnityEngine.Object.Destroy(punishObj);
        UnityEngine.Object.Destroy(hitCountObj);
        attackInfos[playerIndex] = null;
    }
}