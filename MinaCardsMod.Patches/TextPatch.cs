using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.Windows;

namespace MinaCardsMod.Patches
{
    
    // Video Title Background
    
    [HarmonyPatch(typeof(TitleScreen), "Start")]
    public class TitleScreenVideoBackground
    {
        private static VideoPlayer videoPlayer;
        private static RenderTexture renderTexture;

        static void Postfix(TitleScreen __instance)
        {
            ReplaceBackgroundWithVideo();
        }

        private static void ReplaceBackgroundWithVideo()
        {
            DisableBackgroundImage();
            GameObject canvasObject = new GameObject("VideoCanvas");
            Canvas canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasObject.AddComponent<GraphicRaycaster>();
            GameObject videoDisplayObject = new GameObject("VideoDisplay");
            videoDisplayObject.transform.SetParent(canvasObject.transform, false);
            RawImage videoRawImage = videoDisplayObject.AddComponent<RawImage>();
            RectTransform rectTransform = videoDisplayObject.GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            renderTexture = new RenderTexture(3840, 2160, 0, RenderTextureFormat.ARGB32)
            {
                antiAliasing = 4,
                filterMode = FilterMode.Trilinear
            };
            renderTexture.Create();
            videoRawImage.texture = renderTexture;
            videoPlayer = videoDisplayObject.AddComponent<VideoPlayer>();
            videoPlayer.playOnAwake = false;
            videoPlayer.isLooping = true;
            videoPlayer.renderMode = VideoRenderMode.RenderTexture;
            videoPlayer.targetTexture = renderTexture;
            videoPlayer.url = "file://" + Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Video/Lore.mp4");
            if (!File.Exists(videoPlayer.url.Replace("file://", "")))
            {
                MinaCardsModPlugin.Log.LogError($"[Mod] Video file not found at: {videoPlayer.url}");
                return;
            }
            videoPlayer.Prepare();
            videoPlayer.prepareCompleted += (VideoPlayer vp) => vp.Play();
        }

        private static void DisableBackgroundImage()
        {
            GameObject backgroundObject = GameObject.Find("BG");
            if (backgroundObject == null) return;
            var bgImage = backgroundObject.GetComponent<UnityEngine.UI.Image>();
            if (bgImage != null) bgImage.enabled = false;
            var bgRawImage = backgroundObject.GetComponent<UnityEngine.UI.RawImage>();
            if (bgRawImage != null) bgRawImage.enabled = false;
            var renderer = backgroundObject.GetComponent<Renderer>();
            if (renderer != null) renderer.enabled = false;
        }
    }

    // Plushie sound effects
    
    [HarmonyPatch(typeof(ShelfCompartment), "TakeItemToHand")]
    public class ToySqueakInteractPatch
    {
        static void Postfix(ShelfCompartment __instance, Item __result)
        {
            if (__result == null) return;

            EItemType itemType = __result.GetItemType();
            if (itemType == EItemType.Toy_PiggyA || itemType == EItemType.Toy_StarfishA || itemType == EItemType.Toy_BatA || itemType == EItemType.Toy_GolemA)
            {
                MinaCardsModPlugin.Log.LogInfo($"[Info :MinaCardsMod] Picked up {itemType} from shelf");
                ToySqueakHandler.EnableToyAction();
            }
            else
            {
                MinaCardsModPlugin.Log.LogInfo($"[Info :MinaCardsMod] Picked up a different item: {itemType}");
            }
        }
    }

    [HarmonyPatch(typeof(ShelfCompartment), "AddItem")]
    public class ToySqueakAddItemPatch
    {
        static void Postfix(ShelfCompartment __instance, Item item)
        {
            if (item == null) return;

            EItemType itemType = item.GetItemType();
            if (itemType == EItemType.Toy_PiggyA || itemType == EItemType.Toy_StarfishA || itemType == EItemType.Toy_BatA || itemType == EItemType.Toy_GolemA)
            {
                MinaCardsModPlugin.Log.LogInfo($"[Info :MinaCardsMod] {itemType} placed back on shelf, removing tooltip");
                ToySqueakHandler.DisableToyAction();
            }
        }
    }

    public static class ToySqueakHandler
    {
        private static bool isToyActive = false;
        private static float cooldownTime = 1.5f;
        private static float timer = 0.0f;
        private static GameObject tooltipTextObject;

        private static AudioSource audioSource;
        private static readonly string soundsPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Sounds/");
        private static readonly string audioFileName = "Squeak.wav";

