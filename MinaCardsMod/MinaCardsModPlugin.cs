using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;
using MinaCardsMod.Handlers;
using UnityEngine;

#nullable disable
namespace MinaCardsMod
{
  [BepInPlugin("com.KalinuxYT.MinaCardsMod", "MinaCardsMod", "1.3.1")]
  public class MinaCardsModPlugin : BaseUnityPlugin
  {
    private const string MyGUID = "com.KalinuxYT.MinaCardsMod";
    private const string PluginName = "MinaCardsMod";
    private const string VersionString = "1.3.1";
    public static string SwapExpansionsKey = "Album expansions toggle";
    public static string CustomNewExpansionImagesKey = "Enable custom card images for new expansions";
    public static string CustomNewExpansionConfigsKey = "Enable custom configs for new expansions";
    public static string CustomBaseMonsterImagesKey = "Enable custom card images for original expansions";
    public static string CustomBaseConfigsKey = "Enable custom configs for original expansions";
    public static string SwapExpansionKeyboardShortcutKey = "Toggle between expansions";
    public static string ReCacheFilesKey = "Re-Cache all images and config files and reload changes";
    public static ConfigEntry<bool> SwapExpansions;
    public static ConfigEntry<bool> CustomNewExpansionImages;
    public static ConfigEntry<bool> CustomNewExpansionConfigs;
    public static ConfigEntry<bool> CustomBaseConfigs;
    public static ConfigEntry<bool> CustomBaseMonsterImages;
    public static ConfigEntry<KeyboardShortcut> SwapExpansionKeyboardShortcut;
    public static ConfigEntry<KeyboardShortcut> ReCacheFiles;
    private static readonly Harmony Harmony = new Harmony("com.KalinuxYT.MinaCardsMod");
    public static ManualLogSource Log = new ManualLogSource("MinaCardsMod");
    public static bool isConfigGeneratorBuild = false;

    private void Awake()
    {
      this.Logger.LogInfo("Awake method called");
      MinaCardsModPlugin.SwapExpansions = this.Config.Bind<bool>("General", MinaCardsModPlugin.SwapExpansionsKey, false, new ConfigDescription("Swap between the expansions in the collection book.", (AcceptableValueBase) null, Array.Empty<object>()));
      MinaCardsModPlugin.CustomNewExpansionImages = this.Config.Bind<bool>("General", MinaCardsModPlugin.CustomNewExpansionImagesKey, true, new ConfigDescription("Enable custom card images for new expansions", (AcceptableValueBase) null, Array.Empty<object>()));
      MinaCardsModPlugin.CustomNewExpansionConfigs = this.Config.Bind<bool>("General", MinaCardsModPlugin.CustomNewExpansionConfigsKey, true, new ConfigDescription("Enable custom configs for new expansions", (AcceptableValueBase) null, Array.Empty<object>()));
      MinaCardsModPlugin.CustomBaseMonsterImages = this.Config.Bind<bool>("General", MinaCardsModPlugin.CustomBaseMonsterImagesKey, true, new ConfigDescription("Enable custom card images for original expansions", (AcceptableValueBase) null, Array.Empty<object>()));
      MinaCardsModPlugin.CustomBaseConfigs = this.Config.Bind<bool>("General", MinaCardsModPlugin.CustomBaseConfigsKey, true, new ConfigDescription("Enable custom configs for original expansions", (AcceptableValueBase) null, Array.Empty<object>()));
      MinaCardsModPlugin.SwapExpansionKeyboardShortcut = this.Config.Bind<KeyboardShortcut>("General", MinaCardsModPlugin.SwapExpansionKeyboardShortcutKey, new KeyboardShortcut(KeyCode.U, Array.Empty<KeyCode>()));
      MinaCardsModPlugin.ReCacheFiles = this.Config.Bind<KeyboardShortcut>("General", MinaCardsModPlugin.ReCacheFilesKey, new KeyboardShortcut(KeyCode.None, Array.Empty<KeyCode>()));
      MinaCardsModPlugin.SwapExpansions.SettingChanged += new EventHandler(this.ConfigSettingChanged);
      MinaCardsModPlugin.SwapExpansionKeyboardShortcut.SettingChanged += new EventHandler(this.ConfigSettingChanged);
      MinaCardsModPlugin.CustomNewExpansionImages.SettingChanged += new EventHandler(this.ConfigSettingChanged);
      MinaCardsModPlugin.CustomNewExpansionConfigs.SettingChanged += new EventHandler(this.ConfigSettingChanged);
      MinaCardsModPlugin.CustomBaseConfigs.SettingChanged += new EventHandler(this.ConfigSettingChanged);
      MinaCardsModPlugin.CustomBaseMonsterImages.SettingChanged += new EventHandler(this.ConfigSettingChanged);
      this.Logger.LogInfo((object) "PluginName: MinaCardsMod, VersionString: 1.3.1-Hardened is loading...");
      MinaCardsModPlugin.Harmony.PatchAll();
      this.Logger.LogInfo((object) "PluginName: MinaCardsMod, VersionString: 1.3.1-Hardened is loaded.");
      this.Logger.LogWarning((object) "This is a 'Hardened' build, this means any update to the game should not cause major issues. This mod is compatible with v0.47.3 of the game.");
      MinaCardsModPlugin.Log = this.Logger;
    }

