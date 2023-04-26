using System;
using Unity.Netcode;
using UnityEngine;

public class FixingGameMultiplayer : MonoBehaviour
{
    [SerializeField] private GameSceneSO _characterSelectionSceneSO;

    [Header("Broadcasting To")]
    [SerializeField]
    private VoidEventChannelSO _hostStartedEvent;
    [SerializeField]
    private StringEventChannelSO _rejectedToServerEvent;

    [Header("Listening To")]
    [SerializeField]
    private VoidEventChannelSO _lobbyCreatedEvent;
    [SerializeField]
    private VoidEventChannelSO _lobbyJoinedEvent;

    [Header("Invoking Func")]
    [SerializeField]
    private StringFuncSO _getCurrentSceneNameFunc;

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
        NetworkManager.Singleton.ConnectionApprovalCallback = NetworkManager_ConnectionApprovalCallback;
        NetworkManager.Singleton.StartHost();

        // send event
        _hostStartedEvent.RaiseEvent();
    }

    private void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest connectionApprovalRequest,
        NetworkManager.ConnectionApprovalResponse connectionApprovalResponse)
    {
        string rejectedReason = string.Empty;

        // server no está en characerselection
        UnityEngine.SceneManagement.Scene characterSelectionScene = UnityEngine.SceneManagement.SceneManager.GetSceneByName(_characterSelectionSceneSO.name);
        string currentSceneName = _getCurrentSceneNameFunc.RaiseFunc();

        if (characterSelectionScene.name != currentSceneName) {
            connectionApprovalResponse.Approved = false;
            // No existe en esta versión de la librería
            //connectionApprovalResponse.Reason = "Game has already started";

            return;
        }

        connectionApprovalResponse.Approved = true;
    }

    private void StartClient()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
        NetworkManager.Singleton.StartClient();
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong obj)
    {
        _rejectedToServerEvent.RaiseEvent("Failed to join the lobby!");
    }
}
