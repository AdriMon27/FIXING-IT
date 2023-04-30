using ProgramadorCastellano.MyEvents;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// PONER TODO PRIVADO DESPUES DE TESTEAR
/// EN LA VERSION FINAL QUE NO SEA SINGLETON, SOLO PARA TESTEO
/// </summary>
public class TestCharacterSelection : NetworkBehaviour
{
    public static TestCharacterSelection Instance { get; private set; }

    [Header("Broadcasting To")]
    [SerializeField]
    private VoidEventChannelSO _allPlayersReadyEvent;
    [SerializeField]
    private VoidEventChannelSO _networkToMainMenuEvent;

    private Dictionary<ulong, bool> _playerReadyDictionary;

    private void Awake()
    {
        Instance = this;
        _playerReadyDictionary = new Dictionary<ulong, bool>();
    }

    public void SetPlayerReady()
    {
        SetPlayerReadyServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        _playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;

        // check if all clients are ready
        bool allClientsReady = true;
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds) {
            if (!_playerReadyDictionary.ContainsKey(clientId)
                || !_playerReadyDictionary[clientId]) {
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

    // REFACTORIZAR EN LA NO TEST
    public void LeaveToMainMenu()
    {
        NetworkManager.Singleton.Shutdown();
        
        // send event to change scene
        _networkToMainMenuEvent.RaiseEvent();
    }
}
