using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectsManager : MonoBehaviour
{
    public static SoundEffectsManager instance;
    public AudioSource footstepAudioSource; 
    public AudioSource hitAudioSource;
    public AudioSource dyingAudioSource;
    public AudioSource lowHealthAudioSource;
    public AudioSource grabSwordAudioSource;
    public AudioSource releaseSwordAudioSource;
    public AudioSource blockingAudioSource;

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
    }

    public void SetVolume(float volume)
    {
        footstepAudioSource.volume = volume;
        hitAudioSource.volume = volume;
        dyingAudioSource.volume = volume;
        lowHealthAudioSource.volume = volume;
        grabSwordAudioSource.volume = volume;
        releaseSwordAudioSource.volume = volume;
    }

    private void PlaySound(AudioSource audioSource)
    {
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    public void PlayFootstepSound()
    {
        PlaySound(footstepAudioSource);
    }

    public void StopFootstepSound()
    {
        if (footstepAudioSource != null && footstepAudioSource.isPlaying)
        {
            footstepAudioSource.Stop();
        }
    }

    public void PlayHitSound()
    {
        PlaySound(hitAudioSource);
    }

    public void PlayDyingSound()
    {
        PlaySound(dyingAudioSource);
    }

    public void PlayLowHealthSound()
    {
        PlaySound(lowHealthAudioSource);
    }

    public void PlayGrabSwordSound()
    {
        PlaySound(grabSwordAudioSource);
    }
    public void PlayReleaseSwordSound()
    {
        PlaySound(releaseSwordAudioSource);
    }

    public void PlayBlockingSound()
    {
        PlaySound(blockingAudioSource); 
    }
}
