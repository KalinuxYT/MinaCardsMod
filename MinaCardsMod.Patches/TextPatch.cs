using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using TMPro;
using UnityEngine;

namespace MinaCardsMod.Patches
{
    // Phone Service
    [HarmonyPatch(typeof(PhoneManager), "EnterPhoneMode")]
    public class ChangeProviderTextPatch
    {
        static void Postfix()
        {
            if (GameObject.Find("MobileProviderText")?.GetComponent<TextMeshProUGUI>() is TextMeshProUGUI textComponent)
            {
                textComponent.text = "WanWan-Mobile";
            }
            {
                MinaCardsModPlugin.Log.LogWarning("ChangeProviderTextPatch: Could not find 'MobileProviderText' or its TextMeshProUGUI component.");
            }
        }
    }
    
    // Difficulty modification
    // If there is a bug in this code I will kill myself :ohCry:
    
    public static class RestockState
{
    public static bool IsModified = false;
    public static Dictionary<string, float> OriginalLicensePrices = new Dictionary<string, float>();
    public static Dictionary<string, int> OriginalLicenseLevels = new Dictionary<string, int>();
    public static Dictionary<string, float> OriginalItemCosts = new Dictionary<string, float>();
}

[HarmonyPatch(typeof(PhoneManager), "EnterPhoneMode")]
public class DifficultyModifier
{
    static void Postfix()
    {
        if (!RestockState.IsModified)
        {
            ModifyRestockData();
            RestockState.IsModified = true;
        }
    }

    private static void ModifyRestockData()
    {
        var stockData = CSingleton<InventoryBase>.Instance.m_StockItemData_SO;
        if (stockData == null)
        {
            MinaCardsModPlugin.Log.LogWarning("ModifyRestockData: StockItemData_ScriptableObject instance not found.");
            return;
        }
        float licenseCostReduction = Mathf.Clamp(MinaCardsModPlugin.LicenseCostReductionPercentage.Value / 100f, 0f, 1f);
        float levelRequirementReduction = Mathf.Clamp(MinaCardsModPlugin.LevelRequirementReductionPercentage.Value / 100f, 0f, 1f);
        float itemCostReduction = Mathf.Clamp(MinaCardsModPlugin.ItemCostReductionPercentage.Value / 100f, 0f, 1f);
        foreach (var restockData in stockData.m_RestockDataList)
        {
            string uniqueKey = $"{restockData.itemType}_{restockData.amount}";
            if (!RestockState.OriginalLicensePrices.ContainsKey(uniqueKey))
            {
                RestockState.OriginalLicensePrices[uniqueKey] = restockData.licensePrice;
            }
            if (!RestockState.OriginalLicenseLevels.ContainsKey(uniqueKey))
            {
                RestockState.OriginalLicenseLevels[uniqueKey] = restockData.licenseShopLevelRequired;
            }
            AdjustLicenseData(restockData, levelRequirementReduction, licenseCostReduction, uniqueKey);
            AdjustItemCosts(restockData, itemCostReduction, uniqueKey);
        }
    }

    private static void AdjustLicenseData(RestockData restockData, float levelRequirementReduction, float licenseCostReduction, string uniqueKey)
    {
        restockData.licenseShopLevelRequired = Mathf.Max(1, Mathf.CeilToInt(restockData.licenseShopLevelRequired * (1 - levelRequirementReduction)));
        restockData.licensePrice = Mathf.Max(0.01f, restockData.licensePrice * (1 - licenseCostReduction));
        // MinaCardsModPlugin.Log.LogInfo($"Modified License Data: Item Type = {restockData.itemType}, Amount = {restockData.amount}, License Level Required = {restockData.licenseShopLevelRequired}, License Price = {restockData.licensePrice}");
    }

