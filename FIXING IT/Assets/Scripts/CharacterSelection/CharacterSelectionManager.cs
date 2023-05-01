using ProgramadorCastellano.Events;
using ProgramadorCastellano.MyFuncs;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace FixingIt.CharacterSelection
{
    public class CharacterSelectionManager : NetworkBehaviour
    {
        private Dictionary<ulong, bool> _playerReadyDictionary;

        [Header("Broadcasting To")]
        [SerializeField]
        private VoidEventChannelSO _allPlayersReadyEvent;
        [SerializeField]
        private VoidEventChannelSO _clientReadyChangedEvent;

        [Header("Listening To")]
        [SerializeField]
        private VoidEventChannelSO _readyButtonEvent;

        [Header("Setting Func")]
        [SerializeField]
        private ULongBoolFuncSO _isPlayerReadyFunc;

        private void Awake()
        {
            _playerReadyDictionary = new Dictionary<ulong, bool>();

            _isPlayerReadyFunc.TrySetOnFuncRaised(IsPlayerReady);
        }

        private void OnEnable()
        {
            _readyButtonEvent.OnEventRaised += SetPlayerReady;
        }

        private void OnDisable()
        {
            _readyButtonEvent.OnEventRaised -= SetPlayerReady;
        }

        #region SetPlayerReady
        private void SetPlayerReady()
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
            foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds) {
                if (!_playerReadyDictionary.ContainsKey(clientId)
                    || !_playerReadyDictionary[clientId])
                {
                    // this player is not ready
                    allClientsReady = false;
                    break;
                }
            }

            if (allClientsReady) {
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
        #endregion

        private bool IsPlayerReady(ulong clientId)
        {
            return _playerReadyDictionary.ContainsKey(clientId) && _playerReadyDictionary[clientId];
        }
    }
}
