using UnityEngine;
using UnityEngine.UI;
using TMPro;

public sealed class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Vector2 windowSize = new Vector2(360f, 480f);
    [SerializeField, Min(10)] private int titleFontSize = 36;
    [SerializeField, Min(8)] private int stageFontSize = 18;
    [SerializeField, Min(8)] private int lockFontSize = 14;
    [SerializeField, Min(20f)] private float buttonHeight = 44f;
    [SerializeField] private Color lockedColor = new Color(0.5f, 0.5f, 0.5f, 1f);
    [SerializeField] private Color unlockedColor = Color.white;
    [SerializeField] private Color bossStageColor = new Color(1f, 0.7f, 0.2f, 1f);
    [SerializeField] private Color tutorialColor = new Color(0.3f, 0.8f, 1f, 1f);
    [SerializeField] private Sprite backgroundSprite;
    [SerializeField] private Sprite buttonSprite;
    [SerializeField] private Sprite decorativeCharacterSprite;
    [SerializeField] private Vector2 decorativePosition = new Vector2(-380f, 0f);
    [SerializeField] private Vector3 decorativeScale = new Vector3(1.2f, 1.2f, 1f);
    [SerializeField] private GameObject characterSelectionGO;
    [SerializeField] private GameObject weaponSelectionGO;

    private enum MenuState { Main, StageSelect, Settings }
    private MenuState currentState = MenuState.Main;

    private GameObject mainButtonPanel;
    private GameObject settingsPanel;

    private GUIStyle titleStyle;
    private GUIStyle stageStyle;
    private GUIStyle lockStyle;
    private GUIStyle buttonStyle;
    private GUIStyle tutorialButtonStyle;

    private static Texture2D _boxBgTexture;
    private static TMP_FontAsset _defaultFont;

    private void Awake()
    {
        if (backgroundSprite == null)
            backgroundSprite = Resources.Load<Sprite>("MainMenu/bg");
        if (buttonSprite == null)
            buttonSprite = Resources.Load<Sprite>("MainMenu/button");

        if (_defaultFont == null)
            _defaultFont = TryGetDefaultFont();

        CreateCanvasUI();
        CreateSelectionUIs();
    }

    private void CreateSelectionUIs()
    {
        if (characterSelectionGO == null)
        {
            characterSelectionGO = new GameObject("CharacterSelection");
            characterSelectionGO.transform.SetParent(transform, false);
            characterSelectionGO.SetActive(false);
            CharacterSelectionUI csUI = characterSelectionGO.AddComponent<CharacterSelectionUI>();
            csUI.mainMenuUI = this;
            csUI.enabled = false;
        }

        if (weaponSelectionGO == null)
        {
            weaponSelectionGO = new GameObject("WeaponSelection");
            weaponSelectionGO.transform.SetParent(transform, false);
            weaponSelectionGO.SetActive(false);
            WeaponSelectionUI wsUI = weaponSelectionGO.AddComponent<WeaponSelectionUI>();
            wsUI.mainMenuUI = this;
            wsUI.enabled = false;
        }
    }

    private static TMP_FontAsset TryGetDefaultFont()
    {
        try
        {
            if (TMP_Settings.defaultFontAsset != null)
                return TMP_Settings.defaultFontAsset;
        }
        catch { }

        TMP_FontAsset font = Resources.Load<TMP_FontAsset>("LiberationSans SDF");
        if (font == null)
            font = Resources.Load<TMP_FontAsset>("LiberationSans SDF - Fallback");

        return font;
    }

    private void CreateCanvasUI()
    {
        GameObject canvasGO = new GameObject("MainMenuCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvasGO.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvasGO.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        GameObject bgGO = new GameObject("Background", typeof(RawImage));
        bgGO.transform.SetParent(canvasGO.transform, false);
        RawImage bgRaw = bgGO.GetComponent<RawImage>();
        if (backgroundSprite != null)
            bgRaw.texture = backgroundSprite.texture;
        else
            bgRaw.color = new Color(0.12f, 0.12f, 0.18f);

        RectTransform bgRT = bgGO.GetComponent<RectTransform>();
        bgRT.anchorMin = Vector2.zero;
        bgRT.anchorMax = Vector2.one;
        bgRT.sizeDelta = Vector2.zero;

        if (decorativeCharacterSprite == null)
            decorativeCharacterSprite = Resources.Load<Sprite>("MainMenu/Clip path group(2)");

        if (decorativeCharacterSprite != null)
        {
            GameObject charGO = new GameObject("DecorativeCharacter", typeof(RectTransform));
            charGO.transform.SetParent(canvasGO.transform, false);
            Image charImage = charGO.AddComponent<Image>();
            charImage.sprite = decorativeCharacterSprite;
            charImage.SetNativeSize();

            RectTransform charRT = charGO.GetComponent<RectTransform>();
            charRT.anchorMin = new Vector2(0.5f, 0f);
            charRT.anchorMax = new Vector2(0.5f, 0f);
            charRT.pivot = new Vector2(0.5f, 0f);
            charRT.anchoredPosition = decorativePosition;
            charRT.localScale = decorativeScale;
        }

        mainButtonPanel = new GameObject("ButtonPanel", typeof(RectTransform));
        mainButtonPanel.transform.SetParent(canvasGO.transform, false);

        RectTransform bpRT = mainButtonPanel.GetComponent<RectTransform>();
        bpRT.anchorMin = new Vector2(0f, 0f);
        bpRT.anchorMax = new Vector2(0f, 0f);
        bpRT.pivot = new Vector2(0f, 0f);
        bpRT.anchoredPosition = new Vector2(30f, 30f);

        VerticalLayoutGroup vlg = mainButtonPanel.AddComponent<VerticalLayoutGroup>();
        vlg.spacing = 12f;
        vlg.padding = new RectOffset(10, 10, 10, 10);
        vlg.childAlignment = TextAnchor.LowerLeft;
        vlg.childControlWidth = false;
        vlg.childControlHeight = false;
        vlg.childForceExpandWidth = false;
        vlg.childForceExpandHeight = false;

        ContentSizeFitter csf = mainButtonPanel.AddComponent<ContentSizeFitter>();
        csf.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        CreateButton("BtnBatDau", "Bắt đầu", mainButtonPanel.transform, () =>
        {
            mainButtonPanel.SetActive(false);
            characterSelectionGO.SetActive(true);
            CharacterSelectionUI csUI = characterSelectionGO.GetComponent<CharacterSelectionUI>();
            if (csUI != null) csUI.Show();
        });

        CreateButton("BtnCaiDat", "Cài đặt", mainButtonPanel.transform, () =>
        {
            currentState = MenuState.Settings;
            settingsPanel.SetActive(true);
        });

        CreateButton("BtnThoat", "Thoát", mainButtonPanel.transform, Application.Quit);

        settingsPanel = CreateSettingsPanel(canvasGO.transform);
        settingsPanel.SetActive(false);

        if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            new GameObject("EventSystem", typeof(UnityEngine.EventSystems.EventSystem),
                typeof(UnityEngine.EventSystems.StandaloneInputModule));
        }
    }

    private void CreateButton(string name, string text, Transform parent, UnityEngine.Events.UnityAction onClick)
    {
        GameObject btnGO = new GameObject(name, typeof(RectTransform));
        btnGO.transform.SetParent(parent, false);

        Image image = btnGO.AddComponent<Image>();
        if (buttonSprite != null) image.sprite = buttonSprite;
        image.type = Image.Type.Sliced;

        Button button = btnGO.AddComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(onClick);

        try
        {
            GameObject textGO = new GameObject("Text", typeof(RectTransform));
            textGO.transform.SetParent(btnGO.transform, false);

            TextMeshProUGUI textComp = textGO.AddComponent<TextMeshProUGUI>();
            textComp.font = _defaultFont;
            textComp.text = text;
            textComp.fontSize = 28;
            textComp.alignment = TextAlignmentOptions.Center;
            textComp.color = Color.white;

            RectTransform textRT = textGO.GetComponent<RectTransform>();
            textRT.anchorMin = Vector2.zero;
            textRT.anchorMax = Vector2.one;
            textRT.sizeDelta = Vector2.zero;
        }
        catch { }

        RectTransform btnRT = btnGO.GetComponent<RectTransform>();
        btnRT.sizeDelta = new Vector2(220f, 64f);
    }

    private GameObject CreateSettingsPanel(Transform parent)
    {
        GameObject panelGO = new GameObject("SettingsPanel", typeof(RectTransform));
        panelGO.transform.SetParent(parent, false);

        RectTransform panelRT = panelGO.GetComponent<RectTransform>();
        panelRT.anchorMin = new Vector2(0.5f, 0.5f);
        panelRT.anchorMax = new Vector2(0.5f, 0.5f);
        panelRT.pivot = new Vector2(0.5f, 0.5f);
        panelRT.sizeDelta = new Vector2(420f, 220f);

        Image panelImage = panelGO.AddComponent<Image>();
        panelImage.color = new Color(0.1f, 0.1f, 0.1f, 0.92f);

        GameObject labelGO = new GameObject("Label", typeof(RectTransform));
        labelGO.transform.SetParent(panelGO.transform, false);

        try
        {
            TextMeshProUGUI labelText = labelGO.AddComponent<TextMeshProUGUI>();
            labelText.font = _defaultFont;
            labelText.text = "Âm lượng";
            labelText.fontSize = 30;
            labelText.alignment = TextAlignmentOptions.Center;
            labelText.color = Color.white;
        }
        catch { }

        RectTransform labelRT = labelGO.GetComponent<RectTransform>();
        labelRT.anchorMin = new Vector2(0f, 0.5f);
        labelRT.anchorMax = new Vector2(1f, 1f);
        labelRT.sizeDelta = Vector2.zero;
        labelRT.offsetMin = new Vector2(20f, 20f);
        labelRT.offsetMax = new Vector2(-20f, -20f);

        GameObject sliderGO = new GameObject("VolumeSlider", typeof(RectTransform));
        sliderGO.transform.SetParent(panelGO.transform, false);

        RectTransform sliderRT = sliderGO.GetComponent<RectTransform>();
        sliderRT.anchorMin = new Vector2(0f, 0f);
        sliderRT.anchorMax = new Vector2(1f, 0.5f);
        sliderRT.sizeDelta = Vector2.zero;
        sliderRT.offsetMin = new Vector2(40f, 30f);
        sliderRT.offsetMax = new Vector2(-40f, 50f);

        GameObject sliderBG = new GameObject("Background", typeof(RectTransform));
        sliderBG.transform.SetParent(sliderGO.transform, false);
        Image sliderBgImage = sliderBG.AddComponent<Image>();
        sliderBgImage.color = new Color(0.3f, 0.3f, 0.3f, 1f);
        RectTransform sbRT = sliderBG.GetComponent<RectTransform>();
        sbRT.anchorMin = Vector2.zero;
        sbRT.anchorMax = Vector2.one;
        sbRT.sizeDelta = new Vector2(0f, 16f);

        GameObject fillArea = new GameObject("Fill Area", typeof(RectTransform));
        fillArea.transform.SetParent(sliderGO.transform, false);
        RectTransform faRT = fillArea.GetComponent<RectTransform>();
        faRT.anchorMin = new Vector2(0f, 0f);
        faRT.anchorMax = new Vector2(1f, 1f);
        faRT.sizeDelta = new Vector2(-20f, 0f);
        faRT.offsetMin = new Vector2(10f, 0f);
        faRT.offsetMax = new Vector2(-10f, 0f);

        GameObject fill = new GameObject("Fill", typeof(RectTransform));
        fill.transform.SetParent(fillArea.transform, false);
        Image fillImage = fill.AddComponent<Image>();
        fillImage.color = new Color(0.2f, 0.6f, 1f, 1f);
        RectTransform fillRT = fill.GetComponent<RectTransform>();
        fillRT.anchorMin = Vector2.zero;
        fillRT.anchorMax = Vector2.one;
        fillRT.sizeDelta = Vector2.zero;

        GameObject handleArea = new GameObject("Handle Slide Area", typeof(RectTransform));
        handleArea.transform.SetParent(sliderGO.transform, false);
        RectTransform haRT = handleArea.GetComponent<RectTransform>();
        haRT.anchorMin = new Vector2(0f, 0f);
        haRT.anchorMax = new Vector2(1f, 1f);
        haRT.sizeDelta = new Vector2(-20f, 0f);
        haRT.offsetMin = new Vector2(10f, 0f);
        haRT.offsetMax = new Vector2(-10f, 0f);

        GameObject handle = new GameObject("Handle", typeof(RectTransform));
        handle.transform.SetParent(handleArea.transform, false);
        Image handleImage = handle.AddComponent<Image>();
        handleImage.color = Color.white;
        RectTransform handleRT = handle.GetComponent<RectTransform>();
        handleRT.sizeDelta = new Vector2(28f, 28f);

        Slider slider = sliderGO.AddComponent<Slider>();
        slider.fillRect = fillRT;
        slider.handleRect = handleRT;
        slider.targetGraphic = handleImage;
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = AudioListener.volume;
        slider.onValueChanged.AddListener(v => AudioListener.volume = v);

        GameObject closeBtnGO = new GameObject("CloseBtn", typeof(RectTransform));
        closeBtnGO.transform.SetParent(panelGO.transform, false);

        Image closeBtnImage = closeBtnGO.AddComponent<Image>();
        closeBtnImage.color = new Color(0.5f, 0.1f, 0.1f, 1f);

        try
        {
            TextMeshProUGUI closeText = closeBtnGO.AddComponent<TextMeshProUGUI>();
            closeText.font = _defaultFont;
            closeText.text = "X";
            closeText.fontSize = 22;
            closeText.alignment = TextAlignmentOptions.Center;
            closeText.color = Color.white;
        }
        catch { }

        Button closeBtn = closeBtnGO.AddComponent<Button>();
        closeBtn.targetGraphic = closeBtnImage;
        closeBtn.onClick.AddListener(() =>
        {
            settingsPanel.SetActive(false);
            currentState = MenuState.Main;
        });

        RectTransform closeRT = closeBtnGO.GetComponent<RectTransform>();
        closeRT.anchorMin = new Vector2(1f, 1f);
        closeRT.anchorMax = new Vector2(1f, 1f);
        closeRT.pivot = new Vector2(1f, 1f);
        closeRT.sizeDelta = new Vector2(40f, 40f);
        closeRT.anchoredPosition = Vector2.zero;

        return panelGO;
    }

    private void OnGUI()
    {
        if (currentState != MenuState.StageSelect) return;

        if (GameManager.Instance == null)
        {
            return;
        }

        if (GameManager.Instance.StageProgression == null)
        {
            GUI.Box(new Rect(0, 0, Screen.width, Screen.height), GUIContent.none);
            GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("StageProgression not found!\nRun Tools > Create All Stage Assets in Unity Editor.",
                new GUIStyle(GUI.skin.label) { fontSize = 20, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter });
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.EndArea();
            return;
        }

        EnsureStyles();

        StageProgression progression = GameManager.Instance.StageProgression;

        Rect windowRect = new Rect(
            (Screen.width - windowSize.x) * 0.5f,
            (Screen.height - windowSize.y) * 0.5f,
            windowSize.x,
            windowSize.y);

        if (_boxBgTexture == null)
        {
            _boxBgTexture = new Texture2D(1, 1);
            _boxBgTexture.SetPixel(0, 0, new Color(0f, 0f, 0f, 0.75f));
            _boxBgTexture.Apply();
        }
        GUI.DrawTexture(windowRect, _boxBgTexture);

        GUILayout.BeginArea(new Rect(windowRect.x + 20f, windowRect.y + 16f,
                                     windowRect.width - 40f, windowRect.height - 32f));

        titleStyle.normal.textColor = bossStageColor;
        GUILayout.Label("Nghịch Mệnh Tiên Đồ", titleStyle);
        GUILayout.Space(16f);

        Color originalColor = GUI.color;
        GUI.color = tutorialColor;
        if (GUILayout.Button("Hướng dẫn", tutorialButtonStyle, GUILayout.Height(buttonHeight)))
        {
            GameManager.Instance.LoadTutorial();
        }
        GUI.color = originalColor;
        GUILayout.Space(12f);

        GUI.color = Color.gray;
        if (GUILayout.Button("\u2190 Quay l\u1ea1i", buttonStyle, GUILayout.Height(buttonHeight)))
        {
            currentState = MenuState.Main;
            if (mainButtonPanel != null) mainButtonPanel.SetActive(true);
        }
        GUI.color = originalColor;
        GUILayout.Space(12f);

        for (int i = 0; i < progression.TotalStages; i++)
        {
            StageData stage = progression.GetStage(i);
            if (stage == null)
            {
                continue;
            }

            bool unlocked = GameManager.Instance.IsStageUnlocked(i);
            bool isBoss = stage.CombatMode == CombatManager.CombatMode.BossCombat;

            string label = unlocked
                ? $"{(isBoss ? "\U0001f451 " : "")}Stage {stage.StageNumber}: {stage.StageName}"
                : $"Stage {stage.StageNumber}: ???";

            if (unlocked)
            {
                GUI.color = isBoss ? bossStageColor : unlockedColor;
            }
            else
            {
                GUI.color = lockedColor;
            }

            if (unlocked)
            {
                if (GUILayout.Button(label, buttonStyle, GUILayout.Height(buttonHeight)))
                {
                    GameManager.Instance.TryLoadStage(i);
                }
            }
            else
            {
                GUILayout.Label(label, lockStyle, GUILayout.Height(buttonHeight));
            }

            GUI.color = originalColor;
        }

        GUILayout.FlexibleSpace();
        GUILayout.EndArea();
    }

    public void ShowWeaponSelection()
    {
        weaponSelectionGO.SetActive(true);
        WeaponSelectionUI wsUI = weaponSelectionGO.GetComponent<WeaponSelectionUI>();
        if (wsUI != null) wsUI.Show();
    }

    public void ShowStageSelect()
    {
        currentState = MenuState.StageSelect;
        weaponSelectionGO.SetActive(false);
    }

    private void EnsureStyles()
    {
        if (titleStyle != null && titleStyle.fontSize == titleFontSize)
        {
            return;
        }

        titleStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = titleFontSize,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter
        };

        stageStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = stageFontSize,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleLeft
        };

        lockStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = lockFontSize,
            fontStyle = FontStyle.Italic,
            alignment = TextAnchor.MiddleCenter
        };

        buttonStyle = new GUIStyle(GUI.skin.button)
        {
            fontSize = stageFontSize,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleLeft
        };

        tutorialButtonStyle = new GUIStyle(GUI.skin.button)
        {
            fontSize = stageFontSize,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter
        };
    }
}
