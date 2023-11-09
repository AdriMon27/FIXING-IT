using FixingIt.InputSystem;
using ProgramadorCastellano.Events;
using ProgramadorCastellano.Funcs;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FixingIt.UI.CharacterSelection
{
    public class UICharacterSelectionManager : MonoBehaviour
    {
        private const string READY = "READY";
        private const string NOT_READY = "NOT READY";

        [SerializeField] private InputReaderSO _inputReaderSO;

        [SerializeField] private Button _readyButton;
        [SerializeField] private TextMeshProUGUI _readyText;
        [SerializeField] private Button _mainMenuButton;

        [SerializeField] private TextMeshProUGUI _lobbyNameText;
        [SerializeField] private TextMeshProUGUI _lobbyCodeText;

        [SerializeField] private TextMeshProUGUI _countdownText;

        [Header("Panels")]
        [SerializeField]
        private UILeaveToMenuPanel _leaveToMainMenuPanel;
        //[SerializeField]
        //private UIKickedPanel _kickedPanel;

        [Header("Broadcasting To")]
        [SerializeField]
        private VoidEventChannelSO _readyButtonEvent;

        [Header("Listening To")]
        [SerializeField]
        private VoidEventChannelSO _cancelLeaveToMainMenuEvent;
        [SerializeField]
        private FloatEventChannelSO _countdownEvent;
        [SerializeField]
        private VoidEventChannelSO _allPlayersReadyCancelledEvent;

        [Header("Invoking Func")]
        [SerializeField]
        private StringFuncSO _getLobbyName;
        [SerializeField]
        private StringFuncSO _getLobbyCode;


        private void Start()
        {
            _readyButton.onClick.AddListener(ReadyButtonAction);
            _mainMenuButton.onClick.AddListener(MainMenuButtonAction);

            _readyText.text = READY;

            HideLeaveToMainMenuPopup();
            SetLobbyText();
        }

        private void OnEnable()
        {
            _inputReaderSO.MenuCancelEvent += HideLeaveToMainMenuPopup;

            _cancelLeaveToMainMenuEvent.OnEventRaised += HideLeaveToMainMenuPopup;

            _countdownEvent.OnEventRaised += ShowCountdown;
            _allPlayersReadyCancelledEvent.OnEventRaised += HideCountdown;
        }

        private void OnDisable()
        {
            _inputReaderSO.MenuCancelEvent -= HideLeaveToMainMenuPopup;

            _cancelLeaveToMainMenuEvent.OnEventRaised -= HideLeaveToMainMenuPopup;

            _countdownEvent.OnEventRaised -= ShowCountdown;
            _allPlayersReadyCancelledEvent.OnEventRaised -= HideCountdown;
        }

        private void ReadyButtonAction()
        {
            _readyButtonEvent.RaiseEvent();

            ToggleReadyText();
        }

        private void MainMenuButtonAction()
        {
            _leaveToMainMenuPanel.gameObject.SetActive(true);
            EventSystem.current.SetSelectedGameObject(_leaveToMainMenuPanel.FirstSelected);
        }

        private void HideLeaveToMainMenuPopup()
        {
            _leaveToMainMenuPanel.gameObject.SetActive(false);
            EventSystem.current.SetSelectedGameObject(GetFirstSelected());
        }

        private void ToggleReadyText()
        {
            if (_readyText.text == READY) {
                _readyText.text = NOT_READY;
            }
            else {
                _readyText.text = READY;
            }
        }

        private void SetLobbyText()
        {
            _lobbyNameText.text = $"LobbyName: {_getLobbyName.RaiseFunc()}";
            _lobbyCodeText.text = $"LobbyCode: {_getLobbyCode.RaiseFunc()}";
        }

        private GameObject GetFirstSelected()
        {
            return _readyButton.gameObject;
        }

        private void ShowCountdown(float secondsRemaining)
        {
            float secondsCeil = Mathf.Ceil(secondsRemaining);

            _countdownText.gameObject.SetActive(true);
            _countdownText.text = secondsCeil.ToString();
        }

        private void HideCountdown()
        {
            _countdownText.gameObject.SetActive(false);
        }
    }
}
