using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayButtonSound : MonoBehaviour

{
    public AudioSource buttonAudioSource;
   

    public void playButtonSound()
    {
        buttonAudioSource.Play();
    }
}
