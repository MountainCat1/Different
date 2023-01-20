using UnityEngine;

[CreateAssetMenu(fileName = "NarrationDialogue", menuName = "GameDialogues/NarrationDialogue", order = 1)]
public class NarrationDialogue : Dialogue
{
    [SerializeField] private string text;
    [Space]
    [SerializeField] private Dialogue next;

    public Dialogue Next { get => next; set => next = value; }
    public string Text { get => text; set => text = value; }
}
