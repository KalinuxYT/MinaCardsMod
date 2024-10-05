using HarmonyLib;
using I2.Loc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;

// Abandon all hope, ye who read this code. 345 warnings, but 0 errors WICKED.

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
    public static string basePackImages = PlayerPatches.customExpansionPackImages + "TetramonPackImages/";
    public static string ghostPackImages = PlayerPatches.customExpansionPackImages + "GhostPackImages/";
    public static string configPath = PlayerPatches.customExpansionPackImages + "/Configs/Custom/";

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
      Color color1 = new Color(0.118f, 0.309f, 0.537f, 1f);
      Color color2 = new Color(0.09f, 0.664f, 1f, 1f);
      Color color3 = new Color(0.353f, 0.909f, 1f, 1f);
      Color color4 = new Color(0.4f, 0.125f, 0.05f, 1f);
      Color color5 = new Color(0.5f, 0.125f, 0.7f, 1f);
      Color color6 = new Color(0.6f, 0.2f, 0.75f, 1f);
      if (MinaCardsModPlugin.SwapExpansions.Value)
      {
        ((Component) __instance.m_ExpansionBtnList[0]).GetComponentInChildren<TMP_Text>().text = "Megabot";
        ((Component) __instance.m_ExpansionBtnList[1]).GetComponentInChildren<TMP_Text>().text = "Fantasy";
        ((Component) __instance.m_ExpansionBtnList[2]).GetComponentInChildren<TMP_Text>().text = "Cat Job";
        foreach (Component expansionBtn in __instance.m_ExpansionBtnList)
        {
          foreach (Image componentsInChild in expansionBtn.GetComponentsInChildren<Image>())
          {
            if (componentsInChild.name == "BGBorder")
              ((Graphic) componentsInChild).color = color4;
            else if (componentsInChild.name == "BGMidtone")
              ((Graphic) componentsInChild).color = color5;
            else if (componentsInChild.name == "BGHighlight")
              ((Graphic) componentsInChild).color = color6;
          }
        }
      }
      else
      {
        ((Component) __instance.m_ExpansionBtnList[0]).GetComponentInChildren<TMP_Text>().text = "Tetramon";
        ((Component) __instance.m_ExpansionBtnList[1]).GetComponentInChildren<TMP_Text>().text = "Destiny";
        ((Component) __instance.m_ExpansionBtnList[2]).GetComponentInChildren<TMP_Text>().text = "Ghost";
        foreach (Component expansionBtn in __instance.m_ExpansionBtnList)
        {
          foreach (Image componentsInChild in expansionBtn.GetComponentsInChildren<Image>())
          {
            if (((UnityEngine.Object) componentsInChild).name == "BGBorder")
              ((Graphic) componentsInChild).color = color1;
            else if (((UnityEngine.Object) componentsInChild).name == "BGMidtone")
              ((Graphic) componentsInChild).color = color2;
            else if (((UnityEngine.Object) componentsInChild).name == "BGHighlight")
              ((Graphic) componentsInChild).color = color3;
          }
        }
      }
      ((Component) __instance.m_ExpansionBtnList[0]).GetComponentInChildren<TMP_Text>();
      ((Component) __instance.m_ExpansionBtnList[0]).GetComponentInChildren<TMP_Text>();
      ((Component) __instance.m_ExpansionBtnList[0]).GetComponentInChildren<TMP_Text>();
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

    public static Texture2D LoadCustomPNG(string fileName, string imagePath)
    {
      string path = imagePath + fileName + ".png";
      if (!File.Exists(path))
        return (Texture2D) null;
      byte[] numArray = File.ReadAllBytes(path);
      Texture2D texture2D = new Texture2D(2, 2, (DefaultFormat) 0, (TextureCreationFlags) 0);
      ImageConversion.LoadImage(texture2D, numArray);
      return texture2D;
    }

    public static Sprite GetCustomImage(string fileName, string imagePath)
    {
      Sprite customImage1 = null;
      
      var loadedSpriteDictField = AccessTools.Field(typeof(LoadStreamTexture), "m_LoadedSpriteDict");
      var loadedSpriteDict = (Dictionary<string, Sprite>)loadedSpriteDictField.GetValue(CSingleton<LoadStreamTexture>.Instance);
      
      if (loadedSpriteDict.ContainsKey(fileName))
      {
        loadedSpriteDict.TryGetValue(fileName, out customImage1);
        if (customImage1 != null)
        {
          if (CSingleton<LoadStreamTexture>.Instance.m_Image == null)
          {
            CSingleton<LoadStreamTexture>.Instance.m_Image = new GameObject("CustomImage").AddComponent<Image>();
            MinaCardsModPlugin.Log.LogInfo("Image NULL making new one");
          }

          CSingleton<LoadStreamTexture>.Instance.m_Image.sprite = customImage1;
          return customImage1;
        }
      }
      
      Texture2D texture2D = PlayerPatches.LoadCustomPNG(fileName, imagePath);
      if (texture2D != null)
      {
        Sprite customImage2 = Sprite.Create(texture2D, new Rect(0.0f, 0.0f, texture2D.width, texture2D.height), Vector2.zero);
        customImage2.name = fileName;

        if (!loadedSpriteDict.ContainsKey(fileName))
        {
          if (CSingleton<LoadStreamTexture>.Instance.m_LoadedSpriteList == null)
          {
            MinaCardsModPlugin.Log.LogInfo("m_LoadedSpriteList is NULL making new one");
            CSingleton<LoadStreamTexture>.Instance.m_LoadedSpriteList = new List<Sprite>();
          }
          CSingleton<LoadStreamTexture>.Instance.m_LoadedSpriteList.Add(customImage2);
          loadedSpriteDict.TryAdd(fileName, customImage2);
        }
        else
        {
          loadedSpriteDict[fileName] = customImage2;
        }

        return customImage2;
      }

      if (!CSingleton<LoadStreamTexture>.Instance.m_CurrentLoadingFileNameList.Contains(fileName))
      {
        CSingleton<LoadStreamTexture>.Instance.m_CurrentLoadingFileNameList.Add(fileName);
        var loadTextureFromWebMethod = AccessTools.Method(typeof(LoadStreamTexture), "LoadTextureFromWeb");
        if (loadTextureFromWebMethod != null)
        {
          ((MonoBehaviour)CSingleton<LoadStreamTexture>.Instance).StartCoroutine(
            (IEnumerator)loadTextureFromWebMethod.Invoke(CSingleton<LoadStreamTexture>.Instance,
              new object[] { fileName })
          );
        }
        else
        {
          MinaCardsModPlugin.Log.LogError("LoadTextureFromWeb method not found.");
        }
      }
      return null;
    }

    public static string findConfigFilePath(CardData cardData)
    {
      MinaCardsModPlugin.Log.LogInfo("Called findConfigFilePath with MonsterType: " + cardData.monsterType + " and ExpansionType: " + cardData.expansionType);

      string str = PlayerPatches.configPath + cardData.monsterType.ToString();

      MinaCardsModPlugin.Log.LogInfo("Base Config Path: " + str);
      
      int expansionTypeValue = (int)cardData.expansionType;
      
      MinaCardsModPlugin.Log.LogInfo("Raw Expansion Type: " + cardData.expansionType.ToString());
      MinaCardsModPlugin.Log.LogInfo("Expansion Type Value: " + expansionTypeValue);
      
      switch (expansionTypeValue + 1)
      {
        case 0:
          MinaCardsModPlugin.Log.LogInfo("No Expansion Config Path returned");
          return PlayerPatches.configPath + "NoExpansionConfigs/" + cardData.monsterType.ToString();
        case 1:
          MinaCardsModPlugin.Log.LogInfo("Tetramon Config Path returned");
          return PlayerPatches.configPath + "TetramonConfigs/" + cardData.monsterType.ToString();
        case 2:
          MinaCardsModPlugin.Log.LogInfo("Destiny Config Path returned");
          return PlayerPatches.configPath + "DestinyConfigs/" + cardData.monsterType.ToString();
        case 3:
          MinaCardsModPlugin.Log.LogInfo("Ghost Config Path returned");
          return PlayerPatches.configPath + "GhostConfigs/" + cardData.monsterType.ToString();
        case 4:
          MinaCardsModPlugin.Log.LogInfo("Megabot Config Path returned");
          return PlayerPatches.configPath + "MegabotConfigs/" + cardData.monsterType.ToString();
        case 5:
          MinaCardsModPlugin.Log.LogInfo("FantasyRPG Config Path returned");
          return PlayerPatches.configPath + "FantasyRPGConfigs/" + cardData.monsterType.ToString();
        case 6:
          MinaCardsModPlugin.Log.LogInfo("CatJob Config Path returned");
          return PlayerPatches.configPath + "CatJobConfigs/" + cardData.monsterType.ToString();
        case 7:
          MinaCardsModPlugin.Log.LogInfo("Foodie Config Path returned");
          return PlayerPatches.configPath + "FoodieConfigs/" + cardData.monsterType.ToString();
        case 8:
          MinaCardsModPlugin.Log.LogInfo("Max Config Path returned");
          return PlayerPatches.configPath + "MaxConfigs/" + cardData.monsterType.ToString();
        default:
          MinaCardsModPlugin.Log.LogError("Invalid expansion type: " + expansionTypeValue);
          return (string) null;
      }
    }

    public static void writeMonsterData(CardData cardData, CardUI cardUI)
    {
      string configFilePath = PlayerPatches.findConfigFilePath(cardData);
      if (!File.Exists(configFilePath + ".ini") && (int)cardData.borderType != 5)
      {
        MinaCardsModPlugin.Log.LogInfo("Creating config for " + cardData.monsterType.ToString());
        PlayerPatches.WriteCardDataToFile(configFilePath + ".ini", cardData, cardUI);
      }
      if (File.Exists(configFilePath + "FullArt.ini") || (int)cardData.borderType != 5)
        return;
      MinaCardsModPlugin.Log.LogInfo("Creating Full Art config for " + cardData.monsterType.ToString());
      PlayerPatches.WriteCardDataToFile(configFilePath + "FullArt.ini", cardData, cardUI);
    }
    
    public static void WriteCardDataToFile(string filePath, CardData cardData, CardUI cardUI)
    {
      bool isGhost = (int)cardData.expansionType == 2;
      bool flag1 = (int)cardData.borderType == 5;
      using (StreamWriter streamWriter1 = new StreamWriter(filePath))
      {
        if (flag1)
          streamWriter1.WriteLine("[" + cardData.monsterType.ToString() + "FullArt]");
        else
          streamWriter1.WriteLine("[" + cardData.monsterType.ToString() + "]");
        CardUI cardUi = !isGhost || cardUI.m_GhostCard != null 
          ? cardUI : PlayerPatches.CurrentCardUI(isGhost, cardUI, cardUI.m_GhostCard);
        float num;
        Color color;
        Vector2 vector2;
        if (cardUi.m_MonsterNameText != null)
        {
          streamWriter1.WriteLine("Name = " + ((TMP_Text) cardUi.m_MonsterNameText).text);
          streamWriter1.WriteLine("Name Enabled = " + ((Behaviour) cardUi.m_MonsterNameText).enabled.ToString());
          streamWriter1.WriteLine("Name Font Size = " + ((TMP_Text) cardUi.m_MonsterNameText).fontSize.ToString());
          StreamWriter streamWriter2 = streamWriter1;
          num = ((TMP_Text) cardUi.m_MonsterNameText).fontSizeMin;
          string str1 = "Name Font Size Min = " + num.ToString();
          streamWriter2.WriteLine(str1);
          StreamWriter streamWriter3 = streamWriter1;
          num = ((TMP_Text) cardUi.m_MonsterNameText).fontSizeMax;
          string str2 = "Name Font Size Max = " + num.ToString();
          streamWriter3.WriteLine(str2);
          StreamWriter streamWriter4 = streamWriter1;
          color = ((Graphic) cardUi.m_MonsterNameText).color;
          string str3 = "Name Font Color RGBA = " + color.ToString();
          streamWriter4.WriteLine(str3);
          StreamWriter streamWriter5 = streamWriter1;
          vector2 = ((TMP_Text) cardUi.m_MonsterNameText).rectTransform.anchoredPosition;
          string str4 = "Name Position = " + vector2.ToString();
          streamWriter5.WriteLine(str4);
          streamWriter1.WriteLine("");
        }
        if (!isGhost && cardUi.m_DescriptionText != null)
        {
          streamWriter1.WriteLine("Description = " + ((TMP_Text) cardUi.m_DescriptionText).text);
          streamWriter1.WriteLine("Description Enabled = " + ((Behaviour) cardUi.m_DescriptionText).enabled.ToString());
          StreamWriter streamWriter6 = streamWriter1;
          num = ((TMP_Text) cardUi.m_DescriptionText).fontSize;
          string str5 = "Description Font Size = " + num.ToString();
          streamWriter6.WriteLine(str5);
          StreamWriter streamWriter7 = streamWriter1;
          num = ((TMP_Text) cardUi.m_DescriptionText).fontSizeMin;
          string str6 = "Description Font Size Min = " + num.ToString();
          streamWriter7.WriteLine(str6);
          StreamWriter streamWriter8 = streamWriter1;
          num = ((TMP_Text) cardUi.m_DescriptionText).fontSizeMax;
          string str7 = "Description Font Size Max = " + num.ToString();
          streamWriter8.WriteLine(str7);
          StreamWriter streamWriter9 = streamWriter1;
          color = ((Graphic) cardUi.m_DescriptionText).color;
          string str8 = "Description Font Color RGBA = " + color.ToString();
          streamWriter9.WriteLine(str8);
          StreamWriter streamWriter10 = streamWriter1;
          vector2 = ((TMP_Text) cardUi.m_DescriptionText).rectTransform.anchoredPosition;
          string str9 = "Description Position = " + vector2.ToString();
          streamWriter10.WriteLine(str9);
          streamWriter1.WriteLine("");
        }
        StreamWriter streamWriter11 = streamWriter1;
        bool flag2 = false;
        string str10 = "Individual Overrides = " + flag2.ToString();
        streamWriter11.WriteLine(str10);
        streamWriter1.WriteLine("");
        if (!isGhost && !flag1 && cardUi.m_NumberText != null)
        {
          streamWriter1.WriteLine("Number = " + ((TMP_Text) cardUi.m_NumberText).text);
          StreamWriter streamWriter12 = streamWriter1;
          flag2 = ((Behaviour) cardUi.m_NumberText).enabled;
          string str11 = "Number Enabled = " + flag2.ToString();
          streamWriter12.WriteLine(str11);
          StreamWriter streamWriter13 = streamWriter1;
          num = ((TMP_Text) cardUi.m_NumberText).fontSize;
          string str12 = "Number Font Size = " + num.ToString();
          streamWriter13.WriteLine(str12);
          StreamWriter streamWriter14 = streamWriter1;
          num = ((TMP_Text) cardUi.m_NumberText).fontSizeMin;
          string str13 = "Number Font Size Min = " + num.ToString();
          streamWriter14.WriteLine(str13);
          StreamWriter streamWriter15 = streamWriter1;
          num = ((TMP_Text) cardUi.m_NumberText).fontSizeMax;
          string str14 = "Number Font Size Max = " + num.ToString();
          streamWriter15.WriteLine(str14);
          StreamWriter streamWriter16 = streamWriter1;
          color = ((Graphic) cardUi.m_NumberText).color;
          string str15 = "Number Font Color RGBA = " + color.ToString();
          streamWriter16.WriteLine(str15);
          StreamWriter streamWriter17 = streamWriter1;
          vector2 = ((TMP_Text) cardUi.m_NumberText).rectTransform.anchoredPosition;
          string str16 = "Number Position = " + vector2.ToString();
          streamWriter17.WriteLine(str16);
          streamWriter1.WriteLine("");
        }
        if (!isGhost)
        {
          var monsterDataField = AccessTools.Field(typeof(CardUI), "m_MonsterData");
          var monsterData = (MonsterData)monsterDataField.GetValue(cardUi);

          EMonsterType previousEvolution = monsterData.PreviousEvolution;
          if (true)
          {
            streamWriter1.WriteLine("Previous Evolution = " + previousEvolution.ToString());
            StreamWriter streamWriter18 = streamWriter1;
            flag2 = ((Behaviour) cardUi.m_EvoPreviousStageIcon).enabled;
            string str17 = "Previous Evolution Icon Enabled = " + flag2.ToString();
            streamWriter18.WriteLine(str17);
            StreamWriter streamWriter19 = streamWriter1;
            flag2 = ((Behaviour) cardUi.m_EvoPreviousStageNameText).enabled;
            string str18 = "Previous Evolution Name Enabled = " + flag2.ToString();
            streamWriter19.WriteLine(str18);
            StreamWriter streamWriter20 = streamWriter1;
            num = ((TMP_Text) cardUi.m_EvoPreviousStageNameText).fontSize;
            string str19 = "Previous Evolution Name Font Size = " + num.ToString();
            streamWriter20.WriteLine(str19);
            StreamWriter streamWriter21 = streamWriter1;
            num = ((TMP_Text) cardUi.m_EvoPreviousStageNameText).fontSizeMin;
            string str20 = "Previous Evolution Name Font Size Min = " + num.ToString();
            streamWriter21.WriteLine(str20);
            StreamWriter streamWriter22 = streamWriter1;
            num = ((TMP_Text) cardUi.m_EvoPreviousStageNameText).fontSizeMax;
            string str21 = "Previous Evolution Name Font Size Max = " + num.ToString();
            streamWriter22.WriteLine(str21);
            StreamWriter streamWriter23 = streamWriter1;
            color = ((Graphic) cardUi.m_EvoPreviousStageNameText).color;
            string str22 = "Previous Evolution Name Font Color RGBA = " + color.ToString();
            streamWriter23.WriteLine(str22);
            StreamWriter streamWriter24 = streamWriter1;
            vector2 = ((TMP_Text) cardUi.m_EvoPreviousStageNameText).rectTransform.anchoredPosition;
            string str23 = "Previous Evolution Name Position = " + vector2.ToString();
            streamWriter24.WriteLine(str23);
          }
          if (PlayerPatches.GetImageComponentByName(((Component) cardUi).gameObject, "EvoBG") != null)
          {
            StreamWriter streamWriter25 = streamWriter1;
            flag2 = ((Behaviour) PlayerPatches.GetImageComponentByName(((Component) cardUi).gameObject, "EvoBG")).enabled;
            string str24 = "Previous Evolution Box Enabled = " + flag2.ToString();
            streamWriter25.WriteLine(str24);
            streamWriter1.WriteLine("");
          }
        }
        if (!isGhost && PlayerPatches.GetTextComponentByName(((Component) cardUi).gameObject, "TitleText") != null)
        {
          TextMeshProUGUI textComponentByName = PlayerPatches.GetTextComponentByName(((Component) cardUi).gameObject, "TitleText");
          streamWriter1.WriteLine("Play Effect Text = " + ((TMP_Text) textComponentByName).text);
          StreamWriter streamWriter26 = streamWriter1;
          flag2 = ((Behaviour) textComponentByName).enabled;
          string str25 = "Play Effect Text Enabled = " + flag2.ToString();
          streamWriter26.WriteLine(str25);
          StreamWriter streamWriter27 = streamWriter1;
          num = ((TMP_Text) textComponentByName).fontSize;
          string str26 = "Play Effect Text Font Size = " + num.ToString();
          streamWriter27.WriteLine(str26);
          StreamWriter streamWriter28 = streamWriter1;
          num = ((TMP_Text) textComponentByName).fontSizeMin;
          string str27 = "Play Effect Text Font Size Min = " + num.ToString();
          streamWriter28.WriteLine(str27);
          StreamWriter streamWriter29 = streamWriter1;
          num = ((TMP_Text) textComponentByName).fontSizeMax;
          string str28 = "Play Effect Text Font Size Max = " + num.ToString();
          streamWriter29.WriteLine(str28);
          StreamWriter streamWriter30 = streamWriter1;
          color = ((Graphic) textComponentByName).color;
          string str29 = "Play Effect Text Font Color RGBA = " + color.ToString();
          streamWriter30.WriteLine(str29);
          StreamWriter streamWriter31 = streamWriter1;
          vector2 = ((TMP_Text) textComponentByName).rectTransform.anchoredPosition;
          string str30 = "Play Effect Text Position = " + vector2.ToString();
          streamWriter31.WriteLine(str30);
          if (PlayerPatches.GetImageComponentByName(((Component) cardUi).gameObject, "TitleBG") != null)
          {
            StreamWriter streamWriter32 = streamWriter1;
            flag2 = ((Behaviour) PlayerPatches.GetImageComponentByName(((Component) cardUi).gameObject, "TitleBG")).enabled;
            string str31 = "Play Effect Box Enabled = " + flag2.ToString();
            streamWriter32.WriteLine(str31);
          }
          streamWriter1.WriteLine("");
        }
        if (!isGhost && !flag1)
        {
          if (cardUi.m_RarityText != null)
          {
            streamWriter1.WriteLine("Rarity = " + ((TMP_Text) cardUi.m_RarityText).text);
            StreamWriter streamWriter33 = streamWriter1;
            flag2 = ((Behaviour) cardUi.m_RarityText).enabled;
            string str32 = "Rarity Enabled = " + flag2.ToString();
            streamWriter33.WriteLine(str32);
            StreamWriter streamWriter34 = streamWriter1;
            num = ((TMP_Text) cardUi.m_RarityText).fontSize;
            string str33 = "Rarity Font Size = " + num.ToString();
            streamWriter34.WriteLine(str33);
            StreamWriter streamWriter35 = streamWriter1;
            num = ((TMP_Text) cardUi.m_RarityText).fontSizeMin;
            string str34 = "Rarity Font Size Min = " + num.ToString();
            streamWriter35.WriteLine(str34);
            StreamWriter streamWriter36 = streamWriter1;
            num = ((TMP_Text) cardUi.m_RarityText).fontSizeMax;
            string str35 = "Rarity Font Size Max = " + num.ToString();
            streamWriter36.WriteLine(str35);
            StreamWriter streamWriter37 = streamWriter1;
            color = ((Graphic) cardUi.m_RarityText).color;
            string str36 = "Rarity Font Color RGBA = " + color.ToString();
            streamWriter37.WriteLine(str36);
            StreamWriter streamWriter38 = streamWriter1;
            vector2 = ((TMP_Text) cardUi.m_RarityText).rectTransform.anchoredPosition;
            string str37 = "Rarity Position = " + vector2.ToString();
            streamWriter38.WriteLine(str37);
            streamWriter1.WriteLine("");
          }
          streamWriter1.WriteLine("First Edition Text = " + LocalizationManager.GetTranslation("1st Edition", true, 0, true, false, (GameObject) null, (string) null, true));
          streamWriter1.WriteLine("Gold Edition Text = " + LocalizationManager.GetTranslation("Gold Edition", true, 0, true, false, (GameObject) null, (string) null, true));
          streamWriter1.WriteLine("Silver Edition Text = " + LocalizationManager.GetTranslation("Silver Edition", true, 0, true, false, (GameObject) null, (string) null, true));
          streamWriter1.WriteLine("EX Edition Text = EX");
          StreamWriter streamWriter39 = streamWriter1;
          flag2 = true;
          string str38 = "Edition Text Enabled = " + flag2.ToString();
          streamWriter39.WriteLine(str38);
          StreamWriter streamWriter40 = streamWriter1;
          num = 21.1f;
          string str39 = "Edition Text Font Size = " + num.ToString();
          streamWriter40.WriteLine(str39);
          StreamWriter streamWriter41 = streamWriter1;
          num = 18f;
          string str40 = "Edition Text Font Size Min = " + num.ToString();
          streamWriter41.WriteLine(str40);
          StreamWriter streamWriter42 = streamWriter1;
          num = 52f;
          string str41 = "Edition Text Font Size Max = " + num.ToString();
          streamWriter42.WriteLine(str41);
          StreamWriter streamWriter43 = streamWriter1;
          color = new Color(1f, 1f, 1f, 1f);
          string str42 = "Edition Text Font Color RGBA = " + color.ToString();
          streamWriter43.WriteLine(str42);
          if ((int)cardData.expansionType == 4)
          {
            StreamWriter streamWriter44 = streamWriter1;
            vector2 = new Vector2(0.0f, -7.5f);
            string str43 = "Edition Text Position = " + vector2.ToString();
            streamWriter44.WriteLine(str43);
            streamWriter1.WriteLine("");
          }
          else if ((int)cardData.expansionType == 5)
          {
            StreamWriter streamWriter45 = streamWriter1;
            vector2 = new Vector2(0.0f, 30f);
            string str44 = "Edition Text Position = " + vector2.ToString();
            streamWriter45.WriteLine(str44);
            streamWriter1.WriteLine("");
          }
          else if ((int)cardData.expansionType == 3)
          {
            StreamWriter streamWriter46 = streamWriter1;
            vector2 = new Vector2(0.0f, 17f);
            string str45 = "Edition Text Position = " + vector2.ToString();
            streamWriter46.WriteLine(str45);
            streamWriter1.WriteLine("");
          }
          else
          {
            StreamWriter streamWriter47 = streamWriter1;
            vector2 = new Vector2(0.0f, 0.0f);
            string str46 = "Edition Text Position = " + vector2.ToString();
            streamWriter47.WriteLine(str46);
            streamWriter1.WriteLine("");
          }
        }
        if (!isGhost && cardUi.m_ChampionText != null)
        {
          streamWriter1.WriteLine("Champion Text = " + ((TMP_Text) cardUi.m_ChampionText).text);
          StreamWriter streamWriter48 = streamWriter1;
          flag2 = ((Behaviour) cardUi.m_ChampionText).enabled;
          string str47 = "Champion Text Enabled = " + flag2.ToString();
          streamWriter48.WriteLine(str47);
          StreamWriter streamWriter49 = streamWriter1;
          num = ((TMP_Text) cardUi.m_ChampionText).fontSize;
          string str48 = "Champion Font Size = " + num.ToString();
          streamWriter49.WriteLine(str48);
          StreamWriter streamWriter50 = streamWriter1;
          num = ((TMP_Text) cardUi.m_ChampionText).fontSizeMin;
          string str49 = "Champion Font Size Min = " + num.ToString();
          streamWriter50.WriteLine(str49);
          StreamWriter streamWriter51 = streamWriter1;
          num = ((TMP_Text) cardUi.m_ChampionText).fontSizeMax;
          string str50 = "Champion Font Size Max = " + num.ToString();
          streamWriter51.WriteLine(str50);
          StreamWriter streamWriter52 = streamWriter1;
          color = ((Graphic) cardUi.m_ChampionText).color;
          string str51 = "Champion Font Color RGBA = " + color.ToString();
          streamWriter52.WriteLine(str51);
          StreamWriter streamWriter53 = streamWriter1;
          vector2 = ((TMP_Text) cardUi.m_ChampionText).rectTransform.anchoredPosition;
          string str52 = "Champion Position = " + vector2.ToString();
          streamWriter53.WriteLine(str52);
          streamWriter1.WriteLine("");
        }
        if (isGhost && PlayerPatches.GetImageComponentByName(((Component) cardUi).gameObject, "CardStat") != null)
        {
          StreamWriter streamWriter54 = streamWriter1;
          flag2 = ((Behaviour) PlayerPatches.GetImageComponentByName(((Component) cardUi).gameObject, "CardStat")).enabled;
          string str53 = "Stat Background Image Enabled = " + flag2.ToString();
          streamWriter54.WriteLine(str53);
          streamWriter1.WriteLine("");
        }
        if (cardUi.m_Stat1Text != null)
        {
          streamWriter1.WriteLine("Stat1 = " + ((TMP_Text) cardUi.m_Stat1Text).text);
          StreamWriter streamWriter55 = streamWriter1;
          flag2 = ((Behaviour) cardUi.m_Stat1Text).enabled;
          string str54 = "Stat1 Enabled = " + flag2.ToString();
          streamWriter55.WriteLine(str54);
          StreamWriter streamWriter56 = streamWriter1;
          num = ((TMP_Text) cardUi.m_Stat1Text).fontSize;
          string str55 = "Stat1 Font Size = " + num.ToString();
          streamWriter56.WriteLine(str55);
          StreamWriter streamWriter57 = streamWriter1;
          num = ((TMP_Text) cardUi.m_Stat1Text).fontSizeMin;
          string str56 = "Stat1 Font Size Min = " + num.ToString();
          streamWriter57.WriteLine(str56);
          StreamWriter streamWriter58 = streamWriter1;
          num = ((TMP_Text) cardUi.m_Stat1Text).fontSizeMax;
          string str57 = "Stat1 Font Size Max = " + num.ToString();
          streamWriter58.WriteLine(str57);
          StreamWriter streamWriter59 = streamWriter1;
          color = ((Graphic) cardUi.m_Stat1Text).color;
          string str58 = "Stat1 Font Color RGBA = " + color.ToString();
          streamWriter59.WriteLine(str58);
          StreamWriter streamWriter60 = streamWriter1;
          vector2 = ((TMP_Text) cardUi.m_Stat1Text).rectTransform.anchoredPosition;
          string str59 = "Stat1 Position = " + vector2.ToString();
          streamWriter60.WriteLine(str59);
          streamWriter1.WriteLine("");
        }
        if (cardUi.m_Stat2Text != null)
        {
          streamWriter1.WriteLine("Stat2 = " + ((TMP_Text) cardUi.m_Stat2Text).text);
          StreamWriter streamWriter61 = streamWriter1;
          flag2 = ((Behaviour) cardUi.m_Stat2Text).enabled;
          string str60 = "Stat2 Enabled = " + flag2.ToString();
          streamWriter61.WriteLine(str60);
          StreamWriter streamWriter62 = streamWriter1;
          num = ((TMP_Text) cardUi.m_Stat2Text).fontSize;
          string str61 = "Stat2 Font Size = " + num.ToString();
          streamWriter62.WriteLine(str61);
          StreamWriter streamWriter63 = streamWriter1;
          num = ((TMP_Text) cardUi.m_Stat2Text).fontSizeMin;
          string str62 = "Stat2 Font Size Min = " + num.ToString();
          streamWriter63.WriteLine(str62);
          StreamWriter streamWriter64 = streamWriter1;
          num = ((TMP_Text) cardUi.m_Stat2Text).fontSizeMax;
          string str63 = "Stat2 Font Size Max = " + num.ToString();
          streamWriter64.WriteLine(str63);
          StreamWriter streamWriter65 = streamWriter1;
          color = ((Graphic) cardUi.m_Stat2Text).color;
          string str64 = "Stat2 Font Color RGBA = " + color.ToString();
          streamWriter65.WriteLine(str64);
          StreamWriter streamWriter66 = streamWriter1;
          vector2 = ((TMP_Text) cardUi.m_Stat2Text).rectTransform.anchoredPosition;
          string str65 = "Stat2 Position = " + vector2.ToString();
          streamWriter66.WriteLine(str65);
          streamWriter1.WriteLine("");
        }
        if (cardUi.m_Stat3Text != null)
        {
          streamWriter1.WriteLine("Stat3 = " + ((TMP_Text) cardUi.m_Stat3Text).text);
          StreamWriter streamWriter67 = streamWriter1;
          num = ((TMP_Text) cardUi.m_Stat3Text).fontSize;
          string str66 = "Stat3 Font Size = " + num.ToString();
          streamWriter67.WriteLine(str66);
          StreamWriter streamWriter68 = streamWriter1;
          flag2 = ((Behaviour) cardUi.m_Stat3Text).enabled;
          string str67 = "Stat3 Enabled = " + flag2.ToString();
          streamWriter68.WriteLine(str67);
          StreamWriter streamWriter69 = streamWriter1;
          num = ((TMP_Text) cardUi.m_Stat3Text).fontSizeMin;
          string str68 = "Stat3 Font Size Min = " + num.ToString();
          streamWriter69.WriteLine(str68);
          StreamWriter streamWriter70 = streamWriter1;
          num = ((TMP_Text) cardUi.m_Stat3Text).fontSizeMax;
          string str69 = "Stat3 Font Size Max = " + num.ToString();
          streamWriter70.WriteLine(str69);
          StreamWriter streamWriter71 = streamWriter1;
          color = ((Graphic) cardUi.m_Stat3Text).color;
          string str70 = "Stat3 Font Color RGBA = " + color.ToString();
          streamWriter71.WriteLine(str70);
          StreamWriter streamWriter72 = streamWriter1;
          vector2 = ((TMP_Text) cardUi.m_Stat3Text).rectTransform.anchoredPosition;
          string str71 = "Stat3 Position = " + vector2.ToString();
          streamWriter72.WriteLine(str71);
          streamWriter1.WriteLine("");
        }
        if (cardUi.m_Stat4Text == null)
          return;
        streamWriter1.WriteLine("Stat4 = " + ((TMP_Text) cardUi.m_Stat4Text).text);
        StreamWriter streamWriter73 = streamWriter1;
        flag2 = ((Behaviour) cardUi.m_Stat4Text).enabled;
        string str72 = "Stat4 Enabled = " + flag2.ToString();
        streamWriter73.WriteLine(str72);
        StreamWriter streamWriter74 = streamWriter1;
        num = ((TMP_Text) cardUi.m_Stat4Text).fontSize;
        string str73 = "Stat4 Font Size = " + num.ToString();
        streamWriter74.WriteLine(str73);
        StreamWriter streamWriter75 = streamWriter1;
        num = ((TMP_Text) cardUi.m_Stat4Text).fontSizeMin;
        string str74 = "Stat4 Font Size Min = " + num.ToString();
        streamWriter75.WriteLine(str74);
        StreamWriter streamWriter76 = streamWriter1;
        num = ((TMP_Text) cardUi.m_Stat4Text).fontSizeMax;
        string str75 = "Stat4 Font Size Max = " + num.ToString();
        streamWriter76.WriteLine(str75);
        StreamWriter streamWriter77 = streamWriter1;
        color = ((Graphic) cardUi.m_Stat4Text).color;
        string str76 = "Stat4 Font Color RGBA = " + color.ToString();
        streamWriter77.WriteLine(str76);
        StreamWriter streamWriter78 = streamWriter1;
        vector2 = ((TMP_Text) cardUi.m_Stat4Text).rectTransform.anchoredPosition;
        string str77 = "Stat4 Position = " + vector2.ToString();
        streamWriter78.WriteLine(str77);
      }
    }

    public static TextMeshProUGUI GetTextComponentByName(GameObject gameObject, string name)
    {
      if (gameObject != null)
      {
        foreach (TextMeshProUGUI componentsInChild in gameObject.GetComponentsInChildren<TextMeshProUGUI>())
        {
          if (componentsInChild != null && componentsInChild.name != null && componentsInChild.name == name)
            return componentsInChild;
        }
      }
      return (TextMeshProUGUI) null;
    }

    public static Image GetImageComponentByName(GameObject gameObject, string name)
    {
      if (gameObject != null)
      {
        foreach (Image componentsInChild in gameObject.GetComponentsInChildren<Image>())
        {
          if (componentsInChild != null && componentsInChild.name == name)
            return componentsInChild;
        }
      }
      return (Image) null;
    }

    public static Image GetImageComponentBySpriteName(GameObject gameObject, string name)
    {
      if (gameObject != null)
      {
        foreach (Image componentsInChild in gameObject.GetComponentsInChildren<Image>())
        {
          if (componentsInChild != null && componentsInChild.name != null && componentsInChild.sprite != null && componentsInChild.sprite.name != null && componentsInChild.sprite.name == name)
            return componentsInChild;
        }
      }
      return (Image) null;
    }

    public static string configSectionRedirect(
      bool redirect,
      string string1,
      string stringRedirect)
    {
      return redirect ? string1 : stringRedirect;
    }

    public static CardUI CurrentCardUI(bool isGhost, CardUI baseCard, CardUI ghostCard)
    {
      return isGhost ? ghostCard : baseCard;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof (CardUI), "SetCardUI")]
    public static void CardUI_SetCardUI_Extras_Postfix(CardUI __instance, CardData cardData)
    {
      if (!MinaCardsModPlugin.CustomConfigs.Value)
        return;
      
      MinaCardsModPlugin.Log.LogInfo("Monster Type: " + cardData.monsterType.ToString());
      MinaCardsModPlugin.Log.LogInfo("Expansion Type: " + cardData.expansionType.ToString());

      bool redirect = false;
      string configFilePath = PlayerPatches.findConfigFilePath(cardData);
      
      MinaCardsModPlugin.Log.LogInfo("Config File Path: " + configFilePath);
      
      string str1 = PlayerPatches.configPath + "FullExpansionsConfigs/" + cardData.expansionType.ToString();
      
      MinaCardsModPlugin.Log.LogInfo("Full Expansions Config Path: " + str1);
      if (string.IsNullOrEmpty(configFilePath))
      {
        MinaCardsModPlugin.Log.LogError("Config file path returned empty.");
        return;
      }
      
      
      bool isGhost = (int)cardData.expansionType == 2;
      bool flag;
      string str2;
      string stringRedirect;
      if ((int)cardData.borderType == 5)
      {
        flag = true;
        str2 = cardData.monsterType.ToString() + "FullArt";
        stringRedirect = cardData.expansionType.ToString() + "FullArt";
        if (File.Exists(configFilePath + "FullArt.ini"))
        {
          IniFile.Load(configFilePath + "FullArt.ini");
          if (IniFile.GetBoolValue(str2, "Individual Overrides"))
            redirect = true;
          if (File.Exists(str1 + "FullArt.ini"))
          {
            IniFile.LoadOther(str1 + "FullArt.ini");
          }
          else
          {
            MinaCardsModPlugin.Log.LogError((object) ("Expansion Config file not file -- " + str1 + "FullArt.ini"));
            return;
          }
        }
        else
        {
          MinaCardsModPlugin.Log.LogError((object) ("Monster Config file not file -- " + configFilePath + "FullArt.ini"));
          return;
        }
      }
      else
      {
        flag = false;
        if (File.Exists(configFilePath + ".ini"))
        {
          str2 = cardData.monsterType.ToString();
          stringRedirect = cardData.expansionType.ToString();
          IniFile.Load(configFilePath + ".ini");
          if (IniFile.GetBoolValue(str2, "Individual Overrides"))
            redirect = true;
          if (File.Exists(str1 + ".ini"))
          {
            IniFile.LoadOther(str1 + ".ini");
          }
          else
          {
            MinaCardsModPlugin.Log.LogError((object) ("Expansion Config file not file -- " + str1 + ".ini"));
            return;
          }
        }
        else
        {
          MinaCardsModPlugin.Log.LogError((object) ("Monster Config file not file -- " + configFilePath + ".ini"));
          return;
        }
      }
      string section = PlayerPatches.configSectionRedirect(redirect, str2, stringRedirect);
      if (IniFile.GetStringValue(str2, "Name") != "")
      {
        CardUI cardUi = !isGhost || __instance.m_GhostCard != null ? __instance : PlayerPatches.CurrentCardUI(isGhost, __instance, __instance.m_GhostCard);
        ((TMP_Text) cardUi.m_MonsterNameText).text = IniFile.GetStringValue(str2, "Name");
        if (!isGhost)
        {
          ((TMP_Text) cardUi.m_DescriptionText).text = IniFile.GetStringValue(str2, "Description");
          var monsterDataField = AccessTools.Field(typeof(CardUI), "m_MonsterData");
          var monsterData = (MonsterData)monsterDataField.GetValue(cardUi);
          EMonsterType previousEvolution = monsterData.PreviousEvolution;
          if (Enum.TryParse<EMonsterType>(IniFile.GetStringValue(str2, "Previous Evolution"), out previousEvolution))
            monsterData.PreviousEvolution = previousEvolution;
          ((TMP_Text) cardUi.m_ChampionText).text = IniFile.GetStringValueRedirect(redirect, section, "Champion Text");
          ((Behaviour) cardUi.m_DescriptionText).enabled = IniFile.GetBoolValueRedirect(redirect, section, "Description Enabled");
          ((TMP_Text) cardUi.m_DescriptionText).fontSize = IniFile.GetFloatValueRedirect(redirect, section, "Description Font Size");
          ((TMP_Text) cardUi.m_DescriptionText).fontSizeMin = IniFile.GetFloatValueRedirect(redirect, section, "Description Font Size Min");
          ((TMP_Text) cardUi.m_DescriptionText).fontSizeMax = IniFile.GetFloatValueRedirect(redirect, section, "Description Font Size Max");
          ((Graphic) cardUi.m_DescriptionText).color = IniFile.GetColorValueRedirect(redirect, section, "Description Font Color RGBA", new Color());
          ((TMP_Text) cardUi.m_DescriptionText).rectTransform.anchoredPosition = IniFile.GetVector2ValueRedirect(redirect, section, "Description Position", new Vector2());
          ((Behaviour) cardUi.m_EvoPreviousStageIcon).enabled = IniFile.GetBoolValueRedirect(redirect, section, "Previous Evolution Icon Enabled");
          ((Behaviour) cardUi.m_EvoPreviousStageNameText).enabled = IniFile.GetBoolValueRedirect(redirect, section, "Previous Evolution Name Enabled");
          ((TMP_Text) cardUi.m_EvoPreviousStageNameText).fontSize = IniFile.GetFloatValueRedirect(redirect, section, "Previous Evolution Name Font Size");
          ((TMP_Text) cardUi.m_EvoPreviousStageNameText).fontSizeMin = IniFile.GetFloatValueRedirect(redirect, section, "Previous Evolution Name Font Size Min");
          ((TMP_Text) cardUi.m_EvoPreviousStageNameText).fontSizeMax = IniFile.GetFloatValueRedirect(redirect, section, "Previous Evolution Name Font Size Max");
          ((Graphic) cardUi.m_EvoPreviousStageNameText).color = IniFile.GetColorValueRedirect(redirect, section, "Previous Evolution Name Font Color RGBA", new Color());
          ((TMP_Text) cardUi.m_EvoPreviousStageNameText).rectTransform.anchoredPosition = IniFile.GetVector2ValueRedirect(redirect, section, "Previous Evolution Name Position", new Vector2());
        }
        if (!flag)
          ((TMP_Text) cardUi.m_NumberText).text = IniFile.GetStringValue(str2, "Number");
        if (PlayerPatches.GetImageComponentByName(((Component) cardUi).gameObject, "EvoBG") != null)
          ((Behaviour) PlayerPatches.GetImageComponentByName(((Component) cardUi).gameObject, "EvoBG")).enabled = IniFile.GetBoolValue(str2, "Previous Evolution Box Enabled");
        if (PlayerPatches.GetTextComponentByName(((Component) cardUi).gameObject, "TitleText") != null)
        {
          TextMeshProUGUI textComponentByName = PlayerPatches.GetTextComponentByName(((Component) cardUi).gameObject, "TitleText");
          ((TMP_Text) textComponentByName).text = IniFile.GetStringValue(str2, "Play Effect Text");
          ((Behaviour) textComponentByName).enabled = IniFile.GetBoolValueRedirect(redirect, section, "Play Effect Text Enabled");
          ((TMP_Text) textComponentByName).fontSize = IniFile.GetFloatValueRedirect(redirect, section, "Play Effect Text Font Size");
          ((TMP_Text) textComponentByName).fontSizeMin = IniFile.GetFloatValueRedirect(redirect, section, "Play Effect Text Font Size Min");
          ((TMP_Text) textComponentByName).fontSizeMax = IniFile.GetFloatValueRedirect(redirect, section, "Play Effect Text Font Size Max");
          ((Graphic) textComponentByName).color = IniFile.GetColorValueRedirect(redirect, section, "Play Effect Text Font Color RGBA", new Color());
          ((TMP_Text) textComponentByName).rectTransform.anchoredPosition = IniFile.GetVector2ValueRedirect(redirect, section, "Play Effect Text Position", new Vector2());
        }
        if (PlayerPatches.GetImageComponentByName(((Component) cardUi).gameObject, "TitleBG") != null)
          ((Behaviour) PlayerPatches.GetImageComponentByName(((Component) cardUi).gameObject, "TitleBG")).enabled = IniFile.GetBoolValue(str2, "Play Effect Box Enabled");
        if (cardUi.m_FirstEditionText != null && !isGhost && !flag && ((TMP_Text) cardUi.m_FirstEditionText).text != null)
        {
          switch ((int) cardData.borderType)
          {
            case 1:
              ((TMP_Text) cardUi.m_FirstEditionText).text = IniFile.GetStringValue(str2, "First Edition Text");
              break;
            case 2:
              ((TMP_Text) cardUi.m_FirstEditionText).text = IniFile.GetStringValue(str2, "Silver Edition Text");
              break;
            case 3:
              ((TMP_Text) cardUi.m_FirstEditionText).text = IniFile.GetStringValue(str2, "Gold Edition Text");
              break;
            case 4:
              ((TMP_Text) cardUi.m_FirstEditionText).text = IniFile.GetStringValue(str2, "EX Edition Text");
              break;
          }
        }
        ((Behaviour) cardUi.m_MonsterNameText).enabled = IniFile.GetBoolValueRedirect(redirect, section, "Name Enabled");
        ((TMP_Text) cardUi.m_MonsterNameText).fontSize = IniFile.GetFloatValueRedirect(redirect, section, "Name Font Size");
        ((TMP_Text) cardUi.m_MonsterNameText).fontSizeMin = IniFile.GetFloatValueRedirect(redirect, section, "Name Font Size Min");
        ((TMP_Text) cardUi.m_MonsterNameText).fontSizeMax = IniFile.GetFloatValueRedirect(redirect, section, "Name Font Size Max");
        ((Graphic) cardUi.m_MonsterNameText).color = IniFile.GetColorValueRedirect(redirect, section, "Name Font Color RGBA", new Color());
        ((TMP_Text) cardUi.m_MonsterNameText).rectTransform.anchoredPosition = IniFile.GetVector2ValueRedirect(redirect, section, "Name Position", new Vector2());
        if (!isGhost && !flag)
        {
          if (cardUi.m_FirstEditionText != null && ((TMP_Text) cardUi.m_FirstEditionText).text != null)
          {
            ((TMP_Text) cardUi.m_FirstEditionText).fontSize = IniFile.GetFloatValueRedirect(redirect, section, "Edition Text Font Size");
            ((TMP_Text) cardUi.m_FirstEditionText).fontSizeMin = IniFile.GetFloatValueRedirect(redirect, section, "Edition Text Font Size Min");
            ((TMP_Text) cardUi.m_FirstEditionText).fontSizeMax = IniFile.GetFloatValueRedirect(redirect, section, "Edition Text Font Size Max");
            ((Graphic) cardUi.m_FirstEditionText).color = IniFile.GetColorValueRedirect(redirect, section, "Edition Text Font Color RGBA", new Color());
            ((TMP_Text) cardUi.m_FirstEditionText).rectTransform.anchoredPosition = IniFile.GetVector2ValueRedirect(redirect, section, "Edition Text Position", new Vector2());
          }
          ((TMP_Text) cardUi.m_RarityText).text = IniFile.GetStringValue(str2, "Rarity");
          ((Behaviour) cardUi.m_NumberText).enabled = IniFile.GetBoolValueRedirect(redirect, section, "Number Enabled");
          ((TMP_Text) cardUi.m_NumberText).fontSize = IniFile.GetFloatValueRedirect(redirect, section, "Number Font Size");
          ((TMP_Text) cardUi.m_NumberText).fontSizeMin = IniFile.GetFloatValueRedirect(redirect, section, "Number Font Size Min");
          ((TMP_Text) cardUi.m_NumberText).fontSizeMax = IniFile.GetFloatValueRedirect(redirect, section, "Number Font Size Max");
          ((Graphic) cardUi.m_NumberText).color = IniFile.GetColorValueRedirect(redirect, section, "Number Font Color RGBA", new Color());
          ((TMP_Text) cardUi.m_NumberText).rectTransform.anchoredPosition = IniFile.GetVector2ValueRedirect(redirect, section, "Number Position", new Vector2());
          ((Behaviour) cardUi.m_FirstEditionText).enabled = IniFile.GetBoolValueRedirect(redirect, section, "Edition Text Enabled");
          ((Behaviour) cardUi.m_RarityText).enabled = IniFile.GetBoolValueRedirect(redirect, section, "Rarity Enabled");
          ((TMP_Text) cardUi.m_RarityText).fontSize = IniFile.GetFloatValueRedirect(redirect, section, "Rarity Font Size");
          ((TMP_Text) cardUi.m_RarityText).fontSizeMin = IniFile.GetFloatValueRedirect(redirect, section, "Rarity Font Size Min");
          ((TMP_Text) cardUi.m_RarityText).fontSizeMax = IniFile.GetFloatValueRedirect(redirect, section, "Rarity Font Size Max");
          ((Graphic) cardUi.m_RarityText).color = IniFile.GetColorValueRedirect(redirect, section, "Rarity Font Color RGBA", new Color());
          ((TMP_Text) cardUi.m_RarityText).rectTransform.anchoredPosition = IniFile.GetVector2ValueRedirect(redirect, section, "Rarity Position", new Vector2());
        }
        if (!isGhost)
        {
          ((Behaviour) cardUi.m_ChampionText).enabled = IniFile.GetBoolValueRedirect(redirect, section, "Champion Text Enabled");
          ((TMP_Text) cardUi.m_ChampionText).fontSize = IniFile.GetFloatValueRedirect(redirect, section, "Champion Font Size");
          ((TMP_Text) cardUi.m_ChampionText).fontSizeMin = IniFile.GetFloatValueRedirect(redirect, section, "Champion Font Size Min");
          ((TMP_Text) cardUi.m_ChampionText).fontSizeMax = IniFile.GetFloatValueRedirect(redirect, section, "Champion Font Size Max");
          ((Graphic) cardUi.m_ChampionText).color = IniFile.GetColorValueRedirect(redirect, section, "Champion Font Color RGBA", new Color());
          ((TMP_Text) cardUi.m_ChampionText).rectTransform.anchoredPosition = IniFile.GetVector2ValueRedirect(redirect, section, "Champion Position", new Vector2());
        }
        if (isGhost && PlayerPatches.GetImageComponentByName(((Component) cardUi).gameObject, "CardStat") != null)
          ((Behaviour) PlayerPatches.GetImageComponentByName(((Component) cardUi).gameObject, "CardStat")).enabled = IniFile.GetBoolValue(str2, "Stat Background Image Enabled");
        ((TMP_Text) cardUi.m_Stat1Text).text = IniFile.GetStringValue(str2, "Stat1");
        ((Behaviour) cardUi.m_Stat1Text).enabled = IniFile.GetBoolValueRedirect(redirect, section, "Stat1 Enabled");
        ((TMP_Text) cardUi.m_Stat1Text).fontSize = IniFile.GetFloatValueRedirect(redirect, section, "Stat1 Font Size");
        ((TMP_Text) cardUi.m_Stat1Text).fontSizeMin = IniFile.GetFloatValueRedirect(redirect, section, "Stat1 Font Size Min");
        ((TMP_Text) cardUi.m_Stat1Text).fontSizeMax = IniFile.GetFloatValueRedirect(redirect, section, "Stat1 Font Size Max");
        ((Graphic) cardUi.m_Stat1Text).color = IniFile.GetColorValueRedirect(redirect, section, "Stat1 Font Color RGBA", new Color());
        ((TMP_Text) cardUi.m_Stat1Text).rectTransform.anchoredPosition = IniFile.GetVector2ValueRedirect(redirect, section, "Stat1 Position", new Vector2());
        ((TMP_Text) cardUi.m_Stat2Text).text = IniFile.GetStringValue(str2, "Stat2");
        ((Behaviour) cardUi.m_Stat2Text).enabled = IniFile.GetBoolValueRedirect(redirect, section, "Stat2 Enabled");
        ((TMP_Text) cardUi.m_Stat2Text).fontSize = IniFile.GetFloatValueRedirect(redirect, section, "Stat2 Font Size");
        ((TMP_Text) cardUi.m_Stat2Text).fontSizeMin = IniFile.GetFloatValueRedirect(redirect, section, "Stat2 Font Size Min");
        ((TMP_Text) cardUi.m_Stat2Text).fontSizeMax = IniFile.GetFloatValueRedirect(redirect, section, "Stat2 Font Size Max");
        ((Graphic) cardUi.m_Stat2Text).color = IniFile.GetColorValueRedirect(redirect, section, "Stat2 Font Color RGBA", new Color());
        ((TMP_Text) cardUi.m_Stat2Text).rectTransform.anchoredPosition = IniFile.GetVector2ValueRedirect(redirect, section, "Stat2 Position", new Vector2());
        ((TMP_Text) cardUi.m_Stat3Text).text = IniFile.GetStringValue(str2, "Stat3");
        ((Behaviour) cardUi.m_Stat3Text).enabled = IniFile.GetBoolValueRedirect(redirect, section, "Stat3 Enabled");
        ((TMP_Text) cardUi.m_Stat3Text).fontSize = IniFile.GetFloatValueRedirect(redirect, section, "Stat3 Font Size");
        ((TMP_Text) cardUi.m_Stat3Text).fontSizeMin = IniFile.GetFloatValueRedirect(redirect, section, "Stat3 Font Size Min");
        ((TMP_Text) cardUi.m_Stat3Text).fontSizeMax = IniFile.GetFloatValueRedirect(redirect, section, "Stat3 Font Size Max");
        ((Graphic) cardUi.m_Stat3Text).color = IniFile.GetColorValueRedirect(redirect, section, "Stat3 Font Color RGBA", new Color());
        ((TMP_Text) cardUi.m_Stat3Text).rectTransform.anchoredPosition = IniFile.GetVector2ValueRedirect(redirect, section, "Stat3 Position", new Vector2());
        ((TMP_Text) cardUi.m_Stat4Text).text = IniFile.GetStringValue(str2, "Stat4");
        ((Behaviour) cardUi.m_Stat4Text).enabled = IniFile.GetBoolValueRedirect(redirect, section, "Stat4 Enabled");
        ((TMP_Text) cardUi.m_Stat4Text).fontSize = IniFile.GetFloatValueRedirect(redirect, section, "Stat4 Font Size");
        ((TMP_Text) cardUi.m_Stat4Text).fontSizeMin = IniFile.GetFloatValueRedirect(redirect, section, "Stat4 Font Size Min");
        ((TMP_Text) cardUi.m_Stat4Text).fontSizeMax = IniFile.GetFloatValueRedirect(redirect, section, "Stat4 Font Size Max");
        ((Graphic) cardUi.m_Stat4Text).color = IniFile.GetColorValueRedirect(redirect, section, "Stat4 Font Color RGBA", new Color());
        ((TMP_Text) cardUi.m_Stat4Text).rectTransform.anchoredPosition = IniFile.GetVector2ValueRedirect(redirect, section, "Stat4 Position", new Vector2());
      }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof (CardUI), "SetCardUI")]
    public static void CardUI_SetCardUI_Postfix(CardUI __instance, CardData cardData)
    {
      bool isGhost = (int)cardData.expansionType == 2;
      CardUI cardUi = !isGhost || __instance.m_GhostCard != null ? __instance : PlayerPatches.CurrentCardUI(isGhost, __instance, __instance.m_GhostCard);
      bool flag = (int)cardData.expansionType == 3 || (int)cardData.expansionType == 4 || (int)cardData.expansionType == 5;
      if (MinaCardsModPlugin.CustomImages.Value && flag)
      {
        List<string> stringList = new List<string>();
        foreach (string file in Directory.GetFiles(PlayerPatches.cardExtrasImages, "*.png"))
        {
          string withoutExtension = Path.GetFileNameWithoutExtension(file);
          stringList.Add(withoutExtension);
        }
        foreach (string str in stringList)
        {
          Image componentBySpriteName = PlayerPatches.GetImageComponentBySpriteName(((Component) cardUi).gameObject, str);
          if (componentBySpriteName != null)
            componentBySpriteName.sprite = PlayerPatches.GetCustomImage(str, PlayerPatches.cardExtrasImages);
        }
      }
      if (!MinaCardsModPlugin.CustomBaseMonsterIcons.Value || flag)
        return;
      List<string> stringList1 = new List<string>();
      foreach (string file in Directory.GetFiles(PlayerPatches.ghostPackImages, "*.png"))
      {
        string withoutExtension = Path.GetFileNameWithoutExtension(file);
        stringList1.Add(withoutExtension);
      }
      foreach (string str in stringList1)
      {
        Image componentBySpriteName = PlayerPatches.GetImageComponentBySpriteName(((Component) cardUi).gameObject, str);
        if (componentBySpriteName != null)
          componentBySpriteName.sprite = PlayerPatches.GetCustomImage(str, PlayerPatches.ghostPackImages);
      }
      List<string> stringList2 = new List<string>();
      foreach (string file in Directory.GetFiles(PlayerPatches.basePackImages, "*.png"))
      {
        string withoutExtension = Path.GetFileNameWithoutExtension(file);
        stringList2.Add(withoutExtension);
      }
      foreach (string str in stringList2)
      {
        Image componentBySpriteName = PlayerPatches.GetImageComponentBySpriteName(((Component) cardUi).gameObject, str);
        if (componentBySpriteName != null)
          componentBySpriteName.sprite = PlayerPatches.GetCustomImage(str, PlayerPatches.basePackImages);
      }
      List<string> stringList3 = new List<string>();
      foreach (string file in Directory.GetFiles(PlayerPatches.cardExtrasImages, "*.png"))
      {
        string withoutExtension = Path.GetFileNameWithoutExtension(file);
        stringList3.Add(withoutExtension);
      }
      foreach (string str in stringList3)
      {
        Image componentBySpriteName = PlayerPatches.GetImageComponentBySpriteName(((Component) cardUi).gameObject, str);
        if (componentBySpriteName != null)
          componentBySpriteName.sprite = PlayerPatches.GetCustomImage(str, PlayerPatches.cardExtrasImages);
      }
      List<string> stringList4 = new List<string>();
      foreach (string file in Directory.GetFiles(PlayerPatches.cardExtrasImages, "*.png"))
      {
        string withoutExtension = Path.GetFileNameWithoutExtension(file);
        stringList3.Add(withoutExtension);
      }
      foreach (string str in stringList4)
      {
        Image componentBySpriteName = PlayerPatches.GetImageComponentBySpriteName(((Component) cardUi).gameObject, str);
        if (componentBySpriteName != null)
          componentBySpriteName.sprite = PlayerPatches.GetCustomImage(str, PlayerPatches.cardExtrasImages);
      }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof (MonsterData), "GetIcon")]
    public static bool MonsterData_GetIcon_Prefix(
      ECardExpansionType cardExpansionType,
      ref Sprite __result,
      ref MonsterData __instance)
    {
      if (MinaCardsModPlugin.CustomImages.Value)
      {
        if ((int)cardExpansionType == 4 || (int)cardExpansionType == 5 || (int)cardExpansionType == 3)
        {
          string path1 = PlayerPatches.fantasyPackImages + __instance.MonsterType.ToString() + ".png";
          string path2 = PlayerPatches.catJobPackImages + __instance.MonsterType.ToString() + ".png";
          string path3 = PlayerPatches.megabotPackImages + __instance.MonsterType.ToString() + ".png";
          if ((int)cardExpansionType == 4)
          {
            if (File.Exists(path1))
            {
              __result = PlayerPatches.GetCustomImage(__instance.MonsterType.ToString(), PlayerPatches.fantasyPackImages);
              return false;
            }
          }
          else if ((int)cardExpansionType == 5)
          {
            if (File.Exists(path2))
            {
              __result = PlayerPatches.GetCustomImage(__instance.MonsterType.ToString(), PlayerPatches.catJobPackImages);
              return false;
            }
          }
          else if ((int)cardExpansionType == 3 && File.Exists(path3))
          {
            __result = PlayerPatches.GetCustomImage(__instance.MonsterType.ToString(), PlayerPatches.megabotPackImages);
            return false;
          }
        }
      }
      else if (!MinaCardsModPlugin.CustomImages.Value)
      {
        InventoryBase.GetMonsterData((EMonsterType) 1);
        int monsterType = (int) __instance.MonsterType;
        if (monsterType > 110)
        {
          int num = monsterType;
          if (monsterType >= 1000 && monsterType <= 1112)
            num = monsterType < 1110 ? monsterType - 999 : monsterType - 1109;
          else if (monsterType >= 2000 && monsterType <= 2049)
            num = monsterType - 1999;
          else if (monsterType >= 3000 && monsterType <= 3039)
            num = monsterType - 2949;
          MonsterData monsterData = InventoryBase.GetMonsterData((EMonsterType) num);
          __result = monsterData.Icon;
          return false;
        }
      }
      return true;
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
        if (!CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_ShownItemType.Contains((EItemType) 18))
        {
          CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_ShownItemType.Add((EItemType) 18);
          CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_ShownItemType.Add((EItemType) 19);
          CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_ShownItemType.Add((EItemType) 17);
          CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_ShownItemType.Add((EItemType) 16);
        }
        if (!CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_ShownAllItemType.Contains((EItemType) 18))
        {
          CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_ShownAllItemType.Add((EItemType) 18);
          CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_ShownAllItemType.Add((EItemType) 19);
          CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_ShownAllItemType.Add((EItemType) 17);
          CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_ShownAllItemType.Add((EItemType) 16);
        }
        RestockData restockData1 = new RestockData();
        RestockData restockData2 = new RestockData();
        RestockData restockData3 = new RestockData();
        RestockData restockData4 = new RestockData();
        RestockData restockData5 = new RestockData();
        RestockData restockData6 = new RestockData();
        RestockData restockData7 = new RestockData();
        RestockData restockData8 = new RestockData();
        restockData1.itemType = (EItemType) 18;
        restockData1.licenseShopLevelRequired = 30;
        restockData1.licensePrice = 2500f;
        CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_RestockDataList.Add(restockData1);
        restockData2.itemType = (EItemType) 18;
        restockData2.licenseShopLevelRequired = 30;
        restockData2.licensePrice = 5000f;
        restockData2.amount = 64;
        restockData2.isBigBox = true;
        CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_RestockDataList.Add(restockData2);
        restockData3.itemType = (EItemType) 19;
        restockData3.licenseShopLevelRequired = 40;
        restockData3.licensePrice = 5000f;
        CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_RestockDataList.Add(restockData3);
        restockData4.itemType = (EItemType) 19;
        restockData4.licenseShopLevelRequired = 40;
        restockData4.licensePrice = 7500f;
        restockData4.amount = 64;
        restockData4.isBigBox = true;
        CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_RestockDataList.Add(restockData4);
        restockData5.itemType = (EItemType) 17;
        restockData5.licenseShopLevelRequired = 50;
        restockData5.licensePrice = 7500f;
        CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_RestockDataList.Add(restockData5);
        restockData6.itemType = (EItemType) 17;
        restockData6.licenseShopLevelRequired = 50;
        restockData6.licensePrice = 10000f;
        restockData6.amount = 64;
        restockData6.isBigBox = true;
        CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_RestockDataList.Add(restockData6);
        restockData7.itemType = (EItemType) 16;
        restockData7.licenseShopLevelRequired = 60;
        restockData7.licensePrice = 10000f;
        CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_RestockDataList.Add(restockData7);
        restockData8.itemType = (EItemType) 16;
        restockData8.licenseShopLevelRequired = 60;
        restockData8.licensePrice = 15000f;
        restockData8.amount = 64;
        restockData8.isBigBox = true;
        CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_RestockDataList.Add(restockData8);
        PlayerPatches.containsNew = true;
      }
      return true;
    }

    public static void ShowText(string customText)
    {
      var resetTimerField = AccessTools.Field(typeof(NotEnoughResourceTextPopup), "m_ResetTimer");
      resetTimerField.SetValue(CSingleton<NotEnoughResourceTextPopup>.Instance, 0.0f);

      string str = customText;
      for (int index = 0; index < CSingleton<NotEnoughResourceTextPopup>.Instance.m_ShowTextGameObjectList.Count; ++index)
      {
        if (!CSingleton<NotEnoughResourceTextPopup>.Instance.m_ShowTextGameObjectList[index].activeSelf)
        {
          ((TMP_Text) CSingleton<NotEnoughResourceTextPopup>.Instance.m_ShowTextList[index]).text = str;
          CSingleton<NotEnoughResourceTextPopup>.Instance.m_ShowTextGameObjectList[index].gameObject.SetActive(true);
          break;
        }
      }
    }

    public static Texture2D LoadTexture(string filePath)
    {
      if (File.Exists(filePath))
      {
        Texture2D texture2D = new Texture2D(2, 2);
        byte[] numArray = File.ReadAllBytes(filePath);
        ImageConversion.LoadImage(texture2D, numArray);
        return texture2D;
      }
      MinaCardsModPlugin.Log.LogError((object) ("Texture file not found: " + filePath));
      return (Texture2D) null;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof (InventoryBase), "Awake")]
    public static void InventoryBase_Awake_Postfix()
    {
      if (InventoryBase.GetItemMeshData((EItemType)19).material.mainTexture.name != "CommonPack_GhostMegabot")
        return;
      ItemMeshData itemMeshData1 = InventoryBase.GetItemMeshData((EItemType) 8);
      ItemMeshData itemMeshData2 = InventoryBase.GetItemMeshData((EItemType) 19);
      ItemMeshData itemMeshData3 = InventoryBase.GetItemMeshData((EItemType) 18);
      ItemMeshData itemMeshData4 = InventoryBase.GetItemMeshData((EItemType) 17);
      ItemMeshData itemMeshData5 = InventoryBase.GetItemMeshData((EItemType) 16);
      Texture2D texture2D1 = PlayerPatches.LoadTexture(PlayerPatches.customExpansionPackImages + "T_CardPackFantasy.png");
      Material material1 = new Material(itemMeshData1.material);
      material1.mainTexture = texture2D1;
      material1.mainTexture.name = "T_CardPackFantasy";
      itemMeshData3.material = material1;
      Texture2D texture2D2 = PlayerPatches.LoadTexture(PlayerPatches.customExpansionPackImages + "T_CardPackCatJob.png");
      Material material2 = new Material(itemMeshData1.material);
      material2.mainTexture = texture2D2;
      material2.mainTexture.name = "T_CardPackCatJob";
      itemMeshData2.material = material2;
      Texture2D texture2D3 = PlayerPatches.LoadTexture(PlayerPatches.customExpansionPackImages + "T_CardPackMegabot.png");
      Material material3 = new Material(itemMeshData1.material);
      material3.mainTexture = texture2D3;
      material3.mainTexture.name = "T_CardPackMegabot";
      itemMeshData4.material = material3;
      Texture2D texture2D4 = PlayerPatches.LoadTexture(PlayerPatches.customExpansionPackImages + "T_CardPackGhost.png");
      Material material4 = new Material(itemMeshData1.material);
      material4.mainTexture = texture2D4;
      material4.mainTexture.name = "T_CardPackGhost";
      itemMeshData5.material = material4;
    }
  }
}