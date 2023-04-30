using System;
using Unity.Netcode;
using UnityEngine;

public class FixingGameMultiplayer : NetworkBehaviour
{
    [SerializeField] private GameSceneSO _characterSelectionSceneSO;

    private NetworkList<PlayerData> _playerDataNetworkList;

    [Header("Broadcasting To")]
    [SerializeField]
    private VoidEventChannelSO _hostStartedEvent;
    [SerializeField]
    private StringEventChannelSO _rejectedToServerEvent;
    [SerializeField]
    private VoidEventChannelSO _playerDataNetworkListChangedEvent;

    [Header("Listening To")]
    [SerializeField]
    private VoidEventChannelSO _lobbyCreatedEvent;
    [SerializeField]
    private VoidEventChannelSO _lobbyJoinedEvent;

    [Header("Invoking Func")]
    [SerializeField]
    private StringFuncSO _getCurrentSceneNameFunc;

    [Header("Setting Func")]
    [SerializeField]
    private IntBoolFuncSO _isPlayerIndexConnected;

    private void Awake()
    {
        _playerDataNetworkList = new NetworkList<PlayerData>();
        _playerDataNetworkList.OnListChanged += OnPlayerDataNetworkListChanged;

        _isPlayerIndexConnected.TrySetOnFuncRaised(IsPlayerIndexConnected);
    }

    private void OnEnable()
    {
        _lobbyCreatedEvent.OnEventRaised += StartHost;
        _lobbyJoinedEvent.OnEventRaised += StartClient;
    }

    private void OnDisable()
    {
        _lobbyCreatedEvent.OnEventRaised -= StartHost;
        _lobbyJoinedEvent.OnEventRaised -= StartClient;
    }

    private void StartHost()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
        NetworkManager.Singleton.StartHost();

        // send event
        _hostStartedEvent.RaiseEvent();
    }
    private void StartClient()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
        NetworkManager.Singleton.StartClient();
    }

    private void OnPlayerDataNetworkListChanged(NetworkListEvent<PlayerData> changeEvent)
    {
        _playerDataNetworkListChangedEvent.RaiseEvent();
    }

    private bool IsPlayerIndexConnected(int playerIndex)
    {
        //if (!IsServer)
            //return false;

        return playerIndex < _playerDataNetworkList.Count;
    }

    #region NetworkCallbacks
    private void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest connectionApprovalRequest,
        NetworkManager.ConnectionApprovalResponse connectionApprovalResponse)
    {
        string rejectedReason = string.Empty;

        // server no est� en characerselection
        UnityEngine.SceneManagement.Scene characterSelectionScene = UnityEngine.SceneManagement.SceneManager.GetSceneByName(_characterSelectionSceneSO.name);
        string currentSceneName = _getCurrentSceneNameFunc.RaiseFunc();

        if (characterSelectionScene.name != currentSceneName) {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "Game has already started!";

            return;
        }

        connectionApprovalResponse.Approved = true;
    }

    /// <summary>
    /// Only subscribed by the host
    /// Host manages what to do when a client connects
    /// </summary>
    /// <param name="clientId">clientId that has connected</param>
    private void NetworkManager_OnClientConnectedCallback(ulong clientId)
    {
        _playerDataNetworkList.Add(new PlayerData() {
            ClientId = clientId,
        });
    }


    private void NetworkManager_OnClientDisconnectCallback(ulong obj)
    {
        _rejectedToServerEvent.RaiseEvent(NetworkManager.Singleton.DisconnectReason);
    }
    #endregion
}