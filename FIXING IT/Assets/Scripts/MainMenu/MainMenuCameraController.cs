using Cinemachine;
using ProgramadorCastellano.Events;
using System;
using UnityEngine;

namespace FixingIt.MainMenu
{
    public class MainMenuCameraController : MonoBehaviour
    {
        [Header("VCams")]
        [SerializeField] CinemachineVirtualCamera _mainMenuCam;
        [SerializeField] CinemachineVirtualCamera _optionsMenuCam;
        [SerializeField] CinemachineVirtualCamera _screenSettingsCam;
        [SerializeField] CinemachineVirtualCamera _audioSettingsCam;

        [Header("Listening To")]
        [SerializeField]
        private VoidEventChannelSO _mainMenuPanelEvent;
        [SerializeField]
        private VoidEventChannelSO _optionsPanelEvent;
        [SerializeField]
        private VoidEventChannelSO _screenSettingsPanelEvent;
        [SerializeField]
        private VoidEventChannelSO _audioSettingsPanelEvent;

        private void OnEnable()
        {
            _mainMenuPanelEvent.OnEventRaised += ToMainMenuCam;
            _optionsPanelEvent.OnEventRaised += ToOptionsMenuCam;
            _screenSettingsPanelEvent.OnEventRaised += ToScreenSettingsCam;
            _audioSettingsPanelEvent.OnEventRaised += ToAudioSettingsCam;
        }

        private void OnDisable()
        {
            _mainMenuPanelEvent.OnEventRaised -= ToMainMenuCam;
            _optionsPanelEvent.OnEventRaised -= ToOptionsMenuCam;
            _screenSettingsPanelEvent.OnEventRaised -= ToScreenSettingsCam;
            _audioSettingsPanelEvent.OnEventRaised -= ToAudioSettingsCam;
        }

        private void ToMainMenuCam()
        {
            AllCamsPriorityZero();
            _mainMenuCam.Priority = 1;
        }

        private void ToOptionsMenuCam()
        {
            AllCamsPriorityZero();
            _optionsMenuCam.Priority = 1;
        }

        private void ToScreenSettingsCam()
        {
            AllCamsPriorityZero();
            _screenSettingsCam.Priority = 1;
        }

        private void ToAudioSettingsCam()
        {
            AllCamsPriorityZero();
            _audioSettingsCam.Priority = 1;
        }

        private void AllCamsPriorityZero()
        {
            _mainMenuCam.Priority = 0;
            _optionsMenuCam.Priority = 0;
            _screenSettingsCam.Priority = 0;
            _audioSettingsCam.Priority = 0;
        }
    }
}
