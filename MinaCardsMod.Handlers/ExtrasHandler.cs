using I2.Loc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using MinaCardsMod.Patches;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#nullable disable
namespace MinaCardsMod.Handlers
{
  internal class ExtrasHandler
  {
    public static bool hasLoadedSaveOnce;

    public static void DoReload()
    {
      ImageSwapHandler.SetBaseGhostMonsterIcons();
      ImageSwapHandler.SetBaseMonsterIcons();
      ImageSwapHandler.SetCatJobMonsterIcons();
      ImageSwapHandler.SetFantasyRPGMonsterIcons();
      ImageSwapHandler.SetMegabotMonsterIcons();
      ExtrasHandler.SetPreviousEvolutions();
      ExtrasHandler.swapPackNames();
      ExtrasHandler.SwapNewPackItemImages();
    }

    public static void DoFirstWorldLoad()
    {
      if (!ExtrasHandler.hasLoadedSaveOnce && CSingleton<CGameManager>.Instance.m_IsGameLevel)
      {
        if (MinaCardsModPlugin.CustomBaseMonsterImages.Value)
        {
          MinaCardsModPlugin.CustomBaseMonsterImages.Value = false;
          ImageSwapHandler.GetOriginalMonsterSprites();
          ImageSwapHandler.GetOriginalGhostMonsterSprites();
          MinaCardsModPlugin.CustomBaseMonsterImages.Value = true;
          ExtrasHandler.hasLoadedSaveOnce = true;
        }
        else if (!MinaCardsModPlugin.CustomBaseMonsterImages.Value)
        {
          ImageSwapHandler.GetOriginalMonsterSprites();
          ImageSwapHandler.GetOriginalGhostMonsterSprites();
          ExtrasHandler.hasLoadedSaveOnce = true;
        }
        PlayerPatches.Log("Copying original monster images to cache");
        ExtrasHandler.DoFirstLoad();
      }
      else
      {
        if (!ExtrasHandler.hasLoadedSaveOnce || !CSingleton<CGameManager>.Instance.m_IsGameLevel)
          return;
        ExtrasHandler.DoReload();
      }
    }

    public static void DoFirstLoad()
    {
      ImageSwapHandler.CloneOriginalSpriteLists();
      ImageSwapHandler.CloneOriginalCardBackTexture();
      ImageSwapHandler.GetOriginalMonsterDatas();
      ImageSwapHandler.SetBaseGhostMonsterIcons();
      ImageSwapHandler.SetBaseMonsterIcons();
      ImageSwapHandler.SetCatJobMonsterIcons();
      ImageSwapHandler.SetFantasyRPGMonsterIcons();
      ImageSwapHandler.SetMegabotMonsterIcons();
      ExtrasHandler.swapPackNames();
      ExtrasHandler.AddHiddenCards();
    }

    public static CustomCardObject GetNewMonsterDataFromCacheByName(
      List<CustomCardObject> customCardList,
      string monsterType)
    {
      foreach (CustomCardObject customCard in customCardList)
      {
        if (customCard.Header == monsterType)
          return customCard;
      }
      return (CustomCardObject) null;
    }

    public static MonsterData GetOldMonsterDataFromCacheByName(
      List<MonsterData> monsterDataList,
      string monsterType)
    {
      foreach (MonsterData monsterData in monsterDataList)
      {
        if (monsterData.MonsterType.ToString() == monsterType)
          return monsterData;
      }
      return (MonsterData) null;
    }

