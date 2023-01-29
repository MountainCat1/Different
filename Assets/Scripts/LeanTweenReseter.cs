using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LeanTweenReseter : MonoBehaviour
{
    private void Awake()
    {
        LeanTween.reset();
        
    }
}