    private void Update()
    {
      if (MinaCardsModPlugin.SwapExpansionKeyboardShortcut.Value.IsUp())
      {
        if (!MinaCardsModPlugin.SwapExpansions.Value)
          SoundManager.PlayAudio("SFX_PercStarJingle3", 0.6f, 1.2f);
        else
          SoundManager.PlayAudio("SFX_PressFindMatch", 0.6f, 1.2f);
        MinaCardsModPlugin.SwapExpansions.Value = !MinaCardsModPlugin.SwapExpansions.Value;
      }
      if (!MinaCardsModPlugin.ReCacheFiles.Value.IsUp())
        return;
      if (MinaCardsModPlugin.isConfigGeneratorBuild && CSingleton<CGameManager>.Instance.m_IsGameLevel)
        ExtrasHandler.AddHiddenCards();
      if (!CacheHandler.isCurrentlyCacheing && !MinaCardsModPlugin.isConfigGeneratorBuild)
      {
        CacheHandler.isCurrentlyCacheing = true;
        ExtrasHandler.swapPackNames();
        CacheHandler.CacheAllFiles();
        if (CSingleton<CGameManager>.Instance.m_IsGameLevel)
          ExtrasHandler.DoReload();
      }
    }

    private void ConfigSettingChanged(object sender, EventArgs e)
    {
      if (!(e is SettingChangedEventArgs changedEventArgs))
        return;
      if (!(changedEventArgs.ChangedSetting.Definition.Key == MinaCardsModPlugin.SwapExpansionsKey))
        ;
      if (changedEventArgs.ChangedSetting.Definition.Key == MinaCardsModPlugin.CustomNewExpansionImagesKey && CSingleton<CGameManager>.Instance.m_IsGameLevel)
        ExtrasHandler.DoReload();
      if (changedEventArgs.ChangedSetting.Definition.Key == MinaCardsModPlugin.CustomNewExpansionConfigsKey && CSingleton<CGameManager>.Instance.m_IsGameLevel)
        ExtrasHandler.DoReload();
      if (changedEventArgs.ChangedSetting.Definition.Key == MinaCardsModPlugin.CustomBaseMonsterImagesKey && CSingleton<CGameManager>.Instance.m_IsGameLevel)
        ExtrasHandler.DoReload();
      if (changedEventArgs.ChangedSetting.Definition.Key == MinaCardsModPlugin.CustomBaseConfigsKey && CSingleton<CGameManager>.Instance.m_IsGameLevel)
        ExtrasHandler.DoReload();
      if (!(changedEventArgs.ChangedSetting.Definition.Key == MinaCardsModPlugin.SwapExpansionKeyboardShortcutKey))
        return;
      KeyboardShortcut boxedValue = (KeyboardShortcut) changedEventArgs.ChangedSetting.BoxedValue;
    }
  }
}