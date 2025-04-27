using PunishInfo.MatchInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PunishInfo.PluginSync;

internal class MatchInfoData
{
    public MatchInfoData()
    {
        playerStatistics = new PlayerStatistics[4];
        for (int i = 0; i < 4; i++)
        {
            playerStatistics[i] = new PlayerStatistics();
        }
    }

    public void Load(MatchInfoData load, bool saving)
    {
        for (int i = 0; i < 4; i++)
        {
            playerStatistics[i].Load(load.playerStatistics[i]);
        }
    }

    internal PlayerStatistics[] playerStatistics;
}