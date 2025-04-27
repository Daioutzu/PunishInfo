using PunishInfo.MatchInfo;
using PunishInfo.PluginSync;
using Multiplayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PunishInfo.FrameSync;

internal class CustomStateHistory
{
    private List<PluginState> history;

    public CustomStateHistory()
    {
        history = new List<PluginState>(50);
    }

    public void LoadFrame(int frame)
    {
        PluginState item = FrameObject.GetItem(history, frame);
        if (item == null)
        {
            PunishInfo_Plugin.Logger.LogError(string.Concat(new object[]
            {
                "Couldn't get savedgame for frame ",
                frame,
                " (history: ",
                GetHistoryString(),
                ")"
            }));
        }
        else
        {
            PunishInfo_Plugin.Instance.pluginData.Load(item.PluginData, false);
            PunishInfo_Plugin.Instance.LoadedState();
            if (history.Count > FrameObject.lastIndex + 1)
            {
                history.RemoveRange(FrameObject.lastIndex + 1, history.Count - (FrameObject.lastIndex + 1));
            }
        }
    }

    public void SaveFrame(int frame)
    {
        MatchInfoData pluginData = MatchDataPool.Get();
        pluginData.Load(PunishInfo_Plugin.Instance.pluginData, true);
        FrameObject.AddToList(history, new PluginState(pluginData, frame));
    }

    public void DeleteFramesBefore(int frame)
    {
        while (this.history.Count > 0 && this.history[0].frame < frame)
        {
            MatchDataPool.Return(this.history[0].PluginData);
            this.history.RemoveAt(0);
        }
    }

    public void DeleteFramesAfter(int frame)
    {
        while (this.history.Count > 0 && this.history[this.history.Count - 1].frame > frame)
        {
            MatchDataPool.Return(this.history[this.history.Count - 1].PluginData);
            this.history.RemoveAt(this.history.Count - 1);
        }
    }

    public string GetHistoryString()
    {
        List<string> list = new List<string>(50);
        foreach (PluginState pluginState in history)
        {
            list.Add(pluginState.frame.ToString());
        }
        return string.Join(",", list.ToArray());
    }

    public override string ToString()
    {
        if (this.history.Count == 0)
        {
            return "[empty]";
        }
        int frame = this.history[0].frame;
        int frame2 = this.history[this.history.Count - 1].frame;
        string text = string.Concat(new object[] { "[", frame, " - ", frame2 });
        if (this.history.Count - 1 != frame2 - frame)
        {
            string text2 = text;
            text = string.Concat(new object[]
            {
                    text2,
                    " (length ",
                    this.history.Count,
                    "??)"
            });
        }
        return text + "]";
    }
}