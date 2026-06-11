using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class PrologueController : MonoBehaviour
{
    [Header("Background")]
    [SerializeField] private string bgResourcePath = "Tutorial/PrologueBg";
    [SerializeField] private int bgSortingOrder = -5;
    [SerializeField] private Color fallbackBgColor = new Color(0.08f, 0.1f, 0.12f);

    [Header("Fade")]
    [SerializeField] private float fadeInDuration = 0.8f;
    [SerializeField] private float fadeOutDuration = 0.8f;

    [Header("References")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform arenaWallsParent;

    private GameObject backgroundGO;
    private SpriteRenderer bgRenderer;
    private Camera mainCamera;

    private readonly List<Component> hiddenComponents = new List<Component>();
    private bool isShowing;

    public bool IsShowing => isShowing;

    private void Awake()
    {
        ResolveReferences();
    }

    private void ResolveReferences()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (playerTransform == null)
        {
            GameObject playerGO = GameObject.FindGameObjectWithTag("Player");
            if (playerGO != null)
                playerTransform = playerGO.transform;
        }

        if (arenaWallsParent == null)
        {
            ArenaBounds bounds = FindComponentInScene<ArenaBounds>();
            if (bounds != null)
                arenaWallsParent = bounds.transform;
        }
    }

    public void ShowPrologue(Action onReady)
    {
        if (isShowing) return;
        isShowing = true;

        ResolveReferences();
        EnsureScreenFader();

        HideGameplayElements();
        CreateBackground();

        ScreenFader.Instance.FadeIn(fadeInDuration, () =>
        {
            onReady?.Invoke();
        });
    }

    public void HidePrologue(Action onComplete)
    {
        if (!isShowing) return;
        isShowing = false;

        ScreenFader fader = ScreenFader.Instance;
        if (fader == null)
        {
            DestroyBackground();
            ShowGameplayElements();
            onComplete?.Invoke();
            return;
        }

        fader.FadeOut(fadeOutDuration, () =>
        {
            DestroyBackground();
            ShowGameplayElements();

            fader.FadeIn(fadeInDuration, () =>
            {
                onComplete?.Invoke();
            });
        });
    }

    private void CreateBackground()
    {
        if (backgroundGO != null)
        {
            DestroyBackground();
        }

        Sprite bgSprite = Resources.Load<Sprite>(bgResourcePath);

        backgroundGO = new GameObject("PrologueBackground", typeof(SpriteRenderer));
        bgRenderer = backgroundGO.GetComponent<SpriteRenderer>();

        if (bgSprite != null)
        {
            bgRenderer.sprite = bgSprite;
            bgRenderer.sortingOrder = bgSortingOrder;

            ScaleToFitCamera();
        }
        else
        {
            Debug.LogWarning($"Prologue background sprite not found at: {bgResourcePath}. Using solid color fallback.");
            Texture2D fallbackTex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            fallbackTex.SetPixel(0, 0, fallbackBgColor);
            fallbackTex.Apply();
            bgRenderer.sprite = Sprite.Create(fallbackTex, new Rect(0, 0, 1, 1), Vector2.one * 0.5f, 1f);
            bgRenderer.sortingOrder = bgSortingOrder;
            ScaleToFitCamera();
        }

        PositionAtCamera();
    }

    private void PositionAtCamera()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
        if (mainCamera == null) return;

        Vector3 camPos = mainCamera.transform.position;
        backgroundGO.transform.position = new Vector3(camPos.x, camPos.y, 1f);
    }

    private void ScaleToFitCamera()
    {
        if (mainCamera == null || bgRenderer == null || bgRenderer.sprite == null) return;

        float worldHeight = mainCamera.orthographicSize * 2f;
        float worldWidth = worldHeight * mainCamera.aspect;

        float spriteWidth = bgRenderer.sprite.bounds.size.x;
        float spriteHeight = bgRenderer.sprite.bounds.size.y;

        if (spriteWidth <= 0f || spriteHeight <= 0f) return;

        float scaleX = worldWidth / spriteWidth;
        float scaleY = worldHeight / spriteHeight;
        float scale = Mathf.Max(scaleX, scaleY);

        backgroundGO.transform.localScale = new Vector3(scale, scale, 1f);
    }

    private void DestroyBackground()
    {
        if (backgroundGO != null)
        {
            Destroy(backgroundGO);
            backgroundGO = null;
            bgRenderer = null;
        }
    }

    private void HideGameplayElements()
    {
        hiddenComponents.Clear();

        if (playerTransform != null)
        {
            HidePlayerRendererAndChildren();
            DisablePlayerActions();
        }

        GameObject dummyGO = GameObject.Find("Dummy");
        if (dummyGO != null)
        {
            SpriteRenderer dummySr = dummyGO.GetComponent<SpriteRenderer>();
            if (dummySr != null && dummySr.enabled)
            {
                dummySr.enabled = false;
                hiddenComponents.Add(dummySr);
            }
        }

        if (arenaWallsParent != null)
        {
            SpriteRenderer[] wallRenderers = arenaWallsParent.GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer wallSr in wallRenderers)
            {
                if (wallSr.enabled)
                {
                    wallSr.enabled = false;
                    hiddenComponents.Add(wallSr);
                }
            }
        }

        if (mainCamera != null)
        {
            CameraFollow2D cameraFollow = mainCamera.GetComponent<CameraFollow2D>();
            if (cameraFollow != null && cameraFollow.enabled)
            {
                cameraFollow.enabled = false;
                hiddenComponents.Add(cameraFollow);
            }
        }
    }

    private void HidePlayerRendererAndChildren()
    {
        SpriteRenderer[] allRenderers = playerTransform.GetComponentsInChildren<SpriteRenderer>(true);
        foreach (SpriteRenderer sr in allRenderers)
        {
            if (sr.enabled)
            {
                sr.enabled = false;
                hiddenComponents.Add(sr);
            }
        }
    }

    private void DisablePlayerActions()
    {
        Behaviour[] actionComponents =
        {
            playerTransform.GetComponent<PlayerMovement2D>(),
            playerTransform.GetComponent<PlayerDash2D>(),
            playerTransform.GetComponent<PlayerAim2D>(),
            playerTransform.GetComponent<PlayerCombat2D>(),
            playerTransform.GetComponent<WeaponController>(),
            playerTransform.GetComponent<PlayerSpellController>(),
            playerTransform.GetComponent<PlayerUltimateController>(),
        };

        foreach (Behaviour comp in actionComponents)
        {
            if (comp != null && comp.enabled)
            {
                comp.enabled = false;
                hiddenComponents.Add(comp);
            }
        }
    }

    private void ShowGameplayElements()
    {
        foreach (Component comp in hiddenComponents)
        {
            if (comp == null) continue;

            if (comp is Behaviour behaviour)
                behaviour.enabled = true;
            else if (comp is Renderer renderer)
                renderer.enabled = true;
        }
        hiddenComponents.Clear();

        if (playerTransform != null)
        {
            PlayerMovement2D movement = playerTransform.GetComponent<PlayerMovement2D>();
            if (movement != null)
                movement.CanMove = true;
        }
    }

    private static void EnsureScreenFader()
    {
        if (ScreenFader.Instance != null) return;

        GameObject faderGO = new GameObject("ScreenFader");
        faderGO.AddComponent<ScreenFader>();
    }

    private void Update()
    {
        if (!isShowing) return;

        if (mainCamera != null && backgroundGO != null)
        {
            PositionAtCamera();
        }
    }

    private static T FindComponentInScene<T>() where T : UnityEngine.Object
    {
#if UNITY_2023_1_OR_NEWER
        return FindFirstObjectByType<T>();
#else
        return FindObjectOfType<T>();
#endif
    }
}
