using ProgramadorCastellano.MyEvents;
using UnityEngine;
using UnityEngine.UI;

namespace FixingIt.UI.LobbySelection
{
    public class UILobbyOptions : MonoBehaviour
    {
        [Header("Buttons")]
        // poner los panel de izq a der tal como estan aqui
        [SerializeField] private Button _createLobbyButton;
        [SerializeField] private Button _joinByCodeButton;
        [SerializeField] private Button _refreshLobbiesButton;
        [SerializeField] private Button _toMainMenuButton;

        public GameObject FirstSelected => _createLobbyButton.gameObject;

        [Header("Broadcasting To")]
        [SerializeField]
        private VoidEventChannelSO _createLobbyPanelEvent;
        [SerializeField]
        private VoidEventChannelSO _joinByCodePanelEvent;
        [SerializeField]
        private VoidEventChannelSO _refreshLobbiesListEvent;
        [SerializeField]
        private VoidEventChannelSO _toMainMenuScreenEvent;

        private void Start()
        {
            _createLobbyButton.onClick.AddListener(CreateLobbyButtonAction);
            _joinByCodeButton.onClick.AddListener(JoinByCodeButtonAction);
            _refreshLobbiesButton.onClick.AddListener(RefreshLobbiesButtonAction);
            _toMainMenuButton.onClick.AddListener(ToMainMenuButtonAction);
        }

        private void CreateLobbyButtonAction()
        {
            _createLobbyPanelEvent.RaiseEvent();
        }

        private void JoinByCodeButtonAction()
        {
            _joinByCodePanelEvent.RaiseEvent();
        }
        private void RefreshLobbiesButtonAction()
        {
            _refreshLobbiesListEvent.RaiseEvent();
        }

        private void ToMainMenuButtonAction()
        {
            _toMainMenuScreenEvent.RaiseEvent();
        }
    }
}