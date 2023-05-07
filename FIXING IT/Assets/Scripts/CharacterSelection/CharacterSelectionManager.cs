using ProgramadorCastellano.Events;
using ProgramadorCastellano.Funcs;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace FixingIt.CharacterSelection
{
    public class CharacterSelectionManager : NetworkBehaviour
    {
        [SerializeField] private float _timeCountdownMax = 5f;
        private float _timeCountdownTimer;

        private Dictionary<ulong, bool> _playerReadyDictionary;

        private bool _allPlayersReady;

        [Header("Broadcasting To")]
        [SerializeField]
        private VoidEventChannelSO _allPlayersReadyEvent;
        [SerializeField]
        private VoidEventChannelSO _clientReadyChangedEvent;
        [SerializeField]
        private FloatEventChannelSO _countdownEvent;
        [SerializeField]
        private VoidEventChannelSO _allPlayersReadyCancelledEvent;

        [Header("Listening To")]
        [SerializeField]
        private VoidEventChannelSO _readyButtonEvent;

        [Header("Setting Func")]
        [SerializeField]
        private ULongBoolFuncSO _isPlayerReadyFunc;

        private void Awake()
        {
            _timeCountdownTimer = _timeCountdownMax;

            _playerReadyDictionary = new Dictionary<ulong, bool>();
            _allPlayersReady = false;

            // clear func just in case
            _isPlayerReadyFunc.ClearOnFuncRaised();

            // set func
            _isPlayerReadyFunc.TrySetOnFuncRaised(IsPlayerReady);
        }

        private void OnEnable()
        {
            _readyButtonEvent.OnEventRaised += TogglePlayerReady;
        }

        private void OnDisable()
        {
            _readyButtonEvent.OnEventRaised -= TogglePlayerReady;
        }

        private void Update()
        {
            HandleCountdown();
        }

        #region SetPlayerReady
        private void TogglePlayerReady()
        {
            TogglePlayerReadyServerRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        private void TogglePlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
        {
            bool isPlayerReady = true;

            if (_playerReadyDictionary.ContainsKey(serverRpcParams.Receive.SenderClientId)) { 
                isPlayerReady = !_playerReadyDictionary[serverRpcParams.Receive.SenderClientId];
            }

            SetPlayerReadyClientRpc(isPlayerReady, serverRpcParams.Receive.SenderClientId);
            _playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = isPlayerReady;

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
                _timeCountdownTimer = _timeCountdownMax;
                _allPlayersReady = true;
            }
            else {
                _allPlayersReady = false;

                // send event
                _allPlayersReadyCancelledEvent.RaiseEvent();
            }
        }

        [ClientRpc]
        private void SetPlayerReadyClientRpc(bool isPlayerReady, ulong clientId)
        {
            _playerReadyDictionary[clientId] = isPlayerReady;

            // send event
            _clientReadyChangedEvent.RaiseEvent();
        }
        #endregion

        private bool IsPlayerReady(ulong clientId)
        {
            return _playerReadyDictionary.ContainsKey(clientId) && _playerReadyDictionary[clientId];
        }

        private void HandleCountdown()
        {
            if (!_allPlayersReady) {
                return;
            }

            _timeCountdownTimer -= Time.deltaTime;
            _countdownEvent.RaiseEvent(_timeCountdownTimer);
            if (_timeCountdownTimer < 0f) {
                
                // send event
                _allPlayersReadyEvent.RaiseEvent();
            }
        }
    }
}
