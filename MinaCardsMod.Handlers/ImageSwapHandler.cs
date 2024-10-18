using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

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

    public static Material GetMaterialByName(string name)
    {
      foreach (Material materialByName in Resources.FindObjectsOfTypeAll<Material>())
      {
        if (materialByName?.name == name)
          return materialByName;
      }
      return (Material) null;
    }

    public static void CloneOriginalCardBackTexture()
    {
      Material materialByName = ImageSwapHandler.GetMaterialByName("MAT_CardBackMesh");
      if (!((UnityEngine.Object) materialByName != (UnityEngine.Object) null))
        return;
      Texture2D mainTexture = materialByName.mainTexture as Texture2D;
      CacheHandler.originalCardBackTexture = Sprite.Create(mainTexture, new Rect(0.0f, 0.0f, (float) mainTexture.width, (float) mainTexture.height), Vector2.zero);
      CacheHandler.originalCardBackTexture.name = "T_CardBackMesh";
    }

    public static MonsterData CreateCopy(MonsterData original)
    {
      return new MonsterData()
      {
        ArtistName = original.ArtistName,
        BaseStats = original.BaseStats,
        Description = original.Description,
        EffectAmount = original.EffectAmount,
        ElementIndex = original.ElementIndex,
        GhostIcon = original.GhostIcon,
        Icon = original.Icon,
        MonsterType = original.MonsterType,
        Name = original.Name,
        NextEvolution = original.NextEvolution,
        PreviousEvolution = original.PreviousEvolution,
        Rarity = original.Rarity,
        Roles = original.Roles,
        SkillList = original.SkillList
      };
    }

    public static void GetOriginalMonsterDatas()
    {
      foreach (MonsterData data in CSingleton<InventoryBase>.Instance.m_MonsterData_SO.m_DataList)
        CacheHandler.originalMonsterDataList.Add(ImageSwapHandler.CreateCopy(data));
      foreach (MonsterData catJobData in CSingleton<InventoryBase>.Instance.m_MonsterData_SO.m_CatJobDataList)
        CacheHandler.originalCatJobDataList.Add(ImageSwapHandler.CreateCopy(catJobData));
      foreach (MonsterData fantasyRpgData in CSingleton<InventoryBase>.Instance.m_MonsterData_SO.m_FantasyRPGDataList)
        CacheHandler.originalFantasyRPGDataList.Add(ImageSwapHandler.CreateCopy(fantasyRpgData));
      foreach (MonsterData megabotData in CSingleton<InventoryBase>.Instance.m_MonsterData_SO.m_MegabotDataList)
        CacheHandler.originalMegabotDataList.Add(ImageSwapHandler.CreateCopy(megabotData));
      ImageSwapHandler.Log("Cloned all original monster data");
    }

    public static void GetOriginalGhostMonsterSprites()
    {
      if (CacheHandler.clonedOriginalGhostMonsterImages)
        return;
      foreach (EMonsterType shownMonster in InventoryBase.GetShownMonsterList(ECardExpansionType.Ghost))
      {
        MonsterData monsterData = InventoryBase.GetMonsterData(shownMonster);
        if (monsterData != null)
        {
          Sprite ghostIcon = monsterData.GhostIcon;
          if ((UnityEngine.Object) ghostIcon != (UnityEngine.Object) null)
          {
            ghostIcon.name = "Ghost_" + shownMonster.ToString();
            CacheHandler.originalGhostMonsterImageList.Add(ghostIcon);
            if (CacheHandler.originalGhostMonsterImageList.Count == 19)
              CacheHandler.clonedOriginalGhostMonsterImages = true;
          }
        }
      }
    }

    public static void GetOriginalMonsterSprites()
    {
      if (CacheHandler.clonedOriginalMonsterImages)
        return;
      foreach (EMonsterType monsterType in Enum.GetValues(typeof (EMonsterType)))
      {
        if (monsterType >= EMonsterType.PiggyA)
        {
          if (monsterType > EMonsterType.FireChickenB)
            break;
          if (monsterType >= EMonsterType.PiggyA && monsterType <= EMonsterType.FireChickenB)
          {
            MonsterData monsterData = InventoryBase.GetMonsterData(monsterType);
            if (monsterData != null)
            {
              Sprite icon = monsterData.Icon;
              if ((UnityEngine.Object) icon != (UnityEngine.Object) null)
              {
                icon.name = monsterType.ToString();
                CacheHandler.originalTetramonMonsterImageList.Add(icon);
                if (CacheHandler.originalTetramonMonsterImageList.Count == 121)
                  CacheHandler.clonedOriginalMonsterImages = true;
              }
            }
          }
        }
      }
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
      ImageSwapHandler.CloneOriginalCardExtrasSprites();
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
      Texture2D texture = ImageSwapHandler.LoadCustomPNG(fileName, imagePath);
      if (!((UnityEngine.Object) texture != (UnityEngine.Object) null))
        return (Sprite) null;
      Sprite customImage = Sprite.Create(texture, new Rect(0.0f, 0.0f, (float) texture.width, (float) texture.height), Vector2.zero);
      customImage.name = fileName;
      return customImage;
    }

    public static void SetBaseMonsterIcons()
    {
      foreach (EMonsterType emonsterType in Enum.GetValues(typeof (EMonsterType)))
      {
        EMonsterType monsterType = emonsterType;
        if (monsterType >= EMonsterType.PiggyA)
        {
          if (monsterType > EMonsterType.FireChickenB)
            break;
          string monsterName = monsterType.ToString();
          monsterName = !(monsterName == "EmeraldA") ? (!(monsterName == "EmeraldB") ? (!(monsterName == "EmeraldC") ? (!(monsterName == "MummyMan") ? monsterType.ToString() : "Mummy") : "CrystalC") : "CrystalB") : "CrystalA";
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
              if (!MinaCardsModPlugin.CustomBaseMonsterImages.Value)
              {
                Sprite sprite2 = CacheHandler.originalTetramonMonsterImageList.FirstOrDefault<Sprite>((Func<Sprite, bool>) (sprite => sprite.name == monsterType.ToString()));
                if ((UnityEngine.Object) sprite2 != (UnityEngine.Object) null)
                  monsterData.Icon = sprite2;
              }
            }
          }
        }
      }
    }

    public static void SetBaseGhostMonsterIcons()
    {
      foreach (EMonsterType shownMonster in InventoryBase.GetShownMonsterList(ECardExpansionType.Ghost))
      {
        EMonsterType monsterType = shownMonster;
        MonsterData monsterData = InventoryBase.GetMonsterData(monsterType);
        if (monsterData != null)
        {
          if (MinaCardsModPlugin.CustomBaseMonsterImages.Value && (UnityEngine.Object) monsterData.GhostIcon != (UnityEngine.Object) null)
          {
            Sprite sprite1 = CacheHandler.ghostPackImagesCache.FirstOrDefault<Sprite>((Func<Sprite, bool>) (sprite => sprite.name == "Ghost_" + monsterType.ToString()));
            if ((UnityEngine.Object) sprite1 != (UnityEngine.Object) null)
            {
              monsterData.GhostIcon = sprite1;
              monsterData.GhostIcon.name = "Ghost_" + monsterType.ToString();
            }
          }
          if (!MinaCardsModPlugin.CustomBaseMonsterImages.Value && (UnityEngine.Object) monsterData.GhostIcon != (UnityEngine.Object) null)
          {
            Sprite sprite2 = CacheHandler.originalGhostMonsterImageList.FirstOrDefault<Sprite>((Func<Sprite, bool>) (sprite => sprite.name == "Ghost_" + monsterType.ToString()));
            if ((UnityEngine.Object) sprite2 != (UnityEngine.Object) null)
            {
              monsterData.GhostIcon = sprite2;
              monsterData.GhostIcon.name = "Ghost_" + monsterType.ToString();
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