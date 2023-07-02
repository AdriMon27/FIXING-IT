using FixingIt.Events;
using ProgramadorCastellano.Events;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FixingIt.UI.LobbySelection
{
    public class UICreateLobbyPanel : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _lobbyNameInputField;
        [SerializeField] private Toggle _lobbyPrivateToggle;
        [SerializeField] private Button _createButton;
        [SerializeField] private Button _cancelButton;

        public GameObject FirstSelected => _lobbyNameInputField.gameObject;

        [Header("Broadcasting To")]
        [SerializeField]
        private CreateLobbyChannelSO _createLobbyChannel;
        [SerializeField]
        private VoidEventChannelSO _cancelLobbyCreationEvent;

        private void Start()
        {
            _createButton.onClick.AddListener(CreateLobby);
            _cancelButton.onClick.AddListener(CancelLobbyCreation);
        }

        private void CreateLobby()
        {
            string lobbyName = _lobbyNameInputField.text;
            bool isPrivate = _lobbyPrivateToggle.isOn;

            _createLobbyChannel.RaiseEvent(lobbyName, isPrivate);
        }

        private void CancelLobbyCreation()
        {
            _cancelLobbyCreationEvent.RaiseEvent();
        }
    }
}