    private static void AdjustItemCosts(RestockData restockData, float itemCostReduction, string uniqueKey)
    {
        int itemTypeIndex = (int)restockData.itemType;
        if (itemTypeIndex >= 0 && itemTypeIndex < CPlayerData.m_GeneratedCostPriceList.Count)
        {
            if (!RestockState.OriginalItemCosts.ContainsKey(uniqueKey))
            {
                RestockState.OriginalItemCosts[uniqueKey] = CPlayerData.m_GeneratedCostPriceList[itemTypeIndex];
            }
            float originalUnitPrice = RestockState.OriginalItemCosts[uniqueKey];
            float modifiedUnitPrice = Mathf.Max(0.01f, originalUnitPrice * (1 - itemCostReduction));
            CPlayerData.m_GeneratedCostPriceList[itemTypeIndex] = modifiedUnitPrice;
            // MinaCardsModPlugin.Log.LogInfo($"Modified Unit Price: Item Type = {restockData.itemType}, Amount = {restockData.amount}, Original Price = {originalUnitPrice}, New Price = {modifiedUnitPrice}");
        }
        else
        {
            MinaCardsModPlugin.Log.LogWarning($"AdjustItemCosts: ItemType index {itemTypeIndex} is out of range in m_GeneratedCostPriceList.");
        }
    }
}

[HarmonyPatch(typeof(PhoneManager), "ExitPhoneMode")]
public class RevertRestockDataPatch
{
    static void Postfix()
    {
        var phoneManager = CSingleton<PhoneManager>.Instance;
        var isPhoneModeField = typeof(PhoneManager).GetField("m_IsPhoneMode", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        bool isPhoneMode = (bool)isPhoneModeField.GetValue(phoneManager);
        if (!isPhoneMode && RestockState.IsModified)
        {
            RevertRestockData();
            RestockState.IsModified = false;
        }
    }

    private static void RevertRestockData()
    {
        var stockData = CSingleton<InventoryBase>.Instance.m_StockItemData_SO;
        if (stockData == null)
        {
            MinaCardsModPlugin.Log.LogWarning("RevertRestockData: StockItemData_ScriptableObject instance not found.");
            return;
        }
        foreach (var restockData in stockData.m_RestockDataList)
        {
            string uniqueKey = $"{restockData.itemType}_{restockData.amount}";
            if (RestockState.OriginalLicensePrices.TryGetValue(uniqueKey, out float originalLicensePrice))
            {
                restockData.licensePrice = originalLicensePrice;
            }
            if (RestockState.OriginalLicenseLevels.TryGetValue(uniqueKey, out int originalLicenseLevel))
            {
                restockData.licenseShopLevelRequired = originalLicenseLevel;
            }
            if (RestockState.OriginalItemCosts.TryGetValue(uniqueKey, out float originalItemCost))
            {
                int itemTypeIndex = (int)restockData.itemType;
                if (itemTypeIndex >= 0 && itemTypeIndex < CPlayerData.m_GeneratedCostPriceList.Count)
                {
                    CPlayerData.m_GeneratedCostPriceList[itemTypeIndex] = originalItemCost;
                }
            }
        }
        // MinaCardsModPlugin.Log.LogInfo("Reverted all modified values to their original state.");
    }
}
    
    // Credits Pause
    
    [HarmonyPatch(typeof(PauseScreen), "OpenScreen")]
    public class PauseScreenTextPatches
    {
        private static readonly string[] textValues = new string[]
        {
            "", "Scaith - Artwork & Audio", "Samsa - Testing", "SCP - Misc. Feedback", "Minawan - Minasonas & Inspiration",
            "Iw3y - Audio", "", "", "Starcat - Testing", "", "Vulgaris - Testing"
        };
        static void Postfix()
        {
            var pauseScreen = CSingleton<PauseScreen>.Instance;
            if (pauseScreen == null)
            {
                MinaCardsModPlugin.Log.LogWarning("PauseScreenTextPatches: PauseScreen instance not found.");
                return;
            }
            if (pauseScreen.m_ScreenGrp != null && pauseScreen.m_ScreenGrp.activeSelf)
            {
                ApplyTextChanges();
            }
        }
        private static void ApplyTextChanges()
        {
            TextPatchUtility.UpdateText("Title", "Special Thanks");
            for (int i = 1; i <= textValues.Length; i++)
            {
                string targetName = $"Text ({i})";
                TextPatchUtility.UpdateText(targetName, textValues[i - 1]);
            }
            TextPatchUtility.UpdateTextByContent("-Customer review app", "Diamond - Artwork");
            TextPatchUtility.UpdateTextByContent("Scaith - Artwork & Audio", "Scaith - Artwork & Audio", removeStrikethrough: true);
        }
    }

