using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TCASystem
{
    public class SceneExitAction : ActionObject
{
        public string sceneName;
        public Vector2Int startingPosition;
        //private AsyncOperation sceneLoadingOperation;

        private void Start()
        {
            //sceneLoadingOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            //sceneLoadingOperation.allowSceneActivation = false;
            //sceneLoadingOperation.completed += SceneLoadingOperation_completed;
        }

        private void SceneLoadingOperation_completed(AsyncOperation obj)
        {
            Debug.Log($"Scene \"{sceneName}\" loaded!");
        }

        public override void Action()
        {
            GameManager.Instance.LoadScene(sceneName, startingPosition);
        }
    }

}
