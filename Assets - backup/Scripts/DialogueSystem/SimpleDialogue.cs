using UnityEngine;

[CreateAssetMenu(fileName = "SimpleDialogue", menuName = "GameDialogues/SimpleDialogue", order = 1)]
public class SimpleDialogue : Dialogue
{
    [SerializeField] private string title;
    [SerializeField] private string text;
    [SerializeField] private Sprite sprite;

    [Space]
    [SerializeField] private Dialogue next;

    [Space (2)]
    [TextArea(2, 10)]
    public string textLocalized;

    public Dialogue Next { get => next; set => next = value; }


    public Sprite Sprite { get => sprite; set => sprite = value; }
    public string Title { get => title; set => title = value; }
    public string Text { get => text; set => text = value; }

    protected override void OnValidate()
    {
        base.OnValidate();

        if (Application.isPlaying)
            return;

        if (text == "")
            text = name;

        textLocalized = GameLocalization.LoadAndGetText(text);
    }
}
