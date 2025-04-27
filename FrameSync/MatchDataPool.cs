using PunishInfo.PluginSync;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PunishInfo.FrameSync;

internal class MatchDataPool
{
    private static Stack<MatchInfoData> unused;
    private static bool inited;

    private static void Init()
    {
        unused = new Stack<MatchInfoData>();
        AddSome();
        inited = true;
    }

    private static void AddSome()
    {
        for (int i = 0; i < 10; i++)
        {
            unused.Push(new MatchInfoData());
        }
    }

    public static MatchInfoData Get()
    {
        if (!inited)
        {
            Init();
        }
        if (unused.Count == 0)
        {
            AddSome();
        }
        return unused.Pop();
    }

    public static void Return(MatchInfoData pluginData)
    {
        unused.Push(pluginData);
    }
}