    // Credits Title
    
    [HarmonyPatch(typeof(TitleScreen), "Start")]
    public class TitleScreenTextPatches
    {
        private static readonly string[] textValues = new string[]
        {
            "", "Scaith - Artwork & Audio", "Samsa - Testing", "SCP - Misc. Feedback", "Minawan - Minasonas & Inspiration",
            "Iw3y - Audio", "", "", "Starcat - Testing", "", "Vulgaris - Testing"
        };
        static void Postfix()
        {
            ApplyTextChanges();
        }
        private static void ApplyTextChanges()
        {
            TextPatchUtility.UpdateText("Title", "Special Thanks");
            for (int i = 1; i <= textValues.Length; i++)
            {
                string targetName = $"Text ({i})";
                TextPatchUtility.UpdateText(targetName, textValues[i - 1]);
            }
            TextPatchUtility.UpdateTextByContent("-Customer review app", "Diamond - Artwork");
            TextPatchUtility.UpdateTextByContent("Scaith - Artwork & Audio", "Scaith - Artwork & Audio", removeStrikethrough: true);
        }
    }

    // For both Pause and Title text modification
    
    public static class TextPatchUtility
    {
        public static void UpdateText(string objectName, string newText)
        {
            if (GameObject.Find(objectName)?.GetComponent<TextMeshProUGUI>() is TextMeshProUGUI textComponent)
            {
                textComponent.text = newText;
            }
            else
            {
                MinaCardsModPlugin.Log.LogWarning($"TextPatchUtility: Could not find '{objectName}' or its TextMeshProUGUI component.");
            }
        }
        public static void UpdateTextByContent(string originalText, string newText, bool removeStrikethrough = false)
        {
            TextMeshProUGUI[] allTextComponents = GameObject.FindObjectsOfType<TextMeshProUGUI>();
            if (allTextComponents.Length == 0)
            {
                MinaCardsModPlugin.Log.LogWarning("TextPatchUtility: No TextMeshProUGUI components found in the scene.");
                return;
            }
            foreach (var textComponent in allTextComponents)
            {
                if (textComponent.text == originalText)
                {
                    textComponent.text = newText;
                    if (removeStrikethrough && (textComponent.fontStyle & FontStyles.Strikethrough) == FontStyles.Strikethrough)
                    {
                        textComponent.fontStyle &= ~FontStyles.Strikethrough;
                    }
                }
            }
        }
    }
    
    // Replace Discord URL
    
    [HarmonyPatch(typeof(PauseScreen), "OnPressDiscordBtn")]
    public class PauseScreenDiscordPatch
    {
        static bool Prefix()
        {
            Application.OpenURL("https://uwumarket.us/collections/cerbervt");
            UnityAnalytic.JoinDiscord();
            return false;
        }
    }
    [HarmonyPatch(typeof(TitleScreen), "OnPressDiscordBtn")]
    public class TitleScreenDiscordPatch
    {
        static bool Prefix()
        {
            Application.OpenURL("https://uwumarket.us/collections/cerbervt");
            UnityAnalytic.JoinDiscord();
            return false;
        }
    }
    
    
    
    // Replace customer review names
    
