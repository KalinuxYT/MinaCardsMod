using System;
using System.Collections.Generic;
using System.IO;
using MinaCardsMod.Patches;
using UnityEngine;
using Object = UnityEngine.Object;

#nullable disable
namespace MinaCardsMod.Handlers
{
  internal class CacheHandler
  {
    public static List<Sprite> cardExtrasImagesCache = new List<Sprite>();
    public static List<Sprite> catJobPackImagesCache = new List<Sprite>();
    public static List<Sprite> fantasyRPGPackImagesCache = new List<Sprite>();
    public static List<Sprite> ghostPackImagesCache = new List<Sprite>();
    public static List<Sprite> megabotPackImagesCache = new List<Sprite>();
    public static List<Sprite> tetramonPackImagesCache = new List<Sprite>();
    public static List<CustomCardObject> catJobConfigCache = new List<CustomCardObject>();
    public static List<CustomCardObject> destinyConfigCache = new List<CustomCardObject>();
    public static List<CustomCardObject> fantasyRPGConfigCache = new List<CustomCardObject>();
    public static List<CustomCardObject> fullExpansionsConfigCache = new List<CustomCardObject>();
    public static List<CustomCardObject> ghostConfigCache = new List<CustomCardObject>();
    public static List<CustomCardObject> megabotConfigCache = new List<CustomCardObject>();
    public static List<CustomCardObject> tetramonConfigCache = new List<CustomCardObject>();
    public static List<Sprite> originalCardEditionBorderList = new List<Sprite>();
    public static List<Sprite> originalCardBorderList = new List<Sprite>();
    public static List<Sprite> originalCardBGList = new List<Sprite>();
    public static List<Sprite> originalCardFrontImageList = new List<Sprite>();
    public static List<Sprite> originalCardBackImageList = new List<Sprite>();
    public static List<Sprite> originalCardFoilMaskImageList = new List<Sprite>();
    public static List<Sprite> originalTetramonMonsterImageList = new List<Sprite>();
    public static List<Sprite> originalCardExtrasImagesList = new List<Sprite>();
    public static Sprite originalCardBackTexture;
    public static bool clonedOriginalMonsterImages = false;
    public static bool clonedOriginalSpriteLists = false;
    public static bool hasLoadedCache;
    public static bool isCurrentlyCacheing;
    public static bool allLoadedCachesFull;
    public static bool firstLoad;
    private const int ExpectedCatJobCount = 80;
    private const int ExpectedDestinyCount = 242;
    private const int ExpectedFantasyCount = 100;
    private const int ExpectedFullExpansionsCount = 11;
    private const int ExpectedGhostCount = 19;
    private const int ExpectedMegabotCount = 226;
    private const int ExpectedTetramonCount = 242;
    private const int ExpectedCardExtrasCount = 52;
    private const int ExpectedCatJobPackCount = 40;
    private const int ExpectedFantasyRPGPackCount = 50;
    private const int ExpectedGhostPackCount = 19;
    private const int ExpectedMegabotPackCount = 113;
    private const int ExpectedTetramonPackCount = 121;

    public static bool VerifyFullConfigCaches()
    {
      CacheHandler.Log("Verifying config caches");
      bool flag1 = CacheHandler.catJobConfigCache.Count == 80;
      bool flag2 = CacheHandler.destinyConfigCache.Count == 242;
      bool flag3 = CacheHandler.fantasyRPGConfigCache.Count == 100;
      bool flag4 = CacheHandler.fullExpansionsConfigCache.Count == 11;
      bool flag5 = CacheHandler.ghostConfigCache.Count == 19;
      bool flag6 = CacheHandler.megabotConfigCache.Count == 226;
      bool flag7 = CacheHandler.tetramonConfigCache.Count == 242;
      if (!flag1)
        CacheHandler.LogError("CatJob Config Cache NOT FULL. Count should be " + 80.ToString() + " but it is " + CacheHandler.catJobConfigCache.Count.ToString());
      if (!flag2)
        CacheHandler.LogError("Destiny Config Cache NOT FULL. Count should be " + 242.ToString() + " but it is " + CacheHandler.destinyConfigCache.Count.ToString());
      if (!flag3)
        CacheHandler.LogError("FantasyRPG Config Cache NOT FULL. Count should be " + 100.ToString() + " but it is " + CacheHandler.fantasyRPGConfigCache.Count.ToString());
      if (!flag4)
        CacheHandler.LogError("Full Expansions Config Cache NOT FULL. Count should be " + 11.ToString() + " but it is " + CacheHandler.fullExpansionsConfigCache.Count.ToString());
      if (!flag5)
        CacheHandler.LogError("Ghost Config Cache NOT FULL. Count should be " + 19.ToString() + " but it is " + CacheHandler.ghostConfigCache.Count.ToString());
      if (!flag6)
        CacheHandler.LogError("Megabot Config Cache NOT FULL. Count should be " + 226.ToString() + " but it is " + CacheHandler.megabotConfigCache.Count.ToString());
      if (!flag7)
        CacheHandler.LogError("Tetramon Config Cache NOT FULL. Count should be " + 242.ToString() + " but it is " + CacheHandler.tetramonConfigCache.Count.ToString());
      if (!(flag1 & flag2 & flag3 & flag4 & flag5 & flag6 & flag7))
      {
        CacheHandler.LogError("Config caches are not all full");
        return false;
      }
      CacheHandler.Log("Config caches populated successfully");
      return true;
    }

