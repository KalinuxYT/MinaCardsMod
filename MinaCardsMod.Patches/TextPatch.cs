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
    public class ChangeRoadmapPauseHeaderPatch
    {
        static void Postfix()
        {
            if (GameObject.Find("Title")?.GetComponent<TextMeshProUGUI>() is TextMeshProUGUI textComponent)
            {
                textComponent.text = "Special Thanks";
            }
            else
            {
                MinaCardsModPlugin.Log.LogWarning("ChangeRoadmapPauseHeaderPatch: Could not find 'RoadmapText' or its TextMeshProUGUI component. Probably a false positive due to bad, stupid, no good code, ignore if you just unpaused the game.");
            }
        }
    }
    [HarmonyPatch(typeof(PauseScreen), "OpenScreen")]
    public class ChangeRoadmapPauseTextPatch
    {
        private static readonly string[] textValues = new string[]
        {
            "", "Kali", "Vulgaris - Testing", "Samsa - Testing", "SCP - Misc. Feedback",
            "Diamond - Artwork", "", "Minawan - Minasonas & Inspiration", "Iw3y - Audio", "",
            "Starcat - Testing"
        };
        static void Postfix()
        {
            for (int i = 1; i <= textValues.Length; i++)
            {
                string targetName = $"Text ({i})";
                GameObject targetObject = GameObject.Find(targetName);
                if (targetObject == null)
                {
                    MinaCardsModPlugin.Log.LogWarning($"ChangeRoadmapPauseTextPatch: GameObject '{targetName}' not found in the scene. Probably a false positive due to bad, stupid, no good code, ignore if you just unpaused the game.");
                    continue; 
                }
                TextMeshProUGUI textComponent = targetObject.GetComponent<TextMeshProUGUI>();
                if (textComponent == null)
                {
                    MinaCardsModPlugin.Log.LogWarning($"ChangeRoadmapPauseTextPatch: TextMeshProUGUI component not found on GameObject '{targetName}'.");
                    continue;
                }
                textComponent.text = textValues[i - 1];
            }
            TextMeshProUGUI[] allTextComponents = GameObject.FindObjectsOfType<TextMeshProUGUI>();
            if (allTextComponents.Length == 0)
            {
                MinaCardsModPlugin.Log.LogWarning("ChangeRoadmapPauseTextPatch: No TextMeshProUGUI components found in the scene.");
                return;
            }
            foreach (var textComponent in allTextComponents)
            {
                if (textComponent.text == "-Customer review app")
                {
                    textComponent.text = "Scaith - Artwork & Audio";
                }
            }
        }
    }
    
    // Credits Title
    
    [HarmonyPatch(typeof(TitleScreen), "Start")]
    public class ChangeRoadmapTitleHeaderPatch
    {
        static void Postfix()
        {
            if (GameObject.Find("Title")?.GetComponent<TextMeshProUGUI>() is TextMeshProUGUI textComponent)
            {
                textComponent.text = "Special Thanks";
            }
            else
            {
                MinaCardsModPlugin.Log.LogWarning("ChangeRoadmapTitleHeaderPatch: Could not find 'RoadmapText' or its TextMeshProUGUI component.");
            }
        }
    }
    [HarmonyPatch(typeof(TitleScreen), "Start")]
    public class ChangeRoadmapTitleTextPatch
    {
        private static readonly string[] textValues = new string[]
        {
            "", "Kali", "Vulgaris - Testing", "Samsa - Testing", "SCP - Misc. Feedback",
            "Diamond - Artwork", "", "Minawan - Minasonas & Inspiration", "Iw3y - Audio", "",
            "Starcat - Testing"
        };
        static void Postfix()
        {
            for (int i = 1; i <= textValues.Length; i++)
            {
                string targetName = $"Text ({i})";
                GameObject targetObject = GameObject.Find(targetName);
                if (targetObject == null)
                {
                    MinaCardsModPlugin.Log.LogWarning($"ChangeRoadmapTitleTextPatch: GameObject '{targetName}' not found in the scene.");
                    continue;
                }
                TextMeshProUGUI textComponent = targetObject.GetComponent<TextMeshProUGUI>();
                if (textComponent == null)
                {
                    MinaCardsModPlugin.Log.LogWarning($"ChangeRoadmapTitleTextPatch: TextMeshProUGUI component not found on GameObject '{targetName}'.");
                    continue;
                }
                textComponent.text = textValues[i - 1];
            }
            TextMeshProUGUI[] allTextComponents = GameObject.FindObjectsOfType<TextMeshProUGUI>();
            if (allTextComponents.Length == 0)
            {
                MinaCardsModPlugin.Log.LogWarning("ChangeRoadmapTitleTextPatch: No TextMeshProUGUI components found in the scene.");
                return;
            }
            foreach (var textComponent in allTextComponents)
            {
                if (textComponent.text == "-Customer review app")
                {
                    textComponent.text = "Scaith - Artwork & Audio";
                }
            }
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