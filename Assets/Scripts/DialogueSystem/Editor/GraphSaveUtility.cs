using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PlasticGui.Configuration.CloudEdition.Welcome;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;


public class GraphSaveUtility
{
    private DialogueGraphView _targetGraphView;
    private DialogueContainer _containerCache;

    private List<Edge> Edges => _targetGraphView.edges.ToList();
    private List<DialogueNode> Nodes => _targetGraphView.nodes.ToList().Cast<DialogueNode>().ToList();

    public static GraphSaveUtility GetInstance(DialogueGraphView targetGrapgView)
    {
        return new GraphSaveUtility()
        {
            _targetGraphView = targetGrapgView
        };
    }

    public void SaveGraph(string fileDirectory, string fileName)
    {
        var dialogueContainer = ScriptableObject.CreateInstance<DialogueContainer>();

        var connectedPorts = Edges.Where(x => x.input.node != null).ToArray();

        for (int i = 0; i < connectedPorts.Length; i++)
        {
            var outputNode = connectedPorts[i].output.node as DialogueNode;
            var inputNode = connectedPorts[i].input.node as DialogueNode;

            dialogueContainer.nodeLinks.Add(new NodeLinkData()
            {
                baseNodeGuid = outputNode.GUID,
                portName = connectedPorts[i].output.portName,
                targetNodeGuid = inputNode.GUID
            });
        }

        foreach (var dialogueNode in Nodes.Where(node => !node.EntryPoint))
        {
            if (dialogueNode is SimpleDialogueNode)
                dialogueContainer.simpleDialogueNodeData.Add(
                    (dialogueNode.GetData(dialogueNode.GetPosition().position) as SimpleDialogueData));
            
            else if (dialogueNode is NarrationDialogueNode)
                dialogueContainer.narrationDialogueNodeData.Add(
                    (dialogueNode.GetData(dialogueNode.GetPosition().position) as NarrationDialogueData));
            
            else if(dialogueNode is OptionDialogueNode)
                dialogueContainer.optionDialogueNodeData.Add(
                    (dialogueNode.GetData(dialogueNode.GetPosition().position) as OptionDialogueData));
        }

        string filePath = $"Assets/Resources/{fileDirectory}/{fileName}.asset";

        AssetDatabase.CreateAsset(dialogueContainer, filePath);
        AssetDatabase.SaveAssets();
        
        Debug.Log($"Dialogue saved at {filePath}");
    }

    public void LoadGraph(string fileDirectory, string fileName)
    {
        string filePath = $"{fileDirectory}/{fileName}";
        _containerCache = Resources.Load<DialogueContainer>(filePath);
        if(_containerCache == null)
        {
            EditorUtility.DisplayDialog("File Not Found", "Target dialogue graph does not exists!", "Ok");
            return;
        }

        ClearGraph();
        CreateNodes();
        ConnectNodes();
    }

    private void ConnectNodes()
    {
        for (int i = 0; i < Nodes.Count; i++)
        {
            var connections = _containerCache.nodeLinks.Where(x => x.baseNodeGuid == Nodes[i].GUID).ToList();

            for (int j = 0; j < connections.Count; j++)
            {
                var targetNodeGuid = connections[j].targetNodeGuid;
                var targetNode = Nodes.First(x => x.GUID == targetNodeGuid);
                LinkNodes(Nodes[i].outputContainer[j].Q<Port>(), (Port)targetNode.inputContainer[0]);
            }
        }
    }

    private void LinkNodes(Port output, Port input)
    {
        var tempEdge = new Edge()
        {
            output = output,
            input = input,
        };

        tempEdge.input.Connect(tempEdge);
        tempEdge.output.Connect(tempEdge);
        _targetGraphView.Add(tempEdge);
    }


    private void CreateNodes()
    {
        // Create Simple Dialogue Nodes
        foreach (var nodeData in _containerCache.simpleDialogueNodeData)
        {
            var tempNode = _targetGraphView.CreateSimpleDialogueNode(nodeData.text, nodeData.portrait, nodeData.title);
            tempNode.GUID = nodeData.guid;
            _targetGraphView.AddDialogueNode(tempNode, nodeData.position);

            var nodePorts = _containerCache.nodeLinks.Where(x => x.baseNodeGuid == nodeData.guid).ToList();
            nodePorts.ForEach(x => _targetGraphView.AddChoicePort(tempNode, x.portName));
        }
        
        // Create Narration Dialogue Nodes
        foreach (var nodeData in _containerCache.narrationDialogueNodeData)
        {
            var tempNode = _targetGraphView.CreateNarrativeDialogueNode(nodeData.text);
            tempNode.GUID = nodeData.guid;
            _targetGraphView.AddDialogueNode(tempNode, nodeData.position);

            var nodePorts = _containerCache.nodeLinks.Where(x => x.baseNodeGuid == nodeData.guid).ToList();
            nodePorts.ForEach(x => _targetGraphView.AddChoicePort(tempNode, x.portName));
        }
        
        // Create Option Dialogue Nodes
        foreach (var nodeData in _containerCache.optionDialogueNodeData)
        {
            var tempNode = _targetGraphView.CreateOptionDialogueNode();
            tempNode.GUID = nodeData.guid;
            _targetGraphView.AddDialogueNode(tempNode, nodeData.position);

            var nodePorts = _containerCache.nodeLinks.Where(x => x.baseNodeGuid == nodeData.guid).ToList();
            nodePorts.ForEach(x => _targetGraphView.AddChoicePort(tempNode, x.portName, true));
        }
        
    }

    private void ClearGraph()
    {
        Nodes.Find(x => x.EntryPoint).GUID = _containerCache.nodeLinks[0].baseNodeGuid;
        foreach (var node in Nodes)
        {
            if (node.EntryPoint) continue;

            Edges.Where(x => x.input.node == node).ToList()
                .ForEach(edge => _targetGraphView.RemoveElement(edge));

            _targetGraphView.RemoveElement(node);
        }
    }
}
