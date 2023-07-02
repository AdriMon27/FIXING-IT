using ProgramadorCastellano.Events;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

namespace FixingIt.UI.LobbySelection
{
    public class UILobbiesScrollAreaItem : MonoBehaviour
    {
        private Button _lobbyButton;

        [SerializeField] private TextMeshProUGUI _lobbyName;
        [SerializeField] private TextMeshProUGUI _lobbyNumberOfPeople;

        private Lobby _lobby;

        [Header("Broadcasting To")]
        [SerializeField]
        private StringEventChannelSO _joinLobbyByIdEvent;

        private void Awake()
        {
            _lobbyButton = GetComponent<Button>();

            _lobbyButton.onClick.AddListener(JoinLobbyById);
        }

        private void JoinLobbyById()
        {
            if (_lobby == null)
            {
                Debug.LogError("This Item hasn´t got a lobby!!!");
                return;
            }

            string lobbyId = _lobby.Id;

            _joinLobbyByIdEvent.RaiseEvent(lobbyId);
        }

        /// <summary>
        /// Function that must be called after creating a UILobbiesScrollAreaItem.
        /// </summary>
        /// <param name="lobby">Lobby that the new object will be related to</param>
        public void SetLobby(Lobby lobby)
        {
            _lobby = lobby;

            _lobbyName.text = _lobby.Name;
            _lobbyNumberOfPeople.text = $"{_lobby.Players.Count}/{_lobby.MaxPlayers}";
        }
    }
}