    public static void SetPreviousEvolutions()
    {
      foreach (MonsterData data in CSingleton<InventoryBase>.Instance.m_MonsterData_SO.m_DataList)
      {
        if (MinaCardsModPlugin.CustomBaseConfigs.Value)
        {
          CustomCardObject dataFromCacheByName = ExtrasHandler.GetNewMonsterDataFromCacheByName(CacheHandler.tetramonConfigCache, data.MonsterType.ToString());
          EMonsterType result;
          if (dataFromCacheByName != null && Enum.TryParse<EMonsterType>(dataFromCacheByName.PreviousEvolution, out result))
            data.PreviousEvolution = result;
        }
        else if (!MinaCardsModPlugin.CustomBaseConfigs.Value)
        {
          MonsterData dataFromCacheByName = ExtrasHandler.GetOldMonsterDataFromCacheByName(CacheHandler.originalMonsterDataList, data.MonsterType.ToString());
          if (dataFromCacheByName != null)
            data.PreviousEvolution = dataFromCacheByName.PreviousEvolution;
        }
      }
      foreach (MonsterData catJobData in CSingleton<InventoryBase>.Instance.m_MonsterData_SO.m_CatJobDataList)
      {
        if (MinaCardsModPlugin.CustomBaseConfigs.Value)
        {
          CustomCardObject dataFromCacheByName = ExtrasHandler.GetNewMonsterDataFromCacheByName(CacheHandler.catJobConfigCache, catJobData.MonsterType.ToString());
          EMonsterType result;
          if (dataFromCacheByName != null && Enum.TryParse<EMonsterType>(dataFromCacheByName.PreviousEvolution, out result))
            catJobData.PreviousEvolution = result;
        }
        else if (!MinaCardsModPlugin.CustomBaseConfigs.Value)
        {
          MonsterData dataFromCacheByName = ExtrasHandler.GetOldMonsterDataFromCacheByName(CacheHandler.originalCatJobDataList, catJobData.MonsterType.ToString());
          if (dataFromCacheByName != null)
            catJobData.PreviousEvolution = dataFromCacheByName.PreviousEvolution;
        }
      }
      foreach (MonsterData fantasyRpgData in CSingleton<InventoryBase>.Instance.m_MonsterData_SO.m_FantasyRPGDataList)
      {
        if (MinaCardsModPlugin.CustomBaseConfigs.Value)
        {
          CustomCardObject dataFromCacheByName = ExtrasHandler.GetNewMonsterDataFromCacheByName(CacheHandler.fantasyRPGConfigCache, fantasyRpgData.MonsterType.ToString());
          EMonsterType result;
          if (dataFromCacheByName != null && Enum.TryParse<EMonsterType>(dataFromCacheByName.PreviousEvolution, out result))
            fantasyRpgData.PreviousEvolution = result;
        }
        else if (!MinaCardsModPlugin.CustomBaseConfigs.Value)
        {
          MonsterData dataFromCacheByName = ExtrasHandler.GetOldMonsterDataFromCacheByName(CacheHandler.originalFantasyRPGDataList, fantasyRpgData.MonsterType.ToString());
          if (dataFromCacheByName != null)
            fantasyRpgData.PreviousEvolution = dataFromCacheByName.PreviousEvolution;
        }
      }
      foreach (MonsterData megabotData in CSingleton<InventoryBase>.Instance.m_MonsterData_SO.m_MegabotDataList)
      {
        if (MinaCardsModPlugin.CustomBaseConfigs.Value)
        {
          CustomCardObject dataFromCacheByName = ExtrasHandler.GetNewMonsterDataFromCacheByName(CacheHandler.megabotConfigCache, megabotData.MonsterType.ToString());
          EMonsterType result;
          if (dataFromCacheByName != null && Enum.TryParse<EMonsterType>(dataFromCacheByName.PreviousEvolution, out result))
            megabotData.PreviousEvolution = result;
        }
        else if (!MinaCardsModPlugin.CustomBaseConfigs.Value)
        {
          MonsterData dataFromCacheByName = ExtrasHandler.GetOldMonsterDataFromCacheByName(CacheHandler.originalMegabotDataList, megabotData.MonsterType.ToString());
          if (dataFromCacheByName != null)
            megabotData.PreviousEvolution = dataFromCacheByName.PreviousEvolution;
        }
      }
    }