    public static void EnableToyAction()
    {
        isToyActive = true;
        timer = cooldownTime; 

        MinaCardsModPlugin.Log.LogInfo("[Info :MinaCardsMod] Toy action enabled, showing tooltip");
        ShowCustomTooltip("Press G!");

        if (GameObject.FindObjectOfType<ToySqueakUpdater>() == null)
        {
            GameObject updaterObject = new GameObject("ToySqueakUpdater");
            updaterObject.AddComponent<ToySqueakUpdater>();
            GameObject.DontDestroyOnLoad(updaterObject);
        }
    }

    public static void DisableToyAction()
    {
        isToyActive = false;
        MinaCardsModPlugin.Log.LogInfo("[Info :MinaCardsMod] Toy action disabled, hiding tooltip");
        HideCustomTooltip();

        var updater = GameObject.FindObjectOfType<ToySqueakUpdater>();
        if (updater != null)
        {
            GameObject.Destroy(updater.gameObject);
        }
    }

    public static void Update()
    {
        if (isToyActive)
        {
            timer += Time.deltaTime;

            if (timer >= cooldownTime && UnityEngine.Input.GetKeyDown(KeyCode.G))
            {
                MinaCardsModPlugin.Log.LogInfo("[Info :MinaCardsMod] 'G' key pressed to use toy");
                UseToyAction();
                timer = 0.0f;
            }
            else if (timer >= cooldownTime && timer < cooldownTime + Time.deltaTime)
            {
                MinaCardsModPlugin.Log.LogInfo("[Info :MinaCardsMod] Toy action cooldown reset");
            }
        }
    }

    private static void UseToyAction()
    {
        MinaCardsModPlugin.Log.LogInfo("[Info :MinaCardsMod] Using toy action");
        PlayCustomAudio();
    }

    private static void PlayCustomAudio()
    {
        if (audioSource == null)
        {
            audioSource = new GameObject("ToySoundPlayer").AddComponent<AudioSource>();
            GameObject.DontDestroyOnLoad(audioSource.gameObject);
        }

        string audioPath = Path.Combine(soundsPath, audioFileName);
        if (File.Exists(audioPath))
        {
            ToySqueakUpdater.Instance.StartCoroutine(LoadAudioClipAndPlay(audioPath));
            ToySqueakUpdater.Instance.StartCoroutine(UpdateVolume());
        }
        else
        {
            MinaCardsModPlugin.Log.LogError($"Audio file not found at path: {audioPath}");
        }
    }

    private static IEnumerator LoadAudioClipAndPlay(string filePath)
    {
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + filePath, AudioType.WAV))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                audioSource.clip = clip;
                audioSource.Play();
            }
            else
            {
                MinaCardsModPlugin.Log.LogError($"Failed to load custom audio: {www.error}");
            }
        }
    }

    private static IEnumerator UpdateVolume()
    {
        while (audioSource != null)
        {
            audioSource.volume = SoundManager.SFXVolume;
            yield return new WaitForSeconds(0.1f);
        }
    }

    private static void ShowCustomTooltip(string message)
    {
        if (tooltipTextObject == null)
        {
            GameObject tooltipCanvasObject = new GameObject("TooltipCanvas");
            Canvas tooltipCanvas = tooltipCanvasObject.AddComponent<Canvas>();
            tooltipCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            tooltipCanvas.sortingOrder = 100;

            tooltipTextObject = new GameObject("TooltipText");
            tooltipTextObject.transform.SetParent(tooltipCanvas.transform, false);

            TextMeshProUGUI textComponent = tooltipTextObject.AddComponent<TextMeshProUGUI>();
            textComponent.alignment = TextAlignmentOptions.Center;
            textComponent.fontSize = 124;
            textComponent.color = Color.black;
            textComponent.text = message;
            textComponent.enableAutoSizing = false;

            textComponent.fontMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, 0.3f);
            textComponent.outlineColor = Color.black;
            textComponent.outlineWidth = 10f;

            RectTransform rectTransform = tooltipTextObject.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0.5f, 0);
            rectTransform.anchorMax = new Vector2(0.5f, 0);
            rectTransform.anchoredPosition = new Vector2(0, 100);
            rectTransform.sizeDelta = new Vector2(500, 100);
        }

        tooltipTextObject.SetActive(true);
        MinaCardsModPlugin.Log.LogInfo("[Info :MinaCardsMod] Tooltip displayed on screen");
    }

    private static void HideCustomTooltip()
    {
        if (tooltipTextObject != null)
        {
            tooltipTextObject.SetActive(false);
            MinaCardsModPlugin.Log.LogInfo("[Info :MinaCardsMod] Tooltip hidden");
        }
    }
}

