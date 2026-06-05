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

    [Header("Settings")]
    [SerializeField] private float stepCompleteDelay = 1.2f;
    [SerializeField] private float movementRequiredDistance = 5f;
    [SerializeField] private float aimDeltaThreshold = 0.5f;

    private Step currentStep;
    private float stepCompleteTime;
    private bool stepCompleted;
    private bool allCompleted;

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
        OnStepChanged?.Invoke(currentStep);
    }

    private void Update()
    {
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

        if (playerMovement != null)
        {
            playerMovement.CanMove = true;
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

        if (nextIndex > (int)Step.Complete)
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

    private static T FindComponentInScene<T>() where T : Object
    {
#if UNITY_2023_1_OR_NEWER
        return FindFirstObjectByType<T>();
#else
        return Object.FindObjectOfType<T>();
#endif
    }
}
