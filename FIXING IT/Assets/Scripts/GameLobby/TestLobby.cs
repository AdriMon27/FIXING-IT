using System.Collections.Generic;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace FixingIt.GameLobby
{
    //////////////////////////////////////
    //HACER TODO PRIVATE DESPUES DE PROBAR
    //////////////////////////////////////
    public class TestLobby : MonoBehaviour
    {
        public TextMeshProUGUI testText;

        private Lobby _hostLobby;
        private Lobby _joinedLobby;
        private float _heartbeatTimer;
        private float _lobbyUpdateTimer;
        private string _playerName;

        private async void Start()
        {
            await UnityServices.InitializeAsync();

            AuthenticationService.Instance.SignedIn += () =>
                {
                    Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
                };

            await AuthenticationService.Instance.SignInAnonymouslyAsync();

            _playerName = $"Sadrmm {Random.Range(10, 99)}";
            Debug.Log(_playerName);
        }

        private void Update()
        {
            HandleLobbyHeartBeat();
            HandleLobbyPollForUpdates();
        }

        private async void HandleLobbyHeartBeat()
        {
            if (_hostLobby == null)
                return;

            _heartbeatTimer -= Time.deltaTime;
            if (_heartbeatTimer < 0f)
            {
                float heartbeatTimerMax = 15f;
                _heartbeatTimer = heartbeatTimerMax;

                await LobbyService.Instance.SendHeartbeatPingAsync(_hostLobby.Id);
            }
        }

        private async void HandleLobbyPollForUpdates()
        {
            if (_joinedLobby == null)
                return;

            _lobbyUpdateTimer -= Time.deltaTime;
            if (_lobbyUpdateTimer < 0f)
            {
                float lobbyUpdateTimerMax = 1.1f;
                _lobbyUpdateTimer = lobbyUpdateTimerMax;

                Lobby lobby = await LobbyService.Instance.GetLobbyAsync(_joinedLobby.Id);
                _joinedLobby = lobby;
            }
        }

        public async void CreateLobby()
        {
            try
            {
                string lobbyName = "MyLobby";
                int maxPlayers = 4;
                CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions
                {
                    IsPrivate = false,
                    Player = GetPlayer(),
                    // In my game there will be 1 gamemode, so this will not be necessary
                    Data = new Dictionary<string, DataObject> {
                    {"GameMode", new DataObject(DataObject.VisibilityOptions.Public, "SpeedFix"/*, DataObject.IndexOptions.S1*/) }
                }
                };

                Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, createLobbyOptions);

                _hostLobby = lobby;
                _joinedLobby = _hostLobby;

                string msg = $"Created lobby! {lobby.Name}, {lobby.MaxPlayers}, {lobby.Id}, {lobby.LobbyCode}";
                Debug.Log(msg);
                testText.text = msg;

                PrintPlayers(_hostLobby);
            }
            catch (LobbyServiceException e)
            {
                Debug.LogException(e);
            }

        }

        public async void ListLobbies()
        {
            try
            {
                // we can list the lobbies
                QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions
                {
                    Count = 25,
                    Filters = new List<QueryFilter> {
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
                    // I can put another filter to compare the data, for example for the GameMode
                    //new QueryFilter(QueryFilter.FieldOptions.S1, "SpeedFix", QueryFilter.OpOptions.EQ)
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
                    string msg = $"{lobby.Name} {lobby.MaxPlayers} {lobby.Data["GameMode"].Value}";
                    Debug.Log(msg);
                    testText.text += "\n" + msg;
                }
            }
            catch (LobbyServiceException e)
            {
                Debug.LogException(e);
            }
        }

        public async void JoinLobby()
        {
            try
            {
                QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();

                Lobby lobby = await Lobbies.Instance.JoinLobbyByIdAsync(queryResponse.Results[0].Id);
                _joinedLobby = lobby;
            }
            catch (LobbyServiceException e)
            {
                Debug.LogException(e);
            }
        }

        public async void JoinLobbyByCode(string lobbyCode)
        {
            try
            {
                JoinLobbyByCodeOptions joinLobbyByCodeOptions = new JoinLobbyByCodeOptions
                {
                    Player = GetPlayer()
                };

                Lobby lobby = await Lobbies.Instance.JoinLobbyByCodeAsync(lobbyCode, joinLobbyByCodeOptions);
                _joinedLobby = lobby;

                Debug.Log($"Joined Lobby with code: {lobbyCode}");
                PrintPlayers(lobby);
            }
            catch (LobbyServiceException e)
            {
                Debug.LogException(e);
            }
        }

        public async void QuickJoinLobby()
        {
            try
            {
                Lobby lobby = await LobbyService.Instance.QuickJoinLobbyAsync();
                _joinedLobby = lobby;
            }
            catch (LobbyServiceException e)
            {
                Debug.LogException(e);
            }
        }

        public void PrintPlayers()
        {
            testText.text = "";
            PrintPlayers(_joinedLobby);
        }

        private void PrintPlayers(Lobby lobby)
        {
            string msg = $"Players in lobby {lobby.Name} {lobby.Data["GameMode"].Value}";
            Debug.Log(msg);
            testText.text += "\n" + msg;
            foreach (Player player in lobby.Players)
            {
                msg = $"{player.Id}: {player.Data["PlayerName"].Value}";
                Debug.Log(msg);
                testText.text += $"\n {msg}";
            }
        }

        private Player GetPlayer()
        {
            return new Player
            {
                Data = new Dictionary<string, PlayerDataObject> {
                {"PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, _playerName) }
            }
            };
        }

        public async void UpdateLobbyGameMode(string gameMode)
        {
            try
            {
                _hostLobby = await Lobbies.Instance.UpdateLobbyAsync(_hostLobby.Id, new UpdateLobbyOptions
                {
                    Data = new Dictionary<string, DataObject> {
                    {"GameMode", new DataObject(DataObject.VisibilityOptions.Public, gameMode) }
                    // if there are more keys but I do NOT want to update them, I can leave it empty
                }
                });
                _joinedLobby = _hostLobby;

                PrintPlayers(_hostLobby);
            }
            catch (LobbyServiceException e)
            {
                Debug.LogException(e);
            }
        }

        public async void UpdatePlayerName(string newPlayerName)
        {
            try
            {
                _playerName = newPlayerName;
                await LobbyService.Instance.UpdatePlayerAsync(_joinedLobby.Id, AuthenticationService.Instance.PlayerId, new UpdatePlayerOptions
                {
                    Data = new Dictionary<string, PlayerDataObject> {
                {"PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, _playerName) }
            }
                });
            }
            catch (LobbyServiceException e)
            {
                Debug.LogException(e);
            }
        }

        public async void LeaveLobby()
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(_joinedLobby.Id, AuthenticationService.Instance.PlayerId);
            }
            catch (LobbyServiceException e)
            {
                Debug.LogException(e);
            }
        }

        public async void KickPlayer()
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(_joinedLobby.Id, _joinedLobby.Players[1].Id);
            }
            catch (LobbyServiceException e)
            {
                Debug.LogException(e);
            }
        }

        private async void MigrateLobbyHost()
        {
            try
            {
                _hostLobby = await Lobbies.Instance.UpdateLobbyAsync(_hostLobby.Id, new UpdateLobbyOptions
                {
                    HostId = _joinedLobby.Players[1].Id
                });
                _joinedLobby = _hostLobby;

                PrintPlayers(_hostLobby);
            }
            catch (LobbyServiceException e)
            {
                Debug.LogException(e);
            }
        }

        public async void DeleteLobby()
        {
            try
            {
                await LobbyService.Instance.DeleteLobbyAsync(_joinedLobby.Id);
            }
            catch (LobbyServiceException e)
            {
                Debug.LogException(e);
            }
        }
    }
}