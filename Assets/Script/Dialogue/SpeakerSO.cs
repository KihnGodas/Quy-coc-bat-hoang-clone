using UnityEngine;

[CreateAssetMenu(fileName = "Speaker", menuName = "Dialogue/Speaker")]
public sealed class SpeakerSO : ScriptableObject
{
    [SerializeField] private string speakerName = "Speaker";
    [SerializeField] private Texture2D portrait;
    [SerializeField] private Color highlightColor = Color.white;
    [SerializeField] private Color dimColor = new Color(0.5f, 0.5f, 0.5f, 0.35f);

    private Texture2D cachedPlaceholder;

    public string SpeakerName => speakerName;
    public Texture2D Portrait => portrait;
    public Color HighlightColor => highlightColor;
    public Color DimColor => dimColor;

    public Texture2D GetOrCreatePortrait()
    {
        if (portrait != null) return portrait;
        if (cachedPlaceholder != null) return cachedPlaceholder;
        cachedPlaceholder = GeneratePlaceholderPortrait();
        return cachedPlaceholder;
    }

    private Texture2D GeneratePlaceholderPortrait()
    {
        int size = 128;
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Color fill = highlightColor * 0.6f;
        fill.a = 1f;
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dx = x - size * 0.5f;
                float dy = y - size * 0.5f;
                float dist = Mathf.Sqrt(dx * dx + dy * dy);
                float radius = size * 0.45f;
                tex.SetPixel(x, y, dist <= radius ? fill : Color.clear);
            }
        }
        tex.Apply();
        return tex;
    }
}
