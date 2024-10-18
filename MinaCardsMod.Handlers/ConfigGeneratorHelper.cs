using I2.Loc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using MinaCardsMod.Patches;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#nullable disable
namespace MinaCardsMod.Handlers
{
  internal class ConfigGeneratorHelper
  {
    public static List<string> useableBaseConfigs = new List<string>()
    {
      "Angez",
      "BatA",
      "Bear",
      "Beetle",
      "BugA",
      "CloudA",
      "CrabA",
      "CrabB",
      "Crystalmon",
      "DragonEarth",
      "DragonFire",
      "DragonThunder",
      "DragonWater",
      "ElecDragon",
      "EmeraldA",
      "FireBirdA",
      "FireGeckoA",
      "FireSpirit",
      "FireUmbrellaDragon",
      "FireWolfA",
      "FishA",
      "FlowerA",
      "FoxA",
      "GolemA",
      "HalloweenA",
      "HalloweenB",
      "HalloweenC",
      "HalloweenD",
      "HydraA",
      "Jellyfish",
      "Lanternmon",
      "LobsterA",
      "Mosquito",
      "MuffinTreeA",
      "MummyMan",
      "NinjaBirdA",
      "NinjaCrowC",
      "PiggyA",
      "SeedBugA",
      "SerpentA",
      "SharkFishA",
      "ShellyA",
      "Skull",
      "StarfishA",
      "TreeA",
      "TronA",
      "Turtle",
      "WeirdBirdA",
      "Wisp"
    };

