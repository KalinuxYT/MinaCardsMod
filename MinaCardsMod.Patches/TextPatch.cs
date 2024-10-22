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
            MinaCardsModPlugin.Log.LogInfo("PauseScreenDiscordPatch: Discord button URL modified on the pause screen.");
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
            MinaCardsModPlugin.Log.LogInfo("TitleScreenDiscordPatch: Discord button URL modified on the title screen.");
            return false;
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