    public static void CacheAllFiles()
    {
      CacheHandler.Log("CacheAllFiles starting");
      CacheHandler.isCurrentlyCacheing = true;
    
      CacheHandler.Log("CacheAllIniFiles starting");
      CacheHandler.CacheAllIniFiles();
      CacheHandler.Log("CacheAllIniFiles completed");

      CacheHandler.Log("CacheAllImages starting");
      CacheHandler.CacheAllImages();
      CacheHandler.Log("CacheAllImages completed");
    
      CacheHandler.hasLoadedCache = true;
      CacheHandler.allLoadedCachesFull = CacheHandler.VerifyFullConfigCaches() && CacheHandler.VerifyFullImageCaches();
    
      CacheHandler.isCurrentlyCacheing = false;
      CacheHandler.Log("CacheAllFiles completed");
    }

    public static void CacheAllIniFiles()
    {
      CacheHandler.Log("Starting to cache config files");
    
      CacheHandler.catJobConfigCache.Clear();
      CacheHandler.destinyConfigCache.Clear();
      CacheHandler.fantasyRPGConfigCache.Clear();
      CacheHandler.fullExpansionsConfigCache.Clear();
      CacheHandler.ghostConfigCache.Clear();
      CacheHandler.megabotConfigCache.Clear();
      CacheHandler.tetramonConfigCache.Clear();
    
      CacheHandler.Log("Caching CatJob Configs");
      CacheHandler.LoadIniFileToCache(CacheFileListHandler.catJobConfigsFileNameList, CacheHandler.catJobConfigCache, PlayerPatches.catJobConfigPath);
    
      CacheHandler.Log("Caching Destiny Configs");
      CacheHandler.LoadIniFileToCache(CacheFileListHandler.destinyConfigsFileNameList, CacheHandler.destinyConfigCache, PlayerPatches.destinyConfigPath);
    
      CacheHandler.Log("Caching Fantasy RPG Configs");
      CacheHandler.LoadIniFileToCache(CacheFileListHandler.fantasyRPGConfigsFileNameList, CacheHandler.fantasyRPGConfigCache, PlayerPatches.fantasyRPGConfigPath);
    
      CacheHandler.Log("Caching Full Expansions Configs");
      CacheHandler.LoadIniFileToCache(CacheFileListHandler.fullExpansionsConfigsFileNameList, CacheHandler.fullExpansionsConfigCache, PlayerPatches.fullExpansionsConfigPath);
    
      CacheHandler.Log("Caching Ghost Configs");
      CacheHandler.LoadIniFileToCache(CacheFileListHandler.ghostConfigsFileNameList, CacheHandler.ghostConfigCache, PlayerPatches.ghostConfigPath);
    
      CacheHandler.Log("Caching Megabot Configs");
      CacheHandler.LoadIniFileToCache(CacheFileListHandler.megabotConfigsFileNameList, CacheHandler.megabotConfigCache, PlayerPatches.megabotConfigPath);
    
      CacheHandler.Log("Caching Tetramon Configs");
      CacheHandler.LoadIniFileToCache(CacheFileListHandler.tetramonConfigsFileNameList, CacheHandler.tetramonConfigCache, PlayerPatches.tetramonConfigPath);
    
      CacheHandler.Log("Completed config caching");
    }


