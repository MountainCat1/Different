using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "OptionDialogue", menuName = "GameDialogues/OptionDialogue", order = 2)]
public class OptionDialogue : Dialogue
{
    [Space]
    [SerializeField] private Dialogue next;

    public List<DialogueOption> dialogueOptions;
}
