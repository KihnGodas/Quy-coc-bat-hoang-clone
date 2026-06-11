using UnityEngine;

public sealed class TutorialUI : MonoBehaviour
{
    [SerializeField] private TutorialManager tutorialManager;

    [Header("Layout")]
    [SerializeField] private float instructionY = 0f;
    [SerializeField] private float instructionWidth = 560f;
    [SerializeField] private float instructionHeight = 90f;
    [SerializeField] private int titleFontSize = 34;
    [SerializeField] private int instructionFontSize = 20;
    [SerializeField] private int subtitleFontSize = 15;
    [SerializeField] private int progressFontSize = 14;

    [Header("Colors")]
    [SerializeField] private Color instructionBgColor = new Color(0f, 0f, 0f, 0.6f);
    [SerializeField] private Color instructionTextColor = Color.white;
    [SerializeField] private Color completedColor = new Color(0.45f, 1f, 0.55f, 1f);
    [SerializeField] private Color progressColor = new Color(1f, 0.82f, 0.35f, 1f);

    private GUIStyle instructionStyle;
    private GUIStyle subtitleStyle;
    private GUIStyle progressStyle;
    private GUIStyle buttonStyle;
    private Texture2D whiteTexture;

    private static readonly string[] InstructionTexts = new[]
    {
        "Dùng W A S D hoặc các phím mũi tên để di chuyển",
        "Nhấn Space để lướt nhanh (Bộ Pháp)",
        "Di chuyển chuột để ngắm mục tiêu",
        "Nhấn Q để dùng kỹ năng vũ khí",
        "Nhấn E để thi triển Công Pháp",
        "Nhấn R khi Ultimate đã sẵn sàng"
    };

    private void Awake()
    {
        ResolveReferences();
        InitializeWhiteTexture();
    }

    private void OnGUI()
    {
        EnsureStyles();

        if (tutorialManager == null) return;

        DialogueManager dm = DialogueManager.Instance;
        if (dm != null && dm.IsPlaying) return;

        if (tutorialManager.AllCompleted)
        {
            DrawCompletionScreen();
            return;
        }

        DrawInstructionPanel();
    }

    private void DrawInstructionPanel()
    {
        int step = (int)tutorialManager.CurrentStep;
        if (step < 0 || step >= InstructionTexts.Length) return;

        string text = InstructionTexts[step];
        string hint = GetHint(step);

        float panelWidth = instructionWidth + 40f;
        float panelHeight = instructionHeight + 30f;
        float x = (Screen.width - panelWidth) * 0.5f;
        float y = instructionY > 0f ? instructionY : Screen.height * 0.2f;

        GUI.color = instructionBgColor;
        GUI.DrawTexture(new Rect(x, y, panelWidth, panelHeight), whiteTexture);
        GUI.color = Color.white;

        instructionStyle.normal.textColor = instructionTextColor;
        GUI.Label(new Rect(x + 20f, y + 12f, instructionWidth, 40f), text, instructionStyle);

        if (!string.IsNullOrEmpty(hint))
        {
            subtitleStyle.normal.textColor = new Color(0.8f, 0.8f, 0.8f, 0.7f);
            GUI.Label(new Rect(x + 20f, y + 52f, instructionWidth, 25f), hint, subtitleStyle);
        }

        DrawProgressBar(panelWidth, panelY: y, step);
    }

    private void DrawProgressBar(float panelWidth, float panelY, int step)
    {
        float barWidth = 260f;
        float barHeight = 5f;
        float panelX = (Screen.width - panelWidth) * 0.5f;

        float x = panelX + 20f;
        float y = panelY + instructionHeight + 15f;

        GUI.color = new Color(0.2f, 0.2f, 0.2f, 0.7f);
        GUI.DrawTexture(new Rect(x, y, barWidth, barHeight), whiteTexture);

        GUI.color = progressColor;
        float fill = Mathf.Clamp01(tutorialManager.CompletionProgress);
        GUI.DrawTexture(new Rect(x, y, barWidth * fill, barHeight), whiteTexture);
        GUI.color = Color.white;

        int displayStep = step + 1;
        int total = InstructionTexts.Length;
        progressStyle.normal.textColor = progressColor;
        GUI.Label(new Rect(x + barWidth + 8f, y - 4f, 40f, barHeight + 8f), $"{displayStep}/{total}", progressStyle);
    }

