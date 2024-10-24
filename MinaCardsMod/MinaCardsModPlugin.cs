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
  [BepInPlugin("com.KalinuxYT.MinaCardsMod", "MinaCardsMod", "1.4.3")]
  public class MinaCardsModPlugin : BaseUnityPlugin
  {
    private const string MyGUID = "com.KalinuxYT.MinaCardsMod";
    private const string PluginName = "MinaCardsMod";
    private const string VersionString = "1.4.3";
    
    public static string SwapExpansionsKey = "Toggle card expansions";
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
    public static ConfigEntry<KeyboardShortcut> IncrementShopLevelShortcut; 
    public static ConfigEntry<KeyboardShortcut> AddCoinsShortcut;
    public static ConfigEntry<float> LicenseCostReductionPercentage; 
    public static ConfigEntry<float> LevelRequirementReductionPercentage; 
    public static ConfigEntry<float> ItemCostReductionPercentage;
   
    private static readonly Harmony Harmony = new Harmony("com.KalinuxYT.MinaCardsMod");
    public static ManualLogSource Log = new ManualLogSource("MinaCardsMod");
    public static bool isConfigGeneratorBuild = false;

    private void Awake()
    {
      this.Logger.LogInfo("Awake method called");
      
      MinaCardsModPlugin.LicenseCostReductionPercentage = this.Config.Bind<float>("Modifiers", "License Cost Reduction Percentage", 30.0f, new ConfigDescription("Percentage reduction applied to license cost (e.g., 30 for 30%).", new AcceptableValueRange<float>(0f, 100f)));
      MinaCardsModPlugin.LevelRequirementReductionPercentage = this.Config.Bind<float>("Modifiers", "Level Requirement Reduction Percentage", 30.0f, new ConfigDescription("Percentage reduction applied to license level requirement (e.g., 30 for 30%).", new AcceptableValueRange<float>(0f, 100f)));
      MinaCardsModPlugin.ItemCostReductionPercentage = this.Config.Bind<float>("Modifiers", "Item Cost Reduction Percentage", 30.0f, new ConfigDescription("Percentage reduction applied to item costs (e.g., 30 for 30%).", new AcceptableValueRange<float>(0f, 100f)));
      MinaCardsModPlugin.SwapExpansions = this.Config.Bind<bool>("General", MinaCardsModPlugin.SwapExpansionsKey, false, new ConfigDescription("Swap between the expansions in the collection book.", (AcceptableValueBase) null, Array.Empty<object>()));
      MinaCardsModPlugin.CustomNewExpansionImages = this.Config.Bind<bool>("General", MinaCardsModPlugin.CustomNewExpansionImagesKey, true, new ConfigDescription("Enable custom card images for new expansions", (AcceptableValueBase) null, Array.Empty<object>()));
      MinaCardsModPlugin.CustomNewExpansionConfigs = this.Config.Bind<bool>("General", MinaCardsModPlugin.CustomNewExpansionConfigsKey, true, new ConfigDescription("Enable custom configs for new expansions", (AcceptableValueBase) null, Array.Empty<object>()));
      MinaCardsModPlugin.CustomBaseMonsterImages = this.Config.Bind<bool>("General", MinaCardsModPlugin.CustomBaseMonsterImagesKey, true, new ConfigDescription("Enable custom card images for original expansions", (AcceptableValueBase) null, Array.Empty<object>()));
      MinaCardsModPlugin.CustomBaseConfigs = this.Config.Bind<bool>("General", MinaCardsModPlugin.CustomBaseConfigsKey, true, new ConfigDescription("Enable custom configs for original expansions", (AcceptableValueBase) null, Array.Empty<object>()));
      MinaCardsModPlugin.SwapExpansionKeyboardShortcut = this.Config.Bind<KeyboardShortcut>("General", MinaCardsModPlugin.SwapExpansionKeyboardShortcutKey, new KeyboardShortcut(KeyCode.U, Array.Empty<KeyCode>()));
      MinaCardsModPlugin.ReCacheFiles = this.Config.Bind<KeyboardShortcut>("General", MinaCardsModPlugin.ReCacheFilesKey, new KeyboardShortcut(KeyCode.None, Array.Empty<KeyCode>()));
      MinaCardsModPlugin.IncrementShopLevelShortcut = this.Config.Bind<KeyboardShortcut>("Cheats", "Increment Shop Level Shortcut", new KeyboardShortcut(KeyCode.L, KeyCode.LeftControl), new ConfigDescription("Shortcut to increment shop level by 1 while holding CTRL."));
      MinaCardsModPlugin.AddCoinsShortcut = this.Config.Bind<KeyboardShortcut>("Cheats", "Add Coins Shortcut", new KeyboardShortcut(KeyCode.C, KeyCode.LeftControl), new ConfigDescription("Shortcut to add 10,000 coins while holding CTRL."));
      
      MinaCardsModPlugin.SwapExpansions.SettingChanged += new EventHandler(this.ConfigSettingChanged);
      MinaCardsModPlugin.SwapExpansionKeyboardShortcut.SettingChanged += new EventHandler(this.ConfigSettingChanged);
      MinaCardsModPlugin.CustomNewExpansionImages.SettingChanged += new EventHandler(this.ConfigSettingChanged);
      MinaCardsModPlugin.CustomNewExpansionConfigs.SettingChanged += new EventHandler(this.ConfigSettingChanged);
      MinaCardsModPlugin.CustomBaseConfigs.SettingChanged += new EventHandler(this.ConfigSettingChanged);
      MinaCardsModPlugin.CustomBaseMonsterImages.SettingChanged += new EventHandler(this.ConfigSettingChanged);
      MinaCardsModPlugin.LicenseCostReductionPercentage.SettingChanged += new EventHandler(this.ConfigSettingChanged);
      MinaCardsModPlugin.LevelRequirementReductionPercentage.SettingChanged += new EventHandler(this.ConfigSettingChanged);
      MinaCardsModPlugin.ItemCostReductionPercentage.SettingChanged += new EventHandler(this.ConfigSettingChanged);
      MinaCardsModPlugin.IncrementShopLevelShortcut.SettingChanged += new EventHandler(this.ConfigSettingChanged);
      MinaCardsModPlugin.AddCoinsShortcut.SettingChanged += new EventHandler(this.ConfigSettingChanged);
      
      this.Logger.LogInfo((object) "PluginName: MinaCardsMod, VersionString: 1.4.3-Hardened is loading...");
      MinaCardsModPlugin.Harmony.PatchAll();
      this.Logger.LogInfo((object) "PluginName: MinaCardsMod, VersionString: 1.4.3-Hardened is loaded.");
      this.Logger.LogWarning((object) "This is a 'Hardened' build, this means any update to the game should not cause major issues. This mod is compatible with v0.48 of the game.");
      MinaCardsModPlugin.Log = this.Logger;
    }

    private void Update()
    {
      if (MinaCardsModPlugin.IncrementShopLevelShortcut.Value.IsUp())
      {
        IncrementShopLevel();
        MinaCardsModPlugin.Log.LogInfo("Shop level incremented by 1.");
      }
      if (MinaCardsModPlugin.AddCoinsShortcut.Value.IsUp())
      {
        AddCoins(10000);
        MinaCardsModPlugin.Log.LogInfo("10,000 coins added.");
      }
      if (MinaCardsModPlugin.SwapExpansionKeyboardShortcut.Value.IsUp())
      {
        if (!MinaCardsModPlugin.SwapExpansions.Value)
          SoundManager.PlayAudio("SFX_PercStarJingle3", 0.6f, 1.2f);
        else
          SoundManager.PlayAudio("SFX_PressFindMatch", 0.6f, 1.2f);
        MinaCardsModPlugin.SwapExpansions.Value = !MinaCardsModPlugin.SwapExpansions.Value;
      }
      if (!MinaCardsModPlugin.ReCacheFiles.Value.IsUp() || CacheHandler.isCurrentlyCacheing || MinaCardsModPlugin.isConfigGeneratorBuild)
        return; 
      CacheHandler.isCurrentlyCacheing = true;
      ExtrasHandler.swapPackNames();
      CacheHandler.CacheAllFiles();
      if (CSingleton<CGameManager>.Instance.m_IsGameLevel)
      {
        ExtrasHandler.AddHiddenCards();
        ExtrasHandler.DoReload();
      }
    }

    private void ConfigSettingChanged(object sender, EventArgs e)
    {
      if (!(e is SettingChangedEventArgs changedEventArgs))
        return;
      if (changedEventArgs.ChangedSetting.Definition.Key == MinaCardsModPlugin.IncrementShopLevelShortcut.Definition.Key ||
          changedEventArgs.ChangedSetting.Definition.Key == MinaCardsModPlugin.AddCoinsShortcut.Definition.Key)
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
    
    private void IncrementShopLevel()
    {
      CPlayerData.m_ShopLevel += 1;
      CEventManager.QueueEvent(new CEventPlayer_SetShopLevel(CPlayerData.m_ShopLevel));
    }
    private void AddCoins(float amount)
    {
      CPlayerData.m_CoinAmount += amount;
      CEventManager.QueueEvent(new CEventPlayer_SetCoin(CPlayerData.m_CoinAmount));
    }
  }

  public class CEventPlayer_SetShopLevel : CEvent
  {
    public int m_ShopLevel { get; private set; }

    public CEventPlayer_SetShopLevel(int shopLevel)
    {
      this.m_ShopLevel = shopLevel;
    }
  }
}