    public static void LoadIniFileToCache(
      List<string> configList,
      List<CustomCardObject> customCardList,
      string filePath)
    {
      foreach (string config in configList)
      {
        string str = Path.Combine(filePath, config + ".ini");
        if (File.Exists(str))
        {
          CacheHandler.CacheIni(str, config, customCardList);
        }
        else
          CacheHandler.LogError("Ini file not found -- " + str);
      }
    }

    public static void CacheIni(
      string fileToLoad,
      string fileNameOnly,
      List<CustomCardObject> configList)
    {
      if (fileToLoad == null)
        return;
      if (File.Exists(fileToLoad))
      {
        IniFile.Load(fileToLoad);
        configList.Add(new CustomCardObject()
        {
          Header = fileNameOnly,
          Name = IniFile.GetStringValue(fileNameOnly, "Name"),
          NameEnabled = IniFile.GetBoolValue(fileNameOnly, "Name Enabled"),
          NameFontSize = IniFile.GetFloatValue(fileNameOnly, "Name Font Size"),
          NameFontSizeMin = IniFile.GetFloatValue(fileNameOnly, "Name Font Size Min"),
          NameFontSizeMax = IniFile.GetFloatValue(fileNameOnly, "Name Font Size Max"),
          NameFontColorRGBA = IniFile.GetColorValue(fileNameOnly, "Name Font Color RGBA"),
          NamePosition = IniFile.GetVector2Value(fileNameOnly, "Name Position"),
          Description = IniFile.GetStringValue(fileNameOnly, "Description"),
          DescriptionEnabled = IniFile.GetBoolValue(fileNameOnly, "Description Enabled"),
          DescriptionFontSize = IniFile.GetFloatValue(fileNameOnly, "Description Font Size"),
          DescriptionFontSizeMin = IniFile.GetFloatValue(fileNameOnly, "Description Font Size Min"),
          DescriptionFontSizeMax = IniFile.GetFloatValue(fileNameOnly, "Description Font Size Max"),
          DescriptionFontColorRGBA = IniFile.GetColorValue(fileNameOnly, "Description Font Color RGBA"),
          DescriptionPosition = IniFile.GetVector2Value(fileNameOnly, "Description Position"),
          IndividualOverrides = IniFile.GetBoolValue(fileNameOnly, "Individual Overrides"),
          Number = IniFile.GetStringValue(fileNameOnly, "Number"),
          NumberEnabled = IniFile.GetBoolValue(fileNameOnly, "Number Enabled"),
          NumberFontSize = IniFile.GetFloatValue(fileNameOnly, "Number Font Size"),
          NumberFontSizeMin = IniFile.GetFloatValue(fileNameOnly, "Number Font Size Min"),
          NumberFontSizeMax = IniFile.GetFloatValue(fileNameOnly, "Number Font Size Max"),
          NumberFontColorRGBA = IniFile.GetColorValue(fileNameOnly, "Number Font Color RGBA"),
          NumberPosition = IniFile.GetVector2Value(fileNameOnly, "Number Position"),
          BasicEvolutionIconEnabled = IniFile.GetBoolValue(fileNameOnly, "Basic Evolution Icon Enabled"),
          BasicEvolutionText = IniFile.GetStringValue(fileNameOnly, "Basic Evolution Text"),
          BasicEvolutionTextEnabled = IniFile.GetBoolValue(fileNameOnly, "Basic Evolution Text Enabled"),
          BasicEvolutionTextFontSize = IniFile.GetFloatValue(fileNameOnly, "Basic Evolution Text Font Size"),
          BasicEvolutionTextFontSizeMin = IniFile.GetFloatValue(fileNameOnly, "Basic Evolution Text Font Size Min"),
          BasicEvolutionTextFontSizeMax = IniFile.GetFloatValue(fileNameOnly, "Basic Evolution Text Font Size Max"),
          BasicEvolutionTextFontColorRGBA = IniFile.GetColorValue(fileNameOnly, "Basic Evolution Text Font Color RGBA"),
          BasicEvolutionTextPosition = IniFile.GetVector2Value(fileNameOnly, "Basic Evolution Text Position"),
          PreviousEvolution = IniFile.GetStringValue(fileNameOnly, "Previous Evolution"),
          PreviousEvolutionIconEnabled = IniFile.GetBoolValue(fileNameOnly, "Previous Evolution Icon Enabled"),
          PreviousEvolutionNameEnabled = IniFile.GetBoolValue(fileNameOnly, "Previous Evolution Name Enabled"),
          PreviousEvolutionNameFontSize = IniFile.GetFloatValue(fileNameOnly, "Previous Evolution Name Font Size"),
          PreviousEvolutionNameFontSizeMin = IniFile.GetFloatValue(fileNameOnly, "Previous Evolution Name Font Size Min"),
          PreviousEvolutionNameFontSizeMax = IniFile.GetFloatValue(fileNameOnly, "Previous Evolution Name Font Size Max"),
          PreviousEvolutionNameFontColorRGBA = IniFile.GetColorValue(fileNameOnly, "Previous Evolution Name Font Color RGBA"),
          PreviousEvolutionNamePosition = IniFile.GetVector2Value(fileNameOnly, "Previous Evolution Name Position"),
          PreviousEvolutionBoxEnabled = IniFile.GetBoolValue(fileNameOnly, "Previous Evolution Box Enabled"),
          PlayEffectText = IniFile.GetStringValue(fileNameOnly, "Play Effect Text"),
          PlayEffectTextEnabled = IniFile.GetBoolValue(fileNameOnly, "Play Effect Text Enabled"),
          PlayEffectTextFontSize = IniFile.GetFloatValue(fileNameOnly, "Play Effect Text Font Size"),
          PlayEffectTextFontSizeMin = IniFile.GetFloatValue(fileNameOnly, "Play Effect Text Font Size Min"),
          PlayEffectTextFontSizeMax = IniFile.GetFloatValue(fileNameOnly, "Play Effect Text Font Size Max"),
          PlayEffectTextFontColorRGBA = IniFile.GetColorValue(fileNameOnly, "Play Effect Text Font Color RGBA"),
          PlayEffectTextPosition = IniFile.GetVector2Value(fileNameOnly, "Play Effect Text Position"),
          PlayEffectBoxEnabled = IniFile.GetBoolValue(fileNameOnly, "Play Effect Box Enabled"),
          FoilText = IniFile.GetStringValue(fileNameOnly, "Foil Text"),
          Rarity = IniFile.GetStringValue(fileNameOnly, "Rarity"),
          RarityEnabled = IniFile.GetBoolValue(fileNameOnly, "Rarity Enabled"),
          RarityFontSize = IniFile.GetFloatValue(fileNameOnly, "Rarity Font Size"),
          RarityFontSizeMin = IniFile.GetFloatValue(fileNameOnly, "Rarity Font Size Min"),
          RarityFontSizeMax = IniFile.GetFloatValue(fileNameOnly, "Rarity Font Size Max"),
          RarityFontColorRGBA = IniFile.GetColorValue(fileNameOnly, "Rarity Font Color RGBA"),
          RarityPosition = IniFile.GetVector2Value(fileNameOnly, "Rarity Position"),
          RarityImageEnabled = IniFile.GetBoolValue(fileNameOnly, "Rarity Image Enabled"),
          EditionText = IniFile.GetStringValue(fileNameOnly, "Edition Text"),
          BasicEditionText = IniFile.GetStringValue(fileNameOnly, "Basic Edition Text"),
          FirstEditionText = IniFile.GetStringValue(fileNameOnly, "First Edition Text"),
          GoldEditionText = IniFile.GetStringValue(fileNameOnly, "Gold Edition Text"),
          SilverEditionText = IniFile.GetStringValue(fileNameOnly, "Silver Edition Text"),
          EXEditionText = IniFile.GetStringValue(fileNameOnly, "EX Edition Text"),
          EditionTextEnabled = IniFile.GetBoolValue(fileNameOnly, "Edition Text Enabled"),
          EditionTextFontSize = IniFile.GetFloatValue(fileNameOnly, "Edition Text Font Size"),
          EditionTextFontSizeMin = IniFile.GetFloatValue(fileNameOnly, "Edition Text Font Size Min"),
          EditionTextFontSizeMax = IniFile.GetFloatValue(fileNameOnly, "Edition Text Font Size Max"),
          EditionTextFontColorRGBA = IniFile.GetColorValue(fileNameOnly, "Edition Text Font Color RGBA"),
          EditionTextPosition = IniFile.GetVector2Value(fileNameOnly, "Edition Text Position"),
          ChampionText = IniFile.GetStringValue(fileNameOnly, "Champion Text"),
          ChampionTextEnabled = IniFile.GetBoolValue(fileNameOnly, "Champion Text Enabled"),
          ChampionFontSize = IniFile.GetFloatValue(fileNameOnly, "Champion Font Size"),
          ChampionFontSizeMin = IniFile.GetFloatValue(fileNameOnly, "Champion Font Size Min"),
          ChampionFontSizeMax = IniFile.GetFloatValue(fileNameOnly, "Champion Font Size Max"),
          ChampionFontColorRGBA = IniFile.GetColorValue(fileNameOnly, "Champion Font Color RGBA"),
          ChampionPosition = IniFile.GetVector2Value(fileNameOnly, "Champion Position"),
          StatBackgroundImageEnabled = IniFile.GetBoolValue(fileNameOnly, "Stat Background Image Enabled"),
          Stat1 = IniFile.GetStringValue(fileNameOnly, "Stat1"),
          Stat1Enabled = IniFile.GetBoolValue(fileNameOnly, "Stat1 Enabled"),
          Stat1FontSize = IniFile.GetFloatValue(fileNameOnly, "Stat1 Font Size"),
          Stat1FontSizeMin = IniFile.GetFloatValue(fileNameOnly, "Stat1 Font Size Min"),
          Stat1FontSizeMax = IniFile.GetFloatValue(fileNameOnly, "Stat1 Font Size Max"),
          Stat1FontColorRGBA = IniFile.GetColorValue(fileNameOnly, "Stat1 Font Color RGBA"),
          Stat1Position = IniFile.GetVector2Value(fileNameOnly, "Stat1 Position"),
          Stat2 = IniFile.GetStringValue(fileNameOnly, "Stat2"),
          Stat2Enabled = IniFile.GetBoolValue(fileNameOnly, "Stat2 Enabled"),
          Stat2FontSize = IniFile.GetFloatValue(fileNameOnly, "Stat2 Font Size"),
          Stat2FontSizeMin = IniFile.GetFloatValue(fileNameOnly, "Stat2 Font Size Min"),
          Stat2FontSizeMax = IniFile.GetFloatValue(fileNameOnly, "Stat2 Font Size Max"),
          Stat2FontColorRGBA = IniFile.GetColorValue(fileNameOnly, "Stat2 Font Color RGBA"),
          Stat2Position = IniFile.GetVector2Value(fileNameOnly, "Stat2 Position"),
          Stat3 = IniFile.GetStringValue(fileNameOnly, "Stat3"),
          Stat3Enabled = IniFile.GetBoolValue(fileNameOnly, "Stat3 Enabled"),
          Stat3FontSize = IniFile.GetFloatValue(fileNameOnly, "Stat3 Font Size"),
          Stat3FontSizeMin = IniFile.GetFloatValue(fileNameOnly, "Stat3 Font Size Min"),
          Stat3FontSizeMax = IniFile.GetFloatValue(fileNameOnly, "Stat3 Font Size Max"),
          Stat3FontColorRGBA = IniFile.GetColorValue(fileNameOnly, "Stat3 Font Color RGBA"),
          Stat3Position = IniFile.GetVector2Value(fileNameOnly, "Stat3 Position"),
          Stat4 = IniFile.GetStringValue(fileNameOnly, "Stat4"),
          Stat4Enabled = IniFile.GetBoolValue(fileNameOnly, "Stat4 Enabled"),
          Stat4FontSize = IniFile.GetFloatValue(fileNameOnly, "Stat4 Font Size"),
          Stat4FontSizeMin = IniFile.GetFloatValue(fileNameOnly, "Stat4 Font Size Min"),
          Stat4FontSizeMax = IniFile.GetFloatValue(fileNameOnly, "Stat4 Font Size Max"),
          Stat4FontColorRGBA = IniFile.GetColorValue(fileNameOnly, "Stat4 Font Color RGBA"),
          Stat4Position = IniFile.GetVector2Value(fileNameOnly, "Stat4 Position"),
          ArtistText = IniFile.GetStringValue(fileNameOnly, "Artist Text"),
          ArtistTextEnabled = IniFile.GetBoolValue(fileNameOnly, "Artist Text Enabled"),
          ArtistTextFontSize = IniFile.GetFloatValue(fileNameOnly, "Artist Text Font Size"),
          ArtistTextFontSizeMin = IniFile.GetFloatValue(fileNameOnly, "Artist Text Font Size Min"),
          ArtistTextFontSizeMax = IniFile.GetFloatValue(fileNameOnly, "Artist Text Font Size Max"),
          ArtistTextFontColorRGBA = IniFile.GetColorValue(fileNameOnly, "Artist Text Font Color RGBA"),
          ArtistTextPosition = IniFile.GetVector2Value(fileNameOnly, "Artist Text Position"),
          CompanyText = IniFile.GetStringValue(fileNameOnly, "Company Text"),
          CompanyTextEnabled = IniFile.GetBoolValue(fileNameOnly, "Company Text Enabled"),
          CompanyTextFontSize = IniFile.GetFloatValue(fileNameOnly, "Company Text Font Size"),
          CompanyTextFontSizeMin = IniFile.GetFloatValue(fileNameOnly, "Company Text Font Size Min"),
          CompanyTextFontSizeMax = IniFile.GetFloatValue(fileNameOnly, "Company Text Font Size Max"),
          CompanyTextFontColorRGBA = IniFile.GetColorValue(fileNameOnly, "Company Text Font Color RGBA"),
          CompanyTextPosition = IniFile.GetVector2Value(fileNameOnly, "Company Text Position"),
          RemoveMonsterImageSizeLimit = IniFile.GetBoolValue(fileNameOnly, "Remove Monster Image Size Limit"),
          MonsterImageSize = IniFile.GetVector2Value(fileNameOnly, "Monster Image Size"),
          MonsterImagePosition = IniFile.GetVector2Value(fileNameOnly, "Monster Image Position")
        });
      }
      else
        CacheHandler.LogError("Can't find INI file - " + fileToLoad);
    }

