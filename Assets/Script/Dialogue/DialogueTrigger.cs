using UnityEngine;

public sealed class DialogueTrigger : MonoBehaviour
{
    public enum TriggerType
    {
        OnStart,
        OnAwake,
        Manual
    }

    [SerializeField] private DialogueSO dialogue;
    [SerializeField] private TriggerType triggerType = TriggerType.OnStart;

    private void Awake()
    {
        if (triggerType == TriggerType.OnAwake)
            TriggerDialogue();
    }

    private void Start()
    {
        if (triggerType == TriggerType.OnStart)
            TriggerDialogue();
    }

    public void TriggerDialogue()
    {
        if (dialogue == null)
        {
            Debug.LogWarning("DialogueTrigger: no dialogue assigned.", this);
            return;
        }

        if (DialogueManager.Instance != null)
            DialogueManager.Instance.PlayDialogue(dialogue);
        else
            Debug.LogError("DialogueTrigger: DialogueManager not found in scene.", this);
    }
}
