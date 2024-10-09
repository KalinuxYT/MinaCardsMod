using HarmonyLib;
using I2.Loc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
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
        __instance.m_ExpansionBtnList[0].GetComponentInChildren<TMP_Text>().text = "Megabot";
        __instance.m_ExpansionBtnList[1].GetComponentInChildren<TMP_Text>().text = "Fantasy";
        __instance.m_ExpansionBtnList[2].GetComponentInChildren<TMP_Text>().text = "Cat Job";
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
        __instance.m_ExpansionBtnList[0].GetComponentInChildren<TMP_Text>().text = "Tetramon";
        __instance.m_ExpansionBtnList[1].GetComponentInChildren<TMP_Text>().text = "Destiny";
        __instance.m_ExpansionBtnList[2].GetComponentInChildren<TMP_Text>().text = "Ghost";
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

    public static Texture LoadCustomTexture(string fileName, string imagePath)
    {
      string path = Path.Combine(imagePath, fileName + ".png");
      if (!File.Exists(path))
        return (Texture) null;
      byte[] data = File.ReadAllBytes(path);
      Texture2D tex = new Texture2D(2, 2, DefaultFormat.LDR, TextureCreationFlags.None);
      tex.LoadImage(data);
      return (Texture) tex;
    }

    public static Texture2D LoadCustomPNG(string fileName, string imagePath)
    {
      string path = imagePath + fileName + ".png";
      if (!File.Exists(path))
        return (Texture2D) null;
      byte[] data = File.ReadAllBytes(path);
      Texture2D tex = new Texture2D(2, 2, DefaultFormat.LDR, TextureCreationFlags.None);
      tex.LoadImage(data);
      return tex;
    }
    
    public static Sprite GetCustomImage(string fileName, string imagePath)
    {
      Sprite customImage1 = (Sprite) null;
      
      var dictField = typeof(LoadStreamTexture).GetField("m_LoadedSpriteDict", BindingFlags.NonPublic | BindingFlags.Instance);
      var loadedSpriteDict = dictField.GetValue(CSingleton<LoadStreamTexture>.Instance) as Dictionary<string, Sprite>;
      
      if (loadedSpriteDict != null && loadedSpriteDict.ContainsKey(fileName))
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
      Texture2D texture = PlayerPatches.LoadCustomPNG(fileName, imagePath);
      if (texture != null)
      {
        Sprite customImage2 = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), Vector2.zero);
        customImage2.name = fileName;
        if (loadedSpriteDict != null && !loadedSpriteDict.ContainsKey(fileName))
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

        MethodInfo loadTextureMethod = typeof(LoadStreamTexture).GetMethod("LoadTextureFromWeb", BindingFlags.NonPublic | BindingFlags.Instance);
        if (loadTextureMethod != null)
        {
          CSingleton<LoadStreamTexture>.Instance.StartCoroutine((IEnumerator)loadTextureMethod.Invoke(CSingleton<LoadStreamTexture>.Instance, new object[] { fileName }));
        }
      }
      return null;
    }
    public static string findConfigFilePath(CardData cardData)
    {
      string str = PlayerPatches.configPath + cardData.monsterType.ToString();
      switch (cardData.expansionType)
      {
        case ECardExpansionType.None:
          return PlayerPatches.configPath + "NoExpansionConfigs/" + cardData.monsterType.ToString();
        case ECardExpansionType.Tetramon:
          return PlayerPatches.configPath + "TetramonConfigs/" + cardData.monsterType.ToString();
        case ECardExpansionType.Destiny:
          return PlayerPatches.configPath + "DestinyConfigs/" + cardData.monsterType.ToString();
        case ECardExpansionType.Ghost:
          return PlayerPatches.configPath + "GhostConfigs/" + cardData.monsterType.ToString();
        case ECardExpansionType.Megabot:
          return PlayerPatches.configPath + "MegabotConfigs/" + cardData.monsterType.ToString();
        case ECardExpansionType.FantasyRPG:
          return PlayerPatches.configPath + "FantasyRPGConfigs/" + cardData.monsterType.ToString();
        case ECardExpansionType.CatJob:
          return PlayerPatches.configPath + "CatJobConfigs/" + cardData.monsterType.ToString();
        case ECardExpansionType.FoodieGO:
          return PlayerPatches.configPath + "FoodieConfigs/" + cardData.monsterType.ToString();
        case ECardExpansionType.MAX:
          return PlayerPatches.configPath + "MaxConfigs/" + cardData.monsterType.ToString();
        default:
          return (string) null;
      }
    }

    public static void writeMonsterData(CardData cardData, CardUI cardUI)
    {
      string configFilePath = PlayerPatches.findConfigFilePath(cardData);
      if (!File.Exists(configFilePath + ".ini") && cardData.borderType != ECardBorderType.FullArt)
      {
        MinaCardsModPlugin.Log.LogInfo((object) ("Creating config for " + cardData.monsterType.ToString()));
        PlayerPatches.WriteCardDataToFile(configFilePath + ".ini", cardData, cardUI);
      }
      if (File.Exists(configFilePath + "FullArt.ini") || cardData.borderType != ECardBorderType.FullArt)
        return;
      MinaCardsModPlugin.Log.LogInfo((object) ("Creating Full Art config for " + cardData.monsterType.ToString()));
      PlayerPatches.WriteCardDataToFile(configFilePath + "FullArt.ini", cardData, cardUI);
    }

    public static void WriteCardDataToFile(string filePath, CardData cardData, CardUI cardUI)
    {
      bool flag1 = false;
      bool isGhost = cardData.expansionType == ECardExpansionType.Ghost;
      bool flag2 = cardData.borderType == ECardBorderType.FullArt;
      flag1 = cardData.isFoil;
      using (StreamWriter streamWriter1 = new StreamWriter(filePath))
      {
        if (cardData.borderType == ECardBorderType.FullArt)
          streamWriter1.WriteLine("[" + cardData.monsterType.ToString() + "FullArt]");
        else
          streamWriter1.WriteLine("[" + cardData.monsterType.ToString() + "]");
        CardUI cardUi = !isGhost || !((UnityEngine.Object) cardUI.m_GhostCard != (UnityEngine.Object) null) ? cardUI : PlayerPatches.CurrentCardUI(isGhost, cardUI, cardUI.m_GhostCard);
        bool flag3;
        float num;
        if ((UnityEngine.Object) cardUi.m_MonsterNameText != (UnityEngine.Object) null)
        {
          streamWriter1.WriteLine("Name = " + cardUi.m_MonsterNameText.text);
          StreamWriter streamWriter2 = streamWriter1;
          flag3 = cardUi.m_MonsterNameText.enabled;
          string str1 = "Name Enabled = " + flag3.ToString();
          streamWriter2.WriteLine(str1);
          streamWriter1.WriteLine("Name Font Size = " + cardUi.m_MonsterNameText.fontSize.ToString());
          StreamWriter streamWriter3 = streamWriter1;
          num = cardUi.m_MonsterNameText.fontSizeMin;
          string str2 = "Name Font Size Min = " + num.ToString();
          streamWriter3.WriteLine(str2);
          StreamWriter streamWriter4 = streamWriter1;
          num = cardUi.m_MonsterNameText.fontSizeMax;
          string str3 = "Name Font Size Max = " + num.ToString();
          streamWriter4.WriteLine(str3);
          streamWriter1.WriteLine("Name Font Color RGBA = " + cardUi.m_MonsterNameText.color.ToString());
          streamWriter1.WriteLine("Name Position = " + cardUi.m_MonsterNameText.rectTransform.anchoredPosition.ToString());
          streamWriter1.WriteLine("");
        }
        if (!isGhost && (UnityEngine.Object) cardUi.m_DescriptionText != (UnityEngine.Object) null)
        {
          streamWriter1.WriteLine("Description = " + cardUi.m_DescriptionText.text);
          StreamWriter streamWriter5 = streamWriter1;
          flag3 = cardUi.m_DescriptionText.enabled;
          string str4 = "Description Enabled = " + flag3.ToString();
          streamWriter5.WriteLine(str4);
          StreamWriter streamWriter6 = streamWriter1;
          num = cardUi.m_DescriptionText.fontSize;
          string str5 = "Description Font Size = " + num.ToString();
          streamWriter6.WriteLine(str5);
          StreamWriter streamWriter7 = streamWriter1;
          num = cardUi.m_DescriptionText.fontSizeMin;
          string str6 = "Description Font Size Min = " + num.ToString();
          streamWriter7.WriteLine(str6);
          StreamWriter streamWriter8 = streamWriter1;
          num = cardUi.m_DescriptionText.fontSizeMax;
          string str7 = "Description Font Size Max = " + num.ToString();
          streamWriter8.WriteLine(str7);
          streamWriter1.WriteLine("Description Font Color RGBA = " + cardUi.m_DescriptionText.color.ToString());
          streamWriter1.WriteLine("Description Position = " + cardUi.m_DescriptionText.rectTransform.anchoredPosition.ToString());
          streamWriter1.WriteLine("");
        }
        StreamWriter streamWriter9 = streamWriter1;
        flag3 = false;
        string str8 = "Individual Overrides = " + flag3.ToString();
        streamWriter9.WriteLine(str8);
        streamWriter1.WriteLine("");
        if (!isGhost && !flag2 && (UnityEngine.Object) cardUi.m_NumberText != (UnityEngine.Object) null)
        {
          streamWriter1.WriteLine("Number = " + cardUi.m_NumberText.text);
          StreamWriter streamWriter10 = streamWriter1;
          flag3 = cardUi.m_NumberText.enabled;
          string str9 = "Number Enabled = " + flag3.ToString();
          streamWriter10.WriteLine(str9);
          StreamWriter streamWriter11 = streamWriter1;
          num = cardUi.m_NumberText.fontSize;
          string str10 = "Number Font Size = " + num.ToString();
          streamWriter11.WriteLine(str10);
          StreamWriter streamWriter12 = streamWriter1;
          num = cardUi.m_NumberText.fontSizeMin;
          string str11 = "Number Font Size Min = " + num.ToString();
          streamWriter12.WriteLine(str11);
          StreamWriter streamWriter13 = streamWriter1;
          num = cardUi.m_NumberText.fontSizeMax;
          string str12 = "Number Font Size Max = " + num.ToString();
          streamWriter13.WriteLine(str12);
          streamWriter1.WriteLine("Number Font Color RGBA = " + cardUi.m_NumberText.color.ToString());
          streamWriter1.WriteLine("Number Position = " + cardUi.m_NumberText.rectTransform.anchoredPosition.ToString());
          streamWriter1.WriteLine("");
        }
        Color color;
        FieldInfo monsterDataField = typeof(CardUI).GetField("m_MonsterData", BindingFlags.NonPublic | BindingFlags.Instance);
        object monsterData = monsterDataField?.GetValue(cardUi);
        if (!isGhost && monsterData != null)
        {
          FieldInfo previousEvolutionField = monsterData.GetType().GetField("PreviousEvolution", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
          EMonsterType previousEvolution = (EMonsterType)previousEvolutionField?.GetValue(monsterData);
          if (previousEvolution == EMonsterType.None)
          {
            if ((UnityEngine.Object)PlayerPatches.GetImageComponentByName(cardUi.gameObject, "EvoBasicIcon") !=
                (UnityEngine.Object)null)
            {
              StreamWriter streamWriter14 = streamWriter1;
              flag3 = PlayerPatches.GetImageComponentByName(cardUi.gameObject, "EvoBasicIcon").enabled;
              string str13 = "Basic Evolution Icon Enabled = " + flag3.ToString();
              streamWriter14.WriteLine(str13);
            }
            if ((UnityEngine.Object)PlayerPatches.GetTextComponentByName(cardUi.gameObject, "EvoBasicText") !=
                (UnityEngine.Object)null)
            {
              TextMeshProUGUI textComponentByName =
                PlayerPatches.GetTextComponentByName(cardUi.gameObject, "EvoBasicText");
              streamWriter1.WriteLine("Basic Evolution Text = " + textComponentByName.text);
              StreamWriter streamWriter15 = streamWriter1;
              flag3 = textComponentByName.enabled;
              string str14 = "Basic Evolution Text Enabled = " + flag3.ToString();
              streamWriter15.WriteLine(str14);
              StreamWriter streamWriter16 = streamWriter1;
              num = textComponentByName.fontSize;
              string str15 = "Basic Evolution Text Font Size = " + num.ToString();
              streamWriter16.WriteLine(str15);
              StreamWriter streamWriter17 = streamWriter1;
              num = textComponentByName.fontSizeMin;
              string str16 = "Basic Evolution Text Font Size Min = " + num.ToString();
              streamWriter17.WriteLine(str16);
              StreamWriter streamWriter18 = streamWriter1;
              num = textComponentByName.fontSizeMax;
              string str17 = "Basic Evolution Text Font Size Max = " + num.ToString();
              streamWriter18.WriteLine(str17);
              StreamWriter streamWriter19 = streamWriter1;
              color = textComponentByName.color;
              string str18 = "Basic Evolution Text Font Color RGBA = " + color.ToString();
              streamWriter19.WriteLine(str18);
              streamWriter1.WriteLine("Basic Evolution Text Position = " + textComponentByName.rectTransform.anchoredPosition.ToString());
            }
          }
        }
        if (!isGhost && monsterData != null)
        {
          FieldInfo previousEvolutionField = monsterData.GetType().GetField("PreviousEvolution", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
          EMonsterType previousEvolution = (EMonsterType)previousEvolutionField?.GetValue(monsterData);
          int previousEvolutionInt = (int)previousEvolution;
          if (true)
          {
            streamWriter1.WriteLine("Previous Evolution = " + previousEvolution.ToString());
            StreamWriter streamWriter20 = streamWriter1;
            flag3 = cardUi.m_EvoPreviousStageIcon.enabled;
            string str19 = "Previous Evolution Icon Enabled = " + flag3.ToString();
            streamWriter20.WriteLine(str19);
            StreamWriter streamWriter21 = streamWriter1;
            flag3 = cardUi.m_EvoPreviousStageNameText.enabled;
            string str20 = "Previous Evolution Name Enabled = " + flag3.ToString();
            streamWriter21.WriteLine(str20);
            StreamWriter streamWriter22 = streamWriter1;
            num = cardUi.m_EvoPreviousStageNameText.fontSize;
            string str21 = "Previous Evolution Name Font Size = " + num.ToString();
            streamWriter22.WriteLine(str21);
            StreamWriter streamWriter23 = streamWriter1;
            num = cardUi.m_EvoPreviousStageNameText.fontSizeMin;
            string str22 = "Previous Evolution Name Font Size Min = " + num.ToString();
            streamWriter23.WriteLine(str22);
            StreamWriter streamWriter24 = streamWriter1;
            num = cardUi.m_EvoPreviousStageNameText.fontSizeMax;
            string str23 = "Previous Evolution Name Font Size Max = " + num.ToString();
            streamWriter24.WriteLine(str23);
            StreamWriter streamWriter25 = streamWriter1;
            color = cardUi.m_EvoPreviousStageNameText.color;
            string str24 = "Previous Evolution Name Font Color RGBA = " + color.ToString();
            streamWriter25.WriteLine(str24);
            streamWriter1.WriteLine("Previous Evolution Name Position = " + cardUi.m_EvoPreviousStageNameText.rectTransform.anchoredPosition.ToString());
          }
          if ((UnityEngine.Object) PlayerPatches.GetImageComponentByName(cardUi.gameObject, "EvoBG") != (UnityEngine.Object) null)
          {
            StreamWriter streamWriter26 = streamWriter1;
            flag3 = PlayerPatches.GetImageComponentByName(cardUi.gameObject, "EvoBG").enabled;
            string str25 = "Previous Evolution Box Enabled = " + flag3.ToString();
            streamWriter26.WriteLine(str25);
            streamWriter1.WriteLine("");
          }
        }
        if (!isGhost && (UnityEngine.Object) PlayerPatches.GetTextComponentByName(cardUi.gameObject, "TitleText") != (UnityEngine.Object) null)
        {
          TextMeshProUGUI textComponentByName = PlayerPatches.GetTextComponentByName(cardUi.gameObject, "TitleText");
          streamWriter1.WriteLine("Play Effect Text = " + textComponentByName.text);
          StreamWriter streamWriter27 = streamWriter1;
          flag3 = textComponentByName.enabled;
          string str26 = "Play Effect Text Enabled = " + flag3.ToString();
          streamWriter27.WriteLine(str26);
          StreamWriter streamWriter28 = streamWriter1;
          num = textComponentByName.fontSize;
          string str27 = "Play Effect Text Font Size = " + num.ToString();
          streamWriter28.WriteLine(str27);
          StreamWriter streamWriter29 = streamWriter1;
          num = textComponentByName.fontSizeMin;
          string str28 = "Play Effect Text Font Size Min = " + num.ToString();
          streamWriter29.WriteLine(str28);
          StreamWriter streamWriter30 = streamWriter1;
          num = textComponentByName.fontSizeMax;
          string str29 = "Play Effect Text Font Size Max = " + num.ToString();
          streamWriter30.WriteLine(str29);
          StreamWriter streamWriter31 = streamWriter1;
          color = textComponentByName.color;
          string str30 = "Play Effect Text Font Color RGBA = " + color.ToString();
          streamWriter31.WriteLine(str30);
          streamWriter1.WriteLine("Play Effect Text Position = " + textComponentByName.rectTransform.anchoredPosition.ToString());
          if ((UnityEngine.Object) PlayerPatches.GetImageComponentByName(cardUi.gameObject, "TitleBG") != (UnityEngine.Object) null)
          {
            StreamWriter streamWriter32 = streamWriter1;
            flag3 = PlayerPatches.GetImageComponentByName(cardUi.gameObject, "TitleBG").enabled;
            string str31 = "Play Effect Box Enabled = " + flag3.ToString();
            streamWriter32.WriteLine(str31);
          }
          streamWriter1.WriteLine("");
        }
        streamWriter1.WriteLine("Foil Text = " + LocalizationManager.GetTranslation("Foil"));
        if (isGhost | flag2)
        {
          streamWriter1.WriteLine("Rarity = " + LocalizationManager.GetTranslation("Legendary"));
          streamWriter1.WriteLine("Edition Text = " + LocalizationManager.GetTranslation("Full Art"));
        }
        if (!isGhost && !flag2)
        {
          if ((UnityEngine.Object) cardUi.m_RarityText != (UnityEngine.Object) null)
          {
            streamWriter1.WriteLine("Rarity = " + cardUi.m_RarityText.text);
            StreamWriter streamWriter33 = streamWriter1;
            flag3 = cardUi.m_RarityText.enabled;
            string str32 = "Rarity Enabled = " + flag3.ToString();
            streamWriter33.WriteLine(str32);
            StreamWriter streamWriter34 = streamWriter1;
            num = cardUi.m_RarityText.fontSize;
            string str33 = "Rarity Font Size = " + num.ToString();
            streamWriter34.WriteLine(str33);
            StreamWriter streamWriter35 = streamWriter1;
            num = cardUi.m_RarityText.fontSizeMin;
            string str34 = "Rarity Font Size Min = " + num.ToString();
            streamWriter35.WriteLine(str34);
            StreamWriter streamWriter36 = streamWriter1;
            num = cardUi.m_RarityText.fontSizeMax;
            string str35 = "Rarity Font Size Max = " + num.ToString();
            streamWriter36.WriteLine(str35);
            StreamWriter streamWriter37 = streamWriter1;
            color = cardUi.m_RarityText.color;
            string str36 = "Rarity Font Color RGBA = " + color.ToString();
            streamWriter37.WriteLine(str36);
            streamWriter1.WriteLine("Rarity Position = " + cardUi.m_RarityText.rectTransform.anchoredPosition.ToString());
            streamWriter1.WriteLine("");
          }
          if ((UnityEngine.Object) PlayerPatches.GetImageComponentByName(cardUi.gameObject, "RarityImage") != (UnityEngine.Object) null)
          {
            Image imageComponentByName = PlayerPatches.GetImageComponentByName(cardUi.gameObject, "RarityImage");
            StreamWriter streamWriter38 = streamWriter1;
            flag3 = imageComponentByName.enabled;
            string str37 = "Rarity Image Enabled = " + flag3.ToString();
            streamWriter38.WriteLine(str37);
          }
          streamWriter1.WriteLine("Basic Edition Text = " + LocalizationManager.GetTranslation("Basic"));
          streamWriter1.WriteLine("First Edition Text = " + LocalizationManager.GetTranslation("1st Edition"));
          streamWriter1.WriteLine("Gold Edition Text = " + LocalizationManager.GetTranslation("Gold Edition"));
          streamWriter1.WriteLine("Silver Edition Text = " + LocalizationManager.GetTranslation("Silver Edition"));
          streamWriter1.WriteLine("EX Edition Text = EX");
          StreamWriter streamWriter39 = streamWriter1;
          flag3 = true;
          string str38 = "Edition Text Enabled = " + flag3.ToString();
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
          if (cardData.expansionType == ECardExpansionType.FantasyRPG)
          {
            streamWriter1.WriteLine("Edition Text Position = " + new Vector2(0.0f, -7.5f).ToString());
            streamWriter1.WriteLine("");
          }
          else if (cardData.expansionType == ECardExpansionType.CatJob)
          {
            streamWriter1.WriteLine("Edition Text Position = " + new Vector2(0.0f, 30f).ToString());
            streamWriter1.WriteLine("");
          }
          else if (cardData.expansionType == ECardExpansionType.Megabot)
          {
            streamWriter1.WriteLine("Edition Text Position = " + new Vector2(0.0f, 17f).ToString());
            streamWriter1.WriteLine("");
          }
          else
          {
            streamWriter1.WriteLine("Edition Text Position = " + new Vector2(0.0f, 0.0f).ToString());
            streamWriter1.WriteLine("");
          }
        }
        if (!isGhost && (UnityEngine.Object) cardUi.m_ChampionText != (UnityEngine.Object) null)
        {
          streamWriter1.WriteLine("Champion Text = " + cardUi.m_ChampionText.text);
          StreamWriter streamWriter44 = streamWriter1;
          flag3 = cardUi.m_ChampionText.enabled;
          string str43 = "Champion Text Enabled = " + flag3.ToString();
          streamWriter44.WriteLine(str43);
          StreamWriter streamWriter45 = streamWriter1;
          num = cardUi.m_ChampionText.fontSize;
          string str44 = "Champion Font Size = " + num.ToString();
          streamWriter45.WriteLine(str44);
          StreamWriter streamWriter46 = streamWriter1;
          num = cardUi.m_ChampionText.fontSizeMin;
          string str45 = "Champion Font Size Min = " + num.ToString();
          streamWriter46.WriteLine(str45);
          StreamWriter streamWriter47 = streamWriter1;
          num = cardUi.m_ChampionText.fontSizeMax;
          string str46 = "Champion Font Size Max = " + num.ToString();
          streamWriter47.WriteLine(str46);
          StreamWriter streamWriter48 = streamWriter1;
          color = cardUi.m_ChampionText.color;
          string str47 = "Champion Font Color RGBA = " + color.ToString();
          streamWriter48.WriteLine(str47);
          streamWriter1.WriteLine("Champion Position = " + cardUi.m_ChampionText.rectTransform.anchoredPosition.ToString());
          streamWriter1.WriteLine("");
        }
        if (isGhost && (UnityEngine.Object) PlayerPatches.GetImageComponentByName(cardUi.gameObject, "CardStat") != (UnityEngine.Object) null)
        {
          StreamWriter streamWriter49 = streamWriter1;
          flag3 = PlayerPatches.GetImageComponentByName(cardUi.gameObject, "CardStat").enabled;
          string str48 = "Stat Background Image Enabled = " + flag3.ToString();
          streamWriter49.WriteLine(str48);
          streamWriter1.WriteLine("");
        }
        if ((UnityEngine.Object) cardUi.m_Stat1Text != (UnityEngine.Object) null)
        {
          streamWriter1.WriteLine("Stat1 = " + cardUi.m_Stat1Text.text);
          StreamWriter streamWriter50 = streamWriter1;
          flag3 = cardUi.m_Stat1Text.enabled;
          string str49 = "Stat1 Enabled = " + flag3.ToString();
          streamWriter50.WriteLine(str49);
          StreamWriter streamWriter51 = streamWriter1;
          num = cardUi.m_Stat1Text.fontSize;
          string str50 = "Stat1 Font Size = " + num.ToString();
          streamWriter51.WriteLine(str50);
          StreamWriter streamWriter52 = streamWriter1;
          num = cardUi.m_Stat1Text.fontSizeMin;
          string str51 = "Stat1 Font Size Min = " + num.ToString();
          streamWriter52.WriteLine(str51);
          StreamWriter streamWriter53 = streamWriter1;
          num = cardUi.m_Stat1Text.fontSizeMax;
          string str52 = "Stat1 Font Size Max = " + num.ToString();
          streamWriter53.WriteLine(str52);
          StreamWriter streamWriter54 = streamWriter1;
          color = cardUi.m_Stat1Text.color;
          string str53 = "Stat1 Font Color RGBA = " + color.ToString();
          streamWriter54.WriteLine(str53);
          streamWriter1.WriteLine("Stat1 Position = " + cardUi.m_Stat1Text.rectTransform.anchoredPosition.ToString());
          streamWriter1.WriteLine("");
        }
        if ((UnityEngine.Object) cardUi.m_Stat2Text != (UnityEngine.Object) null)
        {
          streamWriter1.WriteLine("Stat2 = " + cardUi.m_Stat2Text.text);
          StreamWriter streamWriter55 = streamWriter1;
          flag3 = cardUi.m_Stat2Text.enabled;
          string str54 = "Stat2 Enabled = " + flag3.ToString();
          streamWriter55.WriteLine(str54);
          StreamWriter streamWriter56 = streamWriter1;
          num = cardUi.m_Stat2Text.fontSize;
          string str55 = "Stat2 Font Size = " + num.ToString();
          streamWriter56.WriteLine(str55);
          StreamWriter streamWriter57 = streamWriter1;
          num = cardUi.m_Stat2Text.fontSizeMin;
          string str56 = "Stat2 Font Size Min = " + num.ToString();
          streamWriter57.WriteLine(str56);
          StreamWriter streamWriter58 = streamWriter1;
          num = cardUi.m_Stat2Text.fontSizeMax;
          string str57 = "Stat2 Font Size Max = " + num.ToString();
          streamWriter58.WriteLine(str57);
          StreamWriter streamWriter59 = streamWriter1;
          color = cardUi.m_Stat2Text.color;
          string str58 = "Stat2 Font Color RGBA = " + color.ToString();
          streamWriter59.WriteLine(str58);
          streamWriter1.WriteLine("Stat2 Position = " + cardUi.m_Stat2Text.rectTransform.anchoredPosition.ToString());
          streamWriter1.WriteLine("");
        }
        if ((UnityEngine.Object) cardUi.m_Stat3Text != (UnityEngine.Object) null)
        {
          streamWriter1.WriteLine("Stat3 = " + cardUi.m_Stat3Text.text);
          StreamWriter streamWriter60 = streamWriter1;
          num = cardUi.m_Stat3Text.fontSize;
          string str59 = "Stat3 Font Size = " + num.ToString();
          streamWriter60.WriteLine(str59);
          StreamWriter streamWriter61 = streamWriter1;
          flag3 = cardUi.m_Stat3Text.enabled;
          string str60 = "Stat3 Enabled = " + flag3.ToString();
          streamWriter61.WriteLine(str60);
          StreamWriter streamWriter62 = streamWriter1;
          num = cardUi.m_Stat3Text.fontSizeMin;
          string str61 = "Stat3 Font Size Min = " + num.ToString();
          streamWriter62.WriteLine(str61);
          StreamWriter streamWriter63 = streamWriter1;
          num = cardUi.m_Stat3Text.fontSizeMax;
          string str62 = "Stat3 Font Size Max = " + num.ToString();
          streamWriter63.WriteLine(str62);
          StreamWriter streamWriter64 = streamWriter1;
          color = cardUi.m_Stat3Text.color;
          string str63 = "Stat3 Font Color RGBA = " + color.ToString();
          streamWriter64.WriteLine(str63);
          streamWriter1.WriteLine("Stat3 Position = " + cardUi.m_Stat3Text.rectTransform.anchoredPosition.ToString());
          streamWriter1.WriteLine("");
        }
        if ((UnityEngine.Object) cardUi.m_Stat4Text != (UnityEngine.Object) null)
        {
          streamWriter1.WriteLine("Stat4 = " + cardUi.m_Stat4Text.text);
          StreamWriter streamWriter65 = streamWriter1;
          flag3 = cardUi.m_Stat4Text.enabled;
          string str64 = "Stat4 Enabled = " + flag3.ToString();
          streamWriter65.WriteLine(str64);
          StreamWriter streamWriter66 = streamWriter1;
          num = cardUi.m_Stat4Text.fontSize;
          string str65 = "Stat4 Font Size = " + num.ToString();
          streamWriter66.WriteLine(str65);
          StreamWriter streamWriter67 = streamWriter1;
          num = cardUi.m_Stat4Text.fontSizeMin;
          string str66 = "Stat4 Font Size Min = " + num.ToString();
          streamWriter67.WriteLine(str66);
          StreamWriter streamWriter68 = streamWriter1;
          num = cardUi.m_Stat4Text.fontSizeMax;
          string str67 = "Stat4 Font Size Max = " + num.ToString();
          streamWriter68.WriteLine(str67);
          StreamWriter streamWriter69 = streamWriter1;
          color = cardUi.m_Stat4Text.color;
          string str68 = "Stat4 Font Color RGBA = " + color.ToString();
          streamWriter69.WriteLine(str68);
          streamWriter1.WriteLine("Stat4 Position = " + cardUi.m_Stat4Text.rectTransform.anchoredPosition.ToString());
          streamWriter1.WriteLine("");
        }
        if ((UnityEngine.Object) cardUi.m_ArtistText != (UnityEngine.Object) null)
        {
          streamWriter1.WriteLine("Artist Text = " + cardUi.m_ArtistText.text);
          StreamWriter streamWriter70 = streamWriter1;
          flag3 = cardUi.m_ArtistText.enabled;
          string str69 = "Artist Text Enabled = " + flag3.ToString();
          streamWriter70.WriteLine(str69);
          StreamWriter streamWriter71 = streamWriter1;
          num = cardUi.m_ArtistText.fontSize;
          string str70 = "Artist Text Font Size = " + num.ToString();
          streamWriter71.WriteLine(str70);
          StreamWriter streamWriter72 = streamWriter1;
          num = cardUi.m_ArtistText.fontSizeMin;
          string str71 = "Artist Text Font Size Min = " + num.ToString();
          streamWriter72.WriteLine(str71);
          StreamWriter streamWriter73 = streamWriter1;
          num = cardUi.m_ArtistText.fontSizeMax;
          string str72 = "Artist Text Font Size Max = " + num.ToString();
          streamWriter73.WriteLine(str72);
          StreamWriter streamWriter74 = streamWriter1;
          color = cardUi.m_ArtistText.color;
          string str73 = "Artist Text Font Color RGBA = " + color.ToString();
          streamWriter74.WriteLine(str73);
          streamWriter1.WriteLine("Artist Text Position = " + cardUi.m_ArtistText.rectTransform.anchoredPosition.ToString());
          streamWriter1.WriteLine("");
        }
        if ((UnityEngine.Object) PlayerPatches.GetTextComponentByName(cardUi.gameObject, "CompanyText") != (UnityEngine.Object) null)
        {
          TextMeshProUGUI textComponentByName = PlayerPatches.GetTextComponentByName(cardUi.gameObject, "CompanyText");
          streamWriter1.WriteLine("Company Text = " + textComponentByName.text);
          StreamWriter streamWriter75 = streamWriter1;
          flag3 = textComponentByName.enabled;
          string str74 = "Company Text Enabled = " + flag3.ToString();
          streamWriter75.WriteLine(str74);
          StreamWriter streamWriter76 = streamWriter1;
          num = textComponentByName.fontSize;
          string str75 = "Company Text Font Size = " + num.ToString();
          streamWriter76.WriteLine(str75);
          StreamWriter streamWriter77 = streamWriter1;
          num = textComponentByName.fontSizeMin;
          string str76 = "Company Text Font Size Min = " + num.ToString();
          streamWriter77.WriteLine(str76);
          StreamWriter streamWriter78 = streamWriter1;
          num = textComponentByName.fontSizeMax;
          string str77 = "Company Text Font Size Max = " + num.ToString();
          streamWriter78.WriteLine(str77);
          StreamWriter streamWriter79 = streamWriter1;
          color = textComponentByName.color;
          string str78 = "Company Text Font Color RGBA = " + color.ToString();
          streamWriter79.WriteLine(str78);
          streamWriter1.WriteLine("Company Text Position = " + textComponentByName.rectTransform.anchoredPosition.ToString());
          streamWriter1.WriteLine("");
        }
        if ((UnityEngine.Object) cardUi.m_MonsterMask != (UnityEngine.Object) null)
        {
          if (cardUi.m_MonsterMask.enabled)
          {
            StreamWriter streamWriter80 = streamWriter1;
            flag3 = false;
            string str79 = "Remove Monster Image Size Limit = " + flag3.ToString();
            streamWriter80.WriteLine(str79);
          }
          else
          {
            StreamWriter streamWriter81 = streamWriter1;
            flag3 = true;
            string str80 = "Remove Monster Image Size Limit = " + flag3.ToString();
            streamWriter81.WriteLine(str80);
          }
        }
        if (!((UnityEngine.Object) cardUi.m_MonsterImage != (UnityEngine.Object) null))
          return;
        StreamWriter streamWriter82 = streamWriter1;
        Vector2 vector2 = cardUi.m_MonsterImage.rectTransform.sizeDelta;
        string str81 = "Monster Image Size = " + vector2.ToString();
        streamWriter82.WriteLine(str81);
        StreamWriter streamWriter83 = streamWriter1;
        vector2 = cardUi.m_MonsterImage.rectTransform.anchoredPosition;
        string str82 = "Monster Image Position = " + vector2.ToString();
        streamWriter83.WriteLine(str82);
      }
    }

    public static void WriteAllFullExpansionConfigs()
    {
      string path1 = PlayerPatches.configPath + "TetramonConfigs/";
      string path2 = PlayerPatches.configPath + "DestinyConfigs/";
      string path3 = PlayerPatches.configPath + "GhostConfigs/";
      string path4 = PlayerPatches.configPath + "MegabotConfigs/";
      string path5 = PlayerPatches.configPath + "CatJobConfigs/";
      string path6 = PlayerPatches.configPath + "FantasyRPGConfigs/";
      string[] files1 = Directory.GetFiles(path1, "*.ini");
      string[] files2 = Directory.GetFiles(path2, "*.ini");
      string[] files3 = Directory.GetFiles(path3, "*.ini");
      string[] files4 = Directory.GetFiles(path4, "*.ini");
      string[] files5 = Directory.GetFiles(path5, "*.ini");
      string[] files6 = Directory.GetFiles(path6, "*.ini");
      string str1 = (string) null;
      string str2 = (string) null;
      string str3 = (string) null;
      string str4 = (string) null;
      string str5 = (string) null;
      string str6 = (string) null;
      string str7 = (string) null;
      string str8 = (string) null;
      string str9 = (string) null;
      string str10 = (string) null;
      string str11 = (string) null;
      string str12 = (string) null;
      if (files1.Length != 0)
      {
        foreach (string path7 in files1)
        {
          if (!path7.Contains("FullArt", StringComparison.OrdinalIgnoreCase))
          {
            str1 = Path.GetFileName(path7);
            break;
          }
        }
        foreach (string path8 in files1)
        {
          if (path8.Contains("FullArt", StringComparison.OrdinalIgnoreCase))
          {
            str7 = Path.GetFileName(path8);
            break;
          }
        }
      }
      if (files2.Length != 0)
      {
        foreach (string path9 in files2)
        {
          if (!path9.Contains("FullArt", StringComparison.OrdinalIgnoreCase))
          {
            str2 = Path.GetFileName(path9);
            break;
          }
        }
        foreach (string path10 in files2)
        {
          if (path10.Contains("FullArt", StringComparison.OrdinalIgnoreCase))
          {
            str8 = Path.GetFileName(path10);
            break;
          }
        }
      }
      if (files3.Length != 0)
      {
        foreach (string path11 in files3)
        {
          if (!path11.Contains("FullArt", StringComparison.OrdinalIgnoreCase))
          {
            str3 = Path.GetFileName(path11);
            break;
          }
        }
        foreach (string path12 in files3)
        {
          if (path12.Contains("FullArt", StringComparison.OrdinalIgnoreCase))
          {
            str9 = Path.GetFileName(path12);
            break;
          }
        }
      }
      if (files4.Length != 0)
      {
        foreach (string path13 in files4)
        {
          if (!path13.Contains("FullArt", StringComparison.OrdinalIgnoreCase))
          {
            str4 = Path.GetFileName(path13);
            break;
          }
        }
        foreach (string path14 in files4)
        {
          if (path14.Contains("FullArt", StringComparison.OrdinalIgnoreCase))
          {
            str10 = Path.GetFileName(path14);
            break;
          }
        }
      }
      if (files5.Length != 0)
      {
        foreach (string path15 in files5)
        {
          if (!path15.Contains("FullArt", StringComparison.OrdinalIgnoreCase))
          {
            str5 = Path.GetFileName(path15);
            break;
          }
        }
        foreach (string path16 in files5)
        {
          if (path16.Contains("FullArt", StringComparison.OrdinalIgnoreCase))
          {
            str11 = Path.GetFileName(path16);
            break;
          }
        }
      }
      if (files6.Length != 0)
      {
        foreach (string path17 in files6)
        {
          if (!path17.Contains("FullArt", StringComparison.OrdinalIgnoreCase))
          {
            str6 = Path.GetFileName(path17);
            break;
          }
        }
        foreach (string path18 in files6)
        {
          if (path18.Contains("FullArt", StringComparison.OrdinalIgnoreCase))
          {
            str12 = Path.GetFileName(path18);
            break;
          }
        }
      }
      string outputPath = PlayerPatches.configPath + "FullExpansionsConfigs/";
      string[] sectionsToRemove = new string[11]
      {
        "Name = ",
        "Description = ",
        "Individual Overrides = ",
        "Number = ",
        "Previous Evolution = ",
        "Play Effect Text = ",
        "Rarity = ",
        "Stat1 = ",
        "Stat2 = ",
        "Stat3 = ",
        "Stat4 = "
      };
      if (!File.Exists(outputPath + "Tetramon.ini"))
      {
        PlayerPatches.Log("File doesn't exist -- " + outputPath + "Tetramon.ini");
        if (str1 != null)
          PlayerPatches.WriteFullExpansionConfig(path1 + str1, "Tetramon", outputPath, "Tetramon.ini", sectionsToRemove);
      }
      if (!File.Exists(outputPath + "TetramonFullArt.ini"))
      {
        PlayerPatches.Log("File doesn't exist -- " + outputPath + "TetramonFullArt.ini");
        if (str7 != null)
          PlayerPatches.WriteFullExpansionConfig(path1 + str7, "TetramonFullArt", outputPath, "TetramonFullArt.ini", sectionsToRemove);
      }
      if (!File.Exists(outputPath + "Destiny.ini") && str2 != null)
        PlayerPatches.WriteFullExpansionConfig(path2 + str2, "Destiny", outputPath, "Destiny.ini", sectionsToRemove);
      if (!File.Exists(outputPath + "DestinyFullArt.ini") && str8 != null)
        PlayerPatches.WriteFullExpansionConfig(path2 + str8, "DestinyFullArt", outputPath, "DestinyFullArt.ini", sectionsToRemove);
      if (!File.Exists(outputPath + "Ghost.ini") && str3 != null)
        PlayerPatches.WriteFullExpansionConfig(path3 + str3, "Ghost", outputPath, "Ghost.ini", sectionsToRemove);
      if (!File.Exists(outputPath + "GhostFullArt.ini") && str9 != null)
        PlayerPatches.WriteFullExpansionConfig(path3 + str9, "GhostFullArt", outputPath, "GhostFullArt.ini", sectionsToRemove);
      if (!File.Exists(outputPath + "Megabot.ini") && str4 != null)
        PlayerPatches.WriteFullExpansionConfig(path4 + str4, "Megabot", outputPath, "Megabot.ini", sectionsToRemove);
      if (!File.Exists(outputPath + "MegabotFullArt.ini") && str10 != null)
        PlayerPatches.WriteFullExpansionConfig(path4 + str10, "MegabotFullArt", outputPath, "MegabotFullArt.ini", sectionsToRemove);
      if (!File.Exists(outputPath + "CatJob.ini") && str5 != null)
        PlayerPatches.WriteFullExpansionConfig(path5 + str5, "CatJob", outputPath, "CatJob.ini", sectionsToRemove);
      if (!File.Exists(outputPath + "CatJobFullArt.ini") && str11 != null)
        PlayerPatches.WriteFullExpansionConfig(path5 + str11, "CatJobFullArt", outputPath, "CatJobFullArt.ini", sectionsToRemove);
      if (!File.Exists(outputPath + "FantasyRPG.ini") && str6 != null)
        PlayerPatches.WriteFullExpansionConfig(path6 + str6, "FantasyRPG", outputPath, "FantasyRPG.ini", sectionsToRemove);
      if (File.Exists(outputPath + "FantasyRPGFullArt.ini") || str12 == null)
        return;
      PlayerPatches.WriteFullExpansionConfig(path6 + str12, "FantasyRPGFullArt", outputPath, "FantasyRPGFullArt.ini", sectionsToRemove);
    }

    public static void WriteFullExpansionConfig(
      string inputPath,
      string newSectionName,
      string outputPath,
      string outputName,
      string[] sectionsToRemove)
    {
      string path = Path.Combine(outputPath, outputName);
      string[] strArray = File.ReadAllLines(inputPath);
      using (StreamWriter streamWriter = new StreamWriter(path))
      {
        bool flag1 = false;
        bool flag2 = false;
        foreach (string str in strArray)
        {
          string line = str;
          if (line.StartsWith("[") && line.EndsWith("]"))
          {
            if (!flag1)
            {
              streamWriter.WriteLine("[" + newSectionName + "]");
              flag1 = true;
              flag2 = false;
            }
          }
          else if (!Array.Exists<string>(sectionsToRemove, (Predicate<string>) (section => line.StartsWith(section, StringComparison.OrdinalIgnoreCase))))
          {
            bool flag3 = string.IsNullOrWhiteSpace(line);
            if (!flag3 || !flag2)
            {
              streamWriter.WriteLine(line);
              flag2 = flag3;
            }
          }
        }
      }
    }
    public static TextMeshProUGUI GetTextComponentByName(GameObject gameObject, string name)
    {
      if ((UnityEngine.Object) gameObject != (UnityEngine.Object) null)
      {
        foreach (TextMeshProUGUI componentsInChild in gameObject.GetComponentsInChildren<TextMeshProUGUI>())
        {
          if ((UnityEngine.Object) componentsInChild != (UnityEngine.Object) null && componentsInChild.name != null && componentsInChild.name == name)
            return componentsInChild;
        }
      }
      return (TextMeshProUGUI) null;
    }

    public static Image GetImageComponentByName(GameObject gameObject, string name)
    {
      if ((UnityEngine.Object) gameObject != (UnityEngine.Object) null)
      {
        foreach (Image componentsInChild in gameObject.GetComponentsInChildren<Image>())
        {
          if ((UnityEngine.Object) componentsInChild != (UnityEngine.Object) null && componentsInChild.name == name)
            return componentsInChild;
        }
      }
      return (Image) null;
    }

    public static Image GetImageComponentBySpriteName(GameObject gameObject, string name)
    {
      if ((UnityEngine.Object) gameObject != (UnityEngine.Object) null)
      {
        foreach (Image componentsInChild in gameObject.GetComponentsInChildren<Image>())
        {
          if ((UnityEngine.Object) componentsInChild != (UnityEngine.Object) null && componentsInChild.name != null && (UnityEngine.Object) componentsInChild.sprite != (UnityEngine.Object) null && componentsInChild.sprite.name != null && componentsInChild.sprite.name == name)
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
    [HarmonyPatch(typeof (CollectionBinderFlipAnimCtrl), "EnterViewUpCloseState")]
    public static void CollectionBinderFlipAnimCtrl_EnterViewUpCloseState_Postfix(
      CollectionBinderFlipAnimCtrl __instance)
    {
      if (!MinaCardsModPlugin.CustomConfigs.Value)
        return;
      CardData cardData = __instance.m_CurrentViewInteractableCard3d.m_Card3dUI.m_CardUI.GetCardData();
      bool isGhost = cardData.expansionType == ECardExpansionType.Ghost;
      bool isFoil = cardData.isFoil;
      if (__instance.m_CollectionBinderUI.m_CardNameText.text != __instance.m_CurrentSpawnedInteractableCard3d.m_Card3dUI.m_CardUI.m_MonsterNameText.text)
        __instance.m_CollectionBinderUI.m_CardNameText.text = __instance.m_CurrentSpawnedInteractableCard3d.m_Card3dUI.m_CardUI.m_MonsterNameText.text;
      CardUI cardUi1 = __instance.m_CurrentViewInteractableCard3d.m_Card3dUI.m_CardUI;
      CardUI cardUi2 = !isGhost || !((UnityEngine.Object) cardUi1.m_GhostCard != (UnityEngine.Object) null) ? __instance.m_CurrentViewInteractableCard3d.m_Card3dUI.m_CardUI : PlayerPatches.CurrentCardUI(isGhost, cardUi1, cardUi1.m_GhostCard);
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

    [HarmonyPostfix]
    [HarmonyPatch(typeof (CardUI), "SetCardUI")]
    public static void CardUI_SetCardUI_Extras_Postfix(CardUI __instance, CardData cardData)
    {
      if (!MinaCardsModPlugin.CustomConfigs.Value)
        return;
      bool redirect = false;
      string configFilePath = PlayerPatches.findConfigFilePath(cardData);
      string str1 = PlayerPatches.configPath + "FullExpansionsConfigs/" + cardData.expansionType.ToString();
      bool isGhost = cardData.expansionType == ECardExpansionType.Ghost;
      bool flag;
      string str2;
      string stringRedirect;
      if (cardData.borderType == ECardBorderType.FullArt)
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
        CardUI cardUi1 = !isGhost || !((UnityEngine.Object)__instance.m_GhostCard != (UnityEngine.Object)null) ? __instance : PlayerPatches.CurrentCardUI(isGhost, __instance, __instance.m_GhostCard);
        cardUi1.m_MonsterNameText.text = IniFile.GetStringValue(str2, "Name");

        FieldInfo monsterDataField = typeof(CardUI).GetField("m_MonsterData", BindingFlags.NonPublic | BindingFlags.Instance);
        object monsterData = monsterDataField?.GetValue(cardUi1);

        if (!isGhost && monsterData != null)
        {
          cardUi1.m_DescriptionText.text = IniFile.GetStringValue(str2, "Description");
          if (flag && (UnityEngine.Object)__instance.m_FullArtCard != (UnityEngine.Object)null && (UnityEngine.Object)__instance.m_FullArtCard.m_DescriptionText != (UnityEngine.Object)null && __instance.m_FullArtCard.m_DescriptionText.text != null)
          {
            __instance.m_FullArtCard.m_DescriptionText.text = IniFile.GetStringValue(str2, "Description");
          }
          FieldInfo previousEvolutionField = monsterData.GetType().GetField("PreviousEvolution", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
          if (previousEvolutionField != null)
          {
            EMonsterType result;
            if (Enum.TryParse<EMonsterType>(IniFile.GetStringValue(str2, "Previous Evolution"), out result))
            {
              previousEvolutionField.SetValue(monsterData, result);
            }
          }
          cardUi1.m_ChampionText.text = IniFile.GetStringValueRedirect(redirect, section, "Champion Text");
          cardUi1.m_DescriptionText.enabled = IniFile.GetBoolValueRedirect(redirect, section, "Description Enabled");
          cardUi1.m_DescriptionText.fontSize = IniFile.GetFloatValueRedirect(redirect, section, "Description Font Size");
          cardUi1.m_DescriptionText.fontSizeMin = IniFile.GetFloatValueRedirect(redirect, section, "Description Font Size Min");
          cardUi1.m_DescriptionText.fontSizeMax = IniFile.GetFloatValueRedirect(redirect, section, "Description Font Size Max");
          cardUi1.m_DescriptionText.color = IniFile.GetColorValueRedirect(redirect, section, "Description Font Color RGBA");
          cardUi1.m_DescriptionText.rectTransform.anchoredPosition = IniFile.GetVector2ValueRedirect(redirect, section, "Description Position");
          cardUi1.m_EvoPreviousStageIcon.enabled = IniFile.GetBoolValueRedirect(redirect, section, "Previous Evolution Icon Enabled");
          cardUi1.m_EvoPreviousStageNameText.enabled = IniFile.GetBoolValueRedirect(redirect, section, "Previous Evolution Name Enabled");
          if (previousEvolutionField != null && (EMonsterType)previousEvolutionField.GetValue(monsterData) == EMonsterType.None)
          {
            if ((UnityEngine.Object) PlayerPatches.GetImageComponentByName(cardUi1.gameObject, "EvoBasicIcon") != (UnityEngine.Object) null)
              PlayerPatches.GetImageComponentByName(cardUi1.gameObject, "EvoBasicIcon").enabled = IniFile.GetBoolValueRedirect(redirect, section, "Basic Evolution Icon Enabled");
            if ((UnityEngine.Object) PlayerPatches.GetTextComponentByName(cardUi1.gameObject, "EvoBasicText") != (UnityEngine.Object) null)
            {
              TextMeshProUGUI textComponentByName = PlayerPatches.GetTextComponentByName(cardUi1.gameObject, "EvoBasicText");
              textComponentByName.text = IniFile.GetStringValueRedirect(redirect, section, "Basic Evolution Text");
              textComponentByName.enabled = IniFile.GetBoolValueRedirect(redirect, section, "Basic Evolution Text Enabled");
              textComponentByName.fontSize = IniFile.GetFloatValueRedirect(redirect, section, "Basic Evolution Text Font Size");
              textComponentByName.fontSizeMin = IniFile.GetFloatValueRedirect(redirect, section, "Basic Evolution Text Font Size Min");
              textComponentByName.fontSizeMax = IniFile.GetFloatValueRedirect(redirect, section, "Basic Evolution Text Font Size Max");
              textComponentByName.color = IniFile.GetColorValueRedirect(redirect, section, "Basic Evolution Text Font Color RGBA");
              textComponentByName.rectTransform.anchoredPosition = IniFile.GetVector2ValueRedirect(redirect, section, "Basic Evolution Text Position");
            }
          }
          cardUi1.m_EvoPreviousStageNameText.fontSize = IniFile.GetFloatValueRedirect(redirect, section, "Previous Evolution Name Font Size");
          cardUi1.m_EvoPreviousStageNameText.fontSizeMin = IniFile.GetFloatValueRedirect(redirect, section, "Previous Evolution Name Font Size Min");
          cardUi1.m_EvoPreviousStageNameText.fontSizeMax = IniFile.GetFloatValueRedirect(redirect, section, "Previous Evolution Name Font Size Max");
          cardUi1.m_EvoPreviousStageNameText.color = IniFile.GetColorValueRedirect(redirect, section, "Previous Evolution Name Font Color RGBA");
          cardUi1.m_EvoPreviousStageNameText.rectTransform.anchoredPosition = IniFile.GetVector2ValueRedirect(redirect, section, "Previous Evolution Name Position");
        }
        if (!flag)
          cardUi1.m_NumberText.text = IniFile.GetStringValue(str2, "Number");
        if ((UnityEngine.Object) PlayerPatches.GetImageComponentByName(cardUi1.gameObject, "EvoBG") != (UnityEngine.Object) null)
          PlayerPatches.GetImageComponentByName(cardUi1.gameObject, "EvoBG").enabled = IniFile.GetBoolValueRedirect(redirect, section, "Previous Evolution Box Enabled");
        if ((UnityEngine.Object) PlayerPatches.GetImageComponentByName(cardUi1.gameObject, "EvoBorder") != (UnityEngine.Object) null)
          PlayerPatches.GetImageComponentByName(cardUi1.gameObject, "EvoBorder").enabled = IniFile.GetBoolValueRedirect(redirect, section, "Previous Evolution Box Enabled");
        if ((UnityEngine.Object) PlayerPatches.GetTextComponentByName(cardUi1.gameObject, "TitleText") != (UnityEngine.Object) null)
        {
          TextMeshProUGUI textComponentByName = PlayerPatches.GetTextComponentByName(cardUi1.gameObject, "TitleText");
          textComponentByName.text = IniFile.GetStringValue(str2, "Play Effect Text");
          textComponentByName.enabled = IniFile.GetBoolValueRedirect(redirect, section, "Play Effect Text Enabled");
          textComponentByName.fontSize = IniFile.GetFloatValueRedirect(redirect, section, "Play Effect Text Font Size");
          textComponentByName.fontSizeMin = IniFile.GetFloatValueRedirect(redirect, section, "Play Effect Text Font Size Min");
          textComponentByName.fontSizeMax = IniFile.GetFloatValueRedirect(redirect, section, "Play Effect Text Font Size Max");
          textComponentByName.color = IniFile.GetColorValueRedirect(redirect, section, "Play Effect Text Font Color RGBA");
          textComponentByName.rectTransform.anchoredPosition = IniFile.GetVector2ValueRedirect(redirect, section, "Play Effect Text Position");
        }
        if ((UnityEngine.Object) PlayerPatches.GetTextComponentByName(cardUi1.gameObject, "EvoText") != (UnityEngine.Object) null)
          PlayerPatches.GetTextComponentByName(cardUi1.gameObject, "EvoText").enabled = IniFile.GetBoolValueRedirect(redirect, section, "Previous Evolution Name Enabled");
        if ((UnityEngine.Object) PlayerPatches.GetImageComponentByName(cardUi1.gameObject, "TitleBG") != (UnityEngine.Object) null)
          PlayerPatches.GetImageComponentByName(cardUi1.gameObject, "TitleBG").enabled = IniFile.GetBoolValueRedirect(redirect, section, "Play Effect Box Enabled");
        if (isGhost | flag && (UnityEngine.Object) cardUi1.m_FirstEditionText != (UnityEngine.Object) null && cardUi1.m_FirstEditionText.text != null)
          cardUi1.m_FirstEditionText.text = IniFile.GetStringValueRedirect(redirect, section, "Edition Text");
        if ((UnityEngine.Object) cardUi1.m_FirstEditionText != (UnityEngine.Object) null && !isGhost && !flag && cardUi1.m_FirstEditionText.text != null)
        {
          switch (cardData.borderType)
          {
            case ECardBorderType.Base:
              cardUi1.m_FirstEditionText.text = IniFile.GetStringValueRedirect(redirect, section, "Basic Edition Text");
              break;
            case ECardBorderType.FirstEdition:
              cardUi1.m_FirstEditionText.text = IniFile.GetStringValueRedirect(redirect, section, "First Edition Text");
              break;
            case ECardBorderType.Silver:
              cardUi1.m_FirstEditionText.text = IniFile.GetStringValueRedirect(redirect, section, "Silver Edition Text");
              break;
            case ECardBorderType.Gold:
              cardUi1.m_FirstEditionText.text = IniFile.GetStringValueRedirect(redirect, section, "Gold Edition Text");
              break;
            case ECardBorderType.EX:
              cardUi1.m_FirstEditionText.text = IniFile.GetStringValueRedirect(redirect, section, "EX Edition Text");
              break;
          }
        }
        cardUi1.m_MonsterNameText.enabled = IniFile.GetBoolValueRedirect(redirect, section, "Name Enabled");
        cardUi1.m_MonsterNameText.fontSize = IniFile.GetFloatValueRedirect(redirect, section, "Name Font Size");
        cardUi1.m_MonsterNameText.fontSizeMin = IniFile.GetFloatValueRedirect(redirect, section, "Name Font Size Min");
        cardUi1.m_MonsterNameText.fontSizeMax = IniFile.GetFloatValueRedirect(redirect, section, "Name Font Size Max");
        cardUi1.m_MonsterNameText.color = IniFile.GetColorValueRedirect(redirect, section, "Name Font Color RGBA");
        cardUi1.m_MonsterNameText.rectTransform.anchoredPosition = IniFile.GetVector2ValueRedirect(redirect, section, "Name Position");
        if (!isGhost && !flag)
        {
          if ((UnityEngine.Object) cardUi1.m_FirstEditionText != (UnityEngine.Object) null && cardUi1.m_FirstEditionText.text != null)
          {
            cardUi1.m_FirstEditionText.fontSize = IniFile.GetFloatValueRedirect(redirect, section, "Edition Text Font Size");
            cardUi1.m_FirstEditionText.fontSizeMin = IniFile.GetFloatValueRedirect(redirect, section, "Edition Text Font Size Min");
            cardUi1.m_FirstEditionText.fontSizeMax = IniFile.GetFloatValueRedirect(redirect, section, "Edition Text Font Size Max");
            cardUi1.m_FirstEditionText.color = IniFile.GetColorValueRedirect(redirect, section, "Edition Text Font Color RGBA");
            cardUi1.m_FirstEditionText.rectTransform.anchoredPosition = IniFile.GetVector2ValueRedirect(redirect, section, "Edition Text Position");
          }
          cardUi1.m_RarityText.text = IniFile.GetStringValue(str2, "Rarity");
          cardUi1.m_NumberText.enabled = IniFile.GetBoolValueRedirect(redirect, section, "Number Enabled");
          cardUi1.m_NumberText.fontSize = IniFile.GetFloatValueRedirect(redirect, section, "Number Font Size");
          cardUi1.m_NumberText.fontSizeMin = IniFile.GetFloatValueRedirect(redirect, section, "Number Font Size Min");
          cardUi1.m_NumberText.fontSizeMax = IniFile.GetFloatValueRedirect(redirect, section, "Number Font Size Max");
          cardUi1.m_NumberText.color = IniFile.GetColorValueRedirect(redirect, section, "Number Font Color RGBA");
          cardUi1.m_NumberText.rectTransform.anchoredPosition = IniFile.GetVector2ValueRedirect(redirect, section, "Number Position");
          cardUi1.m_FirstEditionText.enabled = IniFile.GetBoolValueRedirect(redirect, section, "Edition Text Enabled");
          cardUi1.m_RarityText.enabled = IniFile.GetBoolValueRedirect(redirect, section, "Rarity Enabled");
          cardUi1.m_RarityText.fontSize = IniFile.GetFloatValueRedirect(redirect, section, "Rarity Font Size");
          cardUi1.m_RarityText.fontSizeMin = IniFile.GetFloatValueRedirect(redirect, section, "Rarity Font Size Min");
          cardUi1.m_RarityText.fontSizeMax = IniFile.GetFloatValueRedirect(redirect, section, "Rarity Font Size Max");
          cardUi1.m_RarityText.color = IniFile.GetColorValueRedirect(redirect, section, "Rarity Font Color RGBA");
          cardUi1.m_RarityText.rectTransform.anchoredPosition = IniFile.GetVector2ValueRedirect(redirect, section, "Rarity Position");
        }
        if ((UnityEngine.Object) PlayerPatches.GetImageComponentByName(cardUi1.gameObject, "RarityImage") != (UnityEngine.Object) null)
          PlayerPatches.GetImageComponentByName(cardUi1.gameObject, "RarityImage").enabled = IniFile.GetBoolValueRedirect(redirect, section, "Rarity Image Enabled");
        if (!isGhost)
        {
          cardUi1.m_ChampionText.enabled = IniFile.GetBoolValueRedirect(redirect, section, "Champion Text Enabled");
          cardUi1.m_ChampionText.fontSize = IniFile.GetFloatValueRedirect(redirect, section, "Champion Font Size");
          cardUi1.m_ChampionText.fontSizeMin = IniFile.GetFloatValueRedirect(redirect, section, "Champion Font Size Min");
          cardUi1.m_ChampionText.fontSizeMax = IniFile.GetFloatValueRedirect(redirect, section, "Champion Font Size Max");
          cardUi1.m_ChampionText.color = IniFile.GetColorValueRedirect(redirect, section, "Champion Font Color RGBA");
          cardUi1.m_ChampionText.rectTransform.anchoredPosition = IniFile.GetVector2ValueRedirect(redirect, section, "Champion Position");
        }
        if (isGhost && (UnityEngine.Object) PlayerPatches.GetImageComponentByName(cardUi1.gameObject, "CardStat") != (UnityEngine.Object) null)
          PlayerPatches.GetImageComponentByName(cardUi1.gameObject, "CardStat").enabled = IniFile.GetBoolValue(str2, "Stat Background Image Enabled");
        cardUi1.m_Stat1Text.text = IniFile.GetStringValue(str2, "Stat1");
        cardUi1.m_Stat1Text.enabled = IniFile.GetBoolValueRedirect(redirect, section, "Stat1 Enabled");
        cardUi1.m_Stat1Text.fontSize = IniFile.GetFloatValueRedirect(redirect, section, "Stat1 Font Size");
        cardUi1.m_Stat1Text.fontSizeMin = IniFile.GetFloatValueRedirect(redirect, section, "Stat1 Font Size Min");
        cardUi1.m_Stat1Text.fontSizeMax = IniFile.GetFloatValueRedirect(redirect, section, "Stat1 Font Size Max");
        cardUi1.m_Stat1Text.color = IniFile.GetColorValueRedirect(redirect, section, "Stat1 Font Color RGBA");
        cardUi1.m_Stat1Text.rectTransform.anchoredPosition = IniFile.GetVector2ValueRedirect(redirect, section, "Stat1 Position");
        cardUi1.m_Stat2Text.text = IniFile.GetStringValue(str2, "Stat2");
        cardUi1.m_Stat2Text.enabled = IniFile.GetBoolValueRedirect(redirect, section, "Stat2 Enabled");
        cardUi1.m_Stat2Text.fontSize = IniFile.GetFloatValueRedirect(redirect, section, "Stat2 Font Size");
        cardUi1.m_Stat2Text.fontSizeMin = IniFile.GetFloatValueRedirect(redirect, section, "Stat2 Font Size Min");
        cardUi1.m_Stat2Text.fontSizeMax = IniFile.GetFloatValueRedirect(redirect, section, "Stat2 Font Size Max");
        cardUi1.m_Stat2Text.color = IniFile.GetColorValueRedirect(redirect, section, "Stat2 Font Color RGBA");
        cardUi1.m_Stat2Text.rectTransform.anchoredPosition = IniFile.GetVector2ValueRedirect(redirect, section, "Stat2 Position");
        cardUi1.m_Stat3Text.text = IniFile.GetStringValue(str2, "Stat3");
        cardUi1.m_Stat3Text.enabled = IniFile.GetBoolValueRedirect(redirect, section, "Stat3 Enabled");
        cardUi1.m_Stat3Text.fontSize = IniFile.GetFloatValueRedirect(redirect, section, "Stat3 Font Size");
        cardUi1.m_Stat3Text.fontSizeMin = IniFile.GetFloatValueRedirect(redirect, section, "Stat3 Font Size Min");
        cardUi1.m_Stat3Text.fontSizeMax = IniFile.GetFloatValueRedirect(redirect, section, "Stat3 Font Size Max");
        cardUi1.m_Stat3Text.color = IniFile.GetColorValueRedirect(redirect, section, "Stat3 Font Color RGBA");
        cardUi1.m_Stat3Text.rectTransform.anchoredPosition = IniFile.GetVector2ValueRedirect(redirect, section, "Stat3 Position");
        cardUi1.m_Stat4Text.text = IniFile.GetStringValue(str2, "Stat4");
        cardUi1.m_Stat4Text.enabled = IniFile.GetBoolValueRedirect(redirect, section, "Stat4 Enabled");
        cardUi1.m_Stat4Text.fontSize = IniFile.GetFloatValueRedirect(redirect, section, "Stat4 Font Size");
        cardUi1.m_Stat4Text.fontSizeMin = IniFile.GetFloatValueRedirect(redirect, section, "Stat4 Font Size Min");
        cardUi1.m_Stat4Text.fontSizeMax = IniFile.GetFloatValueRedirect(redirect, section, "Stat4 Font Size Max");
        cardUi1.m_Stat4Text.color = IniFile.GetColorValueRedirect(redirect, section, "Stat4 Font Color RGBA");
        cardUi1.m_Stat4Text.rectTransform.anchoredPosition = IniFile.GetVector2ValueRedirect(redirect, section, "Stat4 Position");
        if ((UnityEngine.Object) cardUi1.m_ArtistText != (UnityEngine.Object) null)
        {
          cardUi1.m_ArtistText.text = IniFile.GetStringValueRedirect(redirect, section, "Artist Text");
          cardUi1.m_ArtistText.enabled = IniFile.GetBoolValueRedirect(redirect, section, "Artist Text Enabled");
          cardUi1.m_ArtistText.fontSize = IniFile.GetFloatValueRedirect(redirect, section, "Artist Text Font Size");
          cardUi1.m_ArtistText.fontSizeMin = IniFile.GetFloatValueRedirect(redirect, section, "Artist Text Font Size Min");
          cardUi1.m_ArtistText.fontSizeMax = IniFile.GetFloatValueRedirect(redirect, section, "Artist Text Font Size Max");
          cardUi1.m_ArtistText.color = IniFile.GetColorValueRedirect(redirect, section, "Artist Text Font Color RGBA");
          cardUi1.m_ArtistText.rectTransform.anchoredPosition = IniFile.GetVector2ValueRedirect(redirect, section, "Artist Text Position");
        }
        if ((UnityEngine.Object) PlayerPatches.GetTextComponentByName(cardUi1.gameObject, "CompanyText") != (UnityEngine.Object) null)
        {
          TextMeshProUGUI textComponentByName = PlayerPatches.GetTextComponentByName(cardUi1.gameObject, "CompanyText");
          textComponentByName.text = IniFile.GetStringValueRedirect(redirect, section, "Company Text");
          textComponentByName.enabled = IniFile.GetBoolValueRedirect(redirect, section, "Company Text Enabled");
          textComponentByName.fontSize = IniFile.GetFloatValueRedirect(redirect, section, "Company Text Font Size");
          textComponentByName.fontSizeMin = IniFile.GetFloatValueRedirect(redirect, section, "Company Text Font Size Min");
          textComponentByName.fontSizeMax = IniFile.GetFloatValueRedirect(redirect, section, "Company Text Font Size Max");
          textComponentByName.color = IniFile.GetColorValueRedirect(redirect, section, "Company Text Font Color RGBA");
          textComponentByName.rectTransform.anchoredPosition = IniFile.GetVector2ValueRedirect(redirect, section, "Company Text Position");
        }
        bool boolValueRedirect = IniFile.GetBoolValueRedirect(redirect, section, "Remove Monster Image Size Limit");
        if (boolValueRedirect)
        {
          if (!flag)
          {
            if ((UnityEngine.Object) cardUi1.m_MonsterMask != (UnityEngine.Object) null)
            {
              Image imageComponentByName = PlayerPatches.GetImageComponentByName(cardUi1.m_MonsterMask.gameObject, "Image");
              if ((UnityEngine.Object) imageComponentByName != (UnityEngine.Object) null)
              {
                imageComponentByName.maskable = false;
                cardUi1.m_CardFoilMaskImage.sprite = cardUi1.m_CardBackImage.sprite;
                cardUi1.m_MonsterMaskImage.enabled = false;
              }
            }
          }
          else if (flag)
            cardUi1.m_MonsterMask.enabled = false;
        }
        else if (!boolValueRedirect)
        {
          if (!flag)
          {
            if ((UnityEngine.Object) cardUi1.m_MonsterMask != (UnityEngine.Object) null)
            {
              Image imageComponentByName = PlayerPatches.GetImageComponentByName(cardUi1.m_MonsterMask.gameObject, "Image");
              if ((UnityEngine.Object) imageComponentByName != (UnityEngine.Object) null)
              {
                imageComponentByName.maskable = true;
                cardUi1.m_MonsterMaskImage.enabled = true;
              }
            }
          }
          else if (flag)
            cardUi1.m_MonsterMask.enabled = true;
        }
        if ((UnityEngine.Object) cardUi1.m_MonsterImage != (UnityEngine.Object) null)
        {
          cardUi1.m_MonsterImage.rectTransform.sizeDelta = IniFile.GetVector2ValueRedirect(redirect, section, "Monster Image Size");
          cardUi1.m_MonsterImage.rectTransform.anchoredPosition = IniFile.GetVector2ValueRedirect(redirect, section, "Monster Image Position");
        }
        if (cardData.isFoil)
        {
          CardUI cardUi2 = cardUi1;
          if ((UnityEngine.Object) cardUi2.transform.Find("FoilText") == (UnityEngine.Object) null)
          {
            TextMeshProUGUI textMeshProUgui = new GameObject("FoilText").AddComponent<TextMeshProUGUI>();
            textMeshProUgui.text = IniFile.GetStringValueRedirect(redirect, section, "Foil Text");
            textMeshProUgui.transform.SetParent(cardUi2.transform, false);
            textMeshProUgui.enabled = false;
            if (!((UnityEngine.Object) textMeshProUgui != (UnityEngine.Object) null) || !((UnityEngine.Object) textMeshProUgui.transform.parent == (UnityEngine.Object) cardUi2.transform))
              ;
          }
          Transform transform = cardUi2.transform.Find("FoilText");
          if ((UnityEngine.Object) transform != (UnityEngine.Object) null)
            transform.GetComponent<TextMeshProUGUI>().text = IniFile.GetStringValueRedirect(redirect, section, "Foil Text");
        }
      }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof (UI_PriceTag), "SetItemImage")]
    public static void UI_PriceTag_SetItemImage_Postfix(UI_PriceTag __instance, EItemType itemType)
    {
      switch (itemType)
      {
        case EItemType.GhostPack:
          __instance.m_Icon.sprite = PlayerPatches.GetCustomImage("GhostPack", PlayerPatches.cardExtrasImages);
          break;
        case EItemType.MegabotPack:
          __instance.m_Icon.sprite = PlayerPatches.GetCustomImage("PackMegabot", PlayerPatches.cardExtrasImages);
          break;
        case EItemType.FantasyRPGPack:
          __instance.m_Icon.sprite = PlayerPatches.GetCustomImage("PackFantasyRPG", PlayerPatches.cardExtrasImages);
          break;
        case EItemType.CatJobPack:
          __instance.m_Icon.sprite = PlayerPatches.GetCustomImage("PackCatJob", PlayerPatches.cardExtrasImages);
          break;
      }
    } 
    [HarmonyPostfix] 
    [HarmonyPatch(typeof(RestockItemPanelUI), "Init")] 
    public static void RestockItemPanelUI_Init_Postfix(
      RestockItemPanelUI __instance, 
      RestockItemScreen restockItemScreen, 
      int index) 
    { 
      var itemTypeField = __instance.GetType().GetField("m_ItemType", BindingFlags.NonPublic | BindingFlags.Instance); 
      var itemType = (EItemType) itemTypeField.GetValue(__instance); 
      if (itemType == EItemType.FantasyRPGPack) 
      {
        Sprite customImage = PlayerPatches.GetCustomImage("PackFantasyRPG", PlayerPatches.cardExtrasImages);
        InventoryBase.GetItemData(itemType);
        __instance.m_ItemImage.sprite = customImage;
        __instance.m_ItemImage2.sprite = customImage;
        __instance.m_ItemImageB.sprite = customImage;
        __instance.m_ItemImage2B.sprite = customImage;
      }
      else if (itemType == EItemType.CatJobPack) 
      {
        Sprite customImage = PlayerPatches.GetCustomImage("PackCatJob", PlayerPatches.cardExtrasImages);
        InventoryBase.GetItemData(itemType);
        __instance.m_ItemImage.sprite = customImage;
        __instance.m_ItemImage2.sprite = customImage;
        __instance.m_ItemImageB.sprite = customImage;
        __instance.m_ItemImage2B.sprite = customImage;
      }
      else if (itemType == EItemType.MegabotPack) 
      {
        Sprite customImage = PlayerPatches.GetCustomImage("PackMegabot", PlayerPatches.cardExtrasImages);
        InventoryBase.GetItemData(itemType);
        __instance.m_ItemImage.sprite = customImage;
        __instance.m_ItemImage2.sprite = customImage;
        __instance.m_ItemImageB.sprite = customImage;
        __instance.m_ItemImage2B.sprite = customImage;
      }
      else
      {
        var itemTypeValue = (EItemType)itemTypeField.GetValue(__instance); 
        if (itemTypeValue == EItemType.GhostPack)
        {
          Sprite customImage = PlayerPatches.GetCustomImage("GhostPack", PlayerPatches.cardExtrasImages);
          InventoryBase.GetItemData(itemTypeValue);
          __instance.m_ItemImage.sprite = customImage;
          __instance.m_ItemImage2.sprite = customImage;
          __instance.m_ItemImageB.sprite = customImage;
          __instance.m_ItemImage2B.sprite = customImage;
        }
      }
    }


    [HarmonyPostfix]
    [HarmonyPatch(typeof (CardUI), "SetCardUI")]
    public static void CardUI_SetCardUI_Postfix(CardUI __instance, CardData cardData)
    {
      bool isGhost = cardData.expansionType == ECardExpansionType.Ghost;
      CardUI cardUi = !isGhost || !((UnityEngine.Object) __instance.m_GhostCard != (UnityEngine.Object) null) ? __instance : PlayerPatches.CurrentCardUI(isGhost, __instance, __instance.m_GhostCard);
      bool flag = cardData.expansionType == ECardExpansionType.Megabot || cardData.expansionType == ECardExpansionType.FantasyRPG || cardData.expansionType == ECardExpansionType.CatJob;
      Transform parent = __instance.transform.parent;
      if ((UnityEngine.Object) parent != (UnityEngine.Object) null)
      {
        Transform transform = parent.Find("CardBackMesh");
        if ((UnityEngine.Object) transform != (UnityEngine.Object) null)
        {
          Renderer component = transform.GetComponent<Renderer>();
          if ((UnityEngine.Object) component != (UnityEngine.Object) null)
          {
            Material material = component.material;
            if (material.HasProperty("_BaseMap") && material.HasProperty("_EmissionMap"))
            {
              Texture texture = (Texture) null;
              if (!MinaCardsModPlugin.DisableSwappingBaseCardBacks.Value && (cardData.expansionType == ECardExpansionType.Tetramon || cardData.expansionType == ECardExpansionType.Destiny || cardData.expansionType == ECardExpansionType.Ghost))
              {
                if (cardData.expansionType == ECardExpansionType.Tetramon)
                {
                  if ((UnityEngine.Object) PlayerPatches.LoadCustomTexture("T_CardBackMesh", PlayerPatches.cardExtrasImages) != (UnityEngine.Object) null)
                    texture = PlayerPatches.LoadCustomTexture("T_CardBackMesh", PlayerPatches.cardExtrasImages);
                }
                else if ((UnityEngine.Object) PlayerPatches.LoadCustomTexture("T_CardBackMesh" + cardData.expansionType.ToString(), PlayerPatches.cardExtrasImages) != (UnityEngine.Object) null)
                  texture = PlayerPatches.LoadCustomTexture("T_CardBackMesh" + cardData.expansionType.ToString(), PlayerPatches.cardExtrasImages);
                if ((UnityEngine.Object) texture != (UnityEngine.Object) null)
                {
                  material.SetTexture("_BaseMap", texture);
                  material.SetTexture("_EmissionMap", texture);
                }
              }
              if (cardData.expansionType == ECardExpansionType.FantasyRPG || cardData.expansionType == ECardExpansionType.CatJob || cardData.expansionType == ECardExpansionType.Megabot)
              {
                if ((UnityEngine.Object) PlayerPatches.LoadCustomTexture("T_CardBackMesh" + cardData.expansionType.ToString(), PlayerPatches.cardExtrasImages) != (UnityEngine.Object) null)
                  texture = PlayerPatches.LoadCustomTexture("T_CardBackMesh" + cardData.expansionType.ToString(), PlayerPatches.cardExtrasImages);
                if ((UnityEngine.Object) texture != (UnityEngine.Object) null)
                {
                  material.SetTexture("_BaseMap", texture);
                  material.SetTexture("_EmissionMap", texture);
                }
              }
            }
          }
        }
      }
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
          Image componentBySpriteName = PlayerPatches.GetImageComponentBySpriteName(cardUi.gameObject, str);
          if ((UnityEngine.Object) componentBySpriteName != (UnityEngine.Object) null)
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
        Image componentBySpriteName = PlayerPatches.GetImageComponentBySpriteName(cardUi.gameObject, str);
        if ((UnityEngine.Object) componentBySpriteName != (UnityEngine.Object) null)
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
        Image componentBySpriteName = PlayerPatches.GetImageComponentBySpriteName(cardUi.gameObject, str);
        if ((UnityEngine.Object) componentBySpriteName != (UnityEngine.Object) null)
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
        Image componentBySpriteName = PlayerPatches.GetImageComponentBySpriteName(cardUi.gameObject, str);
        if ((UnityEngine.Object) componentBySpriteName != (UnityEngine.Object) null)
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
        Image componentBySpriteName = PlayerPatches.GetImageComponentBySpriteName(cardUi.gameObject, str);
        if ((UnityEngine.Object) componentBySpriteName != (UnityEngine.Object) null)
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
        if (cardExpansionType == ECardExpansionType.FantasyRPG || cardExpansionType == ECardExpansionType.CatJob || cardExpansionType == ECardExpansionType.Megabot)
        {
          string path1 = PlayerPatches.fantasyPackImages + __instance.MonsterType.ToString() + ".png";
          string path2 = PlayerPatches.catJobPackImages + __instance.MonsterType.ToString() + ".png";
          string path3 = PlayerPatches.megabotPackImages + __instance.MonsterType.ToString() + ".png";
          switch (cardExpansionType)
          {
            case ECardExpansionType.Megabot:
              if (File.Exists(path3))
              {
                __result = PlayerPatches.GetCustomImage(__instance.MonsterType.ToString(), PlayerPatches.megabotPackImages);
                return false;
              }
              break;
            case ECardExpansionType.FantasyRPG:
              if (File.Exists(path1))
              {
                __result = PlayerPatches.GetCustomImage(__instance.MonsterType.ToString(), PlayerPatches.fantasyPackImages);
                return false;
              }
              break;
            case ECardExpansionType.CatJob:
              if (File.Exists(path2))
              {
                __result = PlayerPatches.GetCustomImage(__instance.MonsterType.ToString(), PlayerPatches.catJobPackImages);
                return false;
              }
              break;
          }
        }
      }
      else if (!MinaCardsModPlugin.CustomImages.Value)
      {
        InventoryBase.GetMonsterData(EMonsterType.PiggyA);
        int monsterType1 = (int) __instance.MonsterType;
        if (monsterType1 > 110)
        {
          int monsterType2 = monsterType1;
          if (monsterType1 >= 1000 && monsterType1 <= 1112)
            monsterType2 = monsterType1 < 1110 ? monsterType1 - 999 : monsterType1 - 1109;
          else if (monsterType1 >= 2000 && monsterType1 <= 2049)
            monsterType2 = monsterType1 - 1999;
          else if (monsterType1 >= 3000 && monsterType1 <= 3039)
            monsterType2 = monsterType1 - 2949;
          MonsterData monsterData = InventoryBase.GetMonsterData((EMonsterType) monsterType2);
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

    public static void ShowText(string customText)
    {
      var resetTimerField = typeof(NotEnoughResourceTextPopup).GetField("m_ResetTimer", BindingFlags.NonPublic | BindingFlags.Instance);
      resetTimerField.SetValue(CSingleton<NotEnoughResourceTextPopup>.Instance, 0.0f);
    
      string str = customText;
      for (int index = 0; index < CSingleton<NotEnoughResourceTextPopup>.Instance.m_ShowTextGameObjectList.Count; ++index)
      {
        if (!CSingleton<NotEnoughResourceTextPopup>.Instance.m_ShowTextGameObjectList[index].activeSelf)
        {
          CSingleton<NotEnoughResourceTextPopup>.Instance.m_ShowTextList[index].text = str;
          CSingleton<NotEnoughResourceTextPopup>.Instance.m_ShowTextGameObjectList[index].gameObject.SetActive(true);
          break;
        }
      }
    }


    public static Texture2D LoadTexture(string filePath)
    {
      if (File.Exists(filePath))
      {
        Texture2D tex = new Texture2D(2, 2);
        byte[] data = File.ReadAllBytes(filePath);
        tex.LoadImage(data);
        return tex;
      }
      MinaCardsModPlugin.Log.LogError((object) ("Texture file not found: " + filePath));
      return (Texture2D) null;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof (InventoryBase), "Awake")]
    public static void InventoryBase_Awake_Postfix()
    {
      if (!(InventoryBase.GetItemMeshData(EItemType.CatJobPack).material.mainTexture.name == "CommonPack_GhostMegabot"))
        return;
      ItemMeshData itemMeshData1 = InventoryBase.GetItemMeshData(EItemType.DestinyBasicCardPack);
      ItemMeshData itemMeshData2 = InventoryBase.GetItemMeshData(EItemType.CatJobPack);
      ItemMeshData itemMeshData3 = InventoryBase.GetItemMeshData(EItemType.FantasyRPGPack);
      ItemMeshData itemMeshData4 = InventoryBase.GetItemMeshData(EItemType.MegabotPack);
      ItemMeshData itemMeshData5 = InventoryBase.GetItemMeshData(EItemType.GhostPack);
      Texture2D texture2D1 = PlayerPatches.LoadTexture(PlayerPatches.customExpansionPackImages + "T_CardPackFantasy.png");
      Material material1 = new Material(itemMeshData1.material);
      material1.mainTexture = (Texture) texture2D1;
      material1.mainTexture.name = "T_CardPackFantasy";
      itemMeshData3.material = material1;
      Texture2D texture2D2 = PlayerPatches.LoadTexture(PlayerPatches.customExpansionPackImages + "T_CardPackCatJob.png");
      Material material2 = new Material(itemMeshData1.material);
      material2.mainTexture = (Texture) texture2D2;
      material2.mainTexture.name = "T_CardPackCatJob";
      itemMeshData2.material = material2;
      Texture2D texture2D3 = PlayerPatches.LoadTexture(PlayerPatches.customExpansionPackImages + "T_CardPackMegabot.png");
      Material material3 = new Material(itemMeshData1.material);
      material3.mainTexture = (Texture) texture2D3;
      material3.mainTexture.name = "T_CardPackMegabot";
      itemMeshData4.material = material3;
      Texture2D texture2D4 = PlayerPatches.LoadTexture(PlayerPatches.customExpansionPackImages + "T_CardPackGhost.png");
      Material material4 = new Material(itemMeshData1.material);
      material4.mainTexture = (Texture) texture2D4;
      material4.mainTexture.name = "T_CardPackGhost";
      itemMeshData5.material = material4;
    }
    public static void Log(string log) => MinaCardsModPlugin.Log.LogInfo((object) log);
  }
}