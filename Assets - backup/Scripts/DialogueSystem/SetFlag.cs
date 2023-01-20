
using UnityEngine;

[CreateAssetMenu(fileName = "SetFlag", menuName = "GameDialogues/DialogueAction/SetFlag", order = 1)]
public class SetFlag : DialogueAction
{
    [SerializeField] private string flagName;
    [SerializeField] private bool value;
    protected override void Action()
    {
        FlagManager.SetFlag(flagName, value);
    }
}