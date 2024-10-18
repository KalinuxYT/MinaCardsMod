using HarmonyLib;
using I2.Loc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using MinaCardsMod.Handlers;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#nullable disable
namespace MinaCardsMod.Patches
{
  [HarmonyPatch]
  internal class PlayerPatches
  {
    public static bool containsNew = false;
    public static string gameInstallPath = Path.GetDirectoryName(Application.dataPath);
    public static string gameBepinexPath = PlayerPatches.gameInstallPath + "/BepInEx/plugins";
    public static string customExpansionPackImages = PlayerPatches.gameBepinexPath + "/CustomExpansionPackImages/";
    public static string pluginPath = PlayerPatches.gameBepinexPath + "/MinaCardsMod/";
    public static string fantasyPackImages = PlayerPatches.customExpansionPackImages + "FantasyRPGPackImages/";
    public static string catJobPackImages = PlayerPatches.customExpansionPackImages + "CatJobPackImages/";
    public static string megabotPackImages = PlayerPatches.customExpansionPackImages + "MegabotPackImages/";
    public static string cardExtrasImages = PlayerPatches.customExpansionPackImages + "CardExtrasImages/";
    public static string tetramonPackImages = PlayerPatches.customExpansionPackImages + "TetramonPackImages/";
    public static string ghostPackImages = PlayerPatches.customExpansionPackImages + "GhostPackImages/";
    public static string configPath = PlayerPatches.customExpansionPackImages + "/Configs/Custom/";
    public static string catJobConfigPath = PlayerPatches.configPath + "CatJobConfigs/";
    public static string destinyConfigPath = PlayerPatches.configPath + "DestinyConfigs/";
    public static string fantasyRPGConfigPath = PlayerPatches.configPath + "FantasyRPGConfigs/";
    public static string fullExpansionsConfigPath = PlayerPatches.configPath + "FullExpansionsConfigs/";
    public static string ghostConfigPath = PlayerPatches.configPath + "GhostConfigs/";
    public static string megabotConfigPath = PlayerPatches.configPath + "MegabotConfigs/";
    public static string tetramonConfigPath = PlayerPatches.configPath + "TetramonConfigs/";
    public static CustomCardObject lastLoadedCard;
    public static CustomCardObject lastLoadedFullExpansionCard;
    public static string newCatJobPackName = LocalizationManager.GetTranslation("CatJob");
    public static string newFantasyRPGPackName = LocalizationManager.GetTranslation("FantasyRPG");
    public static string newMegaBotPackName = LocalizationManager.GetTranslation("Megabot");
    public static Color defaultButtonBorder = new Color(0.118f, 0.309f, 0.537f, 1f);
    public static Color defaultButtonMidtone = new Color(0.09f, 0.664f, 1f, 1f);
    public static Color defaultButtonHighlight = new Color(0.353f, 0.909f, 1f, 1f);
    public static Color newButtonBorder = new Color(0.4f, 0.125f, 0.05f, 1f);
    public static Color newButtonMidtone = new Color(0.5f, 0.125f, 0.7f, 1f);
    public static Color newButtonHighlight = new Color(0.6f, 0.2f, 0.75f, 1f);
    public static Vector2 defaultTetramonMonsterImageSize = new Vector2(0.2f, 197f);
    public static Vector2 defaultTetramonMonsterImagePosition = new Vector2(0.0f, -21f);
    public static Vector2 defaultTetramonMonsterFullArtImageSize = new Vector2(0.0f, 442.45f);
    public static Vector2 defaultTetramonMonsterFullArtImagePosition = new Vector2(0.0f, -66f);
    public static Vector2 defaultGhostMonsterImageSize = new Vector2(0.0f, 205.75f);
    public static Vector2 defaultGhostMonsterImagePosition = new Vector2(-9.1f, -6.4f);
    public static Vector2 defaultOtherMonsterImagePosition = new Vector2(0.0f, -96f);
    public static Vector2 defaultOtherMonsterFullArtImagePosition = new Vector2(0.0f, -116f);

    [HarmonyPostfix]
    [HarmonyPatch(typeof (CGameData), "PropagateLoadData")]
    public static void CGameData_PropagateLoadData_Postfix()
    {
      PlayerPatches.Log("Done doing propogate");
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof (CPlayerData), "CPlayer_OnSetFame")]
    public static void CPlayerData_CPlayer_OnSetFame_Postfix() => ExtrasHandler.DoFirstWorldLoad();

