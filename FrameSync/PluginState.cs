using Multiplayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PunishInfo.PluginSync;

internal class PluginState : FrameObject
{
    internal MatchInfoData PluginData { get; private set; }

    public PluginState(MatchInfoData pluginData, int frame)
    {
        PluginData = pluginData;
        this.frame = frame;
    }
}