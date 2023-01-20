using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSavePanel : MonoBehaviour
{
    private GameSave gameSave;

    public GameObject loadSaveButton;
    public GameObject deleteSaveButton;

    public SavePage savePage;
    public Text saveName;
    public Text saveDate;

    public GameSave GameSave { get => gameSave; set => SetGameSave(value); }

    public void LoadSave()
    {
        SaveManager.Instance.Load(GameSave);
    }

    public void DeleteSave()
    {
        SaveManager.Instance.Delete(GameSave);
        Destroy(gameObject);
        savePage.SelectTopSave();
    }

    private void SetGameSave(GameSave gameSave)
    {
        saveName.text = gameSave.Name;
        saveDate.text = gameSave.Time.ToString();
        this.gameSave = gameSave;
    }
}
