using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    public Slider musicVolumeSlider;
    public Slider soundEffectsVolumeSlider;

    // Start is called before the first frame update
    void Start()
    {
        if (!PlayerPrefs.HasKey("musicVolume"))
        {
            PlayerPrefs.SetFloat("musicVolume", 1);
            Load();
        }
        else
        {
            Load();
        }

        if (!PlayerPrefs.HasKey("soundEffectsVolume"))
        {
            PlayerPrefs.SetFloat("soundEffectsVolume", 1);
            LoadSoundEffectsVolume();
        }
        else
        {
            LoadSoundEffectsVolume();
        }
    }


    public void ChangeMusicVolume()
    {
        if (MusicManager.instance != null)
        {
            MusicManager.instance.SetVolume(musicVolumeSlider.value);
        }
        Save();
    }

    public void ChangeSoundEffectsVolume()
    {
        if (SoundEffectsManager.instance != null)
        {
            SoundEffectsManager.instance.SetVolume(soundEffectsVolumeSlider.value);
        }
        SaveSoundEffectsVolume();
    }

    private void Load()
    {
        musicVolumeSlider.value = PlayerPrefs.GetFloat("musicVolume");
    }

    private void Save()
    {
        PlayerPrefs.SetFloat("musicVolume", musicVolumeSlider.value);
    }

    private void LoadSoundEffectsVolume()
    {
        soundEffectsVolumeSlider.value = PlayerPrefs.GetFloat("soundEffectsVolume");
    }

    private void SaveSoundEffectsVolume()
    {
        PlayerPrefs.SetFloat("soundEffectsVolume", soundEffectsVolumeSlider.value);
    }
}
