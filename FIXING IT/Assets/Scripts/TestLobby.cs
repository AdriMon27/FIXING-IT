using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

//////////////////////////////////////
//HACER TODO PRIVATE DESPUES DE PROBAR
//////////////////////////////////////
public class TestLobby : MonoBehaviour
{
    public TextMeshProUGUI testText;

    private Lobby _hostLobby;

    private async void Start()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
            {
                Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
            };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    private void OnDestroy()
    {
        StopAllCoroutines();    //stop all heartbeats
    }

    public async void CreateLobby()
    {
        try {
            string lobbyName = "MyLobby";
            int maxPlayers = 1;
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers);

            _hostLobby = lobby;

            // Send a heartbeat every 15 seconds to keep the room alive
            StartCoroutine(HeartbeatLobbyCoroutine(_hostLobby.Id, 15));

            string msg = "Created lobby! " + lobby.Name + " " + lobby.MaxPlayers;
            Debug.Log(msg);
            testText.text = msg;
        }
        catch (LobbyServiceException e) {
            Debug.Log(e);
        }
        
    }

    private static IEnumerator HeartbeatLobbyCoroutine(string lobbyId, float waitTimeSeconds)
    {
        var delay = new WaitForSecondsRealtime(waitTimeSeconds);
        while (true) {
            Lobbies.Instance.SendHeartbeatPingAsync(lobbyId);
            yield return delay;
        }
    }

    public async void ListLobbies()
    {
        try {
            // we can list the lobbies
            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions {
                Count = 25,
                Filters = new List<QueryFilter> {
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)  
                },
                Order = new List<QueryOrder> {
                    new QueryOrder (false, QueryOrder.FieldOptions.Created)
                }
            };

            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync(queryLobbiesOptions);

            Debug.Log("Lobbies found: " + queryResponse.Results.Count);
            testText.text = "Lobbies found: " + queryResponse.Results.Count;

            foreach (Lobby lobby in queryResponse.Results)
            {
                Debug.Log(lobby.Name + " " + lobby.MaxPlayers);
                testText.text += "\n" + lobby.Name + " " + lobby.MaxPlayers;
            }
        }
        catch (LobbyServiceException e) {
            Debug.Log(e);
        }
    }
}
