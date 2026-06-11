using System;
using UnityEngine;

public sealed class ScreenFader : MonoBehaviour
{
    public static ScreenFader Instance { get; private set; }

    public bool IsFading { get; private set; }
    public bool IsBlackedOut => Mathf.Approximately(currentAlpha, 1f);

    [SerializeField] private Color fadeColor = Color.black;
    [SerializeField] [Range(0.1f, 5f)] private float defaultDuration = 0.8f;

    private float currentAlpha;
    private float fadeSpeed;
    private bool fadingIn;
    private Action onComplete;
    private Texture2D whiteTexture;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        currentAlpha = 1f;
    }

    private void Start()
    {
        InitializeTexture();
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;

        if (whiteTexture != null)
            Destroy(whiteTexture);
    }

    private void Update()
    {
        if (!IsFading) return;

        float delta = fadeSpeed * Time.unscaledDeltaTime;
        if (fadingIn)
        {
            currentAlpha = Mathf.MoveTowards(currentAlpha, 0f, delta);
            if (Mathf.Approximately(currentAlpha, 0f))
            {
                currentAlpha = 0f;
                IsFading = false;
                onComplete?.Invoke();
                onComplete = null;
            }
        }
        else
        {
            currentAlpha = Mathf.MoveTowards(currentAlpha, 1f, delta);
            if (Mathf.Approximately(currentAlpha, 1f))
            {
                currentAlpha = 1f;
                IsFading = false;
                onComplete?.Invoke();
                onComplete = null;
            }
        }
    }

    private void OnGUI()
    {
        if (currentAlpha <= 0f || whiteTexture == null) return;

        GUI.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, currentAlpha);
        GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), whiteTexture);
        GUI.color = Color.white;
    }

    public void FadeIn(float duration, Action onComplete = null)
    {
        if (IsFading) return;

        IsFading = true;
        fadingIn = true;
        fadeSpeed = duration > 0f ? 1f / duration : 1f / defaultDuration;
        currentAlpha = 1f;
        this.onComplete = onComplete;
    }

    public void FadeIn(Action onComplete = null)
    {
        FadeIn(defaultDuration, onComplete);
    }

    public void FadeOut(float duration, Action onComplete = null)
    {
        if (IsFading) return;

        IsFading = true;
        fadingIn = false;
        fadeSpeed = duration > 0f ? 1f / duration : 1f / defaultDuration;
        currentAlpha = 0f;
        this.onComplete = onComplete;
    }

    public void FadeOut(Action onComplete = null)
    {
        FadeOut(defaultDuration, onComplete);
    }

    public void SetBlack()
    {
        currentAlpha = 1f;
        IsFading = false;
    }

    public void SetTransparent()
    {
        currentAlpha = 0f;
        IsFading = false;
    }

    private void InitializeTexture()
    {
        if (whiteTexture == null)
        {
            whiteTexture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            whiteTexture.SetPixel(0, 0, Color.white);
            whiteTexture.Apply();
        }
    }
}
