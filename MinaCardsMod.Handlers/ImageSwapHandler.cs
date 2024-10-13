using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;

#nullable disable
namespace MinaCardsMod.Handlers
{
  internal class ImageSwapHandler
  {
    public static void CloneSpriteList(List<Sprite> sourceList, ref List<Sprite> targetList)
    {
      if (sourceList == null)
      {
        ImageSwapHandler.Log("Source list is null");
      }
      else
      {
        targetList = new List<Sprite>();
        foreach (Sprite source in sourceList)
        {
          if ((UnityEngine.Object) source != (UnityEngine.Object) null)
          {
            Sprite sprite = UnityEngine.Object.Instantiate<Sprite>(source);
            sprite.name = source.name;
            targetList.Add(sprite);
          }
          else
            ImageSwapHandler.Log("Encountered null sprite in source list");
        }
      }
    }

    public static void CloneOriginalCardBackTexture()
    {
      List<Sprite> loadedSpriteList = CSingleton<LoadStreamTexture>.Instance.m_LoadedSpriteList;
      if (loadedSpriteList == null)
        return;
      foreach (Sprite original in loadedSpriteList)
      {
        if ((UnityEngine.Object) original != (UnityEngine.Object) null && original.name == "T_CardBackMesh")
        {
          CacheHandler.originalCardBackTexture = UnityEngine.Object.Instantiate<Sprite>(original);
          CacheHandler.originalCardBackTexture.name = "T_CardBackMesh";
        }
      }
    }

    public static void GetOriginalMonsterSprites()
    {
      if (!CacheHandler.clonedOriginalMonsterImages)
      {
        foreach (EMonsterType monsterType in Enum.GetValues(typeof (EMonsterType)))
        {
          if (monsterType >= EMonsterType.PiggyA)
          {
            if (monsterType > EMonsterType.FireChickenB)
              return;
            if (monsterType >= EMonsterType.PiggyA && monsterType <= EMonsterType.FireChickenB)
            {
              MonsterData monsterData = InventoryBase.GetMonsterData(monsterType);
              if (monsterData != null && (UnityEngine.Object) monsterData.Icon != (UnityEngine.Object) null)
                CacheHandler.originalTetramonMonsterImageList.Add(monsterData.Icon);
            }
          }
        }
      }
      if (CacheHandler.originalTetramonMonsterImageList.Count != 121)
        return;
      CacheHandler.clonedOriginalMonsterImages = true;
    }

    public static void CloneOriginalSpriteLists()
    {
      if (CacheHandler.clonedOriginalSpriteLists)
        return;
      List<Sprite> borderSpriteList = CSingleton<CardUI>.Instance.m_CardBorderSpriteList;
      List<Sprite> cardBorderList = CSingleton<InventoryBase>.Instance.m_MonsterData_SO.m_CardBorderList;
      List<Sprite> cardBgList = CSingleton<InventoryBase>.Instance.m_MonsterData_SO.m_CardBGList;
      List<Sprite> cardFrontImageList = CSingleton<InventoryBase>.Instance.m_MonsterData_SO.m_CardFrontImageList;
      List<Sprite> cardBackImageList = CSingleton<InventoryBase>.Instance.m_MonsterData_SO.m_CardBackImageList;
      List<Sprite> foilMaskImageList = CSingleton<InventoryBase>.Instance.m_MonsterData_SO.m_CardFoilMaskImageList;
      ImageSwapHandler.CloneSpriteList(borderSpriteList, ref CacheHandler.originalCardEditionBorderList);
      ImageSwapHandler.CloneSpriteList(cardBorderList, ref CacheHandler.originalCardBorderList);
      ImageSwapHandler.CloneSpriteList(cardBgList, ref CacheHandler.originalCardBGList);
      ImageSwapHandler.CloneSpriteList(cardFrontImageList, ref CacheHandler.originalCardFrontImageList);
      ImageSwapHandler.CloneSpriteList(cardBackImageList, ref CacheHandler.originalCardBackImageList);
      ImageSwapHandler.CloneSpriteList(foilMaskImageList, ref CacheHandler.originalCardFoilMaskImageList);
      CacheHandler.clonedOriginalSpriteLists = true;
    }

