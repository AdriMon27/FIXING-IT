using FixingIt.Counters;
using FixingIt.Customer;
using FixingIt.Events;
using FixingIt.Funcs;
using FixingIt.InputSystem;
using FixingIt.RoomObjects;
using ProgramadorCastellano.Events;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FixingIt.Minigame
{
    public class FixingGameManager : NetworkBehaviour
    {
        private enum GameState
        {
            WaitingToStart,
            Playing,
            End
        }

        [SerializeField] InputReaderSO _inputReaderSO;

        [SerializeField] ToolRecipeManagerSO _levelToolRecipeManagerSO;
        [SerializeField] Transform _baseTransformToSpawn;

        private float _waitingToStartTimer;
        private float _gameplayTimer;
        private float _customerSpawnerTimer;
        //private GameState _gameState;
        private NetworkVariable<GameState> _gameState = new NetworkVariable<GameState>(GameState.WaitingToStart);

        private NetworkVariable<int> _numberObjectsFixed = new NetworkVariable<int>(0);

        [Header("Player")]
        [SerializeField] GameObject _playerPrefab;
        [SerializeField] Transform[] _playerSpawnPositions;

        [Header("Timers")]
        [SerializeField] private float _waitingToStartTimerMax = 5f;
        [SerializeField] private float _gameplayTimerMax = 60f;
        [SerializeField] private float _customerSpawnerTimerMax = 10f;

        [Header("Customers")]
        [SerializeField] GameObject _customerPrefab;
        [SerializeField] Transform _customerStartPosition;

        [Header("Customer Counters")]
        [SerializeField]
        private CustomerCounter[] _customerCounters;
        [SerializeField]
        private RoomObjectSO[] _objectsToFixSO;
        public int TestIndex;

        [Header("Broadcasting To")]
        [SerializeField]
        private FloatEventChannelSO _waitingToStartTimerEvent;
        [SerializeField]
        private FloatEventChannelSO _gameplayTimerNormalizedEvent;
        [SerializeField]
        private IntEventChannelSO _numberObjectsFixedEvent;

        [Header("Listening To")]
        [SerializeField]
        private VoidEventChannelSO _inMenuEvent;
        [SerializeField]
        private VoidEventChannelSO _outMenuEvent;
        [SerializeField]
        private RoomObjectParentChannelSO _customerWithObjectFixedEvent;

        [Header("Setting Func")]
        [SerializeField]
        private ToolRecipeManagerFuncSO _getLevelToolRecipeManagerSOFunc;

        private void Awake()
        {
            _getLevelToolRecipeManagerSOFunc.ClearOnFuncRaised();
            _getLevelToolRecipeManagerSOFunc.TrySetOnFuncRaised(() => _levelToolRecipeManagerSO);

            _waitingToStartTimer = _waitingToStartTimerMax;
            _gameplayTimer = _gameplayTimerMax;
        }

        private void OnEnable()
        {
            _inMenuEvent.OnEventRaised += ToMenuMode;
            _outMenuEvent.OnEventRaised += ToGameplayMode;

            _customerWithObjectFixedEvent.OnEventRaised += ObjectFixedAndReturned;
        }

        private void OnDisable()
        {
            _inMenuEvent.OnEventRaised -= ToMenuMode;
            _outMenuEvent.OnEventRaised -= ToGameplayMode;

            _customerWithObjectFixedEvent.OnEventRaised += ObjectFixedAndReturned;
        }

        private void Start()
        {
            //_gameState = GameState.WaitingToStart;
            _inputReaderSO.DisableAllInput();
        }

        public override void OnNetworkSpawn()
        {
            _gameState.OnValueChanged += State_OnValueChanged;

            if (IsServer) {
                NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += NM_SM_OnLoadEventCompleted;
            }
        }

        private void State_OnValueChanged(GameState previousValue, GameState newValue)
        {
            switch (newValue)
            {
                case GameState.WaitingToStart:
                    _inputReaderSO.DisableAllInput();
                    break;
                case GameState.Playing:
                    _inputReaderSO.EnableGameplayInput();
                    break;
                case GameState.End:
                    _inputReaderSO.EnableMenuInput();
                    _numberObjectsFixedEvent.RaiseEvent(_numberObjectsFixed.Value);
                    break;
                default:
                    break;
            }
        }

        private void NM_SM_OnLoadEventCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
        {
            for (int i = 0; i < NetworkManager.Singleton.ConnectedClientsIds.Count; i++) {
                ulong clientId = NetworkManager.Singleton.ConnectedClientsIds[i];

                GameObject playerGO = Instantiate(_playerPrefab, _baseTransformToSpawn);
                playerGO.transform.position = _playerSpawnPositions[i].position;
                playerGO.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
                Debug.Log(playerGO.GetComponent<NetworkObject>().OwnerClientId);
                Debug.Log(NetworkManager.Singleton.LocalClientId);
            }
        }

        private void Update()
        {
            if (!IsServer) {
                return;
            }

            switch (_gameState.Value) {
                case GameState.WaitingToStart:
                    // esperar countdown to start
                    _waitingToStartTimer -= Time.deltaTime;
                    if (_waitingToStartTimer < 0f) {
                        _gameState.Value = GameState.Playing;
                        //_inputReaderSO.EnableGameplayInput();
                    }

                    WaitingToStartRaiseEventClientRpc(_waitingToStartTimer);
                    //_waitingToStartTimerEvent.RaiseEvent(_waitingToStartTimer);
                    break;
                case GameState.Playing:
                    // timer juego
                    _gameplayTimer -= Time.deltaTime;
                    if (_gameplayTimer < 0f) {
                        _gameState.Value = GameState.End;
                        //_inputReaderSO.EnableMenuInput();

                        //_numberObjectsFixedEvent.RaiseEvent(_numberObjectsFixed);
                    }

                    // timer npcs
                    _customerSpawnerTimer -= Time.deltaTime;
                    if (_customerSpawnerTimer < 0f) {
                        _customerSpawnerTimer = _customerSpawnerTimerMax;

                        SpawnNewCustomer();
                    }

                    float gameplayTimerNormalized = GetTimerNormalized(_gameplayTimer, _gameplayTimerMax);
                    GameplayTimerNormalizedRaiseEventClientRpc(gameplayTimerNormalized);
                    //_gameplayTimerNormalizedEvent.RaiseEvent(GetTimerNormalized(_gameplayTimer, _gameplayTimerMax));
                    break;
                case GameState.End:
                    // mostrar puntuacion
                    Debug.Log(_numberObjectsFixed);
                    break;
                default:
                    Debug.LogWarning($"{_gameState} is not implemented");
                    break;
            }

            
        }

        [ClientRpc]
        private void WaitingToStartRaiseEventClientRpc(float waitingToStartTimer)
        {
            _waitingToStartTimerEvent.RaiseEvent(waitingToStartTimer);
        }

        [ClientRpc]
        private void GameplayTimerNormalizedRaiseEventClientRpc(float gameplayTimerNormalized)
        {
            _gameplayTimerNormalizedEvent.RaiseEvent(gameplayTimerNormalized);
        }

        private float GetTimerNormalized(float timer, float timerMax)
        {
            return timer / timerMax;
        }

        #region GameplayMode
        private void ToMenuMode()
        {
            _inputReaderSO.EnableMenuInput();
        }

        private void ToGameplayMode()
        {
            _inputReaderSO.EnableGameplayInput();
        }
        #endregion

        #region Game Loop
        private CustomerCounter GetFirstCustomerCounterFree()
        {
            foreach (CustomerCounter counter in _customerCounters) {
                if (!counter.HasCustomerAssigned()) {
                    return counter;
                }
            }

            return null;
        }

        private RoomObjectSO GetRandomObjecToFixSO()
        {
            int randIndex = Random.Range(0, _objectsToFixSO.Length);

            return _objectsToFixSO[randIndex];
        }

        private void SpawnNewCustomer()
        {
            CustomerCounter freeCounter = GetFirstCustomerCounterFree();
            if (freeCounter == null) {
                return;
            }

            GameObject customerGO = Instantiate(_customerPrefab, _customerStartPosition.position, Quaternion.identity);

            CustomerController customerController = customerGO.GetComponent<CustomerController>();

            if (customerController == null) {
                Debug.LogError($"The prefab {_customerPrefab} is not a Customer Controller");
                return;
            }

            customerController.InitCustomer(_customerStartPosition, freeCounter, GetRandomObjecToFixSO());
            freeCounter.SetCustomerAssigned(customerController);
        }

        private void ObjectFixedAndReturned(IRoomObjectParent customerWithObject)
        {
            _numberObjectsFixed.Value++;
        }
        #endregion
    }
}