    public static void SetCardExtrasImages(CardUI __instance, CardData cardData)
    {
      bool flag1 = false;
      bool isGhost = cardData.expansionType == ECardExpansionType.Ghost;
      CardUI cardUi = !isGhost || !((UnityEngine.Object) __instance.m_GhostCard != (UnityEngine.Object) null) ? __instance : ExtrasHandler.CurrentCardUI(isGhost, __instance, __instance.m_GhostCard);
      bool flag2 = cardData.expansionType == ECardExpansionType.Megabot || cardData.expansionType == ECardExpansionType.FantasyRPG || cardData.expansionType == ECardExpansionType.CatJob;
      if (MinaCardsModPlugin.CustomBaseMonsterImages.Value && !flag2)
        flag1 = true;
      if (MinaCardsModPlugin.CustomNewExpansionImages.Value && flag2)
        flag1 = true;
      if (flag1)
      {
        foreach (Sprite sprite in CacheHandler.cardExtrasImagesCache)
        {
          if ((UnityEngine.Object) sprite != (UnityEngine.Object) null)
          {
            Image componentBySpriteName = ExtrasHandler.GetImageComponentBySpriteName(cardUi.gameObject, sprite.name);
            if ((UnityEngine.Object) componentBySpriteName != (UnityEngine.Object) null)
              componentBySpriteName.sprite = sprite;
          }
        }
      }
      else
      {
        if (flag1)
          return;
        foreach (Sprite cardExtrasImages in CacheHandler.originalCardExtrasImagesList)
        {
          if ((UnityEngine.Object) cardExtrasImages != (UnityEngine.Object) null)
          {
            Image componentBySpriteName = ExtrasHandler.GetImageComponentBySpriteName(cardUi.gameObject, cardExtrasImages.name);
            if ((UnityEngine.Object) componentBySpriteName != (UnityEngine.Object) null)
              componentBySpriteName.sprite = cardExtrasImages;
          }
        }
      }
    }
    
    public static void swapPackNames()
    {
      if (File.Exists(PlayerPatches.configPath + "Extras.ini"))
      {
        IniFile.Load(PlayerPatches.configPath + "Extras.ini");
        if (IniFile.GetStringValue("Extras", "CatJob Pack Name") != null)
          PlayerPatches.newCatJobPackName = IniFile.GetStringValue("Extras", "CatJob Pack Name");
        if (IniFile.GetStringValue("Extras", "FantasyRPG Pack Name") != null)
          PlayerPatches.newFantasyRPGPackName = IniFile.GetStringValue("Extras", "FantasyRPG Pack Name");
        if (IniFile.GetStringValue("Extras", "Megabot Pack Name") == null)
          return;
        PlayerPatches.newMegaBotPackName = IniFile.GetStringValue("Extras", "Megabot Pack Name");
      }
      else
        PlayerPatches.LogError("File doesn't exist " + PlayerPatches.configPath + "Extras.ini");
    }

