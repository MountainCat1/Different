using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public Animator blackScreenAnimator;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(this);
            Debug.LogError("Singeleton duplicated!");
            return;
        }
        Instance = this;

        DontDestroyOnLoad(Instance);
    }
    public void FadeOutOfBlackScreen()
    {
        blackScreenAnimator.Play("InstaFullBlack");
    }
}
