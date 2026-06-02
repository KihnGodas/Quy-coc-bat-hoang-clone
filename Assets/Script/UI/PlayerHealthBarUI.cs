using UnityEngine;
using UnityEngine.UI;

public sealed class PlayerHealthBarUI : MonoBehaviour
{
    [SerializeField] private SimpleHealth playerHealth;
    [SerializeField] private Image healthFill;
    [SerializeField] private Image healthBackground;

    [SerializeField] private Vector2 barSize = new Vector2(200f, 24f);
    [SerializeField] private Color fillColor = new Color(0.8f, 0.15f, 0.15f);
    [SerializeField] private Color bgColor = new Color(0.2f, 0.2f, 0.2f, 0.8f);
    [SerializeField] private Vector2 bottomLeftOffset = new Vector2(16f, 16f);

    private void Awake()
    {
        ResolveReferences();
        EnsureCanvas();
    }

    private void ResolveReferences()
    {
        if (playerHealth == null)
        {
            var playerMovement = FindFirstObjectByType<PlayerMovement2D>();
            if (playerMovement != null)
                playerHealth = playerMovement.GetComponent<SimpleHealth>();
        }
    }

    private void EnsureCanvas()
    {
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasGO = new GameObject("HealthBarCanvas",
                typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvas = canvasGO.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }

        if (healthBackground == null)
        {
            GameObject bgGO = new GameObject("HealthBarBG", typeof(Image));
            bgGO.transform.SetParent(canvas.transform, false);
            healthBackground = bgGO.GetComponent<Image>();
            healthBackground.color = bgColor;

            RectTransform bgRT = healthBackground.rectTransform;
            bgRT.anchorMin = Vector2.zero;
            bgRT.anchorMax = Vector2.zero;
            bgRT.pivot = Vector2.zero;
            bgRT.sizeDelta = barSize;
            bgRT.anchoredPosition = bottomLeftOffset;
        }

        if (healthFill == null)
        {
            GameObject fillGO = new GameObject("HealthBarFill", typeof(Image));
            fillGO.transform.SetParent(healthBackground.transform, false);
            healthFill = fillGO.GetComponent<Image>();
            healthFill.color = fillColor;
            
            // BỎ 3 DÒNG SAU: Do Image.Type.Filled không hoạt động nếu thiếu Sprite
            // healthFill.type = Image.Type.Filled;
            // healthFill.fillMethod = Image.FillMethod.Horizontal;
            // healthFill.fillAmount = 1f;

            RectTransform fillRT = healthFill.rectTransform;
            fillRT.anchorMin = Vector2.zero;
            fillRT.anchorMax = Vector2.one; // Ban đầu thanh máu đầy 100%
            fillRT.offsetMin = Vector2.zero;
            fillRT.offsetMax = Vector2.zero;
        }
    }

    private void Update()
    {
        if (healthFill != null)
        {
            // Kiểm tra: nếu playerHealth null (do Player đã chết và bị Destroy)
            // thì thiết lập phần trăm máu hiển thị về 0
            float currentPercent = (playerHealth != null) ? playerHealth.HealthPercent : 0f;

            // Chỉnh lại anchorMax.x để co giãn độ dài thanh màu đỏ hiển thị
            healthFill.rectTransform.anchorMax = new Vector2(currentPercent, 1f);
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void AutoCreate()
    {
        if (FindFirstObjectByType<PlayerHealthBarUI>() == null)
        {
            var go = new GameObject("PlayerHealthBarUI (auto)");
            go.AddComponent<PlayerHealthBarUI>();
        }
    }
}