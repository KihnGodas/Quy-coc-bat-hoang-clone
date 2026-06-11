using UnityEngine;
using UnityEngine.InputSystem;

public sealed class TutorialManager : MonoBehaviour
{
    public enum Step
    {
        Movement,
        Dash,
        Aim,
        WeaponSkill,
        Spell,
        Ultimate,
        BossFight,
        Complete
    }

    [Header("References")]
    [SerializeField] private PlayerMovement2D playerMovement;
    [SerializeField] private PlayerDash2D playerDash;
    [SerializeField] private PlayerAim2D playerAim;
    [SerializeField] private WeaponController weaponController;
    [SerializeField] private PlayerSpellController spellController;
    [SerializeField] private PlayerUltimateController ultimateController;
    [SerializeField] private PlayerCultivationState cultivationState;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private EnemySpawner enemySpawner;

    [Header("Prologue")]
    [SerializeField] private PrologueController prologueController;

    [Header("Background")]
    [SerializeField] private Sprite tutorialBackgroundSprite;
    [SerializeField] private string tutorialBgResourcePath = "Tutorial/bg";
    [SerializeField] private int backgroundSortingOrder = 0;

    [Header("Dialogue")]
    [SerializeField] private DialogueSO prologueNightDialogue;
    [SerializeField] private DialogueSO prologueCeremonyDialogue;
    [SerializeField] private DialogueSO prologueAwakeningDialogue;
    [SerializeField] private DialogueSO trainingDialogue;
    [SerializeField] private DialogueSO bossPreDialogue;
    [SerializeField] private DialogueSO bossPostDialogue;

    [Header("Settings")]
    [SerializeField] private float stepCompleteDelay = 1.2f;
    [SerializeField] private float movementRequiredDistance = 5f;
    [SerializeField] private float aimDeltaThreshold = 0.5f;

    private Step currentStep;
    private float stepCompleteTime;
    private bool stepCompleted;
    private bool allCompleted;
    private bool isPlayingChainDialogue;
    private int openingChainIndex;
    private GameObject tutorialBackgroundGO;
    private Transform tutorialBgTransform;

    private Vector3 movementStartPosition;
    private Vector2 previousMousePosition;
    private float nextResolveTime;
    private const float RESOLVE_INTERVAL = 0.5f;

    public Step CurrentStep => currentStep;
    public bool AllCompleted => allCompleted;
    public float CompletionProgress => (int)currentStep / (float)(int)Step.Complete;

    public event System.Action<Step> OnStepChanged;
    public event System.Action OnTutorialComplete;

    private void Awake()
    {
        ResolveReferences();

        currentStep = Step.Movement;
        movementStartPosition = playerTransform != null ? playerTransform.position : Vector3.zero;
        previousMousePosition = Mouse.current != null ? Mouse.current.position.ReadValue() : Vector2.zero;
    }

    private void Start()
    {
        ResolveReferences();

        EnsurePlayerReadyForTutorial();

        if (prologueController != null)
        {
            prologueController.ShowPrologue(() =>
            {
                PlayOpeningDialogueChain();
            });
        }
        else
        {
            PlayOpeningDialogueChain();
        }
    }

    private void Update()
    {
        if (isPlayingChainDialogue) return;

        if (!ReferencesResolved() && Time.time >= nextResolveTime)
        {
            nextResolveTime = Time.time + RESOLVE_INTERVAL;
            ResolveReferences();
        }

        if (allCompleted) return;

        if (stepCompleted)
        {
            if (Time.time >= stepCompleteTime + stepCompleteDelay)
            {
                AdvanceToNextStep();
            }
            return;
        }

        TryCompleteStep();
    }

    private void LateUpdate()
    {
        if (tutorialBgTransform == null) return;

        Camera cam = Camera.main;
        if (cam != null)
        {
            Vector3 camPos = cam.transform.position;
            tutorialBgTransform.position = new Vector3(camPos.x, camPos.y, 1f);
        }
    }

    private bool ReferencesResolved()
    {
        return playerMovement != null && playerTransform != null;
    }

    private void ResolveReferences()
    {
        if (playerMovement == null)
            playerMovement = FindComponentInScene<PlayerMovement2D>();
        if (playerDash == null)
            playerDash = FindComponentInScene<PlayerDash2D>();
        if (playerAim == null)
            playerAim = FindComponentInScene<PlayerAim2D>();
        if (weaponController == null)
            weaponController = FindComponentInScene<WeaponController>();
        if (spellController == null)
            spellController = FindComponentInScene<PlayerSpellController>();
        if (ultimateController == null)
            ultimateController = FindComponentInScene<PlayerUltimateController>();
        if (cultivationState == null)
            cultivationState = FindComponentInScene<PlayerCultivationState>();
        if (playerTransform == null && playerMovement != null)
            playerTransform = playerMovement.transform;
        if (enemySpawner == null)
            enemySpawner = FindComponentInScene<EnemySpawner>();
    }

