using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace FixingIt.UI.LobbySelection
{
    public class UILobbiesScrollArea : MonoBehaviour
    {
        [SerializeField] private Transform _lobbyTemplate;
        [SerializeField] private Transform _lobbyContainer;

        [Header("Listening To")]
        [SerializeField]
        private LobbiesChannelSO _lobbiesListedEvent;

        private void Awake()
        {
            _lobbyTemplate.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            _lobbiesListedEvent.OnEventRaised += UpdateLobbiesList;
        }

        private void OnDisable()
        {
            _lobbiesListedEvent.OnEventRaised -= UpdateLobbiesList;
        }

        private void UpdateLobbiesList(List<Lobby> lobbies)
        {
            // Destroy all childs except template
            foreach (Transform child in _lobbyContainer)
            {
                if (child == _lobbyTemplate)
                    continue;

                Destroy(child.gameObject);
            }

            // Create a new child for each lobby
            foreach (Lobby lobby in lobbies)
            {
                Transform lobbyItemTransform = Instantiate(_lobbyTemplate, _lobbyContainer);
                lobbyItemTransform.gameObject.SetActive(true);
                lobbyItemTransform.GetComponent<UILobbiesScrollAreaItem>().SetLobby(lobby);
            }
        }
    }
}