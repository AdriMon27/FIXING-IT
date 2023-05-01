using ProgramadorCastellano.Events;
using ProgramadorCastellano.MyFuncs;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace FixingIt.CharacterSelection
{
    /// <summary>
    /// PONER TODO PRIVADO DESPUES DE TESTEAR
    /// EN LA VERSION FINAL QUE NO SEA SINGLETON, SOLO PARA TESTEO
    /// </summary>
    public class TestCharacterSelection : NetworkBehaviour
    {
        public static TestCharacterSelection Instance { get; private set; }

        private Dictionary<ulong, bool> _playerReadyDictionary;

        [Header("Broadcasting To")]
        [SerializeField]
        private VoidEventChannelSO _allPlayersReadyEvent;
        [SerializeField]
        private VoidEventChannelSO _networkToMainMenuEvent;
        [SerializeField]
        private VoidEventChannelSO _clientReadyChangedEvent;

        [Header("Setting Func")]
        [SerializeField]
        private ULongBoolFuncSO _isPlayerReadyFunc;

        private void Awake()
        {
            Instance = this;
            _playerReadyDictionary = new Dictionary<ulong, bool>();

            _isPlayerReadyFunc.TrySetOnFuncRaised(IsPlayerReady);
        }

        public void SetPlayerReady()
        {
            SetPlayerReadyServerRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
        {
            SetPlayerReadyClientRpc(serverRpcParams.Receive.SenderClientId);
            _playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;

            // check if all clients are ready
            bool allClientsReady = true;
            foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                if (!_playerReadyDictionary.ContainsKey(clientId)
                    || !_playerReadyDictionary[clientId])
                {
                    // this player is not ready
                    allClientsReady = false;
                    break;
                }
            }

            if (allClientsReady)
            {
                // send event
                _allPlayersReadyEvent.RaiseEvent();
            }
        }

        [ClientRpc]
        private void SetPlayerReadyClientRpc(ulong clientId)
        {
            _playerReadyDictionary[clientId] = true;

            // send event
            _clientReadyChangedEvent.RaiseEvent();
        }

        private bool IsPlayerReady(ulong clientId)
        {
            return _playerReadyDictionary.ContainsKey(clientId) && _playerReadyDictionary[clientId];
        }

        // REFACTORIZAR EN LA NO TEST
        public void LeaveToMainMenu()
        {
            NetworkManager.Singleton.Shutdown();

            // send event to change scene
            _networkToMainMenuEvent.RaiseEvent();
        }
    }
}