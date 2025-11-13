using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsUI : MonoBehaviour
{
    public Slider musicSlider;
    public Slider sfxSlider;

    private void Start()
    {
        if (SceneLoader.Instance == null)
        {
            Debug.LogWarning("SceneLoader not found in scene.");
            return;
        }

        // Initialize slider values from SceneLoader
        musicSlider.value = SceneLoader.Instance.musicVolume;
        sfxSlider.value = SceneLoader.Instance.sfxVolume;

        // Add listeners to update SceneLoader when sliders move
        musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
    }

    private void OnMusicVolumeChanged(float value)
    {
        SceneLoader.Instance.SetMusicVolume(value);
        SceneLoader.Instance.SaveVolumes();
    }

    private void OnSFXVolumeChanged(float value)
    {
        SceneLoader.Instance.SetSFXVolume(value);
        SceneLoader.Instance.SaveVolumes();
    }
}
