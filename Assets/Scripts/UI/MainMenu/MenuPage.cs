using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class MenuPage : MonoBehaviour
{
    public GameObject start;

    protected MainMenu menu;

    private void Awake()
    {
        menu = GetComponentInParent<MainMenu>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GoBack();
        }
    }

    public void Activate(bool b)
    {
        foreach (var item in GetComponentsInChildren<Selectable>())
        {
            item.interactable = b;
        }

        if (b)
            OnActivate();
    }

    protected virtual void OnActivate()
    {

    }

    public void GoBack()
    {
        menu.ActivatePage(menu.mainPage);
    }
}
