using ProgramadorCastellano.Events;
using UnityEngine;
using UnityEngine.UI;

namespace FixingIt.UI.MainMenu
{
    public class UIOptionsPanel : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private Button _screenSettingsButton;
        [SerializeField] private Button _volumeSettingsButton;
        [SerializeField] private Button _backButton;

        public GameObject FirstSelected => _screenSettingsButton.gameObject;

        [Header("Broadcasting To")]
        [SerializeField]
        private VoidEventChannelSO _screenSettingsPanelEvent;
        [SerializeField]
        private VoidEventChannelSO _audioSettingsPanelEvent;
        [SerializeField]
        private VoidEventChannelSO _mainMenuPanelEvent;

        private void Start()
        {
            _screenSettingsButton.onClick.AddListener(ScreenSettingsButtonAction);
            _volumeSettingsButton.onClick.AddListener(AudioSettingsButtonAction);
            _backButton.onClick.AddListener(BackButtonAction);
        }

        private void ScreenSettingsButtonAction()
        {
            _screenSettingsPanelEvent.RaiseEvent();
        }

        private void AudioSettingsButtonAction()
        {
            _audioSettingsPanelEvent.RaiseEvent();
        }

        private void BackButtonAction()
        {
            _mainMenuPanelEvent.RaiseEvent();
        }
    }
}