public class ToySqueakUpdater : MonoBehaviour
{
    public static ToySqueakUpdater Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        ToySqueakHandler.Update();
    }
}
    
    // High Value Card Threshold
    
    [HarmonyPatch(typeof(CardOpeningSequence), "Start")]
    public class ThresholdAdjusterPatch
    {
        private const int FixedThresholdValue = 100;
        static void Postfix(CardOpeningSequence __instance)
        {
            var thresholdField = AccessTools.Field(typeof(CardOpeningSequence), "m_HighValueCardThreshold");
            if (thresholdField != null)
            {
                thresholdField.SetValue(__instance, FixedThresholdValue);
            }
            else
            {
                MinaCardsModPlugin.Log.LogWarning("Could not access 'm_HighValueCardThreshold' field in CardOpeningSequence.");
            }
        }
    }
    
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
            "", "Scaith - Artwork & Audio", "Samsa - Testing", "Gab - Title Music", "Necro - Data Entry",
            "Iw3y - Audio", "Minawan - Minasonas & Inspiration", "SCP - Misc. Feedback", "Starcat - Testing", "Kali - Other Stuff", "Vulgaris - Testing"
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

    // Credits & Music Title
    
    [HarmonyPatch(typeof(TitleScreen), "Start")]
    public class TitleScreenTextPatches : MonoBehaviour
    {
        private static TitleScreenTextPatches _instance;

        private static readonly string[] textValues = new string[]
        {
            "", "Scaith - Artwork & Audio", "Samsa - Testing", "Gab - Title Music", "Necro - Data Entry",
            "Iw3y - Audio", "Minawan - Minasonas & Inspiration", "SCP - Misc. Feedback", "Starcat - Testing", "Kali - Other Stuff", "Vulgaris - Testing"
        };

        private static AudioSource audioSource;
        private static string soundsPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Sounds/");
        private static string audioFileName = "Main.wav";

        public static TitleScreenTextPatches Instance
        {
            get
            {
                if (_instance == null)
                {
                    var gameObject = new GameObject("TitleScreenTextPatches");
                    _instance = gameObject.AddComponent<TitleScreenTextPatches>();
                    DontDestroyOnLoad(gameObject);
                }
                return _instance;
            }
        }

        static void Postfix()
        {
            ApplyTextChanges();
            LoadAndPlayMenuMusic();
            Instance.StartCoroutine(UpdateVolume());
            SceneManager.activeSceneChanged += OnSceneChanged;
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

        private static void LoadAndPlayMenuMusic()
        {
            if (audioSource == null)
            {
                audioSource = Instance.gameObject.AddComponent<AudioSource>();
                audioSource.loop = true;
            }

            string audioPath = Path.Combine(soundsPath, audioFileName);

            if (File.Exists(audioPath))
            {
                Instance.StartCoroutine(LoadAudioClipAndPlay(audioPath));
            }
            else
            {
                MinaCardsModPlugin.Log.LogError($"Audio file not found at path: {audioPath}");
            }
        }

        private static IEnumerator LoadAudioClipAndPlay(string filePath)
        {
            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + filePath, AudioType.WAV))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                    audioSource.clip = clip;
                    audioSource.Play();
                }
                else
                {
                    MinaCardsModPlugin.Log.LogError($"Failed to load custom audio: {www.error}");
                }
            }
        }

        private static IEnumerator UpdateVolume()
        {
            while (true)
            {
                if (audioSource != null)
                {
                    audioSource.volume = SoundManager.MusicVolume;
                }
                yield return new WaitForSeconds(0.1f);
            }
        }

        private static void OnSceneChanged(Scene current, Scene next)
        {
            if (next.name != "TitleScreen")
            {
                if (audioSource != null)
                {
                    audioSource.Stop();
                }
                SceneManager.activeSceneChanged -= OnSceneChanged;
                Destroy(_instance.gameObject);
            }
        }
    }

    // Pause and Title text modification
    
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
            "#WanEmberFan",
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
                    { 
                        0, new List<string> {
                            "Bought a can of cleanser and it was empty. Wa da heck?!",
                            "The shop owner wont stop leaning over my shoulder as I play cards.",
                            "The owner of XXX got better cards than me. Thats unfair.",
                            "I am allergic to dogs and this place literally kills me.",
                            "I am more of a cat person myself.",
                            "I didnt find what I wanted, but the owner is cute.",
                            "I didnt catch whoever did it, but I was sitting at the table and someone BIT my ankle! What kind of place is this!?",
                            "I am pretty sure the owner of XXX growled at me?",
                            "I asked for a recommendation and the owner just stared at me… for like, 5 minutes. Creepy.",
                            "Went to XXX and the owner turned out the lights while we were there. Played cards in the dark, and I am still not sure if I left with my wallet.",
                            "My Minamon deck got CHEWED on by the OWNER while I was browsing. I mean I get it, but come on!",
                            "Walked in, slipped on a puddle of drool. No one even asked if I was okay. Worst day ever.",
                            "Someone ate my lunch when I left it on the table at XXX. Not even kidding.",
                            "I walked into XXX and found the owner in the middle of a nap. I had to tiptoe around so I wouldnt wake them.",
                            "I think I accidentally entered a cult meeting at XXX. They all started chanting Wan Wan when I walked in.",
                            "Owner threw me out because I said cats are better than dogs. Guess I am banned now.",
                            "Was promised a Minamon card pack for a discount if I barked. I did. They didnt give it to me. What the freak man…" 
                        } 
                    }, 
                    {
                        1, new List<string> {
                            "The owner of XXX yelled at me for no particular reason…",
                            "The floor was full of confetti, as it should be.",
                            "Something came over me and I tried to nibble on the owners ears, last thing I remember was hearing “Down the stairs”. Woke up in the hospital, fair enough.",
                            "The attention of the owner of XXX is not the best.",
                            "They have DOGGIES but no POGGIES.",
                            "I am pretty sure the owner is a dog.",
                            "The owner of XXX told me to go to Hell… not sure how to feel about this.",
                            "Some rando gave me a gift card to this place, it was pretty neat.",
                            "XXX is fine, but the store layout is kinda confusing. I walked into a broom closet thinking it was the bathroom…",
                            "Not bad, but there was a squeaky sound that followed me the entire time. Maybe its haunted?",
                            "They had what I needed, but the owner insisted on calling my name with “-wan” at the end. Felt weird, but alright.",
                            "Its okay. Got my cards, but the cashier was too busy howling at the moon to notice me.",
                            "They have snacks, but the flavor options are odd. Bacon treat flavored crisps? Really?",
                            "The vibe of XXX is weird. Feels like they are always watching me… should I be scared?" 
                        } 
                    }, 
                    { 
                        2, new List<string> {
                            "The vibes were immaculate at XXX",
                            "The owner of XXX was really nice and cute.",
                            "I got some compliments for my clothes by the owner of XXX.",
                            "Did you know you can pet the owner of XXX? Mind the horns though, you will get chocolate on your hands.",
                            "What a wantastic store :D",
                            "Wan Wan Wan Wan Wan Wan Wan Wan Wan Wan Wan Wan",
                            "I would sell my left and right kidney just to be able to enter XXX!",
                            "The music they play at XXX are BANGERS, I would go there just to vibe.",
                            "XXX always makes me feel right at home and the staff are pawsome!",
                            "The atmosphere is like a warm hug at XXX. I have fallen in love with the whole store!",
                            "The owner at XXX gave me a free treat for my birthday. Unexpected and absolutely delightful!",
                            "I walked into XXX and left with a smile so big my cheeks hurt. 10/10 will grin again!",
                            "The store owner at XXX gave me a hug and said it was for emotional support. It worked wonders!",
                            "XXX has the best merch! Got a plushie that looks just like the shop owner. Cuteness overload!",
                            "Got a free card pack at XXX just for saying Wan Wan! Nice!",
                            "I went to XXX, and they had the friendliest staff ever. One of them gave me a belly rub (and I am not even a dog!)" 
                        } 
                    }
                }
            },
            // ItemVariety
            {
                ECustomerReviewType.ItemVariety, new Dictionary<int, List<string>>
                {
                    {
                        0, new List<string> {
                            "They dont sell YYY at XXX, Im calling the Minamaids.",
                            "No YYY? Wan out of ten.",
                            "They sell the same things at XXX.",
                            "XXX should really get more variety…",
                            "All I see is the same stuff! I cant find the YYY I wanted!",
                            "The shelves at XXX were bare today. No YYY in sight, and its not the first time.",
                            "I cant believe XXX doesnt carry YYY anymore. Its like they dont even care about Minamon!",
                            "The YYY section at XXX is drier than my love life. 0/10, would not recommend.",
                            "Came to XXX for YYY, and all I got was the store owner laughing at me. Cruel world.",
                            "I kept asking the owner if they had YYY in stock, but all they would say is Erm. Wtf does Erm even mean?"
                        }
                    },
                    {
                        1, new List<string> {
                            "The YYY at XXX is very interesting, but they tend to run out fast. Maybe stock more?",
                            "They have the basics, but nothing really stands out. More variety would be cool.",
                            "I hope in the future we get more YYY at XXX. I cant get them when I buy in there.",
                            "I found the YYY I wanted, but I wish there was more!",
                            "I found the YYY I was looking for. I hope XXX gets more things.",
                            "YYY is there, but finding it felt like navigating a maze in Hell. Worth it? Maybe.",
                            "YYY is around, but its like a British swan, blink and you will miss it.",
                            "They have got YYY, but its like they are hiding the good stuff behind a paywall. Whose OnlyWans do I have to sub to for some YYY around here?",
                            "YYY is there, but its like they are playing hard to get. LET ME LOVE YOU DAMNIT!",
                            "YYY is available, but its like they are rationing it out. This isnt the apocalypse, XXX, calm down."
                        }
                    },
                    {
                        2, new List<string> {
                            "XXX feels like a treasure trove theres always a new YYY to discover every visit!",
                            "Have you seen the YYY at XXX? They are the cutest thing in the world!!!",
                            "I found a limited edition YYY at XXX, and it made my day! They always have the best finds.",
                            "So many YYY to discover at XXX!",
                            "I love seeing so much YYY everywhere!",
                            "If you need YYY, XXX is the place to be! I was spoiled for choice.",
                            "XXX has so much YYY, I am pretty sure they are running some kind of underground YYY smuggling operation. Respect.",
                            "YYY galore! Its like XXX got drunk and went on a shopping spree. My kinda place, cheers!",
                            "XXX has as much YYY as you could desire. Honestly, I dont know whether to be impressed or concerned.",
                            "XXX has so much YYY, I felt like I stumbled into some forbidden stash. I am not asking questions."
                        }
                    }
                }
            },
            // ItemPrice
            {
                ECustomerReviewType.ItemPrice, new Dictionary<int, List<string>>
                {
                    {
                        0, new List<string> {
                            "They stole all my puppy points…",
                            "Too rich for my taste.",
                            "Why is YYY so expensive?! I am gonna STARVE!",
                            "I could have bought a whole dog house for the price of YYY at XXX!",
                            "Do they think I am made of bones or something? Way too pricey!",
                            "I had to sell my left paw just to afford YYY at XXX.",
                            "YYY was so expensive, I think they charged me in dog years!",
                            "I barked for a discount, but they raised the price instead. What a scam!",
                            "YYY costs more than my rent at XXX… and I live in a doghouse!",
                            "I would have to sell my collection of Minamon cards to afford anything here!",
                            "Is YYY made of solid gold or something?! Absolutely ridiculous."
                        }
                    },
                    {
                        1, new List<string> {
                            "This is fine.",
                            "I mean… YYY has an alright price.",
                            "I found YYY at XXX, but I had to barter my soul to get it. Fair trade, I guess.",
                            "The price of YYY is okay, but my wallet is barking at me.",
                            "YYY was decent, but I still felt a tiny bit robbed. Just a tiny bit.",
                            "YYY price wasnt outrageous, but I wouldnt call it a steal either.",
                            "YYY is there, but I wouldnt call it a deal. Just a standard tag.",
                            "I guess the price of YYY isnt awful… if youre really desperate.",
                            "YYY is fair enough, but I will be sniffing out bargains elsewhere next time.",
                            "Got what I needed, but the price left me wondering if I got scammed."
                        }
                    },
                    {
                        2, new List<string> {
                            "YYY is a bargain at XXX!",
                            "Lots of good deals at XXX.",
                            "Finally… I bought YYY at such a good price! Time to eat it…",
                            "I got YYY at such a low price, I felt like I was committing a crime!",
                            "YYY was practically free at XXX. Whos the real thief now?",
                            "YYY at XXX was so cheap, I thought it was a gift! Best deal ever!",
                            "Best prices on YYY Ive ever seen! My wallets wagging its tail!",
                            "If you love bargains, XXX is the place for YYY. Unreal prices!",
                            "YYY prices at XXX are unbeatable I feel like I am getting away with something!",
                            "I got YYY and still had enough puppy points left for a snack!",
                            "YYY prices are so low, I actually did a little victory dance in the store!"
                        }
                    }
                }
            },
            // CardPrice
            {
                ECustomerReviewType.CardPrice, new Dictionary<int, List<string>>
                {
                    {
                        0, new List<string> {
                            "They charge you an arm, leg, and then eat your notes at XXX.",
                            "Minamon priced at a kings ransom?! Outrageous!",
                            "The pack prices at XXX are so high, I might need to sell my doghouse!",
                            "Who do they think we are, royalty? These card prices are insane!",
                            "One pack at XXX cost me a weeks worth of kibble… not worth it!",
                            "Bought a pack, and now I cant afford dinner. Thanks, XXX.",
                            "If I wanted to go broke, I would just buy a Cerber instead of card packs.",
                            "The only thing rare here is the chance of leaving with a full wallet.",
                            "YYY packs are so pricey I thought I was buying Cerber sneezes, not cards!",
                            "Wanted to buy a box, but at these prices, I will settle for sobbing myself to sleep."
                        }
                    },
                    {
                        1, new List<string> {
                            "Card pack that I see, card pack that I buy.",
                            "More expensive means better odds, right?",
                            "Decent prices at XXX.",
                            "Fair prices for a bit of gamba. Tally ho!",
                            "I can get my card fix without going bankrupt… usually.",
                            "Nothing special about the prices, but at least they dont hurt my wallet too much.",
                            "Prices are fair, but lets not kid ourselves its still gacha luck.",
                            "I have seen worse prices, but these could be better.",
                            "Got my fix of packs, but my wallets a bit lighter than expected.",
                            "Prices are okay, but I might have to start budgeting for this habit."
                        }
                    },
                    {
                        2, new List<string> {
                            "The pack prices are a steal for how cute the Minamon are!",
                            "Cheap and taste good at XXX.",
                            "XXX has really good prices! My YYY was priced more than fair!",
                            "I get to have all the Minamon I could ever want for mere pocket change? Great!",
                            "The card selection at XXX? Pure bliss! Its like walking into a dream world of Minamon.",
                            "I could buy packs all day with these prices. They practically pay you to collect!",
                            "XXX prices are so low, I feel like I am robbing them with every purchase!",
                            "Best value for Minamon packs I have seen! My collections never been this happy.",
                            "YYY for just pocket change? Thank you, XXX, I will be back for more!",
                            "XXX has prices so good, it feels like a sale every day. Perfect for Minamon fanatics!"
                        }
                    }
                }
            },
            // CardRarity
            {
                ECustomerReviewType.CardRarity, new Dictionary<int, List<string>>
                {
                    {
                        0, new List<string> {
                            "Not a lot of rare cards, wish there would be more.",
                            "Where the heck are all the valuable Minawan?! RIGGED!",
                            "Pulled pack after pack and got nothing but commons. Disappointed!",
                            "I could probably find rarer cards in my peetza box...",
                            "Spent all my puppy points and didnt get a single rare Minamon!",
                            "Opening these packs is like trying to find a needle in a haystack… no rares in sight.",
                            "The only thing rare here is finding a rare card! Absolute rip-off.",
                            "All these packs and nothing valuable? Feels like a scam.",
                            "Just commons and basics… feels like they are hiding the rares somewhere.",
                            "The rare cards here are so rare, I am beginning to think they dont exist."
                        }
                    },
                    {
                        1, new List<string> {
                            "Good selection of cards.",
                            "Meh. These packs are ok, I guess.",
                            "Some decent rares show up, but dont expect anything crazy.",
                            "The odds of pulling a rare arent terrible, but they are not amazing either.",
                            "Got some nice cards, but its a bit hit or miss with the rarities.",
                            "I managed to pull a rare here and there, but its pretty average.",
                            "The packs are alright got a few good cards, nothing spectacular.",
                            "Its like a treasure hunt, sometimes you strike gold, sometimes you dont.",
                            "The rares are out there, just gotta be patient (or lucky).",
                            "Not bad pulled a few good ones, but I wouldnt call it jackpot city."
                        }
                    },
                    {
                        2, new List<string> {
                            "You can find some pretty rare stuff here!",
                            "Holy gamba! All the rare Minamon are mine!",
                            "Best place to find ultra rare Minamon, hands down!",
                            "Feels like every pack has something rare! The odds are unreal!",
                            "I struck gold with these packs. My collection has never looked better!",
                            "Pulled more rares here than anywhere else. Definitely my lucky store!",
                            "Rares, epics, legendaries this place has them all. Couldnt be happier!",
                            "Hit the jackpot with the Minamon packs here. Every collectors dream!",
                            "I cant believe how many valuable cards I found here. Totally worth it!",
                            "XXX has the best luck for rare pulls. Its like magic in every pack!"
                        }
                    }
                }
            },
            // PlayablePrice
            {
                ECustomerReviewType.PlaytablePrice, new Dictionary<int, List<string>>
                {
                    {
                        0, new List<string> {
                            "The entry fee is ridiculous! I could host my own event for that much!",
                            "They charge you just to play? Feels like a money grab.",
                            "The cost to join is higher than my decks value! Not worth it.",
                            "They want me to pay how much just to play here? Yeah, no thanks.",
                            "I would rather play in a park for free than pay these outrageous fees.",
                            "The entry fee left my wallet howling. Cant they give us a break?",
                            "Paid a fortune just to join, and didnt even get any snacks. Lame!",
                            "For that price, I would expect a royal treatment. Not impressed.",
                            "Who do they think they are with these prices? Definitely not affordable.",
                            "I cant justify the cost to play here. My wallet is barking in pain."
                        }
                    },
                    {
                        1, new List<string> {
                            "The fee is fine, I guess. Nothing special.",
                            "Decent price for what you get. Could be better, could be worse.",
                            "Not the cheapest entry, but its manageable for a good time.",
                            "Price is okay, but they could throw in a bonus pack or two.",
                            "Its a bit much, but hey, its worth it for a fun event.",
                            "Could be cheaper, but I will pay it for the chance to play here.",
                            "The fee is fair if you are really into these events.",
                            "Didnt break the bank, but its not exactly a steal either.",
                            "Price is fine if you are not expecting too much in return.",
                            "A little pricey, but the fun was worth it, so I cant complain."
                        }
                    },
                    {
                        2, new List<string> {
                            "Great price for event entry! Totally worth it.",
                            "The entry fee is so low, it feels like a gift!",
                            "Awesome deal! I would pay twice as much to play here.",
                            "I couldnt believe how affordable it was! Such a good value.",
                            "For this price, the experience was fantastic. Definitely coming back!",
                            "Best entry price around! I can play all day without going broke.",
                            "Super cheap for the fun you get out of it. Highly recommend!",
                            "At this price, I am organizing events here every weekend!",
                            "Amazing value, great way to get the community together on a budget.",
                            "So affordable! I wish every place charged as little as XXX."
                        }
                    }
                }
            },
            // SmellyCustomer
            {
                ECustomerReviewType.SmellyCustomer, new Dictionary<int, List<string>>
                {
                    {
                        0, new List<string> {
                            "I would say light a candle, but I am worried the noxious fumes may ignite. Maybe if it blew up that would be for the better though.",
                            "The smell of wet dogs at XXX is terrible. Dont go there.",
                            "SNIFFA The trashcan outside smells better.",
                            "Leaping lima beans! This business smells utterly atrocious!",
                            "HOLY, this store owner smells like they just went for a dive in a pool of wet gym socks, stinky cheese, and vinegar! Literally brought tears to my eyes.",
                            "Wow XXX sure stinks… literally and metaphorically, ever heard of deodorant?",
                            "If I wanted to smell this, I would just open my compost bin. Absolutely foul!",
                            "The scent here? Imagine a wet dog party with no ventilation. You have been warned.",
                            "I had to hold my breath while browsing… couldnt take it any longer than five minutes.",
                            "Whatever died in here, please find it and remove it. I beg you."
                        }
                    },
                    {
                        1, new List<string> {
                            "It stinked a bit but i dont know if it was me or the owner.",
                            "The store feels cozy, but the scent of dog fur is strong. Bring an allergy pill if you are sensitive.",
                            "SNIFFA",
                            "Hmmm… the smell of this place is nothing to write home about.",
                            "There was a smell at XXX, probably like peetza? Its not an issue but its weird.",
                            "The owner smells a bit, but I think I kinda like it? Am I weird for that? Do I need help?",
                            "The scent is unique… not bad, just a bit unusual.",
                            "The store has got that lived in smell. You know, like a dog friendly hangout.",
                            "You will notice the smell, but you get used to it. Adds to the charm, I guess?",
                            "A bit of a doggy odor, but nothing overpowering. Just feels like home."
                        }
                    },
                    {
                        2, new List<string> {
                            "The owner of XXX can give you a bath if you dont feel like taking one yourself.",
                            "Didnt know dogs could smell this good!",
                            "No stinkies here. SNIFFA",
                            "My word! The aroma of this establishment is absolutely heavenly!",
                            "The owner of XXX is so dedicated to cleanliness they scream at people who smell. BASED.",
                            "XXX smells as fresh as a mountain breeze. Delightful!",
                            "This place smells like puppy heaven! Fresh, clean, and cozy.",
                            "I expected dog smell, but its actually wonderfully fragrant here. Pleasant surprise!",
                            "Every time I come here, the smell is perfection. Feels like a breath of fresh air.",
                            "The scent in this store? Like roses and sunshine! Truly a cut above the rest."
                        }
                    }
                }
            },
            // BlockedStore
            {
                ECustomerReviewType.BlockedStore, new Dictionary<int, List<string>>
                {
                    {
                        0, new List<string> {
                            "The owner of XXX sometimes stands in the door… menacingly…...",
                            "Is there another entrance at XXX?",
                            "Doko entrance?",
                            "I am on my hands and knees… PLEASE let me in so I can get some minacards…",
                            "I cant enter the store, there is a suspiciously transparent window blocking the way.",
                            "Tried to get in, but a pile of boxes was just… there. Blocking my dreams of Minamon!",
                            "The entrance was blocked by a mountain of dog toys. Is this a test?",
                            "Open na noor.",
                            "Almost got in, but the door is blocked by crates of Minamon. Its like they dont want customers!",
                            "Stuck outside, staring at the Minamon packs through the glass. Truly heartbreaking.",
                            "Is this a secret club? Because the entrance is always mysteriously blocked.",
                            "Every time I come here, something is blocking the door. How am I supposed to shop?!",
                            "The entrance is practically a labyrinth of random stuff. Please clear a path!",
                            "I think they stacked the whole inventory in front of the door. Is this a joke?",
                            "Let me in, let me in! Why is the door blocked by the owners collection again?!"
                        }
                    }
                }
            },
            // OwnerOpenPack
            {
                ECustomerReviewType.OwnerOpenPack, new Dictionary<int, List<string>>
                {
                    {
                        0, new List<string> {
                            "The owner of XXX started to shout “GAMBA!” while opening a card pack.",
                            "The owner just rips through card packs all day.",
                            "Those packs are for me, not them! Stop it, owner!",
                            "Every time I walk in, the owner is tearing into packs. Save some for us!",
                            "The owners obsession with opening packs is out of control. Leave some mystery for the customers!",
                            "I heard the owner muttering, One more, just one more, as they opened yet another pack. It is becoming a problem.",
                            "Cant buy a single pack without seeing the owner crack one open first. Feels unfair!",
                            "The owner goes through packs like there is no tomorrow. I just wanted one unopened pack!",
                            "Its like the owner is their own best customer… stop opening the stock!",
                            "I came for Minamon cards, not a live pack opening show. Show some restraint!",
                            "Trying to buy packs, but the owners opening spree is hard to ignore. Makes me jealous!",
                            "Owner opens packs faster than I can count. Save some for the paying customers!",
                            "The owners pack addiction is real. I just want one unopened pack, please!",
                            "I felt like I was intruding on the owners personal gacha spree. Calm down, please!",
                            "Do they even sell cards here, or does the owner just open them all? Feels like a tease!"
                        }
                    }
                }
            },
            // GiveManyChangePennies
            {
                ECustomerReviewType.GiveManyChangePennies, new Dictionary<int, List<string>>
                {
                    {
                        0, new List<string> {
                            "Looks like they have coins at XXX.",
                            "I wonder if they sell chocolate coins at XXX.",
                            "Wanted notes to eat ended up with metal coins.",
                            "Miyawan would be happy. I am not Miyawan.",
                            "I dont want your dang change! What the heck am I supposed to do with these?!",
                            "Paid in cash, worst mistake of my life. I am bringing a forklift next time.",
                            "Left with more coins than I could carry. Should I open a coin bank now?",
                            "Got enough pennies to open my own treasure chest. Not the loot I wanted!",
                            "They handed me a sack of coins like its a medieval barter. Keep the change!",
                            "I am jingling all the way home from XXX. Feels like a bad prank.",
                            "Tried to pay in bills and got a waterfall of coins in return. Never again.",
                            "Received my change in coins. Feels like I just robbed a wishing well.",
                            "Ended up with enough coins to buy another Minamon pack… if I could lift them.",
                            "I cant fit all these coins in my pocket. Had to leave some behind as tip.",
                            "If you love coins, this is the place. Otherwise, prepare for a metallic mess."
                        }
                    }
                }
            }
        };

        static void Postfix(ref string __result, CustomerReviewData reviewData)
        {
            if (reviewVariations.TryGetValue(reviewData.customerReviewType, out var levelDictionary))
            {
                if (levelDictionary.TryGetValue(reviewData.textSOGoodBadLevel, out var options))
                {
                    string selectedReview = options[UnityEngine.Random.Range(0, options.Count)];
                    selectedReview = selectedReview.Replace("XXX", CPlayerData.PlayerName).Replace("YYY", InventoryBase.GetItemData(reviewData.itemType).GetName());
                    __result = selectedReview;
                }
            }
        }
    }
    
    
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