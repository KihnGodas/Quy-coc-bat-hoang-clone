using UnityEngine;

public sealed class DialogueUI : MonoBehaviour
{
    [Header("Text Bar")]
    [SerializeField] private float textBarHeightPercent = 0.22f;
    [SerializeField] private float textBarPadding = 20f;
    [SerializeField] private float nameHeight = 30f;
    [SerializeField] private float textPadding = 16f;
    [SerializeField] private int nameFontSize = 22;
    [SerializeField] private int dialogueFontSize = 18;
    [SerializeField] private Color textBarBgColor = new Color(0.08f, 0.08f, 0.08f, 0.92f);
    [SerializeField] private Color textBarBorderColor = new Color(0.3f, 0.3f, 0.3f, 1f);
    [SerializeField] private Color nameTextColor = new Color(1f, 0.85f, 0.4f, 1f);
    [SerializeField] private Color dialogueTextColor = Color.white;
    [SerializeField] private Color continueHintColor = new Color(1f, 1f, 1f, 0.5f);

    [Header("Portraits")]
    [SerializeField] private float portraitSizePercent = 0.28f;
    [SerializeField] private float portraitYOffsetPercent = 0.02f;
    [SerializeField] private float portraitBottomMargin = 16f;
    [SerializeField] private float dimAlpha = 0.3f;

    [Header("Typewriter")]
    [SerializeField, Min(0f)] private float typewriterSpeed = 40f;

    private DialogueManager manager;
    private GUIStyle nameStyle;
    private GUIStyle dialogueStyle;
    private GUIStyle continueStyle;
    private Texture2D whiteTexture;

    private SpeakerSO speakerLeft;
    private SpeakerSO speakerRight;
    private int typewriterIndex;
    private float typewriterTimer;
    private bool textFullyRevealed;
    private bool inputPressedThisFrame;

    private void Awake()
    {
        InitializeWhiteTexture();
        FindManager();
    }

    private void OnEnable()
    {
        FindManager();
        if (manager != null)
        {
            manager.OnDialogueStart += HandleDialogueStart;
            manager.OnDialogueLineChanged += HandleDialogueLineChanged;
            manager.OnDialogueEnd += HandleDialogueEnd;
        }
    }

    private void OnDisable()
    {
        if (manager != null)
        {
            manager.OnDialogueStart -= HandleDialogueStart;
            manager.OnDialogueLineChanged -= HandleDialogueLineChanged;
            manager.OnDialogueEnd -= HandleDialogueEnd;
        }
    }

    private void FindManager()
    {
        if (manager == null)
            manager = FindComponentInScene<DialogueManager>();
    }

    private void HandleDialogueStart()
    {
        speakerLeft = null;
        speakerRight = null;
        ResetTypewriter();
    }

    private void HandleDialogueLineChanged()
    {
        ResetTypewriter();
    }

    private void HandleDialogueEnd()
    {
        speakerLeft = null;
        speakerRight = null;
    }

    private void ResetTypewriter()
    {
        typewriterIndex = 0;
        typewriterTimer = 0f;
        textFullyRevealed = false;
        inputPressedThisFrame = false;
    }

    private void Update()
    {
        if (manager == null || !manager.IsPlaying) return;

        inputPressedThisFrame = false;
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            inputPressedThisFrame = true;
        }

        DialogueLine line = manager.CurrentLine;

