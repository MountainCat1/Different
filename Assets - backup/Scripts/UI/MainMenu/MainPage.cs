using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPage : MenuPage
{
    public string firstSceneName;
    public Vector2Int startingPos;

    public MenuPage LoadGamePage;
    public MenuPage OptionsPage;

    public void StartGame()
    {
        Debug.Log("Clearing flags...");
        FlagManager.Flags.Clear();

        GameManager.Instance.LoadScene(firstSceneName, startingPos);
    }
    public void QuitGame()
    {
        Application.Quit();
    }

    public void LoadGame()
    {
        menu.ActivatePage(LoadGamePage);
    }

    public void Options()
    {
        menu.ActivatePage(OptionsPage);
    }
}
