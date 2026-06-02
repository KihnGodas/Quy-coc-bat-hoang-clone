using UnityEngine;
using UnityEngine.UI;

public sealed class GameUI : MonoBehaviour
{
    private Text timerText;
    private GameObject victoryPanel;
    private GameObject defeatPanel;

    private void Awake()
    {
        EnsureCanvas();
    }

    private void EnsureCanvas()
    {
        var canvasGO = new GameObject("GameUICanvas",
            typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        var canvas = canvasGO.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10;

        CreateTimerText(canvas.transform);
        CreateResultPanels(canvas.transform);
    }

    private static Font GetBuiltinFont()
    {
        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (font == null)
            font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        return font;
    }

    private void CreateTimerText(Transform parent)
    {
        var timerGO = new GameObject("TimerText", typeof(Text));
        timerGO.transform.SetParent(parent, false);
        timerText = timerGO.GetComponent<Text>();
        timerText.font = GetBuiltinFont();
        timerText.fontSize = 48;
        timerText.alignment = TextAnchor.UpperCenter;
        timerText.color = Color.white;
        timerText.text = "00:00";

        var outline = timerGO.AddComponent<Outline>();
        outline.effectColor = new Color(0f, 0f, 0f, 0.8f);
        outline.effectDistance = new Vector2(2f, 2f);

        var rt = timerText.rectTransform;
        rt.anchorMin = new Vector2(0.5f, 1f);
        rt.anchorMax = new Vector2(0.5f, 1f);
        rt.pivot = new Vector2(0.5f, 1f);
        rt.anchoredPosition = new Vector2(0f, -20f);
        rt.sizeDelta = new Vector2(200f, 60f);
    }

    private void CreateResultPanels(Transform parent)
    {
        victoryPanel = CreatePanel(parent, "VictoryPanel", "VICTORY",
            new Color(0f, 0.5f, 0f, 0.8f));
        defeatPanel = CreatePanel(parent, "DefeatPanel", "THUA CUỘC",
            new Color(0.5f, 0f, 0f, 0.8f));

        victoryPanel.SetActive(false);
        defeatPanel.SetActive(false);
    }

    private GameObject CreatePanel(Transform parent, string name, string resultText, Color bgColor)
    {
        var panelGO = new GameObject(name, typeof(Image));
        panelGO.transform.SetParent(parent, false);
        var image = panelGO.GetComponent<Image>();
        image.color = bgColor;
        image.raycastTarget = true;

        var rt = panelGO.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;

        var textGO = new GameObject("ResultText", typeof(Text));
        textGO.transform.SetParent(panelGO.transform, false);
        var resultLabel = textGO.GetComponent<Text>();
        resultLabel.font = GetBuiltinFont();
        resultLabel.text = resultText;
        resultLabel.fontSize = 72;
        resultLabel.alignment = TextAnchor.MiddleCenter;
        resultLabel.color = Color.white;

        var textRT = textGO.GetComponent<RectTransform>();
        textRT.anchorMin = new Vector2(0.5f, 0.6f);
        textRT.anchorMax = new Vector2(0.5f, 0.6f);
        textRT.pivot = new Vector2(0.5f, 0.5f);
        textRT.anchoredPosition = Vector2.zero;
        textRT.sizeDelta = new Vector2(500f, 100f);

        var btnGO = new GameObject("RestartButton", typeof(Image), typeof(Button));
        btnGO.transform.SetParent(panelGO.transform, false);
        var btnImage = btnGO.GetComponent<Image>();
        btnImage.color = new Color(0.2f, 0.2f, 0.2f, 0.9f);
        btnImage.raycastTarget = true;

        var btnRT = btnGO.GetComponent<RectTransform>();
        btnRT.anchorMin = new Vector2(0.5f, 0.3f);
        btnRT.anchorMax = new Vector2(0.5f, 0.3f);
        btnRT.pivot = new Vector2(0.5f, 0.5f);
        btnRT.anchoredPosition = Vector2.zero;
        btnRT.sizeDelta = new Vector2(240f, 70f);

        var btnTextGO = new GameObject("ButtonText", typeof(Text));
        btnTextGO.transform.SetParent(btnGO.transform, false);
        var btnLabel = btnTextGO.GetComponent<Text>();
        btnLabel.font = GetBuiltinFont();
        btnLabel.text = "CHƠI LẠI";
        btnLabel.fontSize = 40;
        btnLabel.alignment = TextAnchor.MiddleCenter;
        btnLabel.color = Color.white;

        var btnTextRT = btnTextGO.GetComponent<RectTransform>();
        btnTextRT.anchorMin = Vector2.zero;
        btnTextRT.anchorMax = Vector2.one;
        btnTextRT.sizeDelta = Vector2.zero;

        var button = btnGO.GetComponent<Button>();
        button.targetGraphic = btnImage;
        button.onClick.AddListener(() =>
        {
            var gm = FindFirstObjectByType<GameManager>();
            if (gm != null)
                gm.RestartGame();
        });

        return panelGO;
    }

    public void UpdateTimer(float time)
    {
        if (timerText == null) return;

        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void ShowResult(bool isVictory)
    {
        victoryPanel.SetActive(isVictory);
        defeatPanel.SetActive(!isVictory);
    }
}