    public static void CacheImagesFromLists(
      List<string> imageList,
      List<Sprite> spriteCache,
      string filePath)
    {
      if (imageList == null || spriteCache == null || string.IsNullOrWhiteSpace(filePath))
      {
        CacheHandler.LogError("Invalid input parameters.");
      }
      else
      {
        foreach (string image in imageList)
        {
          string path = Path.Combine(filePath, image + ".png");
          if (File.Exists(path))
          {
            Sprite customImage = ImageSwapHandler.GetCustomImage(image, filePath);
            if ((Object) customImage != (Object) null && !spriteCache.Contains(customImage))
              spriteCache.Add(customImage);
          }
          else
            CacheHandler.LogError("File not found -- " + path);
        }
      }
    }

public static void CacheAllImages()
{
    CacheHandler.Log("Starting to cache images");

    CacheHandler.cardExtrasImagesCache.Clear();
    CacheHandler.catJobPackImagesCache.Clear();
    CacheHandler.fantasyRPGPackImagesCache.Clear();
    CacheHandler.ghostPackImagesCache.Clear();
    CacheHandler.megabotPackImagesCache.Clear();
    CacheHandler.tetramonPackImagesCache.Clear();

    try
    {
        CacheHandler.Log("Caching Card Extras Images");
        CacheHandler.CacheImagesFromLists(CacheFileListHandler.cardExtrasImagesFileNameList, CacheHandler.cardExtrasImagesCache, PlayerPatches.cardExtrasImages);
        CacheHandler.Log("Finished caching Card Extras Images. Count: " + CacheHandler.cardExtrasImagesCache.Count);
    }
    catch (Exception ex)
    {
        CacheHandler.LogError("Error caching Card Extras Images: " + ex.ToString());
    }

    try
    {
        CacheHandler.Log("Caching CatJob Images");
        CacheHandler.CacheImagesFromLists(CacheFileListHandler.catJobPackImagesFileNameList, CacheHandler.catJobPackImagesCache, PlayerPatches.catJobPackImages);
        CacheHandler.Log("Finished caching CatJob Images. Count: " + CacheHandler.catJobPackImagesCache.Count);
    }
    catch (Exception ex)
    {
        CacheHandler.LogError("Error caching CatJob Images: " + ex.ToString());
    }

    try
    {
        CacheHandler.Log("Caching Fantasy RPG Images");
        CacheHandler.CacheImagesFromLists(CacheFileListHandler.fantasyRPGPackImagesFileNameList, CacheHandler.fantasyRPGPackImagesCache, PlayerPatches.fantasyPackImages);
        CacheHandler.Log("Finished caching Fantasy RPG Images. Count: " + CacheHandler.fantasyRPGPackImagesCache.Count);
    }
    catch (Exception ex)
    {
        CacheHandler.LogError("Error caching Fantasy RPG Images: " + ex.ToString());
    }

    try
    {
        CacheHandler.Log("Caching Ghost Pack Images");
        CacheHandler.CacheImagesFromLists(CacheFileListHandler.ghostPackImagesFileNameList, CacheHandler.ghostPackImagesCache, PlayerPatches.ghostPackImages);
        CacheHandler.Log("Finished caching Ghost Pack Images. Count: " + CacheHandler.ghostPackImagesCache.Count);
    }
    catch (Exception ex)
    {
        CacheHandler.LogError("Error caching Ghost Pack Images: " + ex.ToString());
    }

    try
    {
        CacheHandler.Log("Caching Megabot Pack Images");
        CacheHandler.CacheImagesFromLists(CacheFileListHandler.megabotPackImagesFileNameList, CacheHandler.megabotPackImagesCache, PlayerPatches.megabotPackImages);
        CacheHandler.Log("Finished caching Megabot Pack Images. Count: " + CacheHandler.megabotPackImagesCache.Count);
    }
    catch (Exception ex)
    {
        CacheHandler.LogError("Error caching Megabot Pack Images: " + ex.ToString());
    }

    try
    {
        CacheHandler.Log("Caching Tetramon Pack Images");
        CacheHandler.CacheImagesFromLists(CacheFileListHandler.tetramonPackImagesFileNameList, CacheHandler.tetramonPackImagesCache, PlayerPatches.tetramonPackImages);
        CacheHandler.Log("Finished caching Tetramon Pack Images. Count: " + CacheHandler.tetramonPackImagesCache.Count);
    }
    catch (Exception ex)
    {
        CacheHandler.LogError("Error caching Tetramon Pack Images: " + ex.ToString());
    }

    CacheHandler.Log("Completed image caching");
}



