using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public abstract class DialogueNode : Node
{
    public string GUID;
    
    public bool EntryPoint = false;

    public abstract DialogueNodeData GetData(Vector2 position);
}

public class SimpleDialogueNode : DialogueNode
{
    public string dialogueText;
    public string portrait;
    public string dialogueTitle;


    public override DialogueNodeData GetData(Vector2 position)
    {
        return new SimpleDialogueData()
        {
            guid = GUID,
            position = position,
            text = dialogueText,
            portrait = portrait,
            title = dialogueTitle
        };
    }
}

public class OptionDialogueNode : DialogueNode
{
    public override DialogueNodeData GetData(Vector2 position)
    {
        return new OptionDialogueData()
        {
            guid = GUID,
            position = position
        };
    }
}

public class NarrationDialogueNode : DialogueNode
{
    public string dialogueText;
    
    public override DialogueNodeData GetData(Vector2 position)
    {
        return new NarrationDialogueData()
        {
            guid = GUID,
            position = position,
            text = dialogueText
        };
    }
}