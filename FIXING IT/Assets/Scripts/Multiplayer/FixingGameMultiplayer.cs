using FixingIt.Funcs;
using FixingIt.PlayerGame;
using FixingIt.SceneManagement.ScriptableObjects;
using ProgramadorCastellano.Events;
using ProgramadorCastellano.Funcs;
using System.Linq;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FixingIt.Multiplayer
{
    public class FixingGameMultiplayer : NetworkBehaviour
    {
        private const string PLAYER_PREFS_PLAYER_NAME_MULTIPLAYER = "PlayerNameMultiplayer";

        public static FixingGameMultiplayer Instance { get; private set; }

        [SerializeField] private GameSceneSO _characterSelectionSceneSO;
        [SerializeField] private Color[] _playerColorArray;

        private NetworkList<PlayerData> _playerDataNetworkList;
        private string _playerName;

        [Header("Broadcasting To")]
        [SerializeField]
        private VoidEventChannelSO _hostStartedEvent;
        [SerializeField]
        private StringEventChannelSO _rejectedToServerEvent;
        [SerializeField]
        private VoidEventChannelSO _playerDataNetworkListChangedEvent;
        [SerializeField]
        private VoidEventChannelSO _networkToMainMenuEvent;
        [SerializeField]
        private StringEventChannelSO _playerIdDisconnectedEvent;

        [Header("Listening To")]
        [SerializeField]
        private VoidEventChannelSO _lobbyCreatedEvent;
        [SerializeField]
        private VoidEventChannelSO _lobbyJoinedEvent;
        [SerializeField]
        private IntEventChannelSO _changePlayerColorId;
        [SerializeField]
        private VoidEventChannelSO _leaveGameToMainMenuEvent;
        [SerializeField]
        private ULongEventChannelSO _kickPlayerEvent;
        [SerializeField]
        private StringEventChannelSO _setPlayerNameEvent;

        [Header("Invoking Func")]
        [SerializeField]
        private StringFuncSO _getCurrentSceneNameFunc;

        [Header("Setting Func")]
        [SerializeField]
        private IntBoolFuncSO _isPlayerIndexConnected;
        [SerializeField]
        private IntPlayerdataFuncSO _getPlayerDataFromPlayerIndex;
        [SerializeField]
        private IntColorFuncSO _getPlayerColorFunc;
        [SerializeField]
        private PlayerdataFuncSO _getClientPlayerData;
        [SerializeField]
        private StringFuncSO _getPlayerNameFunc;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            // si sobra tiempo, refactorizar a un FuncSO y que el sistema de guardado sea externo
            _playerName = PlayerPrefs.GetString(PLAYER_PREFS_PLAYER_NAME_MULTIPLAYER, $"PlayerName{Random.Range(100, 1000)}");

            _playerDataNetworkList = new NetworkList<PlayerData>(readPerm: NetworkVariableReadPermission.Everyone);
            _playerDataNetworkList.OnListChanged += OnPlayerDataNetworkListChanged;

            // clear funcs just in case
            _isPlayerIndexConnected.ClearOnFuncRaised();
            _getPlayerDataFromPlayerIndex.ClearOnFuncRaised();
            _getPlayerColorFunc.ClearOnFuncRaised();
            _getClientPlayerData.ClearOnFuncRaised();
            _getPlayerNameFunc.ClearOnFuncRaised();

            // set funcs
            _isPlayerIndexConnected.TrySetOnFuncRaised(IsPlayerIndexConnected);
            _getPlayerDataFromPlayerIndex.TrySetOnFuncRaised(GetPlayerDataFromPlayerIndex);
            _getPlayerColorFunc.TrySetOnFuncRaised(GetPlayerColor);
            _getClientPlayerData.TrySetOnFuncRaised(GetPlayerData);
            _getPlayerNameFunc.TrySetOnFuncRaised(GetPlayerName);
        }

        private void OnEnable()
        {
            _lobbyCreatedEvent.OnEventRaised += StartHost;
            _lobbyJoinedEvent.OnEventRaised += StartClient;

            _changePlayerColorId.OnEventRaised += ChangePlayerColor;

            _leaveGameToMainMenuEvent.OnEventRaised += LeaveToMainMenu;

            _kickPlayerEvent.OnEventRaised += KickPlayer;

            _setPlayerNameEvent.OnEventRaised += SetPlayerName;
        }

        private void OnDisable()
        {
            _lobbyCreatedEvent.OnEventRaised -= StartHost;
            _lobbyJoinedEvent.OnEventRaised -= StartClient;

            _changePlayerColorId.OnEventRaised -= ChangePlayerColor;

            _leaveGameToMainMenuEvent.OnEventRaised -= LeaveToMainMenu;

            _kickPlayerEvent.OnEventRaised += KickPlayer;

            _setPlayerNameEvent.OnEventRaised -= SetPlayerName;

            //_playerDataNetworkList.Dispose();
        }

        private void StartHost()
        {
            NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
            NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Server_OnClientDisconnectCallback;
            NetworkManager.Singleton.StartHost();

            // send event
            _hostStartedEvent.RaiseEvent();
        }

        private void StartClient()
        {
            NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_Client_OnClientConnectedCallback;
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Client_OnClientDisconnectCallback;
            NetworkManager.Singleton.StartClient();
        }

        private void OnPlayerDataNetworkListChanged(NetworkListEvent<PlayerData> changeEvent)
        {
            _playerDataNetworkListChangedEvent.RaiseEvent();
        }

        private void LeaveToMainMenu()
        {
            NetworkManager.Singleton.Shutdown();

            // send event
            _networkToMainMenuEvent.RaiseEvent();
        }

        private void KickPlayer(ulong clientId)
        {
            NetworkManager.Singleton.DisconnectClient(clientId);
            NetworkManager_Server_OnClientDisconnectCallback(clientId); 
        }

        #region Player Info
        private bool IsPlayerIndexConnected(int playerIndex)
        {
            //if (!IsServer)
            //return false;

            return playerIndex < _playerDataNetworkList.Count;
        }

        private PlayerData GetPlayerDataFromPlayerIndex(int playerIndex)
        {
            return _playerDataNetworkList[playerIndex];
        }

        private PlayerData GetPlayerData()
        {
            return GetPlayerDataFromClientId(NetworkManager.Singleton.LocalClientId);
        }

        private int GetPlayerDataIndexFromClientId(ulong clientId)
        {
            for (int i = 0; i < _playerDataNetworkList.Count; i++) {
                if (_playerDataNetworkList[i].ClientId == clientId) {
                    return i;
                }
            }

            return -1;
        }

        private PlayerData GetPlayerDataFromClientId(ulong clientId)
        {
            foreach (PlayerData playerData in _playerDataNetworkList) {
                if (playerData.ClientId == clientId) {
                    return playerData;
                }
            }

            return default;
        }

        private Color GetPlayerColor(int colorId)
        {
            return _playerColorArray[colorId];
        }

        private void ChangePlayerColor(int colorId)
        {
            ChangePlayerColorServerRpc(colorId);
        }

        [ServerRpc(RequireOwnership = false)]
        private void ChangePlayerColorServerRpc(int colorId, ServerRpcParams serverRpcParams = default)
        {
            if (!IsColorAvailable(colorId)) {
                return;
            }

            // get playerdata struct. We cannot modify directly the clientId in a NetworkList
            int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);
            PlayerData playerData = _playerDataNetworkList[playerDataIndex];

            // set playerdata struct
            playerData.ColorId = colorId;
            _playerDataNetworkList[playerDataIndex] = playerData;
        }

        private string GetPlayerName()
        {
            return _playerName;
        }

        private void SetPlayerName(string playerName)
        {
            // no dejar nombres vacios
            if (playerName == string.Empty)
                return;

            _playerName = playerName;
            // si sobra tiempo, refactorizar a un FuncSO y que el sistema de guardado sea externo
            PlayerPrefs.SetString(PLAYER_PREFS_PLAYER_NAME_MULTIPLAYER, playerName);
        }

        [ServerRpc(RequireOwnership = false)]
        private void SetPlayerNameServerRpc(string playerName, ServerRpcParams serverRpcParams = default)
        {
            // get playerdata struct. We cannot modify directly the clientId in a NetworkList
            int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);
            PlayerData playerData = _playerDataNetworkList[playerDataIndex];

            // set playerdata struct
            playerData.PlayerName = playerName;
            _playerDataNetworkList[playerDataIndex] = playerData;
        }

        [ServerRpc(RequireOwnership = false)]
        private void SetPlayerIdServerRpc(string playerId, ServerRpcParams serverRpcParams = default)
        {
            // get playerdata struct. We cannot modify directly the clientId in a NetworkList
            int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);
            PlayerData playerData = _playerDataNetworkList[playerDataIndex];

            // set playerdata struct
            playerData.PlayerId = playerId;
            _playerDataNetworkList[playerDataIndex] = playerData;
        }
        #endregion

        #region Color
        private bool IsColorAvailable(int colorId)
        {
            foreach (PlayerData playerData in _playerDataNetworkList) {
                if (playerData.ColorId == colorId) {
                    // already in use
                    return false;
                }
            }

            return true;
        }

        private int GetFirstUnusedColorId()
        {
            for (int i = 0; i < _playerColorArray.Length; i++) {
                if (IsColorAvailable(i)) {
                    return i;
                }
            }

            return -1;
        }
        #endregion

        #region NetworkCallbacks
        private void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest connectionApprovalRequest,
            NetworkManager.ConnectionApprovalResponse connectionApprovalResponse)
        {
            string rejectedReason = string.Empty;

            // server no está en characerselection
            int numberOfActiveScenes = SceneManager.sceneCount;
            string[] activeSceneNames = new string[numberOfActiveScenes];
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                activeSceneNames[i] = SceneManager.GetSceneAt(i).name;
            }

            if (!activeSceneNames.Contains(_characterSelectionSceneSO.name))
            {
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
            _playerDataNetworkList.Add(new PlayerData()
            {
                ClientId = clientId,
                ColorId = GetFirstUnusedColorId(),
                //PlayerName = _playerName,
            });
            SetPlayerNameServerRpc(GetPlayerName());
        }

        private void NetworkManager_Server_OnClientDisconnectCallback(ulong clientId)
        {
            for (int i = 0; i < _playerDataNetworkList.Count; i++) {
                PlayerData playerData = _playerDataNetworkList[i];
                if (playerData.ClientId == clientId) {
                    // Disconnected
                    _playerDataNetworkList.RemoveAt(i);

                    string playerId = playerData.PlayerId.ToString();
                    _playerIdDisconnectedEvent.RaiseEvent(playerId);
                }
            }
        }

        private void NetworkManager_Client_OnClientConnectedCallback(ulong clientId)
        {
            SetPlayerNameServerRpc(GetPlayerName());
            SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);
        }

        private void NetworkManager_Client_OnClientDisconnectCallback(ulong obj)
        {
            _rejectedToServerEvent.RaiseEvent(NetworkManager.Singleton.DisconnectReason);
        }
        #endregion
    }
}