using ProgramadorCastellano.MyEvents;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FixingIt.UI.LobbySelection
{
    public class UILobbySelectionManager : MonoBehaviour
    {
        private enum PanelShowing
        {
            Normal,
            PopUp
        }

        [SerializeField] InputReaderSO _inputReaderSO;
        private PanelShowing _panelShowing;

        [Header("Panels")]
        [SerializeField] private UILobbyOptions _lobbyOptions;
        [SerializeField] private UIPopUpPanel _popUpPanel;

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
        }

        private void Start()
        {
            _inputReaderSO.EnableMenuInput();

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

        private void HidePopUp()
        {
            _popUpPanel.HidePopUp();

            EventSystem.current.SetSelectedGameObject(_lobbyOptions.FirstSelected);
            _panelShowing = PanelShowing.Normal;
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