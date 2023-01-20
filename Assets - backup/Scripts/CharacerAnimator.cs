using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class CharacerAnimator : SpriteAnimator
{
    [SerializeField] private string framesLocation = "";
    private bool animationsLoaded = false;

    public enum Animation
    {
        Idle, Walk
    }

    public void PlayAnimation(Animation animation, bool loop = true, int startFrame = 0)
    {
        Play(animation.ToString().ToLower(), loop, startFrame);
    }

    public void SetAnimation(Animation animation)
    {
        if(currentAnimation.Name != animation.ToString().ToLower())
        {
            PlayAnimation(animation);
        }
    }

    protected override void Awake()
    {
        base.Awake();

        playAnimationOnStart = "idle";

        if (!animationsLoaded)
            LoadAnimations();
    }

    private void LoadAnimations()
    {
        List<SpriteAnimation> loadedAnimations = new List<SpriteAnimation>();

        string basePath = $"Sprites/{framesLocation}";

        loadedAnimations.Add(new SpriteAnimation()
        {
            Frames = ResourceManager.LoadAll<Sprite>($"{basePath}/idle").ToArray(),
            Name = "idle"
        });

        loadedAnimations.Add(new SpriteAnimation()
        {
            Frames = ResourceManager.LoadAll<Sprite>($"{basePath}/walk").ToArray(),
            Name = "walk",
            Type = SpriteAnimation.AnimationType.PingPong
        });

        animations = loadedAnimations;
        animationsLoaded = true;
    }
}