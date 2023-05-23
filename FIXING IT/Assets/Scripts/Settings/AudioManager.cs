using FixingIt.Events;
using ProgramadorCastellano.Events;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace FixingIt.Settings
{
    public class AudioManager : MonoBehaviour
    {
        private const string GENERAL_VOLUME = "GeneralVolume";
        private const string MUSIC_VOLUME = "MusicVolume";
        private const string SFX_VOLUME = "SFXVolume";

        [SerializeField] private AudioMixerSO _mainAudioMixerSO;
        [SerializeField] private AudioClipsSO _sfxClipsSO;

        [Header("Listening To")]
        [SerializeField]
        private VoidEventChannelSO _sceneLoadedEvent;
        [SerializeField]
        private FloatEventChannelSO _generalNormalVolumeChannel;
        [SerializeField]
        private FloatEventChannelSO _musicNormalVolumeChannel;
        [SerializeField]
        private FloatEventChannelSO _sfxNormalVolumeChannel;
        [SerializeField]
        private VoidEventChannelSO _roomObjectBrokenAfterUseEvent;

        [Header("Broadcast To")]
        [SerializeField]
        private AudioVolumeChannelSO _allNormalVolumeChannel;

        private Dictionary<AudioClip, AudioSource> _recyleClips;

        private void Awake()
        {
            _recyleClips = new Dictionary<AudioClip, AudioSource>();
        }

        private void OnEnable()
        {
            _sceneLoadedEvent.OnEventRaised += OnSceneLoaded;

            _generalNormalVolumeChannel.OnEventRaised += SetGeneralVolume;
            _musicNormalVolumeChannel.OnEventRaised += SetMusicVolume;
            _sfxNormalVolumeChannel.OnEventRaised += SetSFXVolume;

            _roomObjectBrokenAfterUseEvent.OnEventRaised += PlayRoomObjectBrokenSound;
        }

        private void OnDisable()
        {
            _sceneLoadedEvent.OnEventRaised -= OnSceneLoaded;

            _generalNormalVolumeChannel.OnEventRaised -= SetGeneralVolume;
            _musicNormalVolumeChannel.OnEventRaised -= SetMusicVolume;
            _sfxNormalVolumeChannel.OnEventRaised -= SetSFXVolume;

            _roomObjectBrokenAfterUseEvent.OnEventRaised -= PlayRoomObjectBrokenSound;
        }

        private void OnSceneLoaded()
        {
            float general, music, sfx;
            _mainAudioMixerSO.Mixer.GetFloat(GENERAL_VOLUME, out general);
            _mainAudioMixerSO.Mixer.GetFloat(MUSIC_VOLUME, out music);
            _mainAudioMixerSO.Mixer.GetFloat(SFX_VOLUME, out sfx);

            NormalizeVolume(ref general);
            NormalizeVolume(ref music);
            NormalizeVolume(ref sfx);

            AudioNormalVolumes audioNormalVolumes = new AudioNormalVolumes(general, music, sfx);
            _allNormalVolumeChannel.RaiseEvent(audioNormalVolumes);
        }

        #region SetVolumes
        private void SetGeneralVolume(float normalizedVolume)
        {
            float volume = DeNormalizedVolume(normalizedVolume);
            _mainAudioMixerSO.Mixer.SetFloat(GENERAL_VOLUME, volume);
        }

        private void SetMusicVolume(float normalizedVolume)
        {
            float volume = DeNormalizedVolume(normalizedVolume);
            _mainAudioMixerSO.Mixer.SetFloat(MUSIC_VOLUME, volume);
        }

        private void SetSFXVolume(float normalizedVolume)
        {
            float volume = DeNormalizedVolume(normalizedVolume);
            _mainAudioMixerSO.Mixer.SetFloat(SFX_VOLUME, volume);
        }
        #endregion

        private float DeNormalizedVolume(float normalizedVolume)
        {
            return normalizedVolume * 100 - 80;
        }

        private void NormalizeVolume(ref float volume)
        {
            volume = (volume + 80) / 100;
        }

        private void PlaySound(AudioClip sound)
        {
            GameObject soundGameObject = new GameObject("Sound");
            AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
            audioSource.outputAudioMixerGroup = _mainAudioMixerSO.SFXMixerGroup;

            audioSource.PlayOneShot(sound);
            Destroy(soundGameObject, sound.length);
        }

        private void PlayRecycleSound(AudioClip sound)
        {
            AudioSource audioSource;
            if (!_recyleClips.ContainsKey(sound)) {
                GameObject soundGameObject = new GameObject(sound.name);
                audioSource = soundGameObject.AddComponent<AudioSource>();
                audioSource.outputAudioMixerGroup = _mainAudioMixerSO.SFXMixerGroup;

                _recyleClips.Add(sound, audioSource);
            }

            audioSource = _recyleClips[sound];
            if (audioSource.isPlaying)
                return;

            audioSource.PlayOneShot(sound);
        }

        #region SFX Sounds
        private void PlayRoomObjectBrokenSound()
        {
            PlaySound(_sfxClipsSO.RoomObjectBroken);
        }
        #endregion
    }
}