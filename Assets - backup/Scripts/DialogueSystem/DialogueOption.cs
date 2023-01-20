using UnityEngine;

[CreateAssetMenu(fileName = "DialogueOption", menuName = "GameDialogues/DialogueOption", order = 3)]
public class DialogueOption : ScriptableObject
{
    [SerializeField] private string optionName;
    [SerializeField] private Dialogue resultDialogue;

    public string OptionName { get => optionName; set => optionName = value; }
    public Dialogue ResultDialogue { get => resultDialogue; set => resultDialogue = value; }

    [Space(2)]
    [TextArea(2, 10)]
    public string textLocalized;
    protected virtual void OnValidate()
    {
        if (Application.isPlaying)
            return;

        if (optionName == "")
            optionName = name;

        textLocalized = GameLocalization.LoadAndGetText(OptionName);
    }
}