    public static void SwapNewPackItemImages()
    {
      string translation = LocalizationManager.GetTranslation("Pack");
      InventoryBase.GetItemMeshData(EItemType.BasicCardPack);
      ItemData itemData1 = InventoryBase.GetItemData(EItemType.CatJobPack);
      ItemData itemData2 = InventoryBase.GetItemData(EItemType.FantasyRPGPack);
      ItemData itemData3 = InventoryBase.GetItemData(EItemType.MegabotPack);
      ItemData itemData4 = InventoryBase.GetItemData(EItemType.GhostPack);
      Sprite sprite1 = CacheHandler.cardExtrasImagesCache.FirstOrDefault<Sprite>((Func<Sprite, bool>) (sprite => sprite.name == "PackCatJob"));
      Sprite sprite2 = CacheHandler.cardExtrasImagesCache.FirstOrDefault<Sprite>((Func<Sprite, bool>) (sprite => sprite.name == "PackFantasyRPG"));
      Sprite sprite3 = CacheHandler.cardExtrasImagesCache.FirstOrDefault<Sprite>((Func<Sprite, bool>) (sprite => sprite.name == "PackMegabot"));
      Sprite sprite4 = CacheHandler.cardExtrasImagesCache.FirstOrDefault<Sprite>((Func<Sprite, bool>) (sprite => sprite.name == "GhostPack"));
      itemData1.name = PlayerPatches.newCatJobPackName + " " + translation;
      itemData2.name = PlayerPatches.newFantasyRPGPackName + " " + translation;
      itemData3.name = PlayerPatches.newMegaBotPackName + " " + translation;
      itemData1.icon = sprite1;
      itemData2.icon = sprite2;
      itemData3.icon = sprite3;
      itemData4.icon = sprite4;
      ItemMeshData itemMeshData1 = InventoryBase.GetItemMeshData(EItemType.BasicCardPack);
      ItemMeshData itemMeshData2 = InventoryBase.GetItemMeshData(EItemType.CatJobPack);
      ItemMeshData itemMeshData3 = InventoryBase.GetItemMeshData(EItemType.FantasyRPGPack);
      ItemMeshData itemMeshData4 = InventoryBase.GetItemMeshData(EItemType.MegabotPack);
      ItemMeshData itemMeshData5 = InventoryBase.GetItemMeshData(EItemType.GhostPack);
      Texture2D texture1 = CacheHandler.cardExtrasImagesCache.FirstOrDefault<Sprite>((Func<Sprite, bool>) (sprite => sprite.name == "T_CardPackCatJob")).texture;
      Texture2D texture2 = CacheHandler.cardExtrasImagesCache.FirstOrDefault<Sprite>((Func<Sprite, bool>) (sprite => sprite.name == "T_CardPackFantasy")).texture;
      Texture2D texture3 = CacheHandler.cardExtrasImagesCache.FirstOrDefault<Sprite>((Func<Sprite, bool>) (sprite => sprite.name == "T_CardPackGhost")).texture;
      Texture2D texture4 = CacheHandler.cardExtrasImagesCache.FirstOrDefault<Sprite>((Func<Sprite, bool>) (sprite => sprite.name == "T_CardPackMegabot")).texture;
      Material material1 = new Material(itemMeshData1.material);
      material1.mainTexture = (Texture) texture2;
      material1.mainTexture.name = "T_CardPackFantasy";
      itemMeshData3.material = material1;
      Material material2 = new Material(itemMeshData1.material);
      material2.mainTexture = (Texture) texture1;
      material2.mainTexture.name = "T_CardPackCatJob";
      itemMeshData2.material = material2;
      Material material3 = new Material(itemMeshData1.material);
      material3.mainTexture = (Texture) texture4;
      material3.mainTexture.name = "T_CardPackMegabot";
      itemMeshData4.material = material3;
      Material material4 = new Material(itemMeshData1.material);
      material4.mainTexture = (Texture) texture3;
      material4.mainTexture.name = "T_CardPackGhost";
      itemMeshData5.material = material4;
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

    public static bool isCardConfigDriven(CardData cardData)
    {
      return cardData.expansionType == ECardExpansionType.Tetramon || cardData.expansionType == ECardExpansionType.Destiny || cardData.expansionType == ECardExpansionType.Ghost ? MinaCardsModPlugin.CustomBaseConfigs.Value : (cardData.expansionType == ECardExpansionType.CatJob || cardData.expansionType == ECardExpansionType.FantasyRPG || cardData.expansionType == ECardExpansionType.Megabot) && MinaCardsModPlugin.CustomNewExpansionConfigs.Value;
    }
    public static CustomCardObject SelectConfig(
      CustomCardObject cardConfig,
      CustomCardObject fullExpansionConfig)
    {
      return cardConfig.IndividualOverrides ? cardConfig : fullExpansionConfig;
    }

    public static void SetCardBacks(Card3dUIGroup card3dUIGroup)
    {
      GameObject cardBackMesh = card3dUIGroup.m_CardBackMesh;
      CardUI cardUi = card3dUIGroup.m_CardUI;
      FieldInfo cardDataField = typeof(CardUI).GetField("m_CardData", BindingFlags.NonPublic | BindingFlags.Instance);
      if (cardDataField == null)
      {
        Debug.LogError("Unable to find 'm_CardData' field.");
        return;
      }

      CardData cardData = (CardData)cardDataField.GetValue(cardUi);
      if (cardData == null)
        return;

      string spriteName = (string)null;
      bool flag = false;

      if (MinaCardsModPlugin.CustomBaseMonsterImages.Value)
      {
        if (cardData.expansionType == ECardExpansionType.Tetramon)
        {
          spriteName = "T_CardBackMesh";
          flag = true;
        }

        if (cardData.expansionType == ECardExpansionType.Destiny)
        {
          spriteName = "T_CardBackMeshDestiny";
          flag = true;
        }

        if (cardData.expansionType == ECardExpansionType.Ghost)
        {
          spriteName = "T_CardBackMeshGhost";
          flag = true;
        }
      }
      else if (!MinaCardsModPlugin.CustomBaseMonsterImages.Value &&
               (cardData.expansionType == ECardExpansionType.Tetramon ||
                cardData.expansionType == ECardExpansionType.Destiny ||
                cardData.expansionType == ECardExpansionType.Megabot))
      {
        spriteName = "T_CardBackMesh";
        flag = false;
      }
      if (MinaCardsModPlugin.CustomNewExpansionImages.Value)
      {
        if (cardData.expansionType == ECardExpansionType.CatJob)
        {
          spriteName = "T_CardBackMeshCatJob";
          flag = true;
        }
        if (cardData.expansionType == ECardExpansionType.FantasyRPG)
        {
          spriteName = "T_CardBackMeshFantasyRPG";
          flag = true;
        }
        if (cardData.expansionType == ECardExpansionType.Megabot)
        {
          spriteName = "T_CardBackMeshMegabot";
          flag = true;
        }
      }
      else if (!MinaCardsModPlugin.CustomNewExpansionImages.Value && (cardData.expansionType == ECardExpansionType.CatJob || cardData.expansionType == ECardExpansionType.FantasyRPG || cardData.expansionType == ECardExpansionType.Megabot))
      {
        spriteName = "T_CardBackMesh";
        flag = false;
      }
      if ((UnityEngine.Object)cardBackMesh != (UnityEngine.Object)null)
      {
        Renderer component = cardBackMesh.GetComponent<Renderer>();
        if ((UnityEngine.Object)component != (UnityEngine.Object)null)
        {
          Material material = component.material;
          if ((UnityEngine.Object) material != (UnityEngine.Object) null && (material.name == "MAT_CardBackMesh" || material.name == "MAT_CardBackMesh (Instance)"))
          {
            Sprite sprite1 = (Sprite) null;
            if (flag)
              sprite1 = CacheHandler.cardExtrasImagesCache.FirstOrDefault<Sprite>((Func<Sprite, bool>) (sprite => sprite.name == spriteName));
            else if (!flag)
              sprite1 = CacheHandler.originalCardBackTexture;
            if ((UnityEngine.Object) sprite1 != (UnityEngine.Object) null)
            {
              Texture texture = (Texture) sprite1.texture;
              material.SetTexture("_BaseMap", texture);
              material.SetTexture("_EmissionMap", texture);
            }
          }
        }
      }
    }

    public static void AddHiddenCards()
    {
      List<EMonsterType> shownMonsterList = InventoryBase.GetShownMonsterList(ECardExpansionType.Tetramon);
      if (shownMonsterList == null || shownMonsterList.Count != 111)
        return;
      shownMonsterList.Add(EMonsterType.SlimeA);
      shownMonsterList.Add(EMonsterType.SlimeB);
      shownMonsterList.Add(EMonsterType.SlimeC);
      shownMonsterList.Add(EMonsterType.SlimeD);
      shownMonsterList.Add(EMonsterType.SeahorseA);
      shownMonsterList.Add(EMonsterType.SeahorseB);
      shownMonsterList.Add(EMonsterType.SeahorseC);
      shownMonsterList.Add(EMonsterType.SeahorseD);
      shownMonsterList.Add(EMonsterType.FireChickenA);
      shownMonsterList.Add(EMonsterType.FireChickenB);
    }

    public static void LogCustomCardObjectProperties(CustomCardObject obj)
    {
      foreach (PropertyInfo property in obj.GetType().GetProperties())
      {
        string name = property.Name;
        object obj1 = property.GetValue((object) obj, (object[]) null);
        MinaCardsModPlugin.Log.LogInfo((object) string.Format("Property: {0}, Value: {1}", (object) name, obj1));
      }
    }
    public static void Log(string log) => MinaCardsModPlugin.Log.LogInfo((object) log);

    public static void LogError(string log) => MinaCardsModPlugin.Log.LogError((object) log);
  }
}