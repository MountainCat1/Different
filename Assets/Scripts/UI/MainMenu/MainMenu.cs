using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public MenuPage ActivePage { get; private set; }

    public MenuPage mainPage;

    private void Start()
    {
        ActivatePage(mainPage);
    }

    private void Update()
    {
        if(EventSystem.current.currentSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(ActivePage.start);
        }
    }

    public void ActivatePage(MenuPage page)
    {
        if(ActivePage != null)
        {
            ActivePage.Activate(false);
            ActivePage.gameObject.gameObject.SetActive(false);
        }

        
        page.gameObject.SetActive(true);
        page.Activate(true);
        EventSystem.current.SetSelectedGameObject(page.start);
        ActivePage = page;
    }
}
