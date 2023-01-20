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

    public bool Active { get => presentDialogue != null; }
    private Dialogue presentDialogue;

    private IEnumerator slowTypingCoroutine;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(this);
            Debug.LogError("Singeleton duplicated!");
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
            if (presentDialogue is SimpleDialogue)
            {
                Dialogue nextDialogue = (presentDialogue as SimpleDialogue).Next;

                TryShowDialogue(nextDialogue);
            } else if (presentDialogue is NarrationDialogue)
            {
                Dialogue nextDialogue = (presentDialogue as NarrationDialogue).Next;

                TryShowDialogue(nextDialogue);
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

        if(EventSystem.current.currentSelectedGameObject == null && presentDialogue is OptionDialogue)
        {
            EventSystem.current.SetSelectedGameObject(dialogueOptionContainer.GetChild(0).gameObject);
        }
    }

    public void InitiateDialogue(Dialogue dialogue)
    {
        ShowDialogueBox(true);

        // Run all actions attached to dialogue
        dialogue.RunActions();

        presentDialogue = dialogue;


        // Depending on a type of dialogue call a right method
        //
        if (dialogue is SimpleDialogue)
        {
            ShowSimpleDialogue(dialogue as SimpleDialogue);
            return;
        }
        if(dialogue is OptionDialogue)
        {
            ShowOptionDialogue(dialogue as OptionDialogue);
            return;
        }
        if (dialogue is NarrationDialogue)
        {
            ShowNarrationDialogue(dialogue as NarrationDialogue);
            return;
        }

        // If there is not method to handle specific Dialogue give up and throw exception
        throw new NotImplementedException();
    }

    public void TryShowDialogue(Dialogue dialogue)
    {
        if (dialogue == null)
        {
            ShowDialogueBox(false);
            presentDialogue = null;
            return;
        }
        InitiateDialogue(dialogue);

    }

    private void ShowNarrationDialogue(NarrationDialogue narrationDialogue)
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
            GameLocalization.GetText(narrationDialogue.Text));

        StartCoroutine(slowTypingCoroutine);
    }
    private void ShowOptionDialogue(OptionDialogue optionDialogue)
    {
        optionDialogueBox.gameObject.SetActive(true);
        simpleDialogueBox.gameObject.SetActive(false);
        narrationDialogueBox.gameObject.SetActive(false);

        InstantinateOptionButtons(optionDialogue);
    }

    /// <summary>
    /// Sets presentDialogue to dialogue, shows dialogue box and display text, title and sprite on it
    /// </summary>
    /// <param name="dialogue"></param>
    private void ShowSimpleDialogue(SimpleDialogue dialogue)
    {
        optionDialogueBox.gameObject.SetActive(false);
        simpleDialogueBox.gameObject.SetActive(true);
        narrationDialogueBox.gameObject.SetActive(false);

        /// Display text in dialogue box
        /// 
        simpleDialogueText.text = "";

        if(slowTypingCoroutine != null)
            StopCoroutine(slowTypingCoroutine);

        slowTypingCoroutine = SlowTypingAnimation(simpleDialogueText, GameLocalization.GetText( dialogue.Text ));
        StartCoroutine(slowTypingCoroutine);

        simpleDialogueTitle.text = GameLocalization.GetText( dialogue.Title );

        dialogueImage.sprite = dialogue.Sprite;
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

    private void InstantinateOptionButtons(OptionDialogue dialogue)
    {
        foreach (Transform child in dialogueOptionContainer) // destroy all option buttonss that were previous
        {
            Destroy(child.gameObject);
        }

        bool first = true;

        foreach (DialogueOption dialogueOption in dialogue.dialogueOptions) // create new buttons for options
        {
            var go = Instantiate(dialogueOptionPrefab.gameObject, dialogueOptionContainer);

            UI.DialogueOptionButton button = go.GetComponent<UI.DialogueOptionButton>();
            button.DialogueOption = dialogueOption;

            Text textComponent = go.GetComponentInChildren<Text>();
            textComponent.text = GameLocalization.GetText( dialogueOption.OptionName );

            if (first)
            {
                Button buttonScript = go.GetComponentInChildren<Button>();
                buttonScript.Select();
                first = false;
            }
        }
    }

    public void ChooseOption(DialogueOption dialogueOption)
    {
        TryShowDialogue(dialogueOption.ResultDialogue);
    }
}
