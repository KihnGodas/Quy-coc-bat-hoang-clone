using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue", menuName = "Dialogue/Dialogue")]
public sealed class DialogueSO : ScriptableObject
{
    [SerializeField] private DialogueLine[] lines;

    public DialogueLine[] Lines => lines;
    public int LineCount => lines != null ? lines.Length : 0;
}
