using UnityEngine;

public sealed class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    public bool IsPlaying { get; private set; }
    public DialogueSO CurrentDialogue { get; private set; }
    public int CurrentLineIndex { get; private set; }
    public DialogueLine CurrentLine => CurrentDialogue != null && CurrentLineIndex < CurrentDialogue.LineCount
        ? CurrentDialogue.Lines[CurrentLineIndex]
        : default;

    public System.Action OnDialogueStart;
    public System.Action OnDialogueLineChanged;
    public System.Action OnDialogueEnd;
    public System.Action<string> OnDialogueEvent;

    private float autoAdvanceTimer;
    private bool waitingAutoAdvance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Update()
    {
        if (!IsPlaying || !waitingAutoAdvance) return;
        autoAdvanceTimer -= Time.deltaTime;
        if (autoAdvanceTimer <= 0f)
            AdvanceLine();
    }

    public void PlayDialogue(DialogueSO dialogue)
    {
        if (dialogue == null || dialogue.LineCount == 0)
        {
            Debug.LogWarning("DialogueManager: dialogue is null or empty.");
            return;
        }

        CurrentDialogue = dialogue;
        CurrentLineIndex = 0;
        IsPlaying = true;
        waitingAutoAdvance = false;
        OnDialogueStart?.Invoke();
        FireLineEvents(CurrentLine.onStartEvent);
        OnDialogueLineChanged?.Invoke();
    }

    public void AdvanceLine()
    {
        if (!IsPlaying || CurrentDialogue == null) return;

        FireLineEvents(CurrentLine.onEndEvent);

        int nextIndex = CurrentLineIndex + 1;
        if (nextIndex >= CurrentDialogue.LineCount)
        {
            StopDialogue();
            return;
        }

        CurrentLineIndex = nextIndex;
        waitingAutoAdvance = false;
        FireLineEvents(CurrentLine.onStartEvent);
        OnDialogueLineChanged?.Invoke();
    }

    public void TryAdvanceLine()
    {
        if (!IsPlaying) return;
        AdvanceLine();
    }

    public void StopDialogue()
    {
        if (IsPlaying)
            FireLineEvents(CurrentLine.onEndEvent);

        IsPlaying = false;
        CurrentDialogue = null;
        CurrentLineIndex = 0;
        waitingAutoAdvance = false;
        OnDialogueEnd?.Invoke();
    }

    public void SetAutoAdvance(float delay)
    {
        waitingAutoAdvance = true;
        autoAdvanceTimer = delay;
    }

    public void CancelAutoAdvance()
    {
        waitingAutoAdvance = false;
    }

    private void FireLineEvents(string eventName)
    {
        if (!string.IsNullOrEmpty(eventName))
            OnDialogueEvent?.Invoke(eventName);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
