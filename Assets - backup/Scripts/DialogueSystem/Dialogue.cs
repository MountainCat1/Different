using System.Collections.Generic;
using UnityEngine;

public class Dialogue : ScriptableObject
{
    [Space]
    [SerializeField] private List<DialogueAction> actions;


    protected virtual void OnValidate()
    {
        
    }

    public void RunActions()
    {
        foreach (DialogueAction action in actions)
        {
            action.RunAction();
        }
    }
}
