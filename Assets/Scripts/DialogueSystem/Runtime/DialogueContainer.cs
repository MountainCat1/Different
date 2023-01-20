using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class DialogueContainer : ScriptableObject
{
    public List<NodeLinkData> nodeLinks = new List<NodeLinkData>();
    
    public List<SimpleDialogueData> simpleDialogueNodeData = new List<SimpleDialogueData>();
    public List<NarrationDialogueData> narrationDialogueNodeData = new List<NarrationDialogueData>();
    public List<OptionDialogueData> optionDialogueNodeData = new List<OptionDialogueData>();
    
    public List<DialogueNodeData> NodeData =>
        new List<DialogueNodeData>()
            .Union(simpleDialogueNodeData)
            .Union(narrationDialogueNodeData)
            .Union(optionDialogueNodeData)
            .ToList();
    
    public DialogueNodeData GetNextFromSimpleDialogue(SimpleDialogueData nodeData)
    {
        var link = GetLinksFrom(nodeData).FirstOrDefault();
        return link == null ? null : GetNodeData(link.targetNodeGuid);
    }
    
    public DialogueNodeData GetNextFromNarrativeDialogue(NarrationDialogueData nodeData)
    {
        var link = GetLinksFrom(nodeData).FirstOrDefault();
        return link == null ? null : GetNodeData(link.targetNodeGuid);
    }

    public IEnumerable GetOptionsFromOptionDialogue(OptionDialogueData nodeData)
    {
        return GetLinksFrom(nodeData)
            .Select(x => new DialogueOption()
            {
                text = x.portName,
                outputDialogue = GetNodeData(x.targetNodeGuid)
            });
    }

    public DialogueNodeData GetStartNode()
    {
        var startNode = nodeLinks.First(x => x.portName == "Next");

        var startNodeGuid = startNode.targetNodeGuid;

        return NodeData.First(x => x.guid == startNodeGuid);
    }
    
    private DialogueNodeData GetNodeData(string guid)
    {
        return NodeData.FirstOrDefault(x => x.guid == guid);
    }
    
    
    
    private IEnumerable<NodeLinkData> GetLinksFrom(DialogueNodeData dialogueNodeData)
    {
        return nodeLinks.Where(x => x.baseNodeGuid == dialogueNodeData.guid);
    }
}
