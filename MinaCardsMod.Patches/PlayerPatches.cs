using HarmonyLib;
using I2.Loc;
using System;
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
    public static string newCatJobPackName = "CatJob";
    public static string newFantasyRPGPackName = "FantasyRPG";
    public static string newMegaBotPackName = "Megabot";

    [HarmonyPostfix]
    [HarmonyPatch(typeof(InventoryBase), "Awake")]
    public static void Awake()
    {
      ImageSwapHandler.CloneOriginalSpriteLists();
      ImageSwapHandler.CloneOriginalCardBackTexture();
      ImageSwapHandler.GetOriginalMonsterSprites();
      ImageSwapHandler.SetBaseMonsterIcons();
      ImageSwapHandler.SetCatJobMonsterIcons();
      ImageSwapHandler.SetFantasyRPGMonsterIcons();
      ImageSwapHandler.SetMegabotMonsterIcons();
      ExtrasHandler.swapPackNames();
      ExtrasHandler.SwapNewPackItemImages();
      ExtrasHandler.AddHiddenCards();
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(CGameManager), "OnLevelFinishedLoading")]
    public static void CGameManager_OnLevelFinishedLoading_Postfix(
      ref Scene scene,
      LoadSceneMode mode)
    {
      if (MinaCardsModPlugin.isConfigGeneratorBuild && CSingleton<CGameManager>.Instance.m_IsGameLevel)
        ExtrasHandler.AddHiddenCards();
      if (!(scene.name == "Title") || CacheHandler.firstLoad || MinaCardsModPlugin.isConfigGeneratorBuild)
        return;
      CacheHandler.CacheAllFiles();
      CacheHandler.firstLoad = true;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Card3dUIGroup), "Start")]
    public static void Card3dUIGroup_Start_Postfix(Card3dUIGroup __instance)
    {
      ExtrasHandler.SetCardBacks(__instance);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(InteractionPlayerController), "AddHoldCard")]
    public static void InteractionPlayerController_AddHoldCard_Postfix(
      InteractionPlayerController __instance,
      InteractableCard3d card3d)
    {
      if (!(bool)(UnityEngine.Object)card3d)
        return;
      ExtrasHandler.SetCardBacks(card3d.m_Card3dUI);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(CollectionBinderUI), "OnPressSwitchExpansion")]
    public static bool CollectionBinderUI_OnPressSwitchExpansion_Prefix(
      ref CollectionBinderUI __instance,
      ref int expansionIndex)
    {
      if (MinaCardsModPlugin.SwapExpansions.Value)
        expansionIndex += 3;
      return true;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(CollectionBinderUI), "OpenSortAlbumScreen")]
    public static void CollectionBinderUI_OpenSortAlbumScreen_HarmonyPostfix(
      ref CollectionBinderUI __instance,
      int sortingMethodIndex,
      ref int currentExpansionIndex)
    {
      Color color1 = new Color(0.118f, 0.309f, 0.537f, 1f);
      Color color2 = new Color(0.09f, 0.664f, 1f, 1f);
      Color color3 = new Color(0.353f, 0.909f, 1f, 1f);
      Color color4 = new Color(0.4f, 0.125f, 0.05f, 1f);
      Color color5 = new Color(0.5f, 0.125f, 0.7f, 1f);
      Color color6 = new Color(0.6f, 0.2f, 0.75f, 1f);
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
              componentsInChild.color = color4;
            else if (componentsInChild.name == "BGMidtone")
              componentsInChild.color = color5;
            else if (componentsInChild.name == "BGHighlight")
              componentsInChild.color = color6;
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
              componentsInChild.color = color1;
            else if (componentsInChild.name == "BGMidtone")
              componentsInChild.color = color2;
            else if (componentsInChild.name == "BGHighlight")
              componentsInChild.color = color3;
          }
        }
      }
      __instance.m_ExpansionBtnList[0].GetComponentInChildren<TMP_Text>();
      __instance.m_ExpansionBtnList[0].GetComponentInChildren<TMP_Text>();
      __instance.m_ExpansionBtnList[0].GetComponentInChildren<TMP_Text>();
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(CollectionBinderUI), "OpenSortAlbumScreen")]
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
    [HarmonyPatch(typeof(RestockItemPanelUI), "Init")]
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
    [HarmonyPatch(typeof(CardUI), "SetCardUI")]
    public static void CardUI_SetCardUI_Main_Postfix(CardUI __instance, CardData cardData)
    {
      if (MinaCardsModPlugin.isConfigGeneratorBuild)
      {
        ConfigGeneratorHelper.writeMonsterData(cardData, __instance);
        ConfigGeneratorHelper.WriteAllFullExpansionConfigs();
      }
      ExtrasHandler.SetCardExtrasImages(__instance, cardData);
      if (!ExtrasHandler.isCardConfigDriven(cardData) || MinaCardsModPlugin.isConfigGeneratorBuild)
        return;
      CustomCardObject cardConfig = (CustomCardObject)null;
      CustomCardObject fullExpansionConfig = (CustomCardObject)null;
      string fileName = (string)null;
      string expansionName = (string)null;
      bool flag = cardData.borderType == ECardBorderType.FullArt;
      bool isGhost = cardData.expansionType == ECardExpansionType.Ghost;
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

      if (cardConfig != null && fullExpansionConfig != null)
      {
        CustomCardObject customCardObject = ExtrasHandler.SelectConfig(cardConfig, fullExpansionConfig);
        CardUI cardUi1 = !isGhost || !((UnityEngine.Object)__instance.m_GhostCard != (UnityEngine.Object)null)
          ? __instance
          : ExtrasHandler.CurrentCardUI(isGhost, __instance, __instance.m_GhostCard);
        cardUi1.m_MonsterNameText.text = cardConfig.Name;
        if (!isGhost)
        {
          cardUi1.m_DescriptionText.text = cardConfig.Description;
          if (flag && (UnityEngine.Object)__instance.m_FullArtCard != (UnityEngine.Object)null &&
              (UnityEngine.Object)__instance.m_FullArtCard.m_DescriptionText != (UnityEngine.Object)null &&
              __instance.m_FullArtCard.m_DescriptionText.text != null)
            __instance.m_FullArtCard.m_DescriptionText.text = cardConfig.Description;

          var monsterDataField =
            typeof(CardUI).GetField("m_MonsterData", BindingFlags.NonPublic | BindingFlags.Instance);
          var monsterData = monsterDataField?.GetValue(cardUi1);
          if (monsterData != null)
          {
            FieldInfo previousEvolutionField = monsterData.GetType().GetField("PreviousEvolution",
              BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            EMonsterType result;
            if (Enum.TryParse<EMonsterType>(cardConfig.PreviousEvolution, out result))
              previousEvolutionField?.SetValue(monsterData, result);
          }

          cardUi1.m_ChampionText.text = customCardObject.ChampionText;
          cardUi1.m_DescriptionText.enabled = customCardObject.DescriptionEnabled;
          cardUi1.m_DescriptionText.fontSize = customCardObject.DescriptionFontSize;
          cardUi1.m_DescriptionText.fontSizeMin = customCardObject.DescriptionFontSizeMin;
          cardUi1.m_DescriptionText.fontSizeMax = customCardObject.DescriptionFontSizeMax;
          cardUi1.m_DescriptionText.color = customCardObject.DescriptionFontColorRGBA;
          cardUi1.m_DescriptionText.rectTransform.anchoredPosition = customCardObject.DescriptionPosition;
          cardUi1.m_EvoPreviousStageIcon.enabled = customCardObject.PreviousEvolutionIconEnabled;
          cardUi1.m_EvoPreviousStageNameText.enabled = customCardObject.PreviousEvolutionBoxEnabled;
          if (monsterData != null)
          {
            FieldInfo previousEvolutionField = monsterData.GetType().GetField("PreviousEvolution",
              BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            EMonsterType previousEvolution = (EMonsterType)previousEvolutionField?.GetValue(monsterData);
            if (previousEvolution == EMonsterType.None)
            {
              if ((UnityEngine.Object)ExtrasHandler.GetImageComponentByName(cardUi1.gameObject, "EvoBasicIcon") !=
                  (UnityEngine.Object)null)
                ExtrasHandler.GetImageComponentByName(cardUi1.gameObject, "EvoBasicIcon").enabled =
                  customCardObject.BasicEvolutionIconEnabled;
              if ((UnityEngine.Object)ExtrasHandler.GetTextComponentByName(cardUi1.gameObject, "EvoBasicText") !=
                  (UnityEngine.Object)null)
              {
                TextMeshProUGUI textComponentByName =
                  ExtrasHandler.GetTextComponentByName(cardUi1.gameObject, "EvoBasicText");
                textComponentByName.text = customCardObject.BasicEvolutionText;
                textComponentByName.enabled = customCardObject.BasicEvolutionTextEnabled;
                textComponentByName.fontSize = customCardObject.BasicEvolutionTextFontSize;
                textComponentByName.fontSizeMin = customCardObject.BasicEvolutionTextFontSizeMin;
                textComponentByName.fontSizeMax = customCardObject.BasicEvolutionTextFontSizeMax;
                textComponentByName.color = customCardObject.BasicEvolutionTextFontColorRGBA;
                textComponentByName.rectTransform.anchoredPosition = customCardObject.BasicEvolutionTextPosition;
              }
            }

            cardUi1.m_EvoPreviousStageNameText.fontSize = customCardObject.PreviousEvolutionNameFontSize;
            cardUi1.m_EvoPreviousStageNameText.fontSizeMin = customCardObject.PreviousEvolutionNameFontSizeMin;
            cardUi1.m_EvoPreviousStageNameText.fontSizeMax = customCardObject.PreviousEvolutionNameFontSizeMax;
            cardUi1.m_EvoPreviousStageNameText.color = customCardObject.PreviousEvolutionNameFontColorRGBA;
            cardUi1.m_EvoPreviousStageNameText.rectTransform.anchoredPosition =
              customCardObject.PreviousEvolutionNamePosition;
          }

          if (!flag)
            cardUi1.m_NumberText.text = cardConfig.Number;
          if ((UnityEngine.Object)ExtrasHandler.GetImageComponentByName(cardUi1.gameObject, "EvoBG") !=
              (UnityEngine.Object)null)
            ExtrasHandler.GetImageComponentByName(cardUi1.gameObject, "EvoBG").enabled =
              customCardObject.PreviousEvolutionBoxEnabled;
          if ((UnityEngine.Object)ExtrasHandler.GetImageComponentByName(cardUi1.gameObject, "EvoBorder") !=
              (UnityEngine.Object)null)
            ExtrasHandler.GetImageComponentByName(cardUi1.gameObject, "EvoBorder").enabled =
              customCardObject.PreviousEvolutionBoxEnabled;
          if ((UnityEngine.Object)ExtrasHandler.GetTextComponentByName(cardUi1.gameObject, "TitleText") !=
              (UnityEngine.Object)null)
          {
            TextMeshProUGUI textComponentByName = ExtrasHandler.GetTextComponentByName(cardUi1.gameObject, "TitleText");
            textComponentByName.text = cardConfig.PlayEffectText;
            textComponentByName.enabled = customCardObject.PlayEffectTextEnabled;
            textComponentByName.fontSize = customCardObject.PlayEffectTextFontSize;
            textComponentByName.fontSizeMin = customCardObject.PlayEffectTextFontSizeMin;
            textComponentByName.fontSizeMax = customCardObject.PlayEffectTextFontSizeMax;
            textComponentByName.color = customCardObject.PlayEffectTextFontColorRGBA;
            textComponentByName.rectTransform.anchoredPosition = customCardObject.PlayEffectTextPosition;
          }

          if ((UnityEngine.Object)ExtrasHandler.GetTextComponentByName(cardUi1.gameObject, "EvoText") !=
              (UnityEngine.Object)null)
            ExtrasHandler.GetTextComponentByName(cardUi1.gameObject, "EvoText").enabled =
              customCardObject.PreviousEvolutionNameEnabled;
          if ((UnityEngine.Object)ExtrasHandler.GetImageComponentByName(cardUi1.gameObject, "TitleBG") !=
              (UnityEngine.Object)null)
            ExtrasHandler.GetImageComponentByName(cardUi1.gameObject, "TitleBG").enabled =
              customCardObject.PlayEffectBoxEnabled;
          if (isGhost | flag && (UnityEngine.Object)cardUi1.m_FirstEditionText != (UnityEngine.Object)null &&
              cardUi1.m_FirstEditionText.text != null)
            cardUi1.m_FirstEditionText.text = customCardObject.EditionText;
          if ((UnityEngine.Object)cardUi1.m_FirstEditionText != (UnityEngine.Object)null && !isGhost && !flag &&
              cardUi1.m_FirstEditionText.text != null)
          {
            switch (cardData.borderType)
            {
              case ECardBorderType.Base:
                cardUi1.m_FirstEditionText.text = customCardObject.BasicEditionText;
                break;
              case ECardBorderType.FirstEdition:
                cardUi1.m_FirstEditionText.text = customCardObject.FirstEditionText;
                break;
              case ECardBorderType.Silver:
                cardUi1.m_FirstEditionText.text = customCardObject.SilverEditionText;
                break;
              case ECardBorderType.Gold:
                cardUi1.m_FirstEditionText.text = customCardObject.GoldEditionText;
                break;
              case ECardBorderType.EX:
                cardUi1.m_FirstEditionText.text = customCardObject.EXEditionText;
                break;
            }
          }

          cardUi1.m_MonsterNameText.enabled = customCardObject.NameEnabled;
          cardUi1.m_MonsterNameText.fontSize = customCardObject.NameFontSize;
          cardUi1.m_MonsterNameText.fontSizeMin = customCardObject.NameFontSizeMin;
          cardUi1.m_MonsterNameText.fontSizeMax = customCardObject.NameFontSizeMax;
          cardUi1.m_MonsterNameText.color = customCardObject.NameFontColorRGBA;
          cardUi1.m_MonsterNameText.rectTransform.anchoredPosition = customCardObject.NamePosition;
          if (!isGhost && !flag)
          {
            if ((UnityEngine.Object)cardUi1.m_FirstEditionText != (UnityEngine.Object)null &&
                cardUi1.m_FirstEditionText.text != null)
            {
              cardUi1.m_FirstEditionText.fontSize = customCardObject.EditionTextFontSize;
              cardUi1.m_FirstEditionText.fontSizeMin = customCardObject.EditionTextFontSizeMin;
              cardUi1.m_FirstEditionText.fontSizeMax = customCardObject.EditionTextFontSizeMax;
              cardUi1.m_FirstEditionText.color = customCardObject.EditionTextFontColorRGBA;
              cardUi1.m_FirstEditionText.rectTransform.anchoredPosition = customCardObject.EditionTextPosition;
            }

            cardUi1.m_RarityText.text = cardConfig.Rarity;
            cardUi1.m_NumberText.enabled = customCardObject.NumberEnabled;
            cardUi1.m_NumberText.fontSize = customCardObject.NumberFontSize;
            cardUi1.m_NumberText.fontSizeMin = customCardObject.NumberFontSizeMin;
            cardUi1.m_NumberText.fontSizeMax = customCardObject.NumberFontSizeMax;
            cardUi1.m_NumberText.color = customCardObject.NumberFontColorRGBA;
            cardUi1.m_NumberText.rectTransform.anchoredPosition = customCardObject.NumberPosition;
            cardUi1.m_FirstEditionText.enabled = customCardObject.EditionTextEnabled;
            cardUi1.m_RarityText.enabled = customCardObject.RarityEnabled;
            cardUi1.m_RarityText.fontSize = customCardObject.RarityFontSize;
            cardUi1.m_RarityText.fontSizeMin = customCardObject.RarityFontSizeMin;
            cardUi1.m_RarityText.fontSizeMax = customCardObject.RarityFontSizeMax;
            cardUi1.m_RarityText.color = customCardObject.RarityFontColorRGBA;
            cardUi1.m_RarityText.rectTransform.anchoredPosition = customCardObject.RarityPosition;
          }

          if ((UnityEngine.Object)ExtrasHandler.GetImageComponentByName(cardUi1.gameObject, "RarityImage") !=
              (UnityEngine.Object)null)
            ExtrasHandler.GetImageComponentByName(cardUi1.gameObject, "RarityImage").enabled =
              customCardObject.RarityImageEnabled;
          if (!isGhost)
          {
            cardUi1.m_ChampionText.enabled = customCardObject.ChampionTextEnabled;
            cardUi1.m_ChampionText.fontSize = customCardObject.ChampionFontSize;
            cardUi1.m_ChampionText.fontSizeMin = customCardObject.ChampionFontSizeMin;
            cardUi1.m_ChampionText.fontSizeMax = customCardObject.ChampionFontSizeMax;
            cardUi1.m_ChampionText.color = customCardObject.ChampionFontColorRGBA;
            cardUi1.m_ChampionText.rectTransform.anchoredPosition = customCardObject.ChampionPosition;
          }

          if (isGhost && (UnityEngine.Object)ExtrasHandler.GetImageComponentByName(cardUi1.gameObject, "CardStat") !=
              (UnityEngine.Object)null)
            ExtrasHandler.GetImageComponentByName(cardUi1.gameObject, "CardStat").enabled =
              cardConfig.StatBackgroundImageEnabled;
          cardUi1.m_Stat1Text.text = cardConfig.Stat1;
          cardUi1.m_Stat1Text.enabled = customCardObject.Stat1Enabled;
          cardUi1.m_Stat1Text.fontSize = customCardObject.Stat1FontSize;
          cardUi1.m_Stat1Text.fontSizeMin = customCardObject.Stat1FontSizeMin;
          cardUi1.m_Stat1Text.fontSizeMax = customCardObject.Stat1FontSizeMax;
          cardUi1.m_Stat1Text.color = customCardObject.Stat1FontColorRGBA;
          cardUi1.m_Stat1Text.rectTransform.anchoredPosition = customCardObject.Stat1Position;
          cardUi1.m_Stat2Text.text = cardConfig.Stat2;
          cardUi1.m_Stat2Text.enabled = customCardObject.Stat2Enabled;
          cardUi1.m_Stat2Text.fontSize = customCardObject.Stat2FontSize;
          cardUi1.m_Stat2Text.fontSizeMin = customCardObject.Stat2FontSizeMin;
          cardUi1.m_Stat2Text.fontSizeMax = customCardObject.Stat2FontSizeMax;
          cardUi1.m_Stat2Text.color = customCardObject.Stat2FontColorRGBA;
          cardUi1.m_Stat2Text.rectTransform.anchoredPosition = customCardObject.Stat2Position;
          cardUi1.m_Stat3Text.text = cardConfig.Stat3;
          cardUi1.m_Stat3Text.enabled = customCardObject.Stat3Enabled;
          cardUi1.m_Stat3Text.fontSize = customCardObject.Stat3FontSize;
          cardUi1.m_Stat3Text.fontSizeMin = customCardObject.Stat3FontSizeMin;
          cardUi1.m_Stat3Text.fontSizeMax = customCardObject.Stat3FontSizeMax;
          cardUi1.m_Stat3Text.color = customCardObject.Stat3FontColorRGBA;
          cardUi1.m_Stat3Text.rectTransform.anchoredPosition = customCardObject.Stat3Position;
          cardUi1.m_Stat4Text.text = cardConfig.Stat4;
          cardUi1.m_Stat4Text.enabled = customCardObject.Stat4Enabled;
          cardUi1.m_Stat4Text.fontSize = customCardObject.Stat4FontSize;
          cardUi1.m_Stat4Text.fontSizeMin = customCardObject.Stat4FontSizeMin;
          cardUi1.m_Stat4Text.fontSizeMax = customCardObject.Stat4FontSizeMax;
          cardUi1.m_Stat4Text.color = customCardObject.Stat4FontColorRGBA;
          cardUi1.m_Stat4Text.rectTransform.anchoredPosition = customCardObject.Stat4Position;
          if ((UnityEngine.Object)cardUi1.m_ArtistText != (UnityEngine.Object)null)
          {
            if (flag && (UnityEngine.Object)cardUi1.m_FullArtCard != (UnityEngine.Object)null &&
                (UnityEngine.Object)cardUi1.m_FullArtCard.m_ArtistText != (UnityEngine.Object)null &&
                cardUi1.m_FullArtCard.m_ArtistText.text != null)
            {
              cardUi1.m_FullArtCard.m_ArtistText.text = customCardObject.ArtistText;
              cardUi1.m_FullArtCard.m_ArtistText.enabled = customCardObject.ArtistTextEnabled;
              cardUi1.m_FullArtCard.m_ArtistText.fontSize = customCardObject.ArtistTextFontSize;
              cardUi1.m_FullArtCard.m_ArtistText.fontSizeMin = customCardObject.ArtistTextFontSizeMin;
              cardUi1.m_FullArtCard.m_ArtistText.fontSizeMax = customCardObject.ArtistTextFontSizeMax;
              cardUi1.m_FullArtCard.m_ArtistText.color = customCardObject.ArtistTextFontColorRGBA;
              cardUi1.m_FullArtCard.m_ArtistText.rectTransform.anchoredPosition = customCardObject.ArtistTextPosition;
            }

            cardUi1.m_ArtistText.text = customCardObject.ArtistText;
            cardUi1.m_ArtistText.enabled = customCardObject.ArtistTextEnabled;
            cardUi1.m_ArtistText.fontSize = customCardObject.ArtistTextFontSize;
            cardUi1.m_ArtistText.fontSizeMin = customCardObject.ArtistTextFontSizeMin;
            cardUi1.m_ArtistText.fontSizeMax = customCardObject.ArtistTextFontSizeMax;
            cardUi1.m_ArtistText.color = customCardObject.ArtistTextFontColorRGBA;
            cardUi1.m_ArtistText.rectTransform.anchoredPosition = customCardObject.ArtistTextPosition;
          }

          if ((UnityEngine.Object)ExtrasHandler.GetTextComponentByName(cardUi1.gameObject, "CompanyText") !=
              (UnityEngine.Object)null)
          {
            TextMeshProUGUI textComponentByName =
              ExtrasHandler.GetTextComponentByName(cardUi1.gameObject, "CompanyText");
            textComponentByName.text = customCardObject.CompanyText;
            textComponentByName.enabled = customCardObject.CompanyTextEnabled;
            textComponentByName.fontSize = customCardObject.CompanyTextFontSize;
            textComponentByName.fontSizeMin = customCardObject.CompanyTextFontSizeMin;
            textComponentByName.fontSizeMax = customCardObject.CompanyTextFontSizeMax;
            textComponentByName.color = customCardObject.CompanyTextFontColorRGBA;
            textComponentByName.rectTransform.anchoredPosition = customCardObject.CompanyTextPosition;
          }

          if (!isGhost)
          {
            bool monsterImageSizeLimit = customCardObject.RemoveMonsterImageSizeLimit;
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
              Image imageComponentByName =
                ExtrasHandler.GetImageComponentByName(cardUi1.m_MonsterMask.gameObject, "Image");
              if (monsterImageSizeLimit)
              {
                if ((UnityEngine.Object)imageComponentByName != (UnityEngine.Object)null)
                {
                  imageComponentByName.maskable = false;
                  cardUi1.m_CardFoilMaskImage.sprite = cardUi1.m_CardBackImage.sprite;
                  cardUi1.m_MonsterMaskImage.enabled = false;
                }
              }
              else if (!monsterImageSizeLimit && (UnityEngine.Object)imageComponentByName != (UnityEngine.Object)null)
              {
                imageComponentByName.maskable = true;
                cardUi1.m_MonsterMaskImage.enabled = true;
              }
            }
          }

          if ((UnityEngine.Object)cardUi1.m_MonsterImage != (UnityEngine.Object)null)
          {
            cardUi1.m_MonsterImage.rectTransform.sizeDelta = customCardObject.MonsterImageSize;
            cardUi1.m_MonsterImage.rectTransform.anchoredPosition = customCardObject.MonsterImagePosition;
          }

          if (cardData.isFoil)
          {
            CardUI cardUi2 = cardUi1;
            if ((UnityEngine.Object)cardUi2.transform.Find("FoilText") == (UnityEngine.Object)null)
            {
              TextMeshProUGUI textMeshProUgui = new GameObject("FoilText").AddComponent<TextMeshProUGUI>();
              textMeshProUgui.text = customCardObject.FoilText;
              textMeshProUgui.transform.SetParent(cardUi2.transform, false);
              textMeshProUgui.enabled = false;
              if (!((UnityEngine.Object)textMeshProUgui != (UnityEngine.Object)null) ||
                  !((UnityEngine.Object)textMeshProUgui.transform.parent == (UnityEngine.Object)cardUi2.transform))
                ;
            }

            Transform transform = cardUi2.transform.Find("FoilText");
            if ((UnityEngine.Object)transform != (UnityEngine.Object)null)
              transform.GetComponent<TextMeshProUGUI>().text = customCardObject.FoilText;
          }
        }
      }
    }
    public static void Log(string log) => MinaCardsModPlugin.Log.LogInfo((object)log);

    public static void LogError(string log) => MinaCardsModPlugin.Log.LogError((object)log);
  }
}