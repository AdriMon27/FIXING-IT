using FixingIt.Events;
using FixingIt.Settings;
using ProgramadorCastellano.MyEvents;
using UnityEngine;
using UnityEngine.UI;

namespace FixingIt.UI.MainMenu
{
    /// <summary>
    /// Its gameobject should be enabled when start scene
    /// </summary>
    public class UIAudioSettings : MonoBehaviour
    {
        [SerializeField] private Slider _generalVolumeSlider;
        [SerializeField] private Slider _musicVolumeSlider;
        [SerializeField] private Slider _sfxVolumeSlider;

        public GameObject FirstSelected => _generalVolumeSlider.gameObject;

        [Header("Listening To")]
        [SerializeField]
        private AudioVolumeChannelSO _allNormalVolumeChannel;

        [Header("Broadcasting To")]
        [SerializeField]
        private FloatEventChannelSO _generalNormalVolumeChannel;
        [SerializeField]
        private FloatEventChannelSO _musicNormalVolumeChannel;
        [SerializeField]
        private FloatEventChannelSO _sfxNormalVolumeChannel;

        private void OnEnable()
        {
            _generalVolumeSlider.onValueChanged.AddListener(GeneralSliderChanged);
            _musicVolumeSlider.onValueChanged.AddListener(MusicSliderChanged);
            _sfxVolumeSlider.onValueChanged.AddListener(SFXSliderChanged);
        }

        private void OnDisable()
        {
            _generalVolumeSlider.onValueChanged.RemoveListener(GeneralSliderChanged);
            _musicVolumeSlider.onValueChanged.RemoveListener(MusicSliderChanged);
            _sfxVolumeSlider.onValueChanged.RemoveListener(SFXSliderChanged);
        }

        private void Start()
        {
            _allNormalVolumeChannel.OnEventRaised += SetVisuals;
        }

        private void OnDestroy()
        {
            _allNormalVolumeChannel.OnEventRaised -= SetVisuals;
        }

        private void GeneralSliderChanged(float normalizedVolume)
        {
            _generalNormalVolumeChannel.RaiseEvent(normalizedVolume);
        }

        private void MusicSliderChanged(float normalizedVolume)
        {
            _musicNormalVolumeChannel.RaiseEvent(normalizedVolume);
        }

        private void SFXSliderChanged(float normalizedVolume)
        {
            _sfxNormalVolumeChannel.RaiseEvent(normalizedVolume);
        }

        private void SetVisuals(AudioNormalVolumes audioNormalVolumes)
        {
            _generalVolumeSlider.value = audioNormalVolumes.GeneralVolume;
            _musicVolumeSlider.value = audioNormalVolumes.MusicVolume;
            _sfxVolumeSlider.value = audioNormalVolumes.SFXVolume;

            gameObject.SetActive(false);
        }
    }
}