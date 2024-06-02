using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectsManager : MonoBehaviour
{
    public static SoundEffectsManager instance;
    public AudioSource[] audioSources; 

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        audioSources = GetComponentsInChildren<AudioSource>();
    }

    public void SetVolume(float volume)
    {
        foreach (AudioSource source in audioSources)
        {
            source.volume = volume;
        }
    }
}