    private void EnsurePlayerReadyForTutorial()
    {
        if (cultivationState == null)
        {
            cultivationState = FindComponentInScene<PlayerCultivationState>();
        }

        if (cultivationState != null)
        {
            cultivationState.SetRealms(CultivationRealm.KimDan, CultivationRealm.KimDan);
        }
    }

    private void TryCompleteStep()
    {
        bool completed = currentStep switch
        {
            Step.Movement => CheckMovementCompleted(),
            Step.Dash => CheckDashCompleted(),
            Step.Aim => CheckAimCompleted(),
            Step.WeaponSkill => CheckWeaponSkillCompleted(),
            Step.Spell => CheckSpellCompleted(),
            Step.Ultimate => CheckUltimateCompleted(),
            Step.BossFight => CheckBossFightCompleted(),
            _ => false
        };

        if (completed)
        {
            stepCompleted = true;
            stepCompleteTime = Time.time;
        }
    }

    private void AdvanceToNextStep()
    {
        int nextIndex = (int)currentStep + 1;

        if (nextIndex >= (int)Step.Complete)
        {
            allCompleted = true;
            OnStepChanged?.Invoke(Step.Complete);
            OnTutorialComplete?.Invoke();
            return;
        }

        currentStep = (Step)nextIndex;
        stepCompleted = false;
        stepCompleteTime = 0f;

        if (currentStep == Step.Movement)
        {
            movementStartPosition = playerTransform != null ? playerTransform.position : Vector3.zero;
        }

        if (currentStep == Step.BossFight)
        {
            StartBossFight();
        }

        if (currentStep == Step.Dash && trainingDialogue != null)
        {
            PlayTrainingHint();
        }

        OnStepChanged?.Invoke(currentStep);
    }

    private bool CheckMovementCompleted()
    {
        if (playerTransform == null || playerMovement == null) return false;
        return Vector3.Distance(playerTransform.position, movementStartPosition) >= movementRequiredDistance;
    }

    private bool CheckDashCompleted()
    {
        Keyboard keyboard = Keyboard.current;
        return keyboard != null && keyboard.spaceKey.wasPressedThisFrame;
    }

    private bool CheckAimCompleted()
    {
        if (Mouse.current == null) return false;
        Vector2 currentPos = Mouse.current.position.ReadValue();
        float deltaSq = (currentPos - previousMousePosition).sqrMagnitude;
        previousMousePosition = currentPos;
        return deltaSq >= aimDeltaThreshold * aimDeltaThreshold;
    }

    private bool CheckWeaponSkillCompleted()
    {
        Keyboard keyboard = Keyboard.current;
        return keyboard != null && keyboard.qKey.wasPressedThisFrame;
    }

    private bool CheckSpellCompleted()
    {
        Keyboard keyboard = Keyboard.current;
        return keyboard != null && keyboard.eKey.wasPressedThisFrame;
    }

    private bool CheckUltimateCompleted()
    {
        Keyboard keyboard = Keyboard.current;
        return keyboard != null && keyboard.rKey.wasPressedThisFrame;
    }

    private bool CheckBossFightCompleted()
    {
        BossBase boss = FindComponentInScene<BossBase>();
        return boss == null || (boss.Health != null && boss.Health.IsDead);
    }

    private void StartBossFight()
    {
        if (bossPreDialogue != null)
        {
            isPlayingChainDialogue = true;
            DialogueManager dm = EnsureDialogueManager();
            if (dm == null) { SpawnBossOrComplete(); return; }

            if (playerMovement != null)
                playerMovement.CanMove = false;

            System.Action onEnd = null;
            onEnd = () =>
            {
                dm.OnDialogueEnd -= onEnd;
                if (playerMovement != null)
                    playerMovement.CanMove = true;
                isPlayingChainDialogue = false;
                SpawnBossOrComplete();
            };
            dm.OnDialogueEnd += onEnd;
            dm.PlayDialogue(bossPreDialogue);
        }
        else
        {
            SpawnBossOrComplete();
        }
    }

    private void SpawnBossOrComplete()
    {
        if (enemySpawner != null)
        {
            enemySpawner.SetSpawningEnabled(true);
        }
    }

    // ---- BACKGROUND ----

