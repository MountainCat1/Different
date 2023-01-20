using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    public AudioSource soundTrackAudioSource;

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

    public void PlaySoundTrack(AudioClip audioClip, bool loop = false)
    {
        soundTrackAudioSource.loop = loop;
        soundTrackAudioSource.clip = audioClip;
        soundTrackAudioSource.Play();
    }
    public void PlaySound(AudioClip audioClip)
    {
        GameObject gameObject = new GameObject("Audio One Shot");
        var audioSource = gameObject.AddComponent<AudioSource>();
        gameObject.transform.parent = transform;
        audioSource.clip = audioClip;
        StartCoroutine(WaitToDestroySoundOneShot(audioSource));
    }

    IEnumerator WaitToDestroySoundOneShot(AudioSource audioSource)
    {
        audioSource.Play();
        yield return new WaitForSeconds(audioSource.clip.length);

        Destroy(audioSource.gameObject);
    }
}