    public static void CloneOriginalCardExtrasSprites()
    {
      foreach (Sprite originalCardBg in CacheHandler.originalCardBGList)
      {
        if (!CacheHandler.originalCardExtrasImagesList.Contains(originalCardBg))
          CacheHandler.originalCardExtrasImagesList.Add(originalCardBg);
      }
      foreach (Sprite originalCardBorder in CacheHandler.originalCardBorderList)
      {
        if (!CacheHandler.originalCardExtrasImagesList.Contains(originalCardBorder))
          CacheHandler.originalCardExtrasImagesList.Add(originalCardBorder);
      }
      foreach (Sprite cardEditionBorder in CacheHandler.originalCardEditionBorderList)
      {
        if (!CacheHandler.originalCardExtrasImagesList.Contains(cardEditionBorder))
          CacheHandler.originalCardExtrasImagesList.Add(cardEditionBorder);
      }
      foreach (Sprite cardFoilMaskImage in CacheHandler.originalCardFoilMaskImageList)
      {
        if (!CacheHandler.originalCardExtrasImagesList.Contains(cardFoilMaskImage))
          CacheHandler.originalCardExtrasImagesList.Add(cardFoilMaskImage);
      }
      foreach (Sprite originalCardFrontImage in CacheHandler.originalCardFrontImageList)
      {
        if (!CacheHandler.originalCardExtrasImagesList.Contains(originalCardFrontImage))
          CacheHandler.originalCardExtrasImagesList.Add(originalCardFrontImage);
      }
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
      Texture2D texture = ImageSwapHandler.LoadCustomPNG(fileName, imagePath);
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

    public static void SetBaseMonsterIcons()
    {
      foreach (EMonsterType monsterType in Enum.GetValues(typeof (EMonsterType)))
      {
        if (monsterType >= EMonsterType.PiggyA)
        {
          if (monsterType > EMonsterType.FireChickenB)
            break;
          string monsterName = monsterType.ToString();
          if (monsterName == "EmeraldA")
            monsterName = "CrystalA";
          else if (monsterName == "EmeraldB")
            monsterName = "CrystalB";
          else if (monsterName == "EmeraldC")
            monsterName = "CrystalC";
          else if (monsterName == "MummyMan")
            monsterName = "Mummy";
          if (monsterType >= EMonsterType.PiggyA && monsterType <= EMonsterType.FireChickenB)
          {
            MonsterData monsterData = InventoryBase.GetMonsterData(monsterType);
            if (monsterData != null)
            {
              if (MinaCardsModPlugin.CustomBaseMonsterImages.Value)
																	
              {
                Sprite sprite1 = CacheHandler.tetramonPackImagesCache.FirstOrDefault<Sprite>((Func<Sprite, bool>) (sprite => sprite.name == monsterName));
                if ((UnityEngine.Object) sprite1 != (UnityEngine.Object) null)
                  monsterData.Icon = sprite1;
              }
              else if (!MinaCardsModPlugin.CustomBaseMonsterImages.Value)
              {
                Sprite sprite2 = CacheHandler.originalTetramonMonsterImageList.FirstOrDefault<Sprite>((Func<Sprite, bool>) (sprite => sprite.name == monsterName));
                if ((UnityEngine.Object) sprite2 != (UnityEngine.Object) null)
                  monsterData.Icon = sprite2;
              }
            }
          }
        }
      }
    }

    public static void SetMegabotMonsterIcons()
    {
      foreach (EMonsterType monsterType1 in Enum.GetValues(typeof (EMonsterType)))
      {
        if (monsterType1 >= EMonsterType.Alpha)
        {
          if (monsterType1 > EMonsterType.WingBooster)
            break;
          if (monsterType1 >= EMonsterType.Alpha && monsterType1 <= EMonsterType.WingBooster)
          {
            MonsterData monsterData1 = InventoryBase.GetMonsterData(monsterType1);
            if (monsterData1 != null)
            {
              if (MinaCardsModPlugin.CustomNewExpansionImages.Value)
              {
                string monsterName = monsterType1.ToString();
                Sprite sprite1 = CacheHandler.megabotPackImagesCache.FirstOrDefault<Sprite>((Func<Sprite, bool>) (sprite => sprite.name == monsterName));
                if ((UnityEngine.Object) sprite1 != (UnityEngine.Object) null)
                  monsterData1.Icon = sprite1;
              }
              else if (!MinaCardsModPlugin.CustomNewExpansionImages.Value)
              {
                InventoryBase.GetMonsterData(EMonsterType.PiggyA);
                int num = (int) monsterType1;
                if (num > 110)
                {
                  int monsterType2 = num;
                  if (num >= 1000 && num <= 1112)
                    monsterType2 = num < 1110 ? num - 999 : num - 1109;
                  else if (num >= 2000 && num <= 2049)
                    monsterType2 = num - 1999;
                  else if (num >= 3000 && num <= 3039)
                    monsterType2 = num - 2949;
                  MonsterData monsterData2 = InventoryBase.GetMonsterData((EMonsterType) monsterType2);
                  monsterData1.Icon = monsterData2.Icon;
                }
              }
            }
          }
        }
      }
    }

    public static void SetFantasyRPGMonsterIcons()
    {
      foreach (EMonsterType monsterType1 in Enum.GetValues(typeof (EMonsterType)))
      {
        if (monsterType1 >= EMonsterType.Archer)
        {
          if (monsterType1 > EMonsterType.WolfFantasy)
            break;
          if (monsterType1 >= EMonsterType.Archer && monsterType1 <= EMonsterType.WolfFantasy)
          {
            MonsterData monsterData1 = InventoryBase.GetMonsterData(monsterType1);
            if (monsterData1 != null)
            {
              if (MinaCardsModPlugin.CustomNewExpansionImages.Value)
              {
                string monsterName = monsterType1.ToString();
                Sprite sprite1 = CacheHandler.fantasyRPGPackImagesCache.FirstOrDefault<Sprite>((Func<Sprite, bool>) (sprite => sprite.name == monsterName));
                if ((UnityEngine.Object) sprite1 != (UnityEngine.Object) null)
                  monsterData1.Icon = sprite1;
              }
              else if (!MinaCardsModPlugin.CustomNewExpansionImages.Value)
              {
                InventoryBase.GetMonsterData(EMonsterType.PiggyA);
                int num = (int) monsterType1;
                if (num > 110)
                {
                  int monsterType2 = num;
                  if (num >= 1000 && num <= 1112)
                    monsterType2 = num < 1110 ? num - 999 : num - 1109;
                  else if (num >= 2000 && num <= 2049)
                    monsterType2 = num - 1999;
                  else if (num >= 3000 && num <= 3039)
                    monsterType2 = num - 2949;
                  MonsterData monsterData2 = InventoryBase.GetMonsterData((EMonsterType) monsterType2);
                  monsterData1.Icon = monsterData2.Icon;
                }
              }
            }
          }
        }
      }
    }

    public static void SetCatJobMonsterIcons()
    {
      foreach (EMonsterType monsterType1 in Enum.GetValues(typeof (EMonsterType)))
      {
        if (monsterType1 >= EMonsterType.EX0Teacher)
        {
          if (monsterType1 > EMonsterType.EX0Pirate)
            break;
          if (monsterType1 >= EMonsterType.EX0Teacher && monsterType1 <= EMonsterType.EX0Pirate)
          {
            MonsterData monsterData1 = InventoryBase.GetMonsterData(monsterType1);
            if (monsterData1 != null)
            {
              if (MinaCardsModPlugin.CustomNewExpansionImages.Value)
              {
                string monsterName = monsterType1.ToString();
                Sprite sprite1 = CacheHandler.catJobPackImagesCache.FirstOrDefault<Sprite>((Func<Sprite, bool>) (sprite => sprite.name == monsterName));
                if ((UnityEngine.Object) sprite1 != (UnityEngine.Object) null)
                  monsterData1.Icon = sprite1;
              }
              else if (!MinaCardsModPlugin.CustomNewExpansionImages.Value)
              {
                InventoryBase.GetMonsterData(EMonsterType.PiggyA);
                int num = (int) monsterType1;
                if (num > 110)
                {
                  int monsterType2 = num;
                  if (num >= 1000 && num <= 1112)
                    monsterType2 = num < 1110 ? num - 999 : num - 1109;
                  else if (num >= 2000 && num <= 2049)
                    monsterType2 = num - 1999;
                  else if (num >= 3000 && num <= 3039)
                    monsterType2 = num - 2949;
                  MonsterData monsterData2 = InventoryBase.GetMonsterData((EMonsterType) monsterType2);
                  monsterData1.Icon = monsterData2.Icon;
                }
              }
            }
          }
        }
      }
    }

    public static Sprite SwapSprite(List<Sprite> newSpritesList, string spriteName)
    {
      if (newSpritesList != null)
      {
        Sprite sprite1 = newSpritesList.FirstOrDefault<Sprite>((Func<Sprite, bool>) (sprite => sprite.name == spriteName));
        if ((UnityEngine.Object) sprite1 != (UnityEngine.Object) null)
          return sprite1;
        ImageSwapHandler.Log("Didn't find sprite with name " + spriteName);
      }
      else
        ImageSwapHandler.Log("Sprites list is null");
      return (Sprite) null;
    }

    public static void Log(string log) => MinaCardsModPlugin.Log.LogInfo((object) log);

    public static void LogError(string log) => MinaCardsModPlugin.Log.LogError((object) log);
  }
}