        if (!textFullyRevealed)
        {
            typewriterTimer += Time.unscaledDeltaTime * typewriterSpeed;
            if (line.text != null && typewriterIndex < line.text.Length)
            {
                typewriterIndex = Mathf.Min(typewriterIndex + Mathf.FloorToInt(typewriterTimer), line.text.Length);
                typewriterTimer = 0f;
            }
            if (typewriterIndex >= (line.text?.Length ?? 0))
            {
                textFullyRevealed = true;
                if (line.autoAdvance)
                    manager.SetAutoAdvance(line.autoAdvanceDelay);
            }
            if (inputPressedThisFrame && !textFullyRevealed)
            {
                typewriterIndex = manager.CurrentLine.text?.Length ?? 0;
                textFullyRevealed = true;
                if (line.autoAdvance)
                    manager.SetAutoAdvance(line.autoAdvanceDelay);
            }
        }
        else if (inputPressedThisFrame)
        {
            if (line.autoAdvance)
            {
                manager.CancelAutoAdvance();
                manager.AdvanceLine();
            }
            else if (manager != null && manager.IsPlaying)
            {
                manager.AdvanceLine();
            }
        }
    }

    private void OnGUI()
    {
        if (manager == null || !manager.IsPlaying) return;

        EnsureStyles();

        DialogueLine currentLine = manager.CurrentLine;
        if (currentLine.speaker == null) return;

        AssignSpeakerSlots(currentLine.speaker);

        Rect textBarRect = GetTextBarRect();
        DrawTextBar(textBarRect, currentLine);

        DrawPortrait(speakerLeft, currentLine.speaker, true);
        DrawPortrait(speakerRight, currentLine.speaker, false);
    }

    private void AssignSpeakerSlots(SpeakerSO currentSpeaker)
    {
        if (speakerLeft == null && speakerRight == null)
        {
            speakerLeft = currentSpeaker;
            return;
        }

        if (speakerLeft != currentSpeaker && speakerRight == null)
        {
            speakerRight = currentSpeaker;
            return;
        }

        if (speakerLeft == null && speakerRight != currentSpeaker)
        {
            speakerLeft = currentSpeaker;
        }
    }

    private Rect GetTextBarRect()
    {
        float barHeight = Screen.height * textBarHeightPercent;
        return new Rect(textBarPadding, Screen.height - barHeight - textBarPadding,
            Screen.width - textBarPadding * 2, barHeight);
    }

    private void DrawTextBar(Rect barRect, DialogueLine line)
    {
        GUI.color = textBarBgColor;
        GUI.DrawTexture(barRect, whiteTexture);
        GUI.color = textBarBorderColor;
        float t = 2f;
        GUI.DrawTexture(new Rect(barRect.x, barRect.y, barRect.width, t), whiteTexture);
        GUI.color = Color.white;

        float nameLabelHeight = nameHeight;
        Rect nameRect = new Rect(barRect.x + textPadding, barRect.y + 6f, barRect.width - textPadding * 2, nameLabelHeight);
        GUI.Label(nameRect, line.speaker.SpeakerName, nameStyle);

        float textY = nameRect.yMax + 2f;
        Rect textRect = new Rect(barRect.x + textPadding, textY, barRect.width - textPadding * 2,
            barRect.yMax - textY - textPadding);
        string displayText = line.text ?? "";
        if (!textFullyRevealed && typewriterIndex < displayText.Length)
            displayText = displayText.Substring(0, typewriterIndex);
        GUI.Label(textRect, displayText, dialogueStyle);

        if (textFullyRevealed)
        {
            float pulse = Mathf.Sin(Time.realtimeSinceStartup * 4f) * 0.3f + 0.7f;
            GUI.color = new Color(continueHintColor.r, continueHintColor.g, continueHintColor.b,
                continueHintColor.a * pulse);
            string hint = manager.CurrentLineIndex >= manager.CurrentDialogue.LineCount - 1 ? "[Kết thúc]" : "[Nhấn để tiếp]";
            Vector2 hintSize = continueStyle.CalcSize(new GUIContent(hint));
            GUI.Label(new Rect(barRect.xMax - hintSize.x - textPadding,
                barRect.yMax - hintSize.y - 4f, hintSize.x, hintSize.y), hint, continueStyle);
            GUI.color = Color.white;
        }
    }

    private void DrawPortrait(SpeakerSO speaker, SpeakerSO activeSpeaker, bool isLeft)
    {
        if (speaker == null) return;

        bool isSpeaking = speaker == activeSpeaker;
        Texture2D portrait = speaker.GetOrCreatePortrait();
        if (portrait == null) return;

        float portraitSize = Screen.width * portraitSizePercent;
        float barTop = Screen.height - Screen.height * textBarHeightPercent - textBarPadding;
        float maxY = barTop - portraitBottomMargin;
        float portraitY = Mathf.Min(portraitYOffsetPercent * Screen.height, maxY - portraitSize);

        float portraitX;
        if (isLeft)
            portraitX = textBarPadding + 10f;
        else
            portraitX = Screen.width - textBarPadding - 10f - portraitSize;

        if (portraitY + portraitSize > maxY)
            portraitY = maxY - portraitSize;
        if (portraitY < 0f)
            portraitY = 0f;

        Rect portraitRect = new Rect(portraitX, portraitY, portraitSize, portraitSize);

        Color portraitColor = isSpeaking ? speaker.HighlightColor : speaker.DimColor;
        GUI.color = portraitColor;
        GUI.DrawTexture(portraitRect, portrait);

        if (isSpeaking)
        {
            GUI.color = new Color(0.3f, 0.3f, 0.3f, 0.3f);
            float border = 3f;
            GUI.DrawTexture(new Rect(portraitRect.x - border, portraitRect.y - border,
                portraitRect.width + border * 2, border), whiteTexture);
            GUI.DrawTexture(new Rect(portraitRect.x - border, portraitRect.yMax,
                portraitRect.width + border * 2, border), whiteTexture);
            GUI.DrawTexture(new Rect(portraitRect.x - border, portraitRect.y, border,
                portraitRect.height), whiteTexture);
            GUI.DrawTexture(new Rect(portraitRect.xMax, portraitRect.y, border,
                portraitRect.height), whiteTexture);
        }

        GUI.color = Color.white;
    }

    private void EnsureStyles()
    {
        if (nameStyle == null || nameStyle.fontSize != nameFontSize)
        {
            nameStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = nameFontSize,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.LowerLeft,
                normal = { textColor = nameTextColor }
            };
        }
        if (dialogueStyle == null || dialogueStyle.fontSize != dialogueFontSize)
        {
            dialogueStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = dialogueFontSize,
                fontStyle = FontStyle.Normal,
                alignment = TextAnchor.UpperLeft,
                wordWrap = true,
                normal = { textColor = dialogueTextColor }
            };
        }
        if (continueStyle == null)
        {
            continueStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = Mathf.RoundToInt(dialogueFontSize * 0.8f),
                fontStyle = FontStyle.Italic,
                alignment = TextAnchor.MiddleRight,
                normal = { textColor = continueHintColor }
            };
        }
    }

    private void InitializeWhiteTexture()
    {
        whiteTexture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        whiteTexture.SetPixel(0, 0, Color.white);
        whiteTexture.Apply();
    }

    private static T FindComponentInScene<T>() where T : Object
    {
#if UNITY_2023_1_OR_NEWER
        return FindFirstObjectByType<T>();
#else
        return FindObjectOfType<T>();
#endif
    }
}
