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
            else
            {
                MinaCardsModPlugin.Log.LogWarning("ChangeProviderTextPatch: Could not find 'MobileProviderText' or its TextMeshProUGUI component.");
            }
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
            "Neuro",
            ""
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
    /*
    // Does replace the review text but is too broad
    
    [HarmonyPatch(typeof(CustomerReviewManager), "GetReviewTextString")]
    public class CustomerReviewTextPatch
    {
        static void Postfix()
        {
            var reviewTables = CSingleton<CustomerReviewManager>.Instance.m_TextSO.m_CustomerReviewTextDataTableList;
            foreach (var reviewTable in reviewTables)
            {
                for (int i = 0; i < reviewTable.bad_TextList.Count; i++)
                {
                    reviewTable.bad_TextList[i] = "Modified Bad Review Text";
                }
                for (int i = 0; i < reviewTable.normal_TextList.Count; i++)
                {
                    reviewTable.normal_TextList[i] = "Modified Normal Review Text";
                }
                for (int i = 0; i < reviewTable.good_TextList.Count; i++)
                {
                    reviewTable.good_TextList[i] = "Modified Good Review Text";
                }
            }
        }
    } */
    
    [HarmonyPatch(typeof(CustomerReviewManager), "GetReviewTextString")]
public class CustomerReviewTextPatch
{
    static void Postfix(ref string __result, CustomerReviewData reviewData)
    {
        // Define custom texts based on the review type
        var customBadTexts = new Dictionary<ECustomerReviewType, string>
        {
            { ECustomerReviewType.StoreGeneric, "Bad, XXX, YYY, StoreGeneric" },
            { ECustomerReviewType.ItemVariety, "Bad, XXX, YYY, ItemVariety" },
            { ECustomerReviewType.ItemPrice, "Bad, XXX, YYY, ItemPrice" },
            { ECustomerReviewType.CardPrice, "Bad, XXX, YYY, CardPrice" },
            { ECustomerReviewType.CardRarity, "Bad, XXX, YYY, CardRarity" },
            { ECustomerReviewType.PlaytablePrice, "Bad, XXX, YYY, PlaytablePrice" },
            { ECustomerReviewType.SmellyCustomer, "Bad, XXX, YYY, SmellyCustomer" },
            { ECustomerReviewType.BlockedStore, "Bad, XXX, YYY, BlockedStore" },
            { ECustomerReviewType.OwnerOpenPack, "Bad, XXX, YYY, OwnerOpenPack" },
            { ECustomerReviewType.GiveManyChangePennies, "Bad, XXX, YYY, GiveManyChangePennies" }
        };

        var customNormalTexts = new Dictionary<ECustomerReviewType, string>
        {
            { ECustomerReviewType.StoreGeneric, "Normal, XXX, YYY, StoreGeneric" },
            { ECustomerReviewType.ItemVariety, "Normal, XXX, YYY, ItemVariety" },
            { ECustomerReviewType.ItemPrice, "Normal, XXX, YYY, ItemPrice" },
            { ECustomerReviewType.CardPrice, "Normal, XXX, YYY, CardPrice" },
            { ECustomerReviewType.CardRarity, "Normal, XXX, YYY, CardRarity" },
            { ECustomerReviewType.PlaytablePrice, "Normal, XXX, YYY, PlaytablePrice" },
            { ECustomerReviewType.SmellyCustomer, "Normal, XXX, YYY, SmellyCustomer" }
        };

        var customGoodTexts = new Dictionary<ECustomerReviewType, string>
        {
            { ECustomerReviewType.StoreGeneric, "Good, XXX, YYY, StoreGeneric" },
            { ECustomerReviewType.ItemVariety, "Good, XXX, YYY, ItemVariety" },
            { ECustomerReviewType.ItemPrice, "Good, XXX, YYY, ItemPrice" },
            { ECustomerReviewType.CardPrice, "Good, XXX, YYY, CardPrice" },
            { ECustomerReviewType.CardRarity, "Good, XXX, YYY, CardRarity" },
            { ECustomerReviewType.PlaytablePrice, "Good, XXX, YYY, PlaytablePrice" },
            { ECustomerReviewType.SmellyCustomer, "Good, XXX, YYY, SmellyCustomer" }
        };

        // Retrieve the custom text based on review type and level
        string customText = GetCustomText(reviewData, customBadTexts, customNormalTexts, customGoodTexts);
        if (!string.IsNullOrEmpty(customText))
        {
            // Replace placeholders while keeping them intact
            customText = customText.Replace("XXX", CPlayerData.PlayerName).Replace("YYY", InventoryBase.GetItemData(reviewData.itemType).GetName());

            // Set the modified result
            __result = customText;
        }
    }

    private static string GetCustomText(CustomerReviewData reviewData, 
                                        Dictionary<ECustomerReviewType, string> badTexts,
                                        Dictionary<ECustomerReviewType, string> normalTexts,
                                        Dictionary<ECustomerReviewType, string> goodTexts)
    {
        switch (reviewData.textSOGoodBadLevel)
        {
            case 0:
                if (badTexts.TryGetValue(reviewData.customerReviewType, out string badText))
                    return badText;
                break;
            case 1:
                if (normalTexts.TryGetValue(reviewData.customerReviewType, out string normalText))
                    return normalText;
                break;
            case 2:
                if (goodTexts.TryGetValue(reviewData.customerReviewType, out string goodText))
                    return goodText;
                break;
        }
        return null; // If no custom text is found, return null
    }
}
    
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