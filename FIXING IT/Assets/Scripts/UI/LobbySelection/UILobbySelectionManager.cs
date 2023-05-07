using FixingIt.Events;
using FixingIt.InputSystem;
using ProgramadorCastellano.Events;
using ProgramadorCastellano.Funcs;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FixingIt.UI.LobbySelection
{
    public class UILobbySelectionManager : MonoBehaviour
    {
        private enum PanelShowing
        {
            Normal,
            PopUp,
            Waiting
        }

        [SerializeField] InputReaderSO _inputReaderSO;
        [SerializeField] TMP_InputField _playerNameInputField;
        private PanelShowing _panelShowing;

        [Header("Panels")]
        [SerializeField] private UILobbyOptions _lobbyOptions;
        [SerializeField] private UIPopUpPanel _popUpPanel;

        [Header("Broadcasting To")]
        [SerializeField]
        private StringEventChannelSO _setPlayerNameEvent;

        [Header("Listening To")]
        [SerializeField]
        private VoidEventChannelSO _createLobbyPanelEvent;
        [SerializeField]
        private VoidEventChannelSO _joinByCodePanelEvent;
        [SerializeField]
        private StringEventChannelSO _lobbyErrorCatchedEvent;
        [SerializeField]
        private StringEventChannelSO _rejectedToServerEvent;
        [SerializeField]
        private VoidEventChannelSO _cancelLobbyCreationEvent;
        [SerializeField]
        private VoidEventChannelSO _cancelJoinByCodeEvent;
        [SerializeField]
        private VoidEventChannelSO _acceptedLobbyErrorEvent;
        [SerializeField]
        private StringEventChannelSO _lobbyStateUpdated;
        [SerializeField]
        private LobbiesChannelSO _lobbiesListedEvent;

        [Header("Invoking Func")]
        [SerializeField]
        private StringFuncSO _getPlayerNameFunc;

        private void OnEnable()
        {
            _inputReaderSO.MenuCancelEvent += GoBackAPanel;

            _createLobbyPanelEvent.OnEventRaised += ShowCreateLobbyPanel;
            _joinByCodePanelEvent.OnEventRaised += ShowJoinByCodePanel;
            _lobbyErrorCatchedEvent.OnEventRaised += ShowErrorToUser;
            _rejectedToServerEvent.OnEventRaised += ShowErrorToUser;

            _cancelLobbyCreationEvent.OnEventRaised += HidePopUp;
            _cancelJoinByCodeEvent.OnEventRaised += HidePopUp;
            _acceptedLobbyErrorEvent.OnEventRaised += HidePopUp;
            _lobbiesListedEvent.OnEventRaised += HidePopUp;

            _lobbyStateUpdated.OnEventRaised += ShowStateUpdated;
        }

        private void OnDisable()
        {
            _inputReaderSO.MenuCancelEvent -= GoBackAPanel;

            _createLobbyPanelEvent.OnEventRaised -= ShowCreateLobbyPanel;
            _joinByCodePanelEvent.OnEventRaised -= ShowJoinByCodePanel;
            _lobbyErrorCatchedEvent.OnEventRaised -= ShowErrorToUser;
            _rejectedToServerEvent.OnEventRaised -= ShowErrorToUser;

            _cancelLobbyCreationEvent.OnEventRaised -= HidePopUp;
            _cancelJoinByCodeEvent.OnEventRaised -= HidePopUp;
            _acceptedLobbyErrorEvent.OnEventRaised -= HidePopUp;
            _lobbiesListedEvent.OnEventRaised -= HidePopUp;

            _lobbyStateUpdated.OnEventRaised -= ShowStateUpdated;
        }

        private void Start()
        {
            _inputReaderSO.EnableMenuInput();

            _playerNameInputField.text = _getPlayerNameFunc.RaiseFunc();
            _playerNameInputField.onValueChanged.AddListener((newName) => _setPlayerNameEvent.RaiseEvent(newName));

            EventSystem.current.SetSelectedGameObject(_lobbyOptions.FirstSelected);
            _panelShowing = PanelShowing.Normal;
        }

        private void ShowCreateLobbyPanel()
        {
            _popUpPanel.ShowPopUp(UIPopUpPanel.PopUpMode.CreateLobby);

            _panelShowing = PanelShowing.PopUp;
        }

        private void ShowJoinByCodePanel()
        {
            _popUpPanel.ShowPopUp(UIPopUpPanel.PopUpMode.JoinByCode);

            _panelShowing = PanelShowing.PopUp;
        }

        private void ShowErrorToUser(string errorMsg)
        {
            _popUpPanel.ShowPopUp(UIPopUpPanel.PopUpMode.Error, errorMsg);

            _panelShowing = PanelShowing.PopUp;
        }

        private void ShowStateUpdated(string stateMsg)
        {
            _popUpPanel.ShowPopUp(UIPopUpPanel.PopUpMode.LobbyState, stateMsg);

            _panelShowing = PanelShowing.Waiting;
        }

        private void HidePopUp()
        {
            _popUpPanel.HidePopUp();

            EventSystem.current.SetSelectedGameObject(_lobbyOptions.FirstSelected);
            _panelShowing = PanelShowing.Normal;
        }

        private void HidePopUp(List<Lobby> lobbies)
        {
            HidePopUp();
        }

        private void GoBackAPanel()
        {
            switch (_panelShowing)
            {
                case PanelShowing.PopUp:
                    HidePopUp();
                    break;
                default:
                    Debug.LogWarning($"We should be in {PanelShowing.Normal} and the Exit is managed by the scene loader" +
                        $"\n PanelShowing: {_panelShowing}");
                    break;
            }
        }
    }
}