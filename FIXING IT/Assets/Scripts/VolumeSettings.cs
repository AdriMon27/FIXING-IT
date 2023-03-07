using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    private const string GENERAL_VOLUME = "GeneralVolume";
    private const string MUSIC_VOLUME = "MusicVolume";
    private const string SOUND_VOLUME = "SoundVolume";

    [SerializeField] private AudioMixer audioMixer;

    [Header("Volume sliders")]
    [SerializeField] private Slider generalVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider soundVolumeSlider;

    private void Start()
    {
        generalVolumeSlider.onValueChanged.AddListener( (volumeNormalized) => SetVolume(volumeNormalized, GENERAL_VOLUME));
        musicVolumeSlider.onValueChanged.AddListener( (volumeNormalized) => SetVolume(volumeNormalized, MUSIC_VOLUME));
        soundVolumeSlider.onValueChanged.AddListener( (volumeNormalized) => SetVolume(volumeNormalized, SOUND_VOLUME));

        SetVolume(generalVolumeSlider.value, GENERAL_VOLUME);
        SetVolume(musicVolumeSlider.value, MUSIC_VOLUME);
        SetVolume(soundVolumeSlider.value, SOUND_VOLUME);
    }

    private void SetVolume(float volumeNormalized, string type)
    {
        float volume = volumeNormalized * 100 - 80;
        audioMixer.SetFloat(type, volume);
    }
}
