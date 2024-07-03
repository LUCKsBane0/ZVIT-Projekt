using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;
    private AudioSource audioSource;

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

        audioSource = GetComponent<AudioSource>();
        LoadVolume();
        audioSource.Play();
    }

    private void LoadVolume()
    {
        float volume = PlayerPrefs.GetFloat("musicVolume", 1); // Standardwert ist 1
        SetVolume(volume);
    }

public void SetVolume(float volume)
    {
        audioSource.volume = volume;
    }
}

