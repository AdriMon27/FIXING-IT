using Unity.Netcode;
using UnityEngine;

public class FixingGameMultiplayer : MonoBehaviour
{
    [Header("Broadcasting To")]
    [SerializeField]
    private VoidEventChannelSO _hostStartedEvent;

    [Header("Listening To")]
    [SerializeField]
    private VoidEventChannelSO _lobbyCreatedEvent;
    [SerializeField]
    private VoidEventChannelSO _lobbyJoinedEvent;

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
        NetworkManager.Singleton.StartHost();

        // send event
        _hostStartedEvent.RaiseEvent();
    }

    private void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest connectionApprovalRequest,
        NetworkManager.ConnectionApprovalResponse connectionApprovalResponse)
    {
        connectionApprovalResponse.Approved = true; // poner a false para no dejar entrar a gente
        //connectionApprovalResponse.CreatePlayerObject = true;
    }

    private void StartClient()
    {
        Debug.Log("aquiii dentro puto");
        NetworkManager.Singleton.StartClient();
    }
}
