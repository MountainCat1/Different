using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TCASystem
{
    public class PlayAudioAction : ActionObject
    {
        [SerializeField] private AudioClip audioClip;

        public override void Action()
        {

            AudioManager.Instance.PlaySound(audioClip);
        }
    }
}