    [HarmonyPatch(typeof(CustomerReviewPanelUI), "Init")]
    public class CustomerReviewPanelUIPatch
    {
        private static List<string> customNames = new List<string>
        {
            "Dave Daveson",
            "Trailerless Trucker",
            "TooMuchPasta",
            "SlugSalter",
            "Peanut",
            "Jim Purrbert",
            "YourStandardBo",
            "Aquwuwa",
            "Evilyn",
            "Eliv",
            "Nwero",
            "Ember’s#WanFan",
            "SucculentsCollector",
            "UndercookedGoose",
            "Rebrec",
            "John Twitch",
            "CerberSocksEnthusiast",
            "Airis",
            "Top3Gamer987",
            "#WanFemaleStreamer",
            "FilipinoBoy",
            "IEatAnywhereBetween3000to4000SpidersADay",
            "DiscordKitten",
            "Grincher",
            "Xdx",
            "SuperMassiveHamburger",
            "Camimi",
            "Super Cool Pupper",
            "BasementDweller(Starving)",
            "YanderePillow",
            "Vulgar",
            "Rick Astley", 
            "ShakenScientificResolve",
            "DoorKnobSucker",
            "John Minamon",
            "TastyBaton",
            "SunshineGoddess",
            "Astarion",
            "Ermber",
            "Cerber Socks",
            "HarpoonConnoisseur",
            "ColdSockMosquito",
            "Toemuh",
            "YesHealsForU",
            "OmnipresentClipper",
            "Maxwell",
            "MagicianWithaVision",
            "ManOfTheSauna", 
            "TheSoul",
            "CarbonCrab",
            "BagelEnjoyer",
            "Nogitsune",
            "Coin",
            "ParentsRRelated",
            "SqueakyFridgeBeetle",
            "CtrlAltVictory", 
            "Gem_Encrusted_Crustacean",
            "RememberedWan",
            "WantedSalad",
            "OverWanMillionCovers",
            "MoonDog",
            "TrustworthyPitFruit" ,
            "SoaringGato",
            "CutestSheep",
            "UnemployedBugBot",
            "ManLackingSkills",
            "TaxDevil",
            "WinningDavid",
            "RequiredWan",
            "JohnBot", 
            "InOrberWeTrust",
            "ConvexGlobularCycloidEnjoyer",
            "Shoemimi",
            "GlorpCat",
            "IronLungGOTY",
            "Competitive Minawan Adventure Gamer",
            "AvidMinamonCollector",
            "Highwaywan",
            "Deleted User",
            "ConspicuousApostrophe",
            "DefiddledDaveCo.",
            "YouKilledMyFather",
            "PregnantWithaHorse",
            "Byonicle",
            "CrazyFilteredRobotBody",
            "OopsAllKaraoke",
            "ASMR Survivor",
            "TheShockingErm",
            "NotaBagel",
            "RedDogWan"
        };
        private static List<string> shuffledNames;
        static void Postfix(CustomerReviewPanelUI __instance, CustomerReviewData reviewData)
        {
            if (shuffledNames == null || shuffledNames.Count == 0)
            {
                ShuffleCustomNames();
            }
            string randomName = shuffledNames[0];
            shuffledNames.RemoveAt(0);
            __instance.m_NameText.text = randomName;
        }
        private static void ShuffleCustomNames()
        {
            shuffledNames = new List<string>(customNames);
            int n = shuffledNames.Count;
            System.Random rng = new System.Random();
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                string value = shuffledNames[k];
                shuffledNames[k] = shuffledNames[n];
                shuffledNames[n] = value;
            }
        }
    }
    
