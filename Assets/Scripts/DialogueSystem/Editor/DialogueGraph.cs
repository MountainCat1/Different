using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueGraph : EditorWindow
{
    private DialogueGraphView _graphView;
    
    private string _fileName = "New Narrative";
    private string _directory = "Dialogues";

    [MenuItem("Graph/Dialogue Graph")]
    public static void OpenDialogueWindow()
    {
        var window = GetWindow<DialogueGraph>();
        window.titleContent = new GUIContent("Dialogue Graph");
    }

    private void OnEnable()
    {
        ConstructGraphView();
        GenerateToolBar();
        GenerateMiniMap();
    }

    private void GenerateMiniMap()
    {
        var miniMap = new MiniMap(){ anchored = true};
        miniMap.SetPosition(new Rect(10, 30, 200, 140));
        _graphView.Add(miniMap);
    }

    private void OnDisable()
    {
        rootVisualElement.Remove(_graphView);
    }


    private void ConstructGraphView()
    {
        _graphView = new DialogueGraphView()
        {
            name = "Dialogue Graph"
        };

        _graphView.StretchToParentSize();
        rootVisualElement.Add(_graphView);
    }

    private void GenerateToolBar()
    {
        var toolBar = new Toolbar();

        var fileDirectoryTextField = new TextField("Directory:");
        fileDirectoryTextField.SetValueWithoutNotify(_directory);
        fileDirectoryTextField.MarkDirtyRepaint();
        fileDirectoryTextField.RegisterValueChangedCallback(evt => _directory = evt.newValue);
        toolBar.Add(fileDirectoryTextField);
        
        var fileNameTextField = new TextField();
        fileNameTextField.SetValueWithoutNotify(_fileName);
        fileNameTextField.MarkDirtyRepaint();
        fileNameTextField.RegisterValueChangedCallback(evt => _fileName = evt.newValue);
        toolBar.Add(fileNameTextField);
        
        
        toolBar.Add(new Button(SaveData) { text = "Save Data"});
        toolBar.Add(new Button(LoadData) { text = "Load Data" });

        toolBar.Add(new Button((() => { _graphView.AddDialogueNode( _graphView.CreateSimpleDialogueNode());}))
        {
            text = "Simple Dialogue",
        });
        
        toolBar.Add(new Button((() => { _graphView.AddDialogueNode( _graphView.CreateOptionDialogueNode());}))
        {
            text = "Option Dialogue",
        });
        
        toolBar.Add(new Button((() => { _graphView.AddDialogueNode( _graphView.CreateNarrativeDialogueNode());}))
        {
            text = "Narrative Dialogue",
        });

        rootVisualElement.Add(toolBar);
    }

    private void LoadData()
    {
        if (string.IsNullOrEmpty(_fileName))
        {
            EditorUtility.DisplayDialog("Invalid file name!", "Please enter a valid file name.", "Ok");
            return;
        }

        var saveUtility = GraphSaveUtility.GetInstance(_graphView);
        saveUtility.LoadGraph(_directory, _fileName);
    }

    private void SaveData()
    {
        if (string.IsNullOrEmpty(_fileName))
        {
            EditorUtility.DisplayDialog("Invalid file name!", "Please enter a valid file name.", "Ok");
            return;
        }

        var saveUtility = GraphSaveUtility.GetInstance(_graphView);
        saveUtility.SaveGraph(_directory, _fileName);
    }
}
