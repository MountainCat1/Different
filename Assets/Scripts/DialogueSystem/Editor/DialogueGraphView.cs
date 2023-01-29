using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueGraphView : GraphView
{
    public readonly Vector2 DefaultNodeSize = new Vector2(150, 200);

    public DialogueGraphView()
    {
        styleSheets.Add(Resources.Load<StyleSheet>("DialogueGraph"));

        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        var grid = new GridBackground();
        Insert(0, grid);
        grid.StretchToParentSize();

        AddElement( GenerateEntryPointNode() );
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        var compatiblePorts = new List<Port>();

        ports.ForEach((port) => 
        {
            if(startPort != port && startPort.node != port.node)
                compatiblePorts.Add(port);
        });

        return compatiblePorts;
    }

    private Port GeneratePort(DialogueNode node, Direction portDirection, Port.Capacity capacity = Port.Capacity.Single)
    {
        return node.InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(float));
    }

    private DialogueNode GenerateEntryPointNode()
    {
        var guid = GUID.Generate().ToString();
        
        var node = new SimpleDialogueNode()
        {
            title = $"START",
            GUID = guid,
            EntryPoint = true
        };

        var generatedPort = GeneratePort(node, Direction.Output);
        generatedPort.portName = NodeLinkData.StartLinkName;
        node.outputContainer.Add(generatedPort);

        node.capabilities &= ~Capabilities.Movable;
        node.capabilities &= ~Capabilities.Deletable;

        node.RefreshExpandedState();
        node.RefreshPorts();

        node.SetPosition(new Rect(100, 200, 100, 150));
        return node;
    }

    public OptionDialogueNode CreateOptionDialogueNode()
    {
        var dialogueNode = new OptionDialogueNode()
        {
            title = "Option Dialogue",
            GUID = GUID.Generate().ToString()
        };

        var styleSheet = Resources.Load<StyleSheet>("Node");
        dialogueNode.styleSheets.Add(styleSheet);

        var inputPort = GeneratePort(dialogueNode, Direction.Input, Port.Capacity.Multi);
        inputPort.portName = "Input";
        dialogueNode.inputContainer.Add(inputPort);

        var button = new Button(() => { AddChoicePort(dialogueNode, "", true); });
        button.text = "New Choice";
        dialogueNode.titleContainer.Add(button);
        
        return dialogueNode;
    }

    public SimpleDialogueNode CreateSimpleDialogueNode(string dialogueText = "Dialogue Text", 
        string dialoguePortrait = "Dialogue Portrait", 
        string dialogueTitle = "Dialogue Title")
    {
        var guid = GUID.Generate().ToString();
        
        var dialogueNode = new SimpleDialogueNode()
        {
            title = $"Simple Dialogue",
            GUID = guid,
            dialogueText = dialogueText,
            portrait = dialoguePortrait,
            dialogueTitle = dialogueTitle
        };

        dialogueNode.styleSheets.Add(Resources.Load<StyleSheet>("Node"));

        var inputPort = GeneratePort(dialogueNode, Direction.Input, Port.Capacity.Multi);
        inputPort.portName = "Input";
        dialogueNode.inputContainer.Add(inputPort);

        AddChoicePort(dialogueNode, "Output", false);
        
        var dialogueTextField = new TextField(string.Empty);
        dialogueTextField.RegisterValueChangedCallback(evt =>
        {
            dialogueNode.dialogueText = evt.newValue;
        });
        dialogueTextField.value = dialogueText;
        dialogueNode.mainContainer.Add(dialogueTextField);

        var dialoguePortraitTextField = new TextField(string.Empty);
        dialoguePortraitTextField.RegisterValueChangedCallback(evt =>
        {
            dialogueNode.portrait = evt.newValue;
        });
        dialoguePortraitTextField.value = dialoguePortrait;
        dialogueNode.mainContainer.Add(dialoguePortraitTextField);

        
        var dialogueTitleTextField = new TextField(string.Empty);
        dialogueTitleTextField.RegisterValueChangedCallback(evt =>
        {
            dialogueNode.dialogueTitle = evt.newValue;
        });
        dialogueTitleTextField.value = dialogueTitle;
        dialogueNode.mainContainer.Add(dialogueTitleTextField);


        return dialogueNode;
    }
    
    public NarrationDialogueNode CreateNarrativeDialogueNode(string dialogueText = "")
    {
        var dialogueNode = new NarrationDialogueNode()
        {
            title = "Narrative Dialogue",
            GUID = GUID.Generate().ToString(),
            dialogueText = dialogueText
        };

        dialogueNode.styleSheets.Add(Resources.Load<StyleSheet>("Node"));

        var inputPort = GeneratePort(dialogueNode, Direction.Input, Port.Capacity.Multi);
        inputPort.portName = "Input";
        dialogueNode.inputContainer.Add(inputPort);

        AddChoicePort(dialogueNode, "Output", false);
        
        var textField = new TextField(string.Empty);
        textField.RegisterValueChangedCallback(evt =>
        {
            dialogueNode.dialogueText = evt.newValue;
        });
        textField.value = dialogueText;    
        dialogueNode.mainContainer.Add(textField);
        
        return dialogueNode;
    }

    public void AddDialogueNode(DialogueNode dialogueNode, Vector2 position)
    {
        AddElement(dialogueNode);
        
        dialogueNode.RefreshExpandedState();
        dialogueNode.RefreshPorts();
        dialogueNode.SetPosition(new Rect(position, DefaultNodeSize));
    }
    
    public void AddDialogueNode(DialogueNode dialogueNode)
    {
        AddElement(dialogueNode);
        
        dialogueNode.RefreshExpandedState();
        dialogueNode.RefreshPorts();
        dialogueNode.SetPosition(new Rect(Vector2.zero, DefaultNodeSize));
    }
    
    public void AddChoicePort(DialogueNode dialogueNode, string overriddenPortName = "", bool allowRemove = false)
    {
        var generatedPort = GeneratePort(dialogueNode, Direction.Output);

        var oldLabel = generatedPort.contentContainer.Q<Label>("type");
        generatedPort.contentContainer.Remove(oldLabel);

        var outputPortCount = dialogueNode.outputContainer.Query("connector").ToList().Count;

        var choicePortName = string.IsNullOrEmpty(overriddenPortName)
            ? $"Choice {outputPortCount}"
            : overriddenPortName;

        var textField = new TextField()
        {
            name = string.Empty,
            value = choicePortName
        };

        textField.RegisterValueChangedCallback(evt => generatedPort.portName = evt.newValue);
        generatedPort.contentContainer.Add(new Label("  "));

        if (allowRemove)
        {
            generatedPort.contentContainer.Add(textField);
            var deleteButton = new Button(() => RemovePort(dialogueNode, generatedPort)){
                text = "X"
            };
            generatedPort.contentContainer.Add(deleteButton);
        }
        
        generatedPort.portName = choicePortName;
        dialogueNode.outputContainer.Add(generatedPort);
        dialogueNode.RefreshPorts();
        dialogueNode.RefreshExpandedState();
    }

    public void RemovePort(DialogueNode dialogueNode, Port portToRemove)
    {
        var targetEdge = edges.ToList().Where(x => 
            x.output.portName == portToRemove.portName && x.output.node == portToRemove.node);

        if (targetEdge.Any())
        {
            var edge = targetEdge.First();
            edge.input.Disconnect(edge);
            RemoveElement(targetEdge.First());
        }
        
        dialogueNode.outputContainer.Remove(portToRemove);
        dialogueNode.RefreshPorts();
        dialogueNode.RefreshExpandedState();
    }
}
