using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SaveGamePopup : MonoBehaviour
{
    public LeanTweenType leenType;
    public InputField saveNameInputfield;
    public AudioSource saveSound;
    public bool Active { get; private set; } = false;

    private void Start()
    {
        ShowSavePopup(false);
    }
    public void SavePopupAccept()
    {
        SaveManager.Instance.Save(saveNameInputfield.text);
        saveSound.Play();
        ShowSavePopup(false);
    }

    public void SavePopupCancel()
    {
        saveNameInputfield.text = "";
        ShowSavePopup(false);
    }

    public void ShowSavePopup(bool show = true)
    {
        EventSystem.current.SetSelectedGameObject(saveNameInputfield.gameObject);

        saveNameInputfield.text = "";

        if (show)
        {
            LeanTween.scale(gameObject, new Vector3(1, 1, 1), 0.1f).setEase(leenType);
        }
        else
        {
            LeanTween.scale(gameObject, new Vector3(1, 0, 0), 0.1f).setEase(leenType);
        }

        foreach (var item in GetComponentsInChildren<Selectable>())
        {
            item.interactable = show;
        }

        Active = show;
    }
}
