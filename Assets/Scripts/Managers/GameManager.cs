using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameObject playerPrefab;
    public Vector2Int debugSpawningPosition;

    public bool spawnPlayer = false;

    public bool LoadingScene { private set; get; }

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
        DontDestroyOnLoad(Instance);

        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
    }

   

    private void SceneManager_sceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.SetActiveScene(scene);
    }

    private void Start()
    {
        GameLocalization.LoadLanguage();

        if (spawnPlayer && !PlayerController.Instance)
        {
            SpawnPlayer(debugSpawningPosition);
        }
    }

    public void LoadScene(string sceneName, Vector2Int startingPosition)
    {
        LoadingScene = true;
        AsyncOperation sceneLoadingOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        sceneLoadingOperation.completed += (AsyncOperation obj) =>
        {
            LoadingScene = false;
            SpawnPlayer(startingPosition); 
        };

        UIManager.Instance.FadeOutOfBlackScreen();

        Debug.Log($"Loding scene {sceneName}...");
    }

    [Obsolete("This method is deprecated, please use LoadScene(string sceneName)")]
    public void LoadScene(AsyncOperation sceneLoadingOperation, Vector2Int startingPosition)
    {
        sceneLoadingOperation.completed += (AsyncOperation obj) => { SpawnPlayer(startingPosition);};
        sceneLoadingOperation.allowSceneActivation = true;
        
        UIManager.Instance.FadeOutOfBlackScreen();
    }

    private void SpawnPlayer(Vector2Int position)
    {
        var go = Instantiate(playerPrefab);
        PlayerController controller = go.GetComponent<PlayerController>();
        controller.Teleport(position);
    }
}
