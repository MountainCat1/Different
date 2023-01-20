using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }
    private string directory { get => Path.Combine(Application.persistentDataPath, "saves"); }

    public SaveGamePopup saveGamePopup;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(this);
            Debug.LogError("Singeleton duplicated!");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

  
    public void ShowPopup()
    {
        saveGamePopup.ShowSavePopup(true);
    }  


    public void Save()
        => Save(DateTime.Now.ToString());
    public void Save(string name)
    {
        Directory.CreateDirectory(directory);

        GameSave gameSave = new GameSave()
        {
            Name = name,
            Time = DateTime.Now,
            Flags = FlagManager.Flags,
            LevelName = SceneManager.GetActiveScene().name,
            Position = PlayerController.Instance.gridPosition
        };

        string json = JsonUtility.ToJson(gameSave);
        string path = Path.Combine(directory, name + ".json");
        File.WriteAllText(path, json);
        Debug.Log($"Game saved! name: {gameSave.Name} at path: {path}");
    }

    public void Load(GameSave gameSave)
    {
        Debug.Log($"Game loaded! name: {gameSave.Name} | time: {gameSave.Time} | flags: {gameSave.Flags.Count}");

        FlagManager.Flags = gameSave.Flags;
        GameManager.Instance.LoadScene(gameSave.LevelName, gameSave.Position);
    }
    public GameSave GetSave(string name)
    {
        string path = Path.Combine(directory, name + ".json");
        string json = File.ReadAllText(path);

        GameSave gameSave = JsonUtility.FromJson<GameSave>(json);

        return gameSave;
    }

    public void Delete(GameSave gameSave)
    {
        File.Delete(Path.Combine(directory, gameSave.Name + ".json"));
    }

    public GameSave[] GetSaves()
    {
        string[] files = Directory
            .GetFiles(directory)
            .Select(x => Path.GetFileNameWithoutExtension(x))
            .ToArray();

        GameSave[] gameSaves = new GameSave[files.Count()];

        for (int i = 0; i < files.Count(); i++)
        {
            gameSaves[i] = GetSave(files[i]);
        }
        return gameSaves;
    }
}

[System.Serializable]
public class GameSave
{
    [SerializeField] private Vector2Int position;
    [SerializeField] private long timeTicks;
    [SerializeField] private string name;
    [SerializeField] private string levelName;
    [SerializeField] private FlagManager.FlagDictionary flags;

    public string Name { get => name; set => name = value; }
    public DateTime Time { get => DateTime.FromBinary(timeTicks); set => timeTicks = value.Ticks; }
    public string LevelName { get => levelName; set => levelName = value; }
    public Vector2Int Position { get => position; set => position = value; }
    public FlagManager.FlagDictionary Flags { get => flags; set => flags = value; }
}
