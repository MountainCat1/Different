using UnityEngine;

public abstract class DialogueAction : ScriptableObject
{
    public void RunAction()
    {
        Action();
    }
    protected abstract void Action();
}