    private void CreateTutorialBackground()
    {
        DestroyTutorialBackground();

        tutorialBackgroundGO = new GameObject("TutorialBackground", typeof(SpriteRenderer));
        tutorialBgTransform = tutorialBackgroundGO.transform;
        SpriteRenderer sr = tutorialBackgroundGO.GetComponent<SpriteRenderer>();
        sr.sortingOrder = backgroundSortingOrder;
        if (sr.sortingOrder < -5) sr.sortingOrder = -5;

        Sprite usedSprite = tutorialBackgroundSprite;
        if (usedSprite == null && !string.IsNullOrEmpty(tutorialBgResourcePath))
        {
            usedSprite = Resources.Load<Sprite>(tutorialBgResourcePath);
            if (usedSprite != null)
                Debug.Log($"TutorialManager: loaded background from Resources path '{tutorialBgResourcePath}'");
        }
        if (usedSprite == null)
        {
            Debug.LogWarning("TutorialManager: tutorialBackgroundSprite is null and Resources.Load failed, using magenta fallback.");
            Texture2D fb = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            fb.SetPixel(0, 0, Color.magenta);
            fb.Apply();
            usedSprite = Sprite.Create(fb, new Rect(0, 0, 1, 1), Vector2.one * 0.5f, 1f);
        }
        sr.sprite = usedSprite;
        Debug.Log($"TutorialManager: background sprite = {(tutorialBackgroundSprite != null ? tutorialBackgroundSprite.name : (usedSprite != null ? usedSprite.name : "FALLBACK"))}, sortingOrder = {sr.sortingOrder}");

        Camera cam = Camera.main;
        if (cam != null)
        {
            Vector3 camPos = cam.transform.position;
            tutorialBackgroundGO.transform.position = new Vector3(camPos.x, camPos.y, 1f);

            float worldHeight = cam.orthographicSize * 2f;
            float worldWidth = worldHeight * cam.aspect;

            float spriteWidth = sr.sprite.bounds.size.x;
            float spriteHeight = sr.sprite.bounds.size.y;

            if (spriteWidth > 0f && spriteHeight > 0f)
            {
                float scaleX = worldWidth / spriteWidth;
                float scaleY = worldHeight / spriteHeight;
                float scale = Mathf.Max(scaleX, scaleY);
                tutorialBackgroundGO.transform.localScale = new Vector3(scale, scale, 1f);
            }
        }
    }

    private void DestroyTutorialBackground()
    {
        if (tutorialBackgroundGO != null)
        {
            Destroy(tutorialBackgroundGO);
            tutorialBackgroundGO = null;
            tutorialBgTransform = null;
        }
    }

    // ---- DIALOGUE CHAIN ----

    private void PlayOpeningDialogueChain()
    {
        DialogueManager dm = EnsureDialogueManager();
        if (dm == null)
        {
            OnStepChanged?.Invoke(currentStep);
            return;
        }

        isPlayingChainDialogue = true;
        openingChainIndex = 0;

        if (playerMovement != null)
            playerMovement.CanMove = false;

        dm.OnDialogueEnd += HandleChainDialogueEnd;
        PlayCurrentChainDialogue(dm);
    }

    private void HandleChainDialogueEnd()
    {
        DialogueManager dm = DialogueManager.Instance;
        if (dm == null) return;

        openingChainIndex++;

        if (openingChainIndex >= 3 || !HasChainDialogueAtIndex(openingChainIndex))
        {
            dm.OnDialogueEnd -= HandleChainDialogueEnd;
            isPlayingChainDialogue = false;

            if (prologueController != null && prologueController.IsShowing)
            {
                prologueController.HidePrologue(OnPrologueHidden);
            }
            else
            {
                OnPrologueHidden();
            }
            return;
        }

        PlayCurrentChainDialogue(dm);
    }

    private void OnPrologueHidden()
    {
        Debug.Log("TutorialManager.OnPrologueHidden CALLED");
        CreateTutorialBackground();
        ResolveReferences();

        if (playerMovement != null)
        {
            playerMovement.enabled = true;
            playerMovement.CanMove = true;
        }
        else
        {
            PlayerMovement2D pm = FindComponentInScene<PlayerMovement2D>();
            if (pm != null)
            {
                pm.enabled = true;
                pm.CanMove = true;
            }
        }

        currentStep = Step.Movement;
        stepCompleted = false;
        stepCompleteTime = 0f;
        movementStartPosition = playerTransform != null ? playerTransform.position : Vector3.zero;

        OnStepChanged?.Invoke(currentStep);
    }

    private bool HasChainDialogueAtIndex(int index)
    {
        return index switch
        {
            0 => prologueNightDialogue != null,
            1 => prologueCeremonyDialogue != null,
            2 => prologueAwakeningDialogue != null,
            _ => false
        };
    }

    private void PlayCurrentChainDialogue(DialogueManager dm)
    {
        DialogueSO d = openingChainIndex switch
        {
            0 => prologueNightDialogue,
            1 => prologueCeremonyDialogue,
            2 => prologueAwakeningDialogue,
            _ => null
        };
        if (d != null)
            dm.PlayDialogue(d);
        else
            HandleChainDialogueEnd();
    }

    private void PlayTrainingHint()
    {
        if (trainingDialogue == null) return;
        DialogueManager dm = DialogueManager.Instance;
        if (dm == null) return;
        dm.PlayDialogue(trainingDialogue);
    }

    private DialogueManager EnsureDialogueManager()
    {
        DialogueManager dm = DialogueManager.Instance;
        if (dm == null)
        {
            GameObject dialogueGO = new GameObject("DialogueSystem");
            dialogueGO.AddComponent<DialogueManager>();
            dialogueGO.AddComponent<DialogueUI>();
            dm = DialogueManager.Instance;
        }
        return dm;
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
