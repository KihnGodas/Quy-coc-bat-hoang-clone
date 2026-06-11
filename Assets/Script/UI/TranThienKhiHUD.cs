using UnityEngine;

public sealed class TranThienKhiHUD : MonoBehaviour
{
    [SerializeField] private Texture2D[] tranThienKhiIcons = new Texture2D[6];
    [SerializeField] private Vector2 screenOffset = new Vector2(12f, 12f);
    [SerializeField] private float iconSize = 44f;
    [SerializeField] private float iconGap = 6f;
    [SerializeField] private string resourcePath = "UI/TranThienKhi";

    [Header("Colors")]
    [SerializeField] private Color collectedColor = Color.white;
    [SerializeField] private Color lockedColor = new Color(0.2f, 0.2f, 0.2f, 0.5f);
    [SerializeField] private Color collectedBorderColor = new Color(1f, 0.9f, 0.3f, 0.9f);
    [SerializeField] private Color lockedBorderColor = new Color(0.3f, 0.3f, 0.3f, 0.5f);
    [SerializeField] private Color labelColor = new Color(1f, 1f, 1f, 0.6f);

    private static readonly string[] TranThienKhiNames = { "", "Moc", "Thuy", "Hoa", "Kim", "Tho" };
    private static readonly string[] TranThienKhiFileNames = { "", "mocThienKhi", "thuyThienKhi", "hoaThienKhi", "kimThienKhi", "thoThienKhi" };

    private Texture2D whiteTexture;
    private GUIStyle labelStyle;

    private void Awake()
    {
        InitializeWhiteTexture();
        EnsureFallbackIcons();
    }

    private void OnDestroy()
    {
        if (whiteTexture != null)
            Destroy(whiteTexture);
    }

    private void OnGUI()
    {
        if (DialogueManager.Instance != null && DialogueManager.Instance.IsPlaying)
            return;

        EnsureStyle();

        float x = Screen.width - screenOffset.x - iconSize;
        float y = screenOffset.y;

        for (int i = 1; i <= 5; i++)
        {
            TranThienKhiType type = (TranThienKhiType)i;
            bool has = GameManager.Instance != null && GameManager.Instance.HasTranThienKhi(type);
            DrawTranThienKhiIcon(x, y, type, has);
            y += iconSize + iconGap;
        }
    }

    private void DrawTranThienKhiIcon(float x, float y, TranThienKhiType type, bool has)
    {
        Rect iconRect = new Rect(x, y, iconSize, iconSize);
        Texture2D icon = GetIcon(type);

        if (icon != null)
        {
            GUI.color = has ? collectedColor : lockedColor;
            GUI.DrawTexture(iconRect, icon);
            GUI.color = Color.white;
        }
        else
        {
            GUI.color = has ? new Color(0.3f, 0.7f, 1f, 0.8f) : lockedColor;
            GUI.DrawTexture(iconRect, whiteTexture);
            GUI.color = Color.white;
        }

        DrawBorder(iconRect, has ? collectedBorderColor : lockedBorderColor);

        string name = GetShortName(type);
        labelStyle.normal.textColor = has ? labelColor : lockedColor;
        GUI.Label(new Rect(x, y + iconSize - 4f, iconSize, 14f), name, labelStyle);
    }

    private Texture2D GetIcon(TranThienKhiType type)
    {
        int index = (int)type;
        if (index >= 0 && index < tranThienKhiIcons.Length && tranThienKhiIcons[index] != null)
            return tranThienKhiIcons[index];
        return null;
    }

    private void EnsureFallbackIcons()
    {
        for (int i = 1; i <= 5; i++)
        {
            if (tranThienKhiIcons[i] == null)
            {
                tranThienKhiIcons[i] = Resources.Load<Texture2D>($"{resourcePath}/{TranThienKhiFileNames[i]}");
            }
        }
    }

    private static string GetShortName(TranThienKhiType type)
    {
        return type switch
        {
            TranThienKhiType.Moc => "Moc",
            TranThienKhiType.Thuy => "Thuy",
            TranThienKhiType.Hoa => "Hoa",
            TranThienKhiType.Kim => "Kim",
            TranThienKhiType.Tho => "Tho",
            _ => ""
        };
    }

    private void DrawBorder(Rect rect, Color color)
    {
        GUI.color = color;
        float t = 2f;
        GUI.DrawTexture(new Rect(rect.x, rect.y, rect.width, t), whiteTexture);
        GUI.DrawTexture(new Rect(rect.x, rect.y + rect.height - t, rect.width, t), whiteTexture);
        GUI.DrawTexture(new Rect(rect.x, rect.y, t, rect.height), whiteTexture);
        GUI.DrawTexture(new Rect(rect.x + rect.width - t, rect.y, t, rect.height), whiteTexture);
        GUI.color = Color.white;
    }

    private void InitializeWhiteTexture()
    {
        whiteTexture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        whiteTexture.SetPixel(0, 0, Color.white);
        whiteTexture.Apply();
    }

    private void EnsureStyle()
    {
        if (labelStyle != null) return;

        labelStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 10,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.LowerCenter
        };
        labelStyle.normal.textColor = labelColor;
    }
}
