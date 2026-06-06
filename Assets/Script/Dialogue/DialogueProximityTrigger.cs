using UnityEngine;

public sealed class DialogueProximityTrigger : MonoBehaviour
{
    [SerializeField] private DialogueSO dialogue;
    [SerializeField] private bool oneTime = true;
    [SerializeField] private bool lockPlayerMovement = true;

    private bool alreadyTriggered;
    private PlayerMovement2D cachedPlayerMovement;

    private void Awake()
    {
        CircleCollider2D col = GetComponent<CircleCollider2D>();
        if (col == null)
        {
            col = gameObject.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius = 3f;
        }
        else
        {
            col.isTrigger = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (oneTime && alreadyTriggered) return;
        if (dialogue == null) return;

        alreadyTriggered = true;

        if (lockPlayerMovement)
        {
            if (cachedPlayerMovement == null)
                cachedPlayerMovement = other.GetComponent<PlayerMovement2D>();
            if (cachedPlayerMovement != null)
                cachedPlayerMovement.CanMove = false;
        }

        DialogueManager dm = DialogueManager.Instance;
        if (dm == null) return;

        if (lockPlayerMovement)
        {
            System.Action unlockHandler = null;
            unlockHandler = () =>
            {
                dm.OnDialogueEnd -= unlockHandler;
                if (cachedPlayerMovement != null)
                    cachedPlayerMovement.CanMove = true;
            };
            dm.OnDialogueEnd += unlockHandler;
        }

        dm.PlayDialogue(dialogue);
    }

    public void SetDialogue(DialogueSO newDialogue)
    {
        dialogue = newDialogue;
    }

    public void ResetTrigger()
    {
        alreadyTriggered = false;
    }
}
