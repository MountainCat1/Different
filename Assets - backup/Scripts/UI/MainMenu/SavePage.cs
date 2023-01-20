using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SavePage : MenuPage
{
    public Transform savesContainer;
    public GameSavePanel gameSaveButtonPrefab;

    List<GameSavePanel> gameSavePanels;

    protected override void OnActivate()
    {
        base.OnActivate();
        InstantiateSaveButtons();
    }

    private void InstantiateSaveButtons()
    {
        foreach (Transform savePanel in savesContainer)
        {
            Destroy(savePanel.gameObject);
        }
        gameSavePanels = new List<GameSavePanel>();

        GameSave[] saves = SaveManager.Instance.GetSaves();

        foreach (GameSave save in saves)
        {
            var go = Instantiate(gameSaveButtonPrefab.gameObject, savesContainer);
            var script = go.GetComponent<GameSavePanel>();

            script.GameSave = save;
            script.savePage = this;

            gameSavePanels.Add(script);
        }
    }

    public void SelectTopSave()
    {
        if(gameSavePanels.Count > 0)
        {
            EventSystem.current.SetSelectedGameObject(gameSavePanels[0].deleteSaveButton);
        }
        else
        {
            EventSystem.current.SetSelectedGameObject(start);
        }
    }
}
