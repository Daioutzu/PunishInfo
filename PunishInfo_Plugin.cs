using BepInEx;
using BepInEx.Logging;
using CodeStage.AntiCheat.ObscuredTypes;
using HarmonyLib;
using LLScreen;
using PunishInfo.MatchInfo;
using PunishInfo.FrameSync;
using PunishInfo.PluginSync;
using Multiplayer;
using UnityEngine;
using UnityEngine.UI;
using PunishInfo.Setup;
using BepInEx.Configuration;

namespace PunishInfo
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    [BepInProcess("LLBlaze.exe")]
    public class PunishInfo_Plugin : BaseUnityPlugin
    {
        public new static ManualLogSource Logger;
        public static PunishInfo_Plugin Instance { get; private set; }
        internal static Harmony Harmony { get; private set; } = new Harmony(MyPluginInfo.PLUGIN_GUID);

        internal MatchInfoData pluginData;
        internal MatchInfoData pluginDataSave;
        internal CustomStateHistory stateHistory;

        #region Config

        internal ConfigEntry<bool> showRankInGame;

        #endregion Config

        private void Awake()
        {
            // Plugin startup logic
            Instance = this;
            Logger = base.Logger;
            if (PluginBundle.Load(this, "ui_prefabs", "ui_sprites") == false)
            {
                Logger.LogFatal($"Plugin {MyPluginInfo.PLUGIN_GUID} has failed to load Bundle!");
                return;
            }
            showRankInGame = Config.Bind("General", "Division Icon", true, "Show player division during ranked match");
            Harmony.PatchAll();
            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        }

        internal void LoadedState()
        {
            for (int i = 0; i < 4; i++)
            {
                if (AttackInfo.attackInfos[i] == null)
                    continue;

                AttackInfo.attackInfos[i].LoadedState();
            }
        }
    }
}