    [HarmonyPostfix]
    [HarmonyPatch(typeof (ItemSpawnManager), "Start")]
    public static void ItemSpawnManager_Start_Postfix()
    {
      ExtrasHandler.SwapNewPackItemImages();
      ExtrasHandler.SetPreviousEvolutions();
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof (CGameManager), "OnLevelFinishedLoading")]
    public static void CGameManager_OnLevelFinishedLoading_Postfix(
      ref Scene scene,
      LoadSceneMode mode)
    {
      if (CSingleton<CGameManager>.Instance.m_IsGameLevel)
        ExtrasHandler.AddHiddenCards();
      if (!(scene.name == "Title") || CacheHandler.firstLoad || MinaCardsModPlugin.isConfigGeneratorBuild)
        return;
      CacheHandler.CacheAllFiles();
      CacheHandler.firstLoad = true;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof (CardOpeningSequence), "InitOpenSequence")]
    public static void InitOpenSequence_Postfix(CardOpeningSequence __instance)
    {
      if (__instance.m_Card3dUIList.Count<Card3dUIGroup>() <= 0)
        return;
      foreach (Card3dUIGroup card3dUi in __instance.m_Card3dUIList)
        ExtrasHandler.SetCardBacks(card3dUi);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof (InteractionPlayerController), "AddHoldCard")]
    public static void InteractionPlayerController_AddHoldCard_Postfix(
      InteractionPlayerController __instance,
      InteractableCard3d card3d)
    {
      if (!(bool) (UnityEngine.Object) card3d)
        return;
      ExtrasHandler.SetCardBacks(card3d.m_Card3dUI);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof (CardExpansionSelectScreen), "OpenScreen")]
    public static void CardExpansionSelectScreen_OpenScreen_Postfix(
      CardExpansionSelectScreen __instance)
    {
      GameObject screenGrp = CSingleton<CardExpansionSelectScreen>.Instance.m_ScreenGrp;
      Transform[] componentsInChildren1 = screenGrp.GetComponentsInChildren<Transform>();
      Transform transform1 = ((IEnumerable<Transform>) componentsInChildren1).FirstOrDefault<Transform>((Func<Transform, bool>) (t => t.name == "Tetramon_Button"));
      Transform transform2 = ((IEnumerable<Transform>) componentsInChildren1).FirstOrDefault<Transform>((Func<Transform, bool>) (t => t.name == "Destiny_Button"));
      Transform transform3 = ((IEnumerable<Transform>) componentsInChildren1).FirstOrDefault<Transform>((Func<Transform, bool>) (t => t.name == "Ghost_Button"));
      if ((bool) (UnityEngine.Object) transform1 && (bool) (UnityEngine.Object) transform2 && (bool) (UnityEngine.Object) transform3)
      {
        List<Transform> transformList = new List<Transform>()
        {
          transform1,
          transform2,
          transform3
        };
        Color color1 = MinaCardsModPlugin.SwapExpansions.Value ? PlayerPatches.newButtonBorder : PlayerPatches.defaultButtonBorder;
        Color color2 = MinaCardsModPlugin.SwapExpansions.Value ? PlayerPatches.newButtonMidtone : PlayerPatches.defaultButtonMidtone;
        Color color3 = MinaCardsModPlugin.SwapExpansions.Value ? PlayerPatches.newButtonHighlight : PlayerPatches.defaultButtonHighlight;
        foreach (Component component in transformList)
        {
          foreach (Image componentsInChild in component.GetComponentsInChildren<Image>())
          {
            switch (componentsInChild.name)
            {
              case "BGBorder":
                componentsInChild.color = color1;
                break;
              case "BGMidtone":
                componentsInChild.color = color2;
                break;
              case "BGHighlight":
                componentsInChild.color = color3;
                break;
            }
          }
        }
      }
      TMP_Text[] componentsInChildren2 = screenGrp.GetComponentsInChildren<TMP_Text>();
      string translation1 = LocalizationManager.GetTranslation("Tetramon Base");
      string translation2 = LocalizationManager.GetTranslation("Tetramon Destiny");
      string translation3 = LocalizationManager.GetTranslation("Tetramon Ghost");
      string str1 = MinaCardsModPlugin.SwapExpansions.Value ? PlayerPatches.newMegaBotPackName : translation1;
      string str2 = MinaCardsModPlugin.SwapExpansions.Value ? PlayerPatches.newFantasyRPGPackName : translation2;
      string str3 = MinaCardsModPlugin.SwapExpansions.Value ? PlayerPatches.newCatJobPackName : translation3;
      foreach (TMP_Text tmpText in componentsInChildren2)
      {
        if (!string.IsNullOrEmpty(tmpText.text))
        {
          if (tmpText.text == translation1 || tmpText.text == PlayerPatches.newMegaBotPackName)
            tmpText.text = str1;
          else if (tmpText.text == translation2 || tmpText.text == PlayerPatches.newFantasyRPGPackName)
            tmpText.text = str2;
          else if (tmpText.text == translation3 || tmpText.text == PlayerPatches.newCatJobPackName)
            tmpText.text = str3;
        }
      }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof (CardExpansionSelectScreen), "OpenScreen")]
    public static void CardExpansionSelectScreen_OpenScreen_Prefix(
      ref ECardExpansionType initCardExpansion)
    {
      int num = (int) initCardExpansion;
      if (num < 3)
        return;
      initCardExpansion = (ECardExpansionType) (num - 3);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(CheckPriceScreen), "OnCardExpansionUpdated")]
    public static void CheckPriceScreen_OnCardExpansionUpdated_Prefix(
      CheckPriceScreen __instance,
      ref CEventPlayer_OnCardExpansionSelectScreenUpdated evt)
    {
      if (!MinaCardsModPlugin.SwapExpansions.Value)
        return;
      var property = AccessTools.Property(typeof(CEventPlayer_OnCardExpansionSelectScreenUpdated), "m_CardExpansionTypeIndex");
      if (property != null)
      {
        int currentValue = (int)property.GetValue(evt);
        property.SetValue(evt, currentValue + 3);
      }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof (InventoryBase), "GetCardExpansionName")]
    public static bool InventoryBase_GetCardExpansionName_Prefix(
      InventoryBase __instance,
      ECardExpansionType cardExpansion,
      ref string __result)
    {
      if (cardExpansion == ECardExpansionType.Tetramon || cardExpansion == ECardExpansionType.Destiny || cardExpansion == ECardExpansionType.Ghost)
        return true;
      switch (cardExpansion)
      {
        case ECardExpansionType.Megabot:
          __result = PlayerPatches.newMegaBotPackName;
          return false;
        case ECardExpansionType.FantasyRPG:
          __result = PlayerPatches.newFantasyRPGPackName;
          return false;
        case ECardExpansionType.CatJob:
          __result = PlayerPatches.newCatJobPackName;
          return false;
        default:
          return true;
      }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof (CollectionBinderUI), "OnPressSwitchExpansion")]
    public static bool CollectionBinderUI_OnPressSwitchExpansion_Prefix(
      ref CollectionBinderUI __instance,
      ref int expansionIndex)
    {
      if (MinaCardsModPlugin.SwapExpansions.Value)
        expansionIndex += 3;
      return true;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof (CollectionBinderUI), "OpenSortAlbumScreen")]
    public static void CollectionBinderUI_OpenSortAlbumScreen_HarmonyPostfix(
      ref CollectionBinderUI __instance,
      int sortingMethodIndex,
      ref int currentExpansionIndex)
    {
      if (MinaCardsModPlugin.SwapExpansions.Value)
      {
        __instance.m_ExpansionBtnList[0].GetComponentInChildren<TMP_Text>().text = PlayerPatches.newMegaBotPackName;
        __instance.m_ExpansionBtnList[1].GetComponentInChildren<TMP_Text>().text = PlayerPatches.newFantasyRPGPackName;
        __instance.m_ExpansionBtnList[2].GetComponentInChildren<TMP_Text>().text = PlayerPatches.newCatJobPackName;
        foreach (Component expansionBtn in __instance.m_ExpansionBtnList)
        {
          foreach (Image componentsInChild in expansionBtn.GetComponentsInChildren<Image>())
          {
            if (componentsInChild.name == "BGBorder")
              componentsInChild.color = PlayerPatches.newButtonBorder;
            else if (componentsInChild.name == "BGMidtone")
              componentsInChild.color = PlayerPatches.newButtonMidtone;
            else if (componentsInChild.name == "BGHighlight")
              componentsInChild.color = PlayerPatches.newButtonHighlight;
          }
        }
      }
      else
      {
        __instance.m_ExpansionBtnList[0].GetComponentInChildren<TMP_Text>().text = LocalizationManager.GetTranslation("Tetramon");
        __instance.m_ExpansionBtnList[1].GetComponentInChildren<TMP_Text>().text = LocalizationManager.GetTranslation("Destiny");
        __instance.m_ExpansionBtnList[2].GetComponentInChildren<TMP_Text>().text = LocalizationManager.GetTranslation("Ghost");
        foreach (Component expansionBtn in __instance.m_ExpansionBtnList)
        {
          foreach (Image componentsInChild in expansionBtn.GetComponentsInChildren<Image>())
          {
            if (componentsInChild.name == "BGBorder")
              componentsInChild.color = PlayerPatches.defaultButtonBorder;
            else if (componentsInChild.name == "BGMidtone")
              componentsInChild.color = PlayerPatches.defaultButtonMidtone;
            else if (componentsInChild.name == "BGHighlight")
              componentsInChild.color = PlayerPatches.defaultButtonHighlight;
          }
        }
      }
      __instance.m_ExpansionBtnList[0].GetComponentInChildren<TMP_Text>();
      __instance.m_ExpansionBtnList[0].GetComponentInChildren<TMP_Text>();
      __instance.m_ExpansionBtnList[0].GetComponentInChildren<TMP_Text>();
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof (CollectionBinderUI), "OpenSortAlbumScreen")]
    public static bool CollectionBinderUI_OpenSortAlbumScreen_Prefix(
      ref CollectionBinderUI __instance,
      int sortingMethodIndex,
      ref int currentExpansionIndex)
    {
      if (currentExpansionIndex >= 3)
        currentExpansionIndex -= 3;
      return true;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof (CollectionBinderFlipAnimCtrl), "EnterViewUpCloseState")]
    public static void CollectionBinderFlipAnimCtrl_EnterViewUpCloseState_Postfix(
      CollectionBinderFlipAnimCtrl __instance)
    {
      CardData cardData = __instance.m_CurrentViewInteractableCard3d.m_Card3dUI.m_CardUI.GetCardData();
      if (!ExtrasHandler.isCardConfigDriven(cardData))
        return;
      bool isGhost = cardData.expansionType == ECardExpansionType.Ghost;
      bool isFoil = cardData.isFoil;
      if (__instance.m_CollectionBinderUI.m_CardNameText.text != __instance.m_CurrentSpawnedInteractableCard3d.m_Card3dUI.m_CardUI.m_MonsterNameText.text)
        __instance.m_CollectionBinderUI.m_CardNameText.text = __instance.m_CurrentSpawnedInteractableCard3d.m_Card3dUI.m_CardUI.m_MonsterNameText.text;
      CardUI cardUi1 = __instance.m_CurrentViewInteractableCard3d.m_Card3dUI.m_CardUI;
      CardUI cardUi2 = !isGhost || !((UnityEngine.Object) cardUi1.m_GhostCard != (UnityEngine.Object) null) ? __instance.m_CurrentViewInteractableCard3d.m_Card3dUI.m_CardUI : ExtrasHandler.CurrentCardUI(isGhost, cardUi1, cardUi1.m_GhostCard);
      string text1 = cardUi2.m_FirstEditionText.text;
      string text2 = cardUi2.m_RarityText.text;
      string str1 = "";
      Transform transform = cardUi2.transform.Find("FoilText");
      if ((UnityEngine.Object) transform != (UnityEngine.Object) null)
      {
        TextMeshProUGUI component = transform.GetComponent<TextMeshProUGUI>();
        if ((UnityEngine.Object) component != (UnityEngine.Object) null)
          str1 = component.text;
      }
      string str2 = "";
      if (!isFoil)
        str2 = text1 + " " + text2;
      else if (isFoil)
        str2 = text1 + " " + text2 + " " + str1;
      __instance.m_CollectionBinderUI.m_CardFullRarityNameText.text = str2;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof (RestockItemPanelUI), "Init")]
    public static bool RestockItemPanelUI_Init_Prefix(
      RestockItemPanelUI __instance,
      RestockItemScreen restockItemScreen,
      int index)
    {
      if (!PlayerPatches.containsNew)
      {
        if (!CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_ShownItemType.Contains(EItemType.FantasyRPGPack))
        {
          CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_ShownItemType.Add(EItemType.FantasyRPGPack);
          CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_ShownItemType.Add(EItemType.CatJobPack);
          CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_ShownItemType.Add(EItemType.MegabotPack);
          CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_ShownItemType.Add(EItemType.GhostPack);
        }
        if (!CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_ShownAllItemType.Contains(EItemType.FantasyRPGPack))
        {
          CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_ShownAllItemType.Add(EItemType.FantasyRPGPack);
          CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_ShownAllItemType.Add(EItemType.CatJobPack);
          CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_ShownAllItemType.Add(EItemType.MegabotPack);
          CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_ShownAllItemType.Add(EItemType.GhostPack);
        }
        RestockData restockData1 = new RestockData();
        RestockData restockData2 = new RestockData();
        RestockData restockData3 = new RestockData();
        RestockData restockData4 = new RestockData();
        RestockData restockData5 = new RestockData();
        RestockData restockData6 = new RestockData();
        RestockData restockData7 = new RestockData();
        RestockData restockData8 = new RestockData();
        restockData1.itemType = EItemType.FantasyRPGPack;
        restockData1.licenseShopLevelRequired = 30;
        restockData1.licensePrice = 2500f;
        CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_RestockDataList.Add(restockData1);
        restockData2.itemType = EItemType.FantasyRPGPack;
        restockData2.licenseShopLevelRequired = 30;
        restockData2.licensePrice = 5000f;
        restockData2.amount = 64;
        restockData2.isBigBox = true;
        CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_RestockDataList.Add(restockData2);
        restockData3.itemType = EItemType.CatJobPack;
        restockData3.licenseShopLevelRequired = 40;
        restockData3.licensePrice = 5000f;
        CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_RestockDataList.Add(restockData3);
        restockData4.itemType = EItemType.CatJobPack;
        restockData4.licenseShopLevelRequired = 40;
        restockData4.licensePrice = 7500f;
        restockData4.amount = 64;
        restockData4.isBigBox = true;
        CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_RestockDataList.Add(restockData4);
        restockData5.itemType = EItemType.MegabotPack;
        restockData5.licenseShopLevelRequired = 50;
        restockData5.licensePrice = 7500f;
        CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_RestockDataList.Add(restockData5);
        restockData6.itemType = EItemType.MegabotPack;
        restockData6.licenseShopLevelRequired = 50;
        restockData6.licensePrice = 10000f;
        restockData6.amount = 64;
        restockData6.isBigBox = true;
        CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_RestockDataList.Add(restockData6);
        restockData7.itemType = EItemType.GhostPack;
        restockData7.licenseShopLevelRequired = 60;
        restockData7.licensePrice = 10000f;
        CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_RestockDataList.Add(restockData7);
        restockData8.itemType = EItemType.GhostPack;
        restockData8.licenseShopLevelRequired = 60;
        restockData8.licensePrice = 15000f;
        restockData8.amount = 64;
        restockData8.isBigBox = true;
        CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_RestockDataList.Add(restockData8);
        PlayerPatches.containsNew = true;
      }
      return true;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof (CardUI), "SetCardUI")]
    public static void CardUI_SetCardUI_Main_Postfix(CardUI __instance, CardData cardData)
    {
      if (MinaCardsModPlugin.isConfigGeneratorBuild)
      {
        ConfigGeneratorHelper.writeMonsterData(cardData, __instance);
        ConfigGeneratorHelper.WriteAllFullExpansionConfigs();
      }
      ExtrasHandler.SetCardExtrasImages(__instance, cardData);
      bool flag = cardData.borderType == ECardBorderType.FullArt;
      bool isGhost = cardData.expansionType == ECardExpansionType.Ghost;
      CardUI cardUi1 = !isGhost || !((UnityEngine.Object) __instance.m_GhostCard != (UnityEngine.Object) null) ? __instance : ExtrasHandler.CurrentCardUI(isGhost, __instance, __instance.m_GhostCard);
      if (ExtrasHandler.isCardConfigDriven(cardData) && !MinaCardsModPlugin.isConfigGeneratorBuild)
      {
        CustomCardObject cardConfig = (CustomCardObject) null;
        CustomCardObject fullExpansionConfig = (CustomCardObject) null;
        string fileName = (string) null;
        string expansionName = (string) null;
        if (isGhost)
        {
          fileName = cardData.monsterType.ToString();
          expansionName = cardData.expansionType.ToString();
        }
        else if (!isGhost)
        {
          if (!flag)
          {
            fileName = cardData.monsterType.ToString();
            expansionName = cardData.expansionType.ToString();
          }
          else if (flag)
          {
            fileName = cardData.monsterType.ToString() + "FullArt";
            expansionName = cardData.expansionType.ToString() + "FullArt";
          }
        }
        if (fileName != null && expansionName != null)
        {
          if (cardData.expansionType == ECardExpansionType.Tetramon)
          {
            CustomCardObject customCardObject1 = CacheHandler.tetramonConfigCache.FirstOrDefault<CustomCardObject>((Func<CustomCardObject, bool>) (customCardObject => customCardObject.Header == fileName));
            if (customCardObject1 != null)
              cardConfig = customCardObject1;
            else
              PlayerPatches.LogError("Null card Tetramon");
          }
          else if (cardData.expansionType == ECardExpansionType.Destiny)
          {
            CustomCardObject customCardObject2 = CacheHandler.destinyConfigCache.FirstOrDefault<CustomCardObject>((Func<CustomCardObject, bool>) (customCardObject => customCardObject.Header == fileName));
            if (customCardObject2 != null)
              cardConfig = customCardObject2;
            else
              PlayerPatches.LogError("Null card Destiny");
          }
          else if (cardData.expansionType == ECardExpansionType.Ghost)
          {
            CustomCardObject customCardObject3 = CacheHandler.ghostConfigCache.FirstOrDefault<CustomCardObject>((Func<CustomCardObject, bool>) (customCardObject => customCardObject.Header == fileName));
            if (customCardObject3 != null)
              cardConfig = customCardObject3;
            else
              PlayerPatches.LogError("Null card Ghost");
          }
          else if (cardData.expansionType == ECardExpansionType.CatJob)
          {
            CustomCardObject customCardObject4 = CacheHandler.catJobConfigCache.FirstOrDefault<CustomCardObject>((Func<CustomCardObject, bool>) (customCardObject => customCardObject.Header == fileName));
            if (customCardObject4 != null)
              cardConfig = customCardObject4;
            else
              PlayerPatches.LogError("Null card CatJob");
          }
          else if (cardData.expansionType == ECardExpansionType.FantasyRPG)
          {
            CustomCardObject customCardObject5 = CacheHandler.fantasyRPGConfigCache.FirstOrDefault<CustomCardObject>((Func<CustomCardObject, bool>) (customCardObject => customCardObject.Header == fileName));
            if (customCardObject5 != null)
              cardConfig = customCardObject5;
            else
              PlayerPatches.LogError("Null card Fantasy");
          }
          else if (cardData.expansionType == ECardExpansionType.Megabot)
          {
            CustomCardObject customCardObject6 = CacheHandler.megabotConfigCache.FirstOrDefault<CustomCardObject>((Func<CustomCardObject, bool>) (customCardObject => customCardObject.Header == fileName));
            if (customCardObject6 != null)
              cardConfig = customCardObject6;
            else
              PlayerPatches.LogError("Null card Megabot");
          }
          CustomCardObject customCardObject7 = CacheHandler.fullExpansionsConfigCache.FirstOrDefault<CustomCardObject>((Func<CustomCardObject, bool>) (customCardObject => customCardObject.Header == expansionName));
          if (customCardObject7 != null)
            fullExpansionConfig = customCardObject7;
          else
            PlayerPatches.LogError("Null card Expansion");
        }
        if (cardConfig == null || fullExpansionConfig == null)
          return;
        CustomCardObject customCardObject8 = ExtrasHandler.SelectConfig(cardConfig, fullExpansionConfig);
        cardUi1.m_MonsterNameText.text = cardConfig.Name;
        if (!isGhost)
        {
          cardUi1.m_DescriptionText.text = cardConfig.Description;
          if (flag && (UnityEngine.Object) __instance.m_FullArtCard != (UnityEngine.Object) null && (UnityEngine.Object) __instance.m_FullArtCard.m_DescriptionText != (UnityEngine.Object) null && __instance.m_FullArtCard.m_DescriptionText.text != null)
            __instance.m_FullArtCard.m_DescriptionText.text = cardConfig.Description;
          cardUi1.m_ChampionText.text = customCardObject8.ChampionText;
          cardUi1.m_DescriptionText.enabled = customCardObject8.DescriptionEnabled;
          cardUi1.m_DescriptionText.fontSize = customCardObject8.DescriptionFontSize;
          cardUi1.m_DescriptionText.fontSizeMin = customCardObject8.DescriptionFontSizeMin;
          cardUi1.m_DescriptionText.fontSizeMax = customCardObject8.DescriptionFontSizeMax;
          cardUi1.m_DescriptionText.color = customCardObject8.DescriptionFontColorRGBA;
          cardUi1.m_DescriptionText.rectTransform.anchoredPosition = customCardObject8.DescriptionPosition;
          cardUi1.m_EvoPreviousStageIcon.enabled = customCardObject8.PreviousEvolutionIconEnabled;
          cardUi1.m_EvoPreviousStageNameText.enabled = customCardObject8.PreviousEvolutionBoxEnabled;
          FieldInfo monsterDataField = typeof(CardUI).GetField("m_MonsterData", BindingFlags.NonPublic | BindingFlags.Instance);
          if (monsterDataField != null)
          {
            MonsterData monsterData = (MonsterData)monsterDataField.GetValue(cardUi1);
            if (monsterData.PreviousEvolution == EMonsterType.None)
            {
              if ((UnityEngine.Object)ExtrasHandler.GetImageComponentByName(cardUi1.gameObject, "EvoBasicIcon") != (UnityEngine.Object)null)
                ExtrasHandler.GetImageComponentByName(cardUi1.gameObject, "EvoBasicIcon").enabled = customCardObject8.BasicEvolutionIconEnabled;
              if ((UnityEngine.Object)ExtrasHandler.GetTextComponentByName(cardUi1.gameObject, "EvoBasicText") != (UnityEngine.Object)null)
              {
                TextMeshProUGUI textComponentByName = ExtrasHandler.GetTextComponentByName(cardUi1.gameObject, "EvoBasicText");
                textComponentByName.text = customCardObject8.BasicEvolutionText;
                textComponentByName.enabled = customCardObject8.BasicEvolutionTextEnabled;
                textComponentByName.fontSize = customCardObject8.BasicEvolutionTextFontSize;
                textComponentByName.fontSizeMin = customCardObject8.BasicEvolutionTextFontSizeMin;
                textComponentByName.fontSizeMax = customCardObject8.BasicEvolutionTextFontSizeMax;
                textComponentByName.color = customCardObject8.BasicEvolutionTextFontColorRGBA;
                textComponentByName.rectTransform.anchoredPosition = customCardObject8.BasicEvolutionTextPosition;
              }
            }
          }
          cardUi1.m_EvoPreviousStageNameText.fontSize = customCardObject8.PreviousEvolutionNameFontSize;
          cardUi1.m_EvoPreviousStageNameText.fontSizeMin = customCardObject8.PreviousEvolutionNameFontSizeMin;
          cardUi1.m_EvoPreviousStageNameText.fontSizeMax = customCardObject8.PreviousEvolutionNameFontSizeMax;
          cardUi1.m_EvoPreviousStageNameText.color = customCardObject8.PreviousEvolutionNameFontColorRGBA;
          cardUi1.m_EvoPreviousStageNameText.rectTransform.anchoredPosition = customCardObject8.PreviousEvolutionNamePosition;
        }
        if (!flag)
          cardUi1.m_NumberText.text = cardConfig.Number;
        if ((UnityEngine.Object) ExtrasHandler.GetImageComponentByName(cardUi1.gameObject, "EvoBG") != (UnityEngine.Object) null)
          ExtrasHandler.GetImageComponentByName(cardUi1.gameObject, "EvoBG").enabled = customCardObject8.PreviousEvolutionBoxEnabled;
        if ((UnityEngine.Object) ExtrasHandler.GetImageComponentByName(cardUi1.gameObject, "EvoBorder") != (UnityEngine.Object) null)
          ExtrasHandler.GetImageComponentByName(cardUi1.gameObject, "EvoBorder").enabled = customCardObject8.PreviousEvolutionBoxEnabled;
        if ((UnityEngine.Object) ExtrasHandler.GetTextComponentByName(cardUi1.gameObject, "TitleText") != (UnityEngine.Object) null)
        {
          TextMeshProUGUI textComponentByName = ExtrasHandler.GetTextComponentByName(cardUi1.gameObject, "TitleText");
          textComponentByName.text = cardConfig.PlayEffectText;
          textComponentByName.enabled = customCardObject8.PlayEffectTextEnabled;
          textComponentByName.fontSize = customCardObject8.PlayEffectTextFontSize;
          textComponentByName.fontSizeMin = customCardObject8.PlayEffectTextFontSizeMin;
          textComponentByName.fontSizeMax = customCardObject8.PlayEffectTextFontSizeMax;
          textComponentByName.color = customCardObject8.PlayEffectTextFontColorRGBA;
          textComponentByName.rectTransform.anchoredPosition = customCardObject8.PlayEffectTextPosition;
        }
        if ((UnityEngine.Object) ExtrasHandler.GetTextComponentByName(cardUi1.gameObject, "EvoText") != (UnityEngine.Object) null)
          ExtrasHandler.GetTextComponentByName(cardUi1.gameObject, "EvoText").enabled = customCardObject8.PreviousEvolutionNameEnabled;
        if ((UnityEngine.Object) ExtrasHandler.GetImageComponentByName(cardUi1.gameObject, "TitleBG") != (UnityEngine.Object) null)
          ExtrasHandler.GetImageComponentByName(cardUi1.gameObject, "TitleBG").enabled = customCardObject8.PlayEffectBoxEnabled;
        if (isGhost | flag && (UnityEngine.Object) cardUi1.m_FirstEditionText != (UnityEngine.Object) null && cardUi1.m_FirstEditionText.text != null)
          cardUi1.m_FirstEditionText.text = customCardObject8.EditionText;
        if ((UnityEngine.Object) cardUi1.m_FirstEditionText != (UnityEngine.Object) null && !isGhost && !flag && cardUi1.m_FirstEditionText.text != null)
        {
          switch (cardData.borderType)
          {
            case ECardBorderType.Base:
              cardUi1.m_FirstEditionText.text = customCardObject8.BasicEditionText;
              break;
            case ECardBorderType.FirstEdition:
              cardUi1.m_FirstEditionText.text = customCardObject8.FirstEditionText;
              break;
            case ECardBorderType.Silver:
              cardUi1.m_FirstEditionText.text = customCardObject8.SilverEditionText;
              break;
            case ECardBorderType.Gold:
              cardUi1.m_FirstEditionText.text = customCardObject8.GoldEditionText;
              break;
            case ECardBorderType.EX:
              cardUi1.m_FirstEditionText.text = customCardObject8.EXEditionText;
              break;
          }
        }
        cardUi1.m_MonsterNameText.enabled = customCardObject8.NameEnabled;
        cardUi1.m_MonsterNameText.fontSize = customCardObject8.NameFontSize;
        cardUi1.m_MonsterNameText.fontSizeMin = customCardObject8.NameFontSizeMin;
        cardUi1.m_MonsterNameText.fontSizeMax = customCardObject8.NameFontSizeMax;
        cardUi1.m_MonsterNameText.color = customCardObject8.NameFontColorRGBA;
        cardUi1.m_MonsterNameText.rectTransform.anchoredPosition = customCardObject8.NamePosition;
        if (!isGhost && !flag)
        {
          if ((UnityEngine.Object) cardUi1.m_FirstEditionText != (UnityEngine.Object) null && cardUi1.m_FirstEditionText.text != null)
          {
            cardUi1.m_FirstEditionText.fontSize = customCardObject8.EditionTextFontSize;
            cardUi1.m_FirstEditionText.fontSizeMin = customCardObject8.EditionTextFontSizeMin;
            cardUi1.m_FirstEditionText.fontSizeMax = customCardObject8.EditionTextFontSizeMax;
            cardUi1.m_FirstEditionText.color = customCardObject8.EditionTextFontColorRGBA;
            cardUi1.m_FirstEditionText.rectTransform.anchoredPosition = customCardObject8.EditionTextPosition;
          }
          cardUi1.m_RarityText.text = cardConfig.Rarity;
          cardUi1.m_NumberText.enabled = customCardObject8.NumberEnabled;
          cardUi1.m_NumberText.fontSize = customCardObject8.NumberFontSize;
          cardUi1.m_NumberText.fontSizeMin = customCardObject8.NumberFontSizeMin;
          cardUi1.m_NumberText.fontSizeMax = customCardObject8.NumberFontSizeMax;
          cardUi1.m_NumberText.color = customCardObject8.NumberFontColorRGBA;
          cardUi1.m_NumberText.rectTransform.anchoredPosition = customCardObject8.NumberPosition;
          cardUi1.m_FirstEditionText.enabled = customCardObject8.EditionTextEnabled;
          cardUi1.m_RarityText.enabled = customCardObject8.RarityEnabled;
          cardUi1.m_RarityText.fontSize = customCardObject8.RarityFontSize;
          cardUi1.m_RarityText.fontSizeMin = customCardObject8.RarityFontSizeMin;
          cardUi1.m_RarityText.fontSizeMax = customCardObject8.RarityFontSizeMax;
          cardUi1.m_RarityText.color = customCardObject8.RarityFontColorRGBA;
          cardUi1.m_RarityText.rectTransform.anchoredPosition = customCardObject8.RarityPosition;
        }
        if ((UnityEngine.Object) ExtrasHandler.GetImageComponentByName(cardUi1.gameObject, "RarityImage") != (UnityEngine.Object) null)
          ExtrasHandler.GetImageComponentByName(cardUi1.gameObject, "RarityImage").enabled = customCardObject8.RarityImageEnabled;
        if (!isGhost)
        {
          cardUi1.m_ChampionText.enabled = customCardObject8.ChampionTextEnabled;
          cardUi1.m_ChampionText.fontSize = customCardObject8.ChampionFontSize;
          cardUi1.m_ChampionText.fontSizeMin = customCardObject8.ChampionFontSizeMin;
          cardUi1.m_ChampionText.fontSizeMax = customCardObject8.ChampionFontSizeMax;
          cardUi1.m_ChampionText.color = customCardObject8.ChampionFontColorRGBA;
          cardUi1.m_ChampionText.rectTransform.anchoredPosition = customCardObject8.ChampionPosition;
        }
        if (isGhost && (UnityEngine.Object) ExtrasHandler.GetImageComponentByName(cardUi1.gameObject, "CardStat") != (UnityEngine.Object) null)
          ExtrasHandler.GetImageComponentByName(cardUi1.gameObject, "CardStat").enabled = cardConfig.StatBackgroundImageEnabled;
        cardUi1.m_Stat1Text.text = cardConfig.Stat1;
        cardUi1.m_Stat1Text.enabled = customCardObject8.Stat1Enabled;
        cardUi1.m_Stat1Text.fontSize = customCardObject8.Stat1FontSize;
        cardUi1.m_Stat1Text.fontSizeMin = customCardObject8.Stat1FontSizeMin;
        cardUi1.m_Stat1Text.fontSizeMax = customCardObject8.Stat1FontSizeMax;
        cardUi1.m_Stat1Text.color = customCardObject8.Stat1FontColorRGBA;
        cardUi1.m_Stat1Text.rectTransform.anchoredPosition = customCardObject8.Stat1Position;
        cardUi1.m_Stat2Text.text = cardConfig.Stat2;
        cardUi1.m_Stat2Text.enabled = customCardObject8.Stat2Enabled;
        cardUi1.m_Stat2Text.fontSize = customCardObject8.Stat2FontSize;
        cardUi1.m_Stat2Text.fontSizeMin = customCardObject8.Stat2FontSizeMin;
        cardUi1.m_Stat2Text.fontSizeMax = customCardObject8.Stat2FontSizeMax;
        cardUi1.m_Stat2Text.color = customCardObject8.Stat2FontColorRGBA;
        cardUi1.m_Stat2Text.rectTransform.anchoredPosition = customCardObject8.Stat2Position;
        cardUi1.m_Stat3Text.text = cardConfig.Stat3;
        cardUi1.m_Stat3Text.enabled = customCardObject8.Stat3Enabled;
        cardUi1.m_Stat3Text.fontSize = customCardObject8.Stat3FontSize;
        cardUi1.m_Stat3Text.fontSizeMin = customCardObject8.Stat3FontSizeMin;
        cardUi1.m_Stat3Text.fontSizeMax = customCardObject8.Stat3FontSizeMax;
        cardUi1.m_Stat3Text.color = customCardObject8.Stat3FontColorRGBA;
        cardUi1.m_Stat3Text.rectTransform.anchoredPosition = customCardObject8.Stat3Position;
        cardUi1.m_Stat4Text.text = cardConfig.Stat4;
        cardUi1.m_Stat4Text.enabled = customCardObject8.Stat4Enabled;
        cardUi1.m_Stat4Text.fontSize = customCardObject8.Stat4FontSize;
        cardUi1.m_Stat4Text.fontSizeMin = customCardObject8.Stat4FontSizeMin;
        cardUi1.m_Stat4Text.fontSizeMax = customCardObject8.Stat4FontSizeMax;
        cardUi1.m_Stat4Text.color = customCardObject8.Stat4FontColorRGBA;
        cardUi1.m_Stat4Text.rectTransform.anchoredPosition = customCardObject8.Stat4Position;
        if ((UnityEngine.Object) cardUi1.m_ArtistText != (UnityEngine.Object) null)
        {
          if (flag && (UnityEngine.Object) cardUi1.m_FullArtCard != (UnityEngine.Object) null && (UnityEngine.Object) cardUi1.m_FullArtCard.m_ArtistText != (UnityEngine.Object) null && cardUi1.m_FullArtCard.m_ArtistText.text != null)
          {
            cardUi1.m_FullArtCard.m_ArtistText.text = customCardObject8.ArtistText;
            cardUi1.m_FullArtCard.m_ArtistText.enabled = customCardObject8.ArtistTextEnabled;
            cardUi1.m_FullArtCard.m_ArtistText.fontSize = customCardObject8.ArtistTextFontSize;
            cardUi1.m_FullArtCard.m_ArtistText.fontSizeMin = customCardObject8.ArtistTextFontSizeMin;
            cardUi1.m_FullArtCard.m_ArtistText.fontSizeMax = customCardObject8.ArtistTextFontSizeMax;
            cardUi1.m_FullArtCard.m_ArtistText.color = customCardObject8.ArtistTextFontColorRGBA;
            cardUi1.m_FullArtCard.m_ArtistText.rectTransform.anchoredPosition = customCardObject8.ArtistTextPosition;
          }
          cardUi1.m_ArtistText.text = customCardObject8.ArtistText;
          cardUi1.m_ArtistText.enabled = customCardObject8.ArtistTextEnabled;
          cardUi1.m_ArtistText.fontSize = customCardObject8.ArtistTextFontSize;
          cardUi1.m_ArtistText.fontSizeMin = customCardObject8.ArtistTextFontSizeMin;
          cardUi1.m_ArtistText.fontSizeMax = customCardObject8.ArtistTextFontSizeMax;
          cardUi1.m_ArtistText.color = customCardObject8.ArtistTextFontColorRGBA;
          cardUi1.m_ArtistText.rectTransform.anchoredPosition = customCardObject8.ArtistTextPosition;
        }
        if ((UnityEngine.Object) ExtrasHandler.GetTextComponentByName(cardUi1.gameObject, "CompanyText") != (UnityEngine.Object) null)
        {
          TextMeshProUGUI textComponentByName = ExtrasHandler.GetTextComponentByName(cardUi1.gameObject, "CompanyText");
          textComponentByName.text = customCardObject8.CompanyText;
          textComponentByName.enabled = customCardObject8.CompanyTextEnabled;
          textComponentByName.fontSize = customCardObject8.CompanyTextFontSize;
          textComponentByName.fontSizeMin = customCardObject8.CompanyTextFontSizeMin;
          textComponentByName.fontSizeMax = customCardObject8.CompanyTextFontSizeMax;
          textComponentByName.color = customCardObject8.CompanyTextFontColorRGBA;
          textComponentByName.rectTransform.anchoredPosition = customCardObject8.CompanyTextPosition;
        }
        if (!isGhost)
        {
          bool monsterImageSizeLimit = customCardObject8.RemoveMonsterImageSizeLimit;
          if (flag)
          {
            if (monsterImageSizeLimit)
              cardUi1.m_MonsterMask.enabled = false;
            else if (!monsterImageSizeLimit)
              cardUi1.m_MonsterMask.enabled = true;
          }
          else if (!flag)
          {
            cardUi1.m_MonsterMask.enabled = true;
            Image imageComponentByName = ExtrasHandler.GetImageComponentByName(cardUi1.m_MonsterMask.gameObject, "Image");
            if (monsterImageSizeLimit)
            {
              if ((UnityEngine.Object) imageComponentByName != (UnityEngine.Object) null)
              {
                imageComponentByName.maskable = false;
                cardUi1.m_CardFoilMaskImage.sprite = cardUi1.m_CardBackImage.sprite;
                cardUi1.m_MonsterMaskImage.enabled = false;
              }
            }
            else if (!monsterImageSizeLimit && (UnityEngine.Object) imageComponentByName != (UnityEngine.Object) null)
            {
              imageComponentByName.maskable = true;
              cardUi1.m_MonsterMaskImage.enabled = true;
            }
          }
        }
        if ((UnityEngine.Object) cardUi1.m_MonsterImage != (UnityEngine.Object) null)
        {
          cardUi1.m_MonsterImage.rectTransform.sizeDelta = customCardObject8.MonsterImageSize;
          cardUi1.m_MonsterImage.rectTransform.anchoredPosition = customCardObject8.MonsterImagePosition;
        }
        if (cardData.isFoil)
        {
          CardUI cardUi2 = cardUi1;
          if ((UnityEngine.Object) cardUi2.transform.Find("FoilText") == (UnityEngine.Object) null)
          {
            TextMeshProUGUI textMeshProUgui = new GameObject("FoilText").AddComponent<TextMeshProUGUI>();
            textMeshProUgui.text = customCardObject8.FoilText;
            textMeshProUgui.transform.SetParent(cardUi2.transform, false);
            textMeshProUgui.enabled = false;
            if (!((UnityEngine.Object) textMeshProUgui != (UnityEngine.Object) null) || !((UnityEngine.Object) textMeshProUgui.transform.parent == (UnityEngine.Object) cardUi2.transform))
              ;
          }
          Transform transform = cardUi2.transform.Find("FoilText");
          if ((UnityEngine.Object) transform != (UnityEngine.Object) null)
            transform.GetComponent<TextMeshProUGUI>().text = customCardObject8.FoilText;
        }
      }
      else if (isGhost)
      {
        cardUi1.m_MonsterImage.rectTransform.sizeDelta = PlayerPatches.defaultGhostMonsterImageSize;
        cardUi1.m_MonsterImage.rectTransform.anchoredPosition = PlayerPatches.defaultGhostMonsterImagePosition;
      }
      else if (flag)
      {
        if (cardData.expansionType == ECardExpansionType.Tetramon || cardData.expansionType == ECardExpansionType.Destiny || cardData.expansionType == ECardExpansionType.Megabot)
        {
          cardUi1.m_MonsterImage.rectTransform.sizeDelta = PlayerPatches.defaultTetramonMonsterFullArtImageSize;
          cardUi1.m_MonsterImage.rectTransform.anchoredPosition = PlayerPatches.defaultTetramonMonsterFullArtImagePosition;
        }
        else if (cardData.expansionType == ECardExpansionType.CatJob || cardData.expansionType == ECardExpansionType.FantasyRPG)
        {
          cardUi1.m_MonsterImage.rectTransform.sizeDelta = PlayerPatches.defaultTetramonMonsterFullArtImageSize;
          cardUi1.m_MonsterImage.rectTransform.anchoredPosition = PlayerPatches.defaultOtherMonsterFullArtImagePosition;
        }
      }
      else
      {
        cardUi1.m_MonsterImage.rectTransform.sizeDelta = PlayerPatches.defaultTetramonMonsterImageSize;
        if (cardData.expansionType == ECardExpansionType.Tetramon || cardData.expansionType == ECardExpansionType.Destiny || cardData.expansionType == ECardExpansionType.Megabot)
          cardUi1.m_MonsterImage.rectTransform.anchoredPosition = PlayerPatches.defaultTetramonMonsterImagePosition;
        else if (cardData.expansionType == ECardExpansionType.CatJob || cardData.expansionType == ECardExpansionType.FantasyRPG)
          cardUi1.m_MonsterImage.rectTransform.anchoredPosition = PlayerPatches.defaultOtherMonsterImagePosition;
      }
    }

    public static void Log(string log) => MinaCardsModPlugin.Log.LogInfo((object) log);

    public static void LogError(string log) => MinaCardsModPlugin.Log.LogError((object) log);
  }
}