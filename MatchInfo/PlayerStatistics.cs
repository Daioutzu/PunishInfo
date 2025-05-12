using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PunishInfo.MatchInfo;

internal class PlayerStatistics
{
    internal int additionalHitCount = 0;
    internal PunishType punishType = PunishType.NONE;
    internal int victimIndex = -1;
    internal int prevHideTime = int.MaxValue;

    internal void Load(PlayerStatistics load)
    {
        additionalHitCount = load.additionalHitCount;
        punishType = load.punishType;
        victimIndex = load.victimIndex;
        prevHideTime = load.prevHideTime;
    }
}