    public static bool VerifyFullImageCaches()
    {
      bool flag1 = CacheHandler.cardExtrasImagesCache.Count == 52;
      bool flag2 = CacheHandler.catJobPackImagesCache.Count == 40;
      bool flag3 = CacheHandler.fantasyRPGPackImagesCache.Count == 50;
      bool flag4 = CacheHandler.ghostPackImagesCache.Count == 19;
      bool flag5 = CacheHandler.megabotPackImagesCache.Count == 113;
      bool flag6 = CacheHandler.tetramonPackImagesCache.Count == 121;
      if (!flag1)
        CacheHandler.LogError("Card Extras Image Cache NOT FULL. Count should be " + 52.ToString() + " but it is " + CacheHandler.cardExtrasImagesCache.Count.ToString());
      if (!flag2)
        CacheHandler.LogError("CatJob Image Cache NOT FULL. Count should be " + 40.ToString() + " but it is " + CacheHandler.catJobPackImagesCache.Count.ToString());
      if (!flag3)
        CacheHandler.LogError("FantasyRPG Image Cache NOT FULL. Count should be " + 50.ToString() + " but it is " + CacheHandler.fantasyRPGPackImagesCache.Count.ToString());
      if (!flag4)
        CacheHandler.LogError("Ghost Image Cache NOT FULL. Count should be " + 19.ToString() + " but it is " + CacheHandler.ghostPackImagesCache.Count.ToString());
      if (!flag5)
        CacheHandler.LogError("Megabot Image Cache NOT FULL. Count should be " + 113.ToString() + " but it is " + CacheHandler.megabotPackImagesCache.Count.ToString());
      if (!flag6)
        CacheHandler.LogError("Tetramon Image Cache NOT FULL. Count should be " + 121.ToString() + " but it is " + CacheHandler.tetramonPackImagesCache.Count.ToString());
      if (!(flag1 & flag2 & flag3 & flag4 & flag5 & flag6))
      {
        CacheHandler.LogError("Image caches are not all full");
        return false;
      }
      CacheHandler.Log("Image caches populated successfully");
      return true;
    }

    public static void Log(string log) => MinaCardsModPlugin.Log.LogInfo((object) log);

    public static void LogError(string log) => MinaCardsModPlugin.Log.LogError((object) log);
  }
}