    public static void writeMonsterData(CardData cardData, CardUI cardUI)
    {
      string configFilePath = ExtrasHandler.findConfigFilePath(cardData);
      if (!File.Exists(configFilePath + ".ini") && cardData.borderType != ECardBorderType.FullArt)
      {
        MinaCardsModPlugin.Log.LogInfo((object) ("Creating config for " + cardData.monsterType.ToString()));
        ConfigGeneratorHelper.WriteCardDataToFile(configFilePath + ".ini", cardData, cardUI);
      }
      if (File.Exists(configFilePath + "FullArt.ini") || cardData.borderType != ECardBorderType.FullArt)
        return;
      MinaCardsModPlugin.Log.LogInfo((object) ("Creating Full Art config for " + cardData.monsterType.ToString()));
      ConfigGeneratorHelper.WriteCardDataToFile(configFilePath + "FullArt.ini", cardData, cardUI);
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
        CardUI cardUi = !isGhost || !((UnityEngine.Object) cardUI.m_GhostCard != (UnityEngine.Object) null) ? cardUI : ExtrasHandler.CurrentCardUI(isGhost, cardUI, cardUI.m_GhostCard);
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
        if (monsterDataField != null)
        {
          var monsterData = monsterDataField.GetValue(cardUi);

          FieldInfo previousEvolutionField = monsterData.GetType().GetField("PreviousEvolution",
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
          if (previousEvolutionField != null)
          {
            EMonsterType previousEvolution = (EMonsterType)previousEvolutionField.GetValue(monsterData);

            if (!isGhost && previousEvolution == EMonsterType.None)
            {
              if ((UnityEngine.Object) ExtrasHandler.GetImageComponentByName(cardUi.gameObject, "EvoBasicIcon") != (UnityEngine.Object) null)
              {
                StreamWriter streamWriter14 = streamWriter1;
                flag3 = ExtrasHandler.GetImageComponentByName(cardUi.gameObject, "EvoBasicIcon").enabled;
                string str13 = "Basic Evolution Icon Enabled = " + flag3.ToString();
                streamWriter14.WriteLine(str13);
              }
              if ((UnityEngine.Object) ExtrasHandler.GetTextComponentByName(cardUi.gameObject, "EvoBasicText") != (UnityEngine.Object) null)
              {
                TextMeshProUGUI textComponentByName = ExtrasHandler.GetTextComponentByName(cardUi.gameObject, "EvoBasicText");
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
                streamWriter1.WriteLine("");
              }
            }

            if (!isGhost)
            {
              int previousEvolutionInt = (int)previousEvolution;
              if (true) 
              { 
                if (!flag2)
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
              if ((UnityEngine.Object) ExtrasHandler.GetImageComponentByName(cardUi.gameObject, "EvoBG") != (UnityEngine.Object) null) 
              {
                StreamWriter streamWriter26 = streamWriter1;
                flag3 = ExtrasHandler.GetImageComponentByName(cardUi.gameObject, "EvoBG").enabled;
                string str25 = "Previous Evolution Box Enabled = " + flag3.ToString();
                streamWriter26.WriteLine(str25);
                streamWriter1.WriteLine(""); 
              } 
            }
          }
        }

        if (!isGhost && (UnityEngine.Object) ExtrasHandler.GetTextComponentByName(cardUi.gameObject, "TitleText") != (UnityEngine.Object) null)
        {
          TextMeshProUGUI textComponentByName = ExtrasHandler.GetTextComponentByName(cardUi.gameObject, "TitleText");
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
          if ((UnityEngine.Object) ExtrasHandler.GetImageComponentByName(cardUi.gameObject, "TitleBG") != (UnityEngine.Object) null)
          {
            StreamWriter streamWriter32 = streamWriter1;
            flag3 = ExtrasHandler.GetImageComponentByName(cardUi.gameObject, "TitleBG").enabled;
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
          if ((UnityEngine.Object) ExtrasHandler.GetImageComponentByName(cardUi.gameObject, "RarityImage") != (UnityEngine.Object) null)
          {
            Image imageComponentByName = ExtrasHandler.GetImageComponentByName(cardUi.gameObject, "RarityImage");
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
        if (isGhost && (UnityEngine.Object) ExtrasHandler.GetImageComponentByName(cardUi.gameObject, "CardStat") != (UnityEngine.Object) null)
        {
          StreamWriter streamWriter49 = streamWriter1;
          flag3 = ExtrasHandler.GetImageComponentByName(cardUi.gameObject, "CardStat").enabled;
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
          flag3 = cardUi.m_Stat3Text.enabled;
          string str59 = "Stat3 Enabled = " + flag3.ToString();
          streamWriter60.WriteLine(str59);
          StreamWriter streamWriter61 = streamWriter1;
          num = cardUi.m_Stat3Text.fontSize;
          string str60 = "Stat3 Font Size = " + num.ToString();
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
        if ((UnityEngine.Object) ExtrasHandler.GetTextComponentByName(cardUi.gameObject, "CompanyText") != (UnityEngine.Object) null)
        {
          TextMeshProUGUI textComponentByName = ExtrasHandler.GetTextComponentByName(cardUi.gameObject, "CompanyText");
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
      foreach (string path7 in files1)
      {
        if (!path7.Contains("FullArt", StringComparison.OrdinalIgnoreCase))
        {
          string withoutExtension = Path.GetFileNameWithoutExtension(path7);
          if (ConfigGeneratorHelper.useableBaseConfigs.Contains(withoutExtension))
          {
            str1 = Path.GetFileName(path7);
            break;
          }
        }
      }
      foreach (string path8 in files1)
      {
        if (path8.Contains("FullArt", StringComparison.OrdinalIgnoreCase))
        {
          string str13 = Path.GetFileNameWithoutExtension(path8).Replace("FullArt", "");
          if (ConfigGeneratorHelper.useableBaseConfigs.Contains(str13))
          {
            str7 = Path.GetFileName(path8);
            break;
          }
        }
      }
      foreach (string path9 in files2)
      {
        if (!path9.Contains("FullArt", StringComparison.OrdinalIgnoreCase))
        {
          string withoutExtension = Path.GetFileNameWithoutExtension(path9);
          if (ConfigGeneratorHelper.useableBaseConfigs.Contains(withoutExtension))
          {
            str2 = Path.GetFileName(path9);
            break;
          }
        }
      }
      foreach (string path10 in files2)
      {
        if (path10.Contains("FullArt", StringComparison.OrdinalIgnoreCase))
        {
          string str14 = Path.GetFileNameWithoutExtension(path10).Replace("FullArt", "");
          if (ConfigGeneratorHelper.useableBaseConfigs.Contains(str14))
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
          ConfigGeneratorHelper.WriteFullExpansionConfig(path1 + str1, "Tetramon", outputPath, "Tetramon.ini", sectionsToRemove);
      }
      if (!File.Exists(outputPath + "TetramonFullArt.ini"))
      {
        PlayerPatches.Log("File doesn't exist -- " + outputPath + "TetramonFullArt.ini");
        if (str7 != null)
          ConfigGeneratorHelper.WriteFullExpansionConfig(path1 + str7, "TetramonFullArt", outputPath, "TetramonFullArt.ini", sectionsToRemove);
      }
      if (!File.Exists(outputPath + "Destiny.ini") && str2 != null)
        ConfigGeneratorHelper.WriteFullExpansionConfig(path2 + str2, "Destiny", outputPath, "Destiny.ini", sectionsToRemove);
      if (!File.Exists(outputPath + "DestinyFullArt.ini") && str8 != null)
        ConfigGeneratorHelper.WriteFullExpansionConfig(path2 + str8, "DestinyFullArt", outputPath, "DestinyFullArt.ini", sectionsToRemove);
      if (!File.Exists(outputPath + "Ghost.ini") && str3 != null)
        ConfigGeneratorHelper.WriteFullExpansionConfig(path3 + str3, "Ghost", outputPath, "Ghost.ini", sectionsToRemove);
      if (!File.Exists(outputPath + "GhostFullArt.ini") && str9 != null)
        ConfigGeneratorHelper.WriteFullExpansionConfig(path3 + str9, "GhostFullArt", outputPath, "GhostFullArt.ini", sectionsToRemove);
      if (!File.Exists(outputPath + "Megabot.ini") && str4 != null)
        ConfigGeneratorHelper.WriteFullExpansionConfig(path4 + str4, "Megabot", outputPath, "Megabot.ini", sectionsToRemove);
      if (!File.Exists(outputPath + "MegabotFullArt.ini") && str10 != null)
        ConfigGeneratorHelper.WriteFullExpansionConfig(path4 + str10, "MegabotFullArt", outputPath, "MegabotFullArt.ini", sectionsToRemove);
      if (!File.Exists(outputPath + "CatJob.ini") && str5 != null)
        ConfigGeneratorHelper.WriteFullExpansionConfig(path5 + str5, "CatJob", outputPath, "CatJob.ini", sectionsToRemove);
      if (!File.Exists(outputPath + "CatJobFullArt.ini") && str11 != null)
        ConfigGeneratorHelper.WriteFullExpansionConfig(path5 + str11, "CatJobFullArt", outputPath, "CatJobFullArt.ini", sectionsToRemove);
      if (!File.Exists(outputPath + "FantasyRPG.ini") && str6 != null)
        ConfigGeneratorHelper.WriteFullExpansionConfig(path6 + str6, "FantasyRPG", outputPath, "FantasyRPG.ini", sectionsToRemove);
      if (File.Exists(outputPath + "FantasyRPGFullArt.ini") || str12 == null)
        return;
      ConfigGeneratorHelper.WriteFullExpansionConfig(path6 + str12, "FantasyRPGFullArt", outputPath, "FantasyRPGFullArt.ini", sectionsToRemove);
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
  }
}