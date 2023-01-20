using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TCASystem {
    public class PlaySoundtrackAction : ActionObject
    {
        [SerializeField] private AudioClip audioClip;
        [SerializeField] private bool loop;

        public override void Action()
        {
            AudioManager.Instance.PlaySoundTrack(audioClip, loop);
        }
    }
}

