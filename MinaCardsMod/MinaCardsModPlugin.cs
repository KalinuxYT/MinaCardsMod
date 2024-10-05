using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;
using UnityEngine;
using UnityEngine.Windows;

#nullable disable
namespace MinaCardsMod
{
  [BepInPlugin("com.KalinuxYT.MinaCardsMod", "MinaCardsMod", "1.0.1")]
  public class MinaCardsModPlugin : BaseUnityPlugin
  {
    private const string MyGUID = "com.KalinuxYT.MinaCardsMod";
    private const string PluginName = "MinaCardsMod";
    private const string VersionString = "1.0.1";
    public static string SwapExpansionsKey = "Access other card expansions";
    public static string CustomImagesKey = "Enable custom card images for new expansions";
    public static string CustomConfigsKey = "Enable custom configs";
    public static string CustomBaseMonsterIconsKey = "Enable swapping base monster icons";
    public static string SwapExpansionKeyboardShortcutKey = "Toggle between expansions";
    public static ConfigEntry<bool> SwapExpansions;
    public static ConfigEntry<bool> CustomImages;
    public static ConfigEntry<bool> CustomConfigs;
    public static ConfigEntry<bool> CustomBaseMonsterIcons;
    public static ConfigEntry<KeyboardShortcut> SwapExpansionKeyboardShortcut;
    private static readonly Harmony Harmony = new Harmony("com.KalinuxYT.MinaCardsMod");
    public static ManualLogSource Log = new ManualLogSource("MinaCardsMod");

    private void Awake()
    {
      this.Logger.LogInfo("Awake method called");
      MinaCardsModPlugin.SwapExpansions = this.Config.Bind<bool>("General", MinaCardsModPlugin.SwapExpansionsKey, false, new ConfigDescription("Swap between the expansions in the collection book.", (AcceptableValueBase) null, Array.Empty<object>()));
      MinaCardsModPlugin.SwapExpansionKeyboardShortcut = this.Config.Bind<KeyboardShortcut>("General", MinaCardsModPlugin.SwapExpansionKeyboardShortcutKey, new KeyboardShortcut((KeyCode) 117, Array.Empty<KeyCode>()), (ConfigDescription) null);
      MinaCardsModPlugin.CustomImages = this.Config.Bind<bool>("General", MinaCardsModPlugin.CustomImagesKey, true, new ConfigDescription("Toggle using custom card images for the new expansions.", (AcceptableValueBase) null, Array.Empty<object>()));
      MinaCardsModPlugin.CustomConfigs = this.Config.Bind<bool>("General", MinaCardsModPlugin.CustomConfigsKey, true, new ConfigDescription("Toggle using the custom config files.", (AcceptableValueBase) null, Array.Empty<object>()));
      MinaCardsModPlugin.CustomBaseMonsterIcons = this.Config.Bind<bool>("General", MinaCardsModPlugin.CustomBaseMonsterIconsKey, true, new ConfigDescription("Toggle swapping base card images for custom images.", (AcceptableValueBase) null, Array.Empty<object>()));
      
      MinaCardsModPlugin.SwapExpansions.SettingChanged += new EventHandler(this.ConfigSettingChanged);
      MinaCardsModPlugin.SwapExpansionKeyboardShortcut.SettingChanged += new EventHandler(this.ConfigSettingChanged);
      MinaCardsModPlugin.CustomImages.SettingChanged += new EventHandler(this.ConfigSettingChanged);
      MinaCardsModPlugin.CustomConfigs.SettingChanged += new EventHandler(this.ConfigSettingChanged);
      MinaCardsModPlugin.CustomBaseMonsterIcons.SettingChanged += new EventHandler(this.ConfigSettingChanged);
      this.Logger.LogInfo((object) "PluginName: MinaCardsMod, VersionString: 1.0.1 is loading...");
      MinaCardsModPlugin.Harmony.PatchAll();
      this.Logger.LogInfo((object) "PluginName: MinaCardsMod, VersionString: 1.0.1 is loaded.");
      MinaCardsModPlugin.Log = this.Logger;
    }

    private void Update()
    {
    KeyboardShortcut keyboardShortcut = MinaCardsModPlugin.SwapExpansionKeyboardShortcut.Value;
      if (!keyboardShortcut.IsUp())
        return;
      if (!MinaCardsModPlugin.SwapExpansions.Value)
        SoundManager.PlayAudio("SFX_PercStarJingle3", 0.6f, 1.2f);
      else
        SoundManager.PlayAudio("SFX_PressFindMatch", 0.6f, 1.2f);
      MinaCardsModPlugin.SwapExpansions.Value = !MinaCardsModPlugin.SwapExpansions.Value;
    }
    
    private void ConfigSettingChanged(object sender, EventArgs e)
    {
      if (!(e is SettingChangedEventArgs changedEventArgs))
        return;
      if (!(changedEventArgs.ChangedSetting.Definition.Key == MinaCardsModPlugin.SwapExpansionsKey))
        ;
      if (!(changedEventArgs.ChangedSetting.Definition.Key == MinaCardsModPlugin.CustomImagesKey))
        ;
      if (!(changedEventArgs.ChangedSetting.Definition.Key == MinaCardsModPlugin.CustomConfigsKey))
        ;
      if (!(changedEventArgs.ChangedSetting.Definition.Key == MinaCardsModPlugin.CustomBaseMonsterIconsKey))
        ;
      if (!(changedEventArgs.ChangedSetting.Definition.Key == MinaCardsModPlugin.SwapExpansionKeyboardShortcutKey))
        return;
      KeyboardShortcut boxedValue = (KeyboardShortcut) changedEventArgs.ChangedSetting.BoxedValue;
    }
  }
}