    private void DrawCompletionScreen()
    {
        float boxWidth = 440f;
        float boxHeight = 290f;
        float x = (Screen.width - boxWidth) * 0.5f;
        float y = (Screen.height - boxHeight) * 0.5f + 20f;

        GUI.color = new Color(0f, 0f, 0f, 0.72f);
        GUI.DrawTexture(new Rect(x, y, boxWidth, boxHeight), whiteTexture);
        GUI.color = Color.white;

        GUIStyle titleStyleLocal = new GUIStyle(GUI.skin.label)
        {
            fontSize = titleFontSize,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter
        };
        titleStyleLocal.normal.textColor = completedColor;
        GUI.Label(new Rect(x + 20f, y + 30f, boxWidth - 40f, 50f), "Chúc mừng!", titleStyleLocal);

        GUIStyle bodyStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = instructionFontSize,
            fontStyle = FontStyle.Normal,
            alignment = TextAnchor.MiddleCenter
        };
        bodyStyle.normal.textColor = Color.white;
        GUI.Label(new Rect(x + 30f, y + 90f, boxWidth - 60f, 35f), "Bạn đã học xong các thao tác cơ bản!", bodyStyle);
        GUI.Label(new Rect(x + 30f, y + 125f, boxWidth - 60f, 30f), "Giờ hãy bắt đầu cuộc phiêu lưu!", bodyStyle);

        float buttonY = y + 170f;
        float buttonHeight = 44f;
        float buttonGap = 12f;

        if (GUI.Button(new Rect(x + 60f, buttonY, boxWidth - 120f, buttonHeight), "Chơi Stage 1", buttonStyle))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.CompleteTutorialAndPlayStage1();
            }
        }

        if (GUI.Button(new Rect(x + 60f, buttonY + buttonHeight + buttonGap, boxWidth - 120f, buttonHeight), "Về Main Menu", buttonStyle))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.CompleteTutorial();
            }
        }
    }

    private static string GetHint(int step)
    {
        return step switch
        {
            0 => "Di chuyển một đoạn đến khi bước tiếp theo xuất hiện",
            1 => "Nhấn phím Space để lướt nhanh",
            2 => "Di chuyển chuột xung quanh",
            3 => "Nhấn Q để dùng kỹ năng",
            4 => "Nhấn E để dùng Công Pháp",
            5 => "Nhấn R để dùng Tuyệt Kỹ",
            _ => ""
        };
    }

    private void ResolveReferences()
    {
        if (tutorialManager == null)
        {
            tutorialManager = FindComponentInScene<TutorialManager>();
        }
    }

    private void EnsureStyles()
    {
        if (instructionStyle != null && instructionStyle.fontSize == instructionFontSize) return;

        instructionStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = instructionFontSize,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleLeft
        };
        instructionStyle.normal.textColor = instructionTextColor;

        subtitleStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = subtitleFontSize,
            fontStyle = FontStyle.Italic,
            alignment = TextAnchor.MiddleLeft
        };
        subtitleStyle.normal.textColor = new Color(0.8f, 0.8f, 0.8f, 0.7f);

        progressStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = progressFontSize,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleLeft
        };
        progressStyle.normal.textColor = progressColor;

        buttonStyle = new GUIStyle(GUI.skin.button)
        {
            fontSize = instructionFontSize,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter
        };
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
        return Object.FindObjectOfType<T>();
#endif
    }
}
