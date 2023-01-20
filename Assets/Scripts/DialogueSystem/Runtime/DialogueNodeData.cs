using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueNodeData
{
    public string guid;
    public Vector2 position;
}

[System.Serializable]
public class SimpleDialogueData : DialogueNodeData
{
    public string portrait;
    public string title;
    public string text;
}

[System.Serializable]
public class NarrationDialogueData: DialogueNodeData
{
    public string text;
}

[System.Serializable]
public class OptionDialogueData: DialogueNodeData
{
}

[System.Serializable]
public class DialogueOption
{
    public string text;
    public DialogueNodeData outputDialogue;
}
