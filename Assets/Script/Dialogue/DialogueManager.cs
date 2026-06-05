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

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
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
        OnDialogueStart?.Invoke();
        OnDialogueLineChanged?.Invoke();
    }

    public void AdvanceLine()
    {
        if (!IsPlaying || CurrentDialogue == null) return;

        int nextIndex = CurrentLineIndex + 1;
        if (nextIndex >= CurrentDialogue.LineCount)
        {
            StopDialogue();
            return;
        }

        CurrentLineIndex = nextIndex;
        OnDialogueLineChanged?.Invoke();
    }

    public void StopDialogue()
    {
        IsPlaying = false;
        CurrentDialogue = null;
        CurrentLineIndex = 0;
        OnDialogueEnd?.Invoke();
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
