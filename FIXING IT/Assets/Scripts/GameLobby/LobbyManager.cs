using FixingIt.Events;
using ProgramadorCastellano.Events;
using ProgramadorCastellano.Funcs;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace FixingIt.GameLobby
{
    // It is a Singleton just to work through scenes and delete it whenever I want
    public class LobbyManager : MonoBehaviour
    {
        public static LobbyManager Instance { get; private set;}

        private const string PLAYER_NAME = "PlayerName";

        private Lobby _hostLobby;
        private Lobby _joinedLobby;
        private float _heartbeatTimer;
        private float _lobbyUpdateTimer;
        private string _playerName;

        [Header("Broadcasting To")]
        [SerializeField]
        private LobbiesChannelSO _lobbiesListedEvent;
        [SerializeField]
        private VoidEventChannelSO _lobbyCreatedEvent;
        [SerializeField]
        private VoidEventChannelSO _lobbyJoinedEvent;
        [SerializeField]
        private StringEventChannelSO _lobbyErrorCatchedEvent;

        [Header("Listening To")]
        [SerializeField]
        private VoidEventChannelSO _refreshLobbiesListEvent;
        [SerializeField]
        private CreateLobbyChannelSO _createLobbyChannel;
        [SerializeField]
        private StringEventChannelSO _joinByIdEvent;
        [SerializeField]
        private StringEventChannelSO _joinByCodeEvent;
        [SerializeField]
        private VoidEventChannelSO _allPlayersReadyEvent;
        [SerializeField]
        private VoidEventChannelSO _toMainMenuScreenEvent;
        [SerializeField]
        private VoidEventChannelSO _leaveGameToMainMenuEvent;
        [SerializeField]
        private StringEventChannelSO _kickPlayerPlayerIdEvent;
        [SerializeField]
        private StringEventChannelSO _playerIdDisconnectedEvent;

        [Header("Setting Func")]
        [SerializeField]
        private StringFuncSO _getLobbyNameFunc;
        [SerializeField]
        private StringFuncSO _getLobbyCodeFunc;

        private void Awake()
        {
            if (Instance != null && Instance != this) {
                Destroy(this);
            }
            else {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }

            _getLobbyNameFunc.TrySetOnFuncRaised(() => _joinedLobby.Name);
            _getLobbyCodeFunc.TrySetOnFuncRaised(() => _joinedLobby.LobbyCode);
        }

        private void OnEnable()
        {
            _refreshLobbiesListEvent.OnEventRaised += ListLobbies;
            _createLobbyChannel.OnEventRaised += CreateLobby;
            _joinByIdEvent.OnEventRaised += JoinLobbyById;
            _joinByCodeEvent.OnEventRaised += JoinLobbyByCode;

            _allPlayersReadyEvent.OnEventRaised += DeleteLobby;

            _toMainMenuScreenEvent.OnEventRaised += LeaveLobby;
            _leaveGameToMainMenuEvent.OnEventRaised += LeaveLobby;

            _kickPlayerPlayerIdEvent.OnEventRaised += KickPlayer;
            _playerIdDisconnectedEvent.OnEventRaised += KickPlayer;
        }

        private void OnDisable()
        {
            _refreshLobbiesListEvent.OnEventRaised -= ListLobbies;
            _createLobbyChannel.OnEventRaised -= CreateLobby;
            _joinByIdEvent.OnEventRaised -= JoinLobbyById;
            _joinByCodeEvent.OnEventRaised -= JoinLobbyByCode;

            _allPlayersReadyEvent.OnEventRaised -= DeleteLobby;

            _toMainMenuScreenEvent.OnEventRaised -= LeaveLobby;
            _leaveGameToMainMenuEvent.OnEventRaised -= LeaveLobby;

            _kickPlayerPlayerIdEvent.OnEventRaised -= KickPlayer;
            _playerIdDisconnectedEvent.OnEventRaised -= KickPlayer;
        }

        private async void Start()
        {
            // to prevent initialize and signin when we are signed
            if (UnityServices.State != ServicesInitializationState.Initialized)
            {
                InitializationOptions initializationOptions = new InitializationOptions();
                initializationOptions.SetProfile(Random.Range(0, 1000).ToString()); // to allow test with multiple builds

                await UnityServices.InitializeAsync(initializationOptions);

                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }

            _playerName = $"Guest{Random.Range(1000, 9999)}";   // It is not unique
            Debug.Log(_playerName);

            ListLobbies();
        }

        private void Update()
        {
            HandleLobbyHeartBeat();
            HandleLobbyPollForUpdates();
        }

        #region HandleLobby
        private async void HandleLobbyHeartBeat()
        {
            if (!IsLobbyHost())
                return;

            _heartbeatTimer -= Time.deltaTime;
            if (_heartbeatTimer < 0f)
            {
                float heartbeatTimerMax = 15f;
                _heartbeatTimer = heartbeatTimerMax;

                await LobbyService.Instance.SendHeartbeatPingAsync(_joinedLobby.Id);
            }
        }

        private async void HandleLobbyPollForUpdates()
        {
            if (!IsLobbyHost())
                return;

            _lobbyUpdateTimer -= Time.deltaTime;
            if (_lobbyUpdateTimer < 0f)
            {
                float lobbyUpdateTimerMax = 1.1f;
                _lobbyUpdateTimer = lobbyUpdateTimerMax;

                Lobby lobby = await LobbyService.Instance.GetLobbyAsync(_joinedLobby.Id);
                _joinedLobby = lobby;

                // TODO: send event to Update UI
            }
        }
        #endregion

        private bool IsLobbyHost()
        {
            return _joinedLobby != null
                && _joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
        }

        private async void ListLobbies()
        {
            try
            {
                // lobbies with al least 1 slot and sorted in created order
                QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions
                {
                    Filters = new List<QueryFilter> {
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT) // con al menos 1 hueco
                    // I can put another filter to compare the data, for example for the GameMode, not necessary in my game
                    //new QueryFilter(QueryFilter.FieldOptions.S1, "SpeedFix", QueryFilter.OpOptions.EQ)
                },
                    Order = new List<QueryOrder> {
                    new QueryOrder (false, QueryOrder.FieldOptions.Created)
                }
                };

                QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync(queryLobbiesOptions);

                // send event
                _lobbiesListedEvent.RaiseEvent(queryResponse.Results);
            }
            catch (LobbyServiceException e)
            {
                ManageLobbyErrors(e);
            }
        }

        private async void CreateLobby(string lobbyName, bool isPrivate)
        {
            try
            {
                int maxPlayers = 4;
                CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions
                {
                    IsPrivate = isPrivate,
                    Player = GetPlayer(),
                };

                Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, createLobbyOptions);

                _hostLobby = lobby;
                _joinedLobby = _hostLobby;

                Debug.Log($"Created lobby! {lobby.Name}, {lobby.MaxPlayers}, {lobby.Id}, {lobby.LobbyCode}");

                // send event
                _lobbyCreatedEvent.RaiseEvent();
            }
            catch (LobbyServiceException e)
            {
                ManageLobbyErrors(e);
            }
        }

        private async void JoinLobbyById(string lobbyId)
        {
            try
            {
                JoinLobbyByIdOptions joinLobbyByIdOptions = new JoinLobbyByIdOptions
                {
                    Player = GetPlayer()
                };

                Lobby lobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobbyId, joinLobbyByIdOptions);
                _joinedLobby = lobby;

                Debug.Log($"Joined Lobby with id: {lobbyId}");

                // send event
                _lobbyJoinedEvent.RaiseEvent();
            }
            catch (LobbyServiceException e)
            {
                ManageLobbyErrors(e);
            }
        }

        private async void JoinLobbyByCode(string lobbyCode)
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

                // send event
                _lobbyJoinedEvent.RaiseEvent();
            }
            catch (LobbyServiceException e)
            {
                ManageLobbyErrors(e);
            }
        }

        private async void DeleteLobby()
        {
            if (!IsLobbyHost())
                return;

            try {
                await LobbyService.Instance.DeleteLobbyAsync(_joinedLobby.Id);

                _joinedLobby = null;
            }
            catch (LobbyServiceException e) {
                ManageLobbyErrors(e);
            }
        }

        private async void LeaveLobby()
        {
            if (_joinedLobby == null)
                return;

            try {
                //if (IsLobbyHost()) {
                //    DeleteLobby();
                //}

                await LobbyService.Instance.RemovePlayerAsync(_joinedLobby.Id, AuthenticationService.Instance.PlayerId);

                _joinedLobby = null;
            }
            catch (LobbyServiceException e) {
                ManageLobbyErrors(e);
            }
        }

        private async void KickPlayer(string playerId)
        {
            if (!IsLobbyHost())
                return;

            try {
                await LobbyService.Instance.RemovePlayerAsync(_joinedLobby.Id, playerId);
            }
            catch (LobbyServiceException e) {
                ManageLobbyErrors(e);
            }
        }

        #region PlayerLobby
        private Player GetPlayer()
        {
            return new Player
            {
                Data = new Dictionary<string, PlayerDataObject> {
                {PLAYER_NAME, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, _playerName) }
            }
            };
        }
        #endregion

        private void ManageLobbyErrors(LobbyServiceException e)
        {
            Debug.LogException(e);

            string userErrorMsg = e.Message;
            userErrorMsg = userErrorMsg[0].ToString().ToUpper() + userErrorMsg.Substring(1);

            _lobbyErrorCatchedEvent.RaiseEvent(userErrorMsg);
        }
    }
}