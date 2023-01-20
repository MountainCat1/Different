using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    public LeanTweenType leenType;

    public Transform dialogueBox;

    public Transform simpleDialogueBox;
    public Text simpleDialogueText;
    public Text simpleDialogueTitle;
    public Image dialogueImage;

    public Transform optionDialogueBox;
    public Transform dialogueOptionContainer;
    public UI.DialogueOptionButton dialogueOptionPrefab;

    public Transform narrationDialogueBox;
    public Text narrationDialogueText;

    public float speed = 5f;

    public bool Active => presentDialogueContainer != null;
    private DialogueContainer presentDialogueContainer;
    private DialogueNodeData presentDialogueNode; 

    private IEnumerator slowTypingCoroutine;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(this);
            Debug.LogError("Singleton duplicated!");
            return;
        }

        Instance = this;
    }


    private void Start()
    {
        ShowDialogueBox(false);
    }

    private void Update()
    {
        if (!Active)
            return;
        if (Input.GetKeyDown(KeyCode.E) && PlayerController.Instance.Interacted) // If character interacted in THIS specific frame,
                                                  // input should not be applied to a dialogue
        {
            //Debug.Log("Dialogue stopped coz player interacted");
            return;
        }
            

        if (Input.GetKeyDown(KeyCode.Return)|| Input.GetKeyDown(KeyCode.E))
        {
            //Debug.Log("Dialogue input handled");
            if (presentDialogueNode is SimpleDialogueData simpleDialogueNodeData)
            {
                var nextDialogue = presentDialogueContainer.GetNextFromSimpleDialogue( simpleDialogueNodeData);

                TryShowDialogueNode(nextDialogue);
            } else if (presentDialogueNode is NarrationDialogueData narrationDialogueData)
            {
                var nextDialogue = presentDialogueContainer.GetNextFromNarrativeDialogue(narrationDialogueData);

                TryShowDialogueNode(nextDialogue);
            }
            else
            {
                var buttons = dialogueOptionContainer.GetComponentsInChildren<Button>();
                foreach (var button in buttons) // destroy all option buttonss that were previous
                {
                    if (EventSystem.current.currentSelectedGameObject == button.gameObject)
                    {
                        ChooseOption(button.GetComponent<UI.DialogueOptionButton>().DialogueOption);
                    }
                }
            }
        }

        if(EventSystem.current.currentSelectedGameObject == null && presentDialogueNode is OptionDialogueData)
        {
            EventSystem.current.SetSelectedGameObject(dialogueOptionContainer.GetChild(0).gameObject);
        }
    }

    public void InitiateDialogue(DialogueContainer dialogue)
    {
        ShowDialogueBox(true);

        // TODO Run all actions attached to dialogue
        // TODO // dialogue.RunActions();
        
        presentDialogueContainer = dialogue;
        
        ShowDialogueNode(dialogue.GetStartNode());
    }

    private void TryShowDialogueNode(DialogueNodeData dialogueNode)
    {
        if (dialogueNode == null)
        {
            ShowDialogueBox(false);
            presentDialogueContainer = null;
            return;
        }
        ShowDialogueNode(dialogueNode);
    }

    private void ShowDialogueNode(DialogueNodeData dialogueNode)
    {
        presentDialogueNode = dialogueNode;
        
        // Depending on a type of dialogue call a right method
        //
        if (presentDialogueNode is SimpleDialogueData simpleDialogue)
        {
            ShowSimpleDialogue(simpleDialogue);
            return;
        }
        if(presentDialogueNode is OptionDialogueData optionDialogue)
        {
            ShowOptionDialogue(optionDialogue);
            return;
        }
        if (presentDialogueNode is NarrationDialogueData narrationDialogue)
        {
            ShowNarrationDialogue(narrationDialogue);
            return;
        }
        // If there is not method to handle specific Dialogue give up and throw exception
        throw new NotImplementedException();
    }

    private void ShowNarrationDialogue(NarrationDialogueData narrationDialogueNode)
    {
        narrationDialogueBox.gameObject.SetActive(true);
        optionDialogueBox.gameObject.SetActive(false);
        simpleDialogueBox.gameObject.SetActive(false);

        /// Display text in dialogue box
        /// 
        narrationDialogueText.text = "";

        if (slowTypingCoroutine != null)
            StopCoroutine(slowTypingCoroutine);

        slowTypingCoroutine = SlowTypingAnimation(narrationDialogueText,
            GameLocalization.GetText(narrationDialogueNode.text));

        StartCoroutine(slowTypingCoroutine);
    }
    private void ShowOptionDialogue(OptionDialogueData optionDialogueNode)
    {
        optionDialogueBox.gameObject.SetActive(true);
        simpleDialogueBox.gameObject.SetActive(false);
        narrationDialogueBox.gameObject.SetActive(false);

        InstantiateOptionButtons(optionDialogueNode);
    }

    /// <summary>
    /// Sets presentDialogue to dialogue, shows dialogue box and display text, title and sprite on it
    /// </summary>
    /// <param name="dialogueNode"></param>
    private void ShowSimpleDialogue(SimpleDialogueData dialogueNode)
    {
        optionDialogueBox.gameObject.SetActive(false);
        simpleDialogueBox.gameObject.SetActive(true);
        narrationDialogueBox.gameObject.SetActive(false);

        // Display text in dialogue box
        // 
        simpleDialogueText.text = "";

        if(slowTypingCoroutine != null)
            StopCoroutine(slowTypingCoroutine);

        slowTypingCoroutine = SlowTypingAnimation(simpleDialogueText, GameLocalization.GetText( dialogueNode.text ));
        StartCoroutine(slowTypingCoroutine);

        simpleDialogueTitle.text = GameLocalization.GetText( dialogueNode.title );

        // TODO load a sprite using portrait path in SimpleDialogueData
        //dialogueImage.sprite = dialogue.Sprite;
    }

    private IEnumerator SlowTypingAnimation(Text component, string text)
    {
        component.text = "";
        foreach (char c in text)
        {
            component.text += c;
            yield return new WaitForSeconds(1f / speed);
        }
    }

    private void ShowDialogueBox(bool show = true)
    {
        if (show)
            LeanTween.scale(dialogueBox.gameObject, new Vector3(1, 1, 1), 0.1f).setEase(leenType);
        else
            LeanTween.scale(dialogueBox.gameObject, new Vector3(1, 0, 0), 0.1f).setEase(leenType);

        foreach (var item in dialogueBox.GetComponentsInChildren<Selectable>())
        {
            item.interactable = show;
        }
    }

    private void InstantiateOptionButtons(OptionDialogueData dialogueNode)
    {
        foreach (Transform child in dialogueOptionContainer) // destroy all option buttonss that were previous
        {
            Destroy(child.gameObject);
        }
        bool first = true;

        var options = presentDialogueContainer.GetOptionsFromOptionDialogue(dialogueNode);
        
        foreach (DialogueOption dialogueOption in options) // create new buttons for options
        {
            var go = Instantiate(dialogueOptionPrefab.gameObject, dialogueOptionContainer);

            UI.DialogueOptionButton button = go.GetComponent<UI.DialogueOptionButton>();
            button.DialogueOption = dialogueOption;

            Text textComponent = go.GetComponentInChildren<Text>();
            textComponent.text = GameLocalization.GetText( dialogueOption.text );

            if (first)
            {
                Button buttonScript = go.GetComponentInChildren<Button>();
                buttonScript.Select();
                first = false;
            }
        }
    }

    private void ChooseOption(DialogueOption dialogueOption)
    {
        TryShowDialogueNode(dialogueOption.outputDialogue);
    }
}