    // Replace customer review text
    /*
    [HarmonyPatch(typeof(CustomerReviewManager), "GetReviewTextString")]
    public class CustomerReviewTextPatch
    {
        private static readonly Dictionary<ECustomerReviewType, Dictionary<int, List<string>>> reviewVariations =
            new Dictionary<ECustomerReviewType, Dictionary<int, List<string>>>
        {
            // StoreGeneric
            {
                ECustomerReviewType.StoreGeneric, new Dictionary<int, List<string>>
                {
                    { 0, new List<string> { "StoreGeneric1, XXX, YYY, Bad", "StoreGeneric2, XXX, YYY, Bad", "StoreGeneric3, XXX, YYY, Bad", "StoreGeneric4, XXX, YYY, Bad", "StoreGeneric5, XXX, YYY, Bad" } },
                    { 1, new List<string> { "StoreGeneric1, XXX, YYY, Normal", "StoreGeneric2, XXX, YYY, Normal", "StoreGeneric3, XXX, YYY, Normal", "StoreGeneric4, XXX, YYY, Normal", "StoreGeneric5, XXX, YYY, Normal" } },
                    { 2, new List<string> { "StoreGeneric1, XXX, YYY, Good", "StoreGeneric2, XXX, YYY, Good", "StoreGeneric3, XXX, YYY, Good", "StoreGeneric4, XXX, YYY, Good", "StoreGeneric5, XXX, YYY, Good" } }
                }
            },
            // ItemVariety
            {
                ECustomerReviewType.ItemVariety, new Dictionary<int, List<string>>
                {
                    { 0, new List<string> { "ItemVariety1, XXX, YYY, Bad", "ItemVariety2, XXX, YYY, Bad", "ItemVariety3, XXX, YYY, Bad", "ItemVariety4, XXX, YYY, Bad", "ItemVariety5, XXX, YYY, Bad" } },
                    { 1, new List<string> { "ItemVariety1, XXX, YYY, Normal", "ItemVariety2, XXX, YYY, Normal", "ItemVariety3, XXX, YYY, Normal", "ItemVariety4, XXX, YYY, Normal", "ItemVariety5, XXX, YYY, Normal" } },
                    { 2, new List<string> { "ItemVariety1, XXX, YYY, Good", "ItemVariety2, XXX, YYY, Good", "ItemVariety3, XXX, YYY, Good", "ItemVariety4, XXX, YYY, Good", "ItemVariety5, XXX, YYY, Good" } }
                }
            },
            // ItemPrice
            {
                ECustomerReviewType.ItemPrice, new Dictionary<int, List<string>>
                {
                    { 0, new List<string> { "ItemPrice1, XXX, YYY, Bad", "ItemPrice2, XXX, YYY, Bad", "ItemPrice3, XXX, YYY, Bad", "ItemPrice4, XXX, YYY, Bad", "ItemPrice5, XXX, YYY, Bad" } },
                    { 1, new List<string> { "ItemPrice1, XXX, YYY, Normal", "ItemPrice2, XXX, YYY, Normal", "ItemPrice3, XXX, YYY, Normal", "ItemPrice4, XXX, YYY, Normal", "ItemPrice5, XXX, YYY, Normal" } },
                    { 2, new List<string> { "ItemPrice1, XXX, YYY, Good", "ItemPrice2, XXX, YYY, Good", "ItemPrice3, XXX, YYY, Good", "ItemPrice4, XXX, YYY, Good", "ItemPrice5, XXX, YYY, Good" } }
                }
            },
            // CardPrice
            {
                ECustomerReviewType.CardPrice, new Dictionary<int, List<string>>
                {
                    { 0, new List<string> { "CardPrice1, XXX, YYY, Bad", "CardPrice2, XXX, YYY, Bad", "CardPrice3, XXX, YYY, Bad", "CardPrice4, XXX, YYY, Bad", "CardPrice5, XXX, YYY, Bad" } },
                    { 1, new List<string> { "CardPrice1, XXX, YYY, Normal", "CardPrice2, XXX, YYY, Normal", "CardPrice3, XXX, YYY, Normal", "CardPrice4, XXX, YYY, Normal", "CardPrice5, XXX, YYY, Normal" } },
                    { 2, new List<string> { "CardPrice1, XXX, YYY, Good", "CardPrice2, XXX, YYY, Good", "CardPrice3, XXX, YYY, Good", "CardPrice4, XXX, YYY, Good", "CardPrice5, XXX, YYY, Good" } }
                }
            },
            // CardRarity
            {
                ECustomerReviewType.CardRarity, new Dictionary<int, List<string>>
                {
                    { 0, new List<string> { "CardRarity1, XXX, YYY, Bad", "CardRarity2, XXX, YYY, Bad", "CardRarity3, XXX, YYY, Bad", "CardRarity4, XXX, YYY, Bad", "CardRarity5, XXX, YYY, Bad" } },
                    { 1, new List<string> { "CardRarity1, XXX, YYY, Normal", "CardRarity2, XXX, YYY, Normal", "CardRarity3, XXX, YYY, Normal", "CardRarity4, XXX, YYY, Normal", "CardRarity5, XXX, YYY, Normal" } },
                    { 2, new List<string> { "CardRarity1, XXX, YYY, Good", "CardRarity2, XXX, YYY, Good", "CardRarity3, XXX, YYY, Good", "CardRarity4, XXX, YYY, Good", "CardRarity5, XXX, YYY, Good" } }
                }
            },
            // PlayablePrice
            {
                ECustomerReviewType.PlaytablePrice, new Dictionary<int, List<string>>
                {
                    { 0, new List<string> { "PlaytablePrice1, XXX, YYY, Bad", "PlaytablePrice2, XXX, YYY, Bad", "PlaytablePrice3, XXX, YYY, Bad", "PlaytablePrice4, XXX, YYY, Bad", "PlaytablePrice5, XXX, YYY, Bad" } },
                    { 1, new List<string> { "PlaytablePrice1, XXX, YYY, Normal", "PlaytablePrice2, XXX, YYY, Normal", "PlaytablePrice3, XXX, YYY, Normal", "PlaytablePrice4, XXX, YYY, Normal", "PlaytablePrice5, XXX, YYY, Normal" } },
                    { 2, new List<string> { "PlaytablePrice1, XXX, YYY, Good", "PlaytablePrice2, XXX, YYY, Good", "PlaytablePrice3, XXX, YYY, Good", "PlaytablePrice4, XXX, YYY, Good", "PlaytablePrice5, XXX, YYY, Good" } }
                }
            },
            // SmellyCustomer
            {
                ECustomerReviewType.SmellyCustomer, new Dictionary<int, List<string>>
                {
                    { 0, new List<string> { "SmellyCustomer1, XXX, YYY, Bad", "SmellyCustomer2, XXX, YYY, Bad", "SmellyCustomer3, XXX, YYY, Bad", "SmellyCustomer4, XXX, YYY, Bad", "SmellyCustomer5, XXX, YYY, Bad" } },
                    { 1, new List<string> { "SmellyCustomer1, XXX, YYY, Normal", "SmellyCustomer2, XXX, YYY, Normal", "SmellyCustomer3, XXX, YYY, Normal", "SmellyCustomer4, XXX, YYY, Normal", "SmellyCustomer5, XXX, YYY, Normal" } },
                    { 2, new List<string> { "SmellyCustomer1, XXX, YYY, Good", "SmellyCustomer2, XXX, YYY, Good", "SmellyCustomer3, XXX, YYY, Good", "SmellyCustomer4, XXX, YYY, Good", "SmellyCustomer5, XXX, YYY, Good" } }
                }
            },
            // BlockedStore
            {
                ECustomerReviewType.BlockedStore, new Dictionary<int, List<string>>
                {
                    { 0, new List<string> { "BlockedStore1, XXX, YYY, Bad", "BlockedStore2, XXX, YYY, Bad", "BlockedStore3, XXX, YYY, Bad", "BlockedStore4, XXX, YYY, Bad", "BlockedStore5, XXX, YYY, Bad" } },
                    { 1, new List<string> { "BlockedStore1, XXX, YYY, Normal", "BlockedStore2, XXX, YYY, Normal", "BlockedStore3, XXX, YYY, Normal", "BlockedStore4, XXX, YYY, Normal", "BlockedStore5, XXX, YYY, Normal" } },
                    { 2, new List<string> { "BlockedStore1, XXX, YYY, Good", "BlockedStore2, XXX, YYY, Good", "BlockedStore3, XXX, YYY, Good", "BlockedStore4, XXX, YYY, Good", "BlockedStore5, XXX, YYY, Good" } }
                }
            },
            // OwnerOpenPack
            {
                ECustomerReviewType.OwnerOpenPack, new Dictionary<int, List<string>>
                {
                    { 0, new List<string> { "OwnerOpenPack1, XXX, YYY, Bad", "OwnerOpenPack2, XXX, YYY, Bad", "OwnerOpenPack3, XXX, YYY, Bad", "OwnerOpenPack4, XXX, YYY, Bad", "OwnerOpenPack5, XXX, YYY, Bad" } },
                    { 1, new List<string> { "OwnerOpenPack1, XXX, YYY, Normal", "OwnerOpenPack2, XXX, YYY, Normal", "OwnerOpenPack3, XXX, YYY, Normal", "OwnerOpenPack4, XXX, YYY, Normal", "OwnerOpenPack5, XXX, YYY, Normal" } },
                    { 2, new List<string> { "OwnerOpenPack1, XXX, YYY, Good", "OwnerOpenPack2, XXX, YYY, Good", "OwnerOpenPack3, XXX, YYY, Good", "OwnerOpenPack4, XXX, YYY, Good", "OwnerOpenPack5, XXX, YYY, Good" } }
                }
            },
            // GiveManyChangePennies
            {
                ECustomerReviewType.GiveManyChangePennies, new Dictionary<int, List<string>>
                {
                    { 0, new List<string> { "GiveManyChangePennies1, XXX, YYY, Bad", "GiveManyChangePennies2, XXX, YYY, Bad", "GiveManyChangePennies3, XXX, YYY, Bad", "GiveManyChangePennies4, XXX, YYY, Bad", "GiveManyChangePennies5, XXX, YYY, Bad" } },
                    { 1, new List<string> { "GiveManyChangePennies1, XXX, YYY, Normal", "GiveManyChangePennies2, XXX, YYY, Normal", "GiveManyChangePennies3, XXX, YYY, Normal", "GiveManyChangePennies4, XXX, YYY, Normal", "GiveManyChangePennies5, XXX, YYY, Normal" } },
                    { 2, new List<string> { "GiveManyChangePennies1, XXX, YYY, Good", "GiveManyChangePennies2, XXX, YYY, Good", "GiveManyChangePennies3, XXX, YYY, Good", "GiveManyChangePennies4, XXX, YYY, Good", "GiveManyChangePennies5, XXX, YYY, Good" } }
                }
            }
        };

        static void Postfix(ref string __result, CustomerReviewData reviewData)
        {
            if (reviewVariations.TryGetValue(reviewData.customerReviewType, out var levelDictionary))
            {
                if (levelDictionary.TryGetValue(reviewData.textSOGoodBadLevel, out var options))
                {
                    string selectedReview = options[Random.Range(0, options.Count)];
                    selectedReview = selectedReview.Replace("XXX", CPlayerData.PlayerName).Replace("YYY", InventoryBase.GetItemData(reviewData.itemType).GetName());
                    __result = selectedReview;
                }
            }
        }
    }*/
    
    
    /*
    [HarmonyPatch(typeof(RestockManager))]
    public class RestockManagerPatch
    {
        // Patching OnGameDataFinishLoaded to modify the RestockData after the game data is loaded
        [HarmonyPatch("OnGameDataFinishLoaded")]
        [HarmonyPostfix]
        public static void Postfix()
        {
            var stockData = CSingleton<InventoryBase>.Instance.m_StockItemData_SO;

            if (stockData == null)
            {
                MinaCardsModPlugin.Log.LogWarning("RestockManagerPatch: StockItemData_ScriptableObject instance not found.");
                return;
            }

            // Iterate through each RestockData in m_RestockDataList and modify fields
            foreach (var restockData in stockData.m_RestockDataList)
            {
                // Log original values
                MinaCardsModPlugin.Log.LogInfo($"Original RestockData: Name = {restockData.name}, License Level Required = {restockData.licenseShopLevelRequired}, License Price = {restockData.licensePrice}");

                // Modify the values
                restockData.licenseShopLevelRequired = Mathf.CeilToInt(restockData.licenseShopLevelRequired * .3f);
                restockData.licensePrice *= 0.25f;

                // Log modified values
                MinaCardsModPlugin.Log.LogInfo($"Modified RestockData: Name = {restockData.name}, License Level Required = {restockData.licenseShopLevelRequired}, License Price = {restockData.licensePrice}");
            }
        }
    }
   */
/*
    // The below code is to investigate the class and/or methods used to set/modify text
    [HarmonyPatch(typeof(TextMeshProUGUI), "Awake")]
    public class Investigator
    {
        static void Postfix(TextMeshProUGUI __instance)
        {
            MinaCardsModPlugin.Log.LogInfo("ChangeProviderTextPatch: Awake method detected.");
            TextMeshProUGUI[] texts = GameObject.FindObjectsOfType<TextMeshProUGUI>();
            if (texts.Length == 0)
            {
                MinaCardsModPlugin.Log.LogWarning("ChangeProviderTextPatch: No TextMeshProUGUI components found in the scene.");
                return;
            }
            foreach (TextMeshProUGUI textComponent in texts)
            {
                MinaCardsModPlugin.Log.LogInfo($"ChangeProviderTextPatch: Found TextMeshProUGUI on GameObject '{textComponent.gameObject.name}' with text: '{textComponent.text}'");
            }
            MinaCardsModPlugin.Log.LogInfo("ChangeProviderTextPatch: Execution completed.");
        }
    } */
}