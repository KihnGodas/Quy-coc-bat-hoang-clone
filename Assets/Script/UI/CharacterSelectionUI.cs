using UnityEngine;

public sealed class CharacterSelectionUI : MonoBehaviour
{
    public MainMenuUI mainMenuUI;
    public Texture2D maleTexture;
    public Texture2D femaleTexture;

    private Texture2D bgTexture;

    private string playerName = "";
    private CharacterType selectedCharacter;
    private bool hasSelectedCharacter;
    private bool isShown;

    private float scaleFactor;
    private float lastStyleScaleFactor;
    private const float REF_WIDTH = 1920f;
    private const float REF_HEIGHT = 1080f;

    [Header("Portrait Settings")]
    [SerializeField] private float portraitWidth = 400f;
    [SerializeField] private float portraitHeight = 500f;
    [SerializeField] private float portraitY = 200f;
    [SerializeField] private float portraitSideMargin = 80f;

    [Header("Name UI Settings")]
    [SerializeField] private float titleY = 30f;
    [SerializeField] private float nameLabelY = 95f;
    [SerializeField] private float nameFieldY = 130f;
    [SerializeField] private float nameFieldWidth = 300f;

    private GUIStyle titleStyle;
    private GUIStyle labelStyle;
    private GUIStyle nameFieldStyle;
    private GUIStyle buttonStyle;
    private GUIStyle instructionStyle;

    private void Awake()
    {
        bgTexture = Resources.Load<Texture2D>("CharacterSelect/bg");
        if (maleTexture == null)
            maleTexture = Resources.Load<Texture2D>("CharacterSelect/male");
        if (femaleTexture == null)
            femaleTexture = Resources.Load<Texture2D>("CharacterSelect/female");
    }

    private void UpdateScaleFactor()
    {
        float scaleX = Screen.width / REF_WIDTH;
        float scaleY = Screen.height / REF_HEIGHT;
        scaleFactor = Mathf.Min(scaleX, scaleY);
    }

    public void Show()
    {
        isShown = true;
        playerName = "";
        hasSelectedCharacter = false;
        enabled = true;
        gameObject.SetActive(true);
    }

    private void OnGUI()
    {
        if (!isShown) return;

        if (bgTexture != null)
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), bgTexture);

        UpdateScaleFactor();

        EnsureStyles();

        float centerX = Screen.width * 0.5f;

        GUI.Label(new Rect(centerX - 200f * scaleFactor, titleY * scaleFactor, 400f * scaleFactor, 55f * scaleFactor), "CHỌN NHÂN VẬT", titleStyle);

        GUI.Label(new Rect(centerX - 200f * scaleFactor, nameLabelY * scaleFactor, 400f * scaleFactor, 30f * scaleFactor), "Tên của bạn:", labelStyle);
        playerName = GUI.TextField(new Rect(centerX - nameFieldWidth * 0.5f * scaleFactor, nameFieldY * scaleFactor, nameFieldWidth * scaleFactor, 40f * scaleFactor), playerName, 20, nameFieldStyle);

        float pw = portraitWidth * scaleFactor;
        float ph = portraitHeight * scaleFactor;
        float py = portraitY * scaleFactor;

        Rect maleRect = new Rect(portraitSideMargin * scaleFactor, py, pw, ph);
        DrawCharacterPortrait(maleRect, maleTexture, CharacterType.Male, "NAM");

        Rect femaleRect = new Rect(Screen.width - pw - portraitSideMargin * scaleFactor, py, pw, ph);
        DrawCharacterPortrait(femaleRect, femaleTexture, CharacterType.Female, "NỮ");

        float btnY = py + ph + 50f * scaleFactor;

        if (hasSelectedCharacter && !string.IsNullOrEmpty(playerName.Trim()))
        {
            if (GUI.Button(new Rect(centerX - 110f * scaleFactor, btnY, 220f * scaleFactor, 55f * scaleFactor), "Xác nhận", buttonStyle))
            {
                ConfirmSelection();
            }
        }
        else
        {
            GUI.Label(new Rect(centerX - 200f * scaleFactor, btnY + 10f * scaleFactor, 400f * scaleFactor, 30f * scaleFactor), "Vui lòng chọn nhân vật và nhập tên", instructionStyle);
        }
    }

    private void DrawCharacterPortrait(Rect rect, Texture2D texture, CharacterType characterType, string label)
    {
        bool isSelected = hasSelectedCharacter && selectedCharacter == characterType;
        Color borderColor = isSelected ? Color.yellow : new Color(0.5f, 0.5f, 0.5f, 0.6f);
        float borderWidth = (isSelected ? 5f : 3f) * scaleFactor;

        GUI.color = borderColor;
        GUI.DrawTexture(new Rect(
            rect.x - borderWidth, rect.y - borderWidth,
            rect.width + borderWidth * 2f, rect.height + borderWidth * 2f), Texture2D.whiteTexture);
        GUI.color = Color.white;

        if (texture != null)
        {
            GUI.DrawTexture(rect, texture);
        }
        else
        {
            GUI.Box(rect, "No Image");
        }

        GUI.Label(new Rect(rect.x, rect.y + rect.height + 5f * scaleFactor, rect.width, 30f * scaleFactor), label, labelStyle);

        if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
        {
            selectedCharacter = characterType;
            hasSelectedCharacter = true;
        }
    }

    private void ConfirmSelection()
    {
        GameManager.Instance.SelectedCharacter = selectedCharacter;
        GameManager.Instance.PlayerName = playerName.Trim();
        isShown = false;
        gameObject.SetActive(false);
        mainMenuUI.ShowWeaponSelection();
    }

    private void EnsureStyles()
    {
        if (titleStyle != null && Mathf.Approximately(lastStyleScaleFactor, scaleFactor))
            return;

        lastStyleScaleFactor = scaleFactor;

        titleStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = Mathf.RoundToInt(42 * scaleFactor),
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter
        };
        titleStyle.normal.textColor = Color.white;

        labelStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = Mathf.RoundToInt(24 * scaleFactor),
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter
        };
        labelStyle.normal.textColor = Color.white;

        nameFieldStyle = new GUIStyle(GUI.skin.textField)
        {
            fontSize = Mathf.RoundToInt(24 * scaleFactor),
            alignment = TextAnchor.MiddleCenter
        };

        buttonStyle = new GUIStyle(GUI.skin.button)
        {
            fontSize = Mathf.RoundToInt(26 * scaleFactor),
            fontStyle = FontStyle.Bold
        };

        instructionStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = Mathf.RoundToInt(18 * scaleFactor),
            fontStyle = FontStyle.Italic,
            alignment = TextAnchor.MiddleCenter
        };
        instructionStyle.normal.textColor = new Color(1f, 1f, 0.6f);
    }
}
