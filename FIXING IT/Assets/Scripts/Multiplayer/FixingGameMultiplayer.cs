using FixingIt.SceneManagement;
using ProgramadorCastellano.MyEvents;
using ProgramadorCastellano.MyFuncs;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FixingIt.Multiplayer
{
    public class FixingGameMultiplayer : NetworkBehaviour
    {
        public static FixingGameMultiplayer Instance { get; private set; }

        [SerializeField] private GameSceneSO _characterSelectionSceneSO;

        private NetworkList<PlayerData> _playerDataNetworkList;
        //private NetworkVariable<int> _testVariable;

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
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }

            _playerDataNetworkList = new NetworkList<PlayerData>(readPerm: NetworkVariableReadPermission.Everyone);
            _playerDataNetworkList.OnListChanged += OnPlayerDataNetworkListChanged;
            //_testVariable = new NetworkVariable<int>(0);

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

            //_testVariable.Value = 1;

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
            //return playerIndex < _testVariable.Value;
        }

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
            });
            //_testVariable.Value++;
        }


        private void NetworkManager_OnClientDisconnectCallback(ulong obj)
        {
            _rejectedToServerEvent.RaiseEvent(NetworkManager.Singleton.DisconnectReason);
        }
        #endregion
    }
}