using UnityEngine;
using UnityEngine.Audio;

namespace FixingIt.Settings
{
    public struct AudioNormalVolumes
    {
        public float GeneralVolume;
        public float MusicVolume;
        public float SFXVolume;

        public AudioNormalVolumes(float general, float music, float sfx)
        {
            GeneralVolume = general;
            MusicVolume = music;
            SFXVolume = sfx;
        }

        public override string ToString()
        {
            return $"GeneralVolume: {GeneralVolume}. MusicVolume: {MusicVolume}. SFXVolume: {SFXVolume}";
        }
    }

    public class AudioManager : MonoBehaviour
    {
        private const string GENERAL_VOLUME = "GeneralVolume";
        private const string MUSIC_VOLUME = "MusicVolume";
        private const string SFX_VOLUME = "SFXVolume";

        [SerializeField] private AudioMixer _mainAudioMixer;

        [Header("Listening To")]
        [SerializeField]
        private VoidEventChannelSO _sceneLoadedEvent;
        [SerializeField]
        private FloatEventChannelSO _generalNormalVolumeChannel;
        [SerializeField]
        private FloatEventChannelSO _musicNormalVolumeChannel;
        [SerializeField]
        private FloatEventChannelSO _sfxNormalVolumeChannel;

        [Header("Broadcast To")]
        [SerializeField]
        private AudioVolumeChannelSO _allNormalVolumeChannel;

        private void OnEnable()
        {
            _sceneLoadedEvent.OnEventRaised += OnSceneLoaded;

            _generalNormalVolumeChannel.OnEventRaised += SetGeneralVolume;
            _musicNormalVolumeChannel.OnEventRaised += SetMusicVolume;
            _sfxNormalVolumeChannel.OnEventRaised += SetSFXVolume;
        }

        private void OnDisable()
        {
            _sceneLoadedEvent.OnEventRaised -= OnSceneLoaded;

            _generalNormalVolumeChannel.OnEventRaised -= SetGeneralVolume;
            _musicNormalVolumeChannel.OnEventRaised -= SetMusicVolume;
            _sfxNormalVolumeChannel.OnEventRaised -= SetSFXVolume;
        }

        private void OnSceneLoaded()
        {
            float general, music, sfx;
            _mainAudioMixer.GetFloat(GENERAL_VOLUME, out general);
            _mainAudioMixer.GetFloat(MUSIC_VOLUME, out music);
            _mainAudioMixer.GetFloat(SFX_VOLUME, out sfx);

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
            _mainAudioMixer.SetFloat(GENERAL_VOLUME, volume);
        }

        private void SetMusicVolume(float normalizedVolume)
        {
            float volume = DeNormalizedVolume(normalizedVolume);
            _mainAudioMixer.SetFloat(MUSIC_VOLUME, volume);
        }

        private void SetSFXVolume(float normalizedVolume)
        {
            float volume = DeNormalizedVolume(normalizedVolume);
            _mainAudioMixer.SetFloat(SFX_VOLUME, volume);
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
    }
}