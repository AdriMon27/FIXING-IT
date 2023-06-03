using FixingIt.Counters;
using FixingIt.Customer;
using FixingIt.Funcs;
using FixingIt.InputSystem;
using FixingIt.RoomObjects;
using ProgramadorCastellano.Events;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace FixingIt.Minigame
{
    public class FixingGameManager : MonoBehaviour
    {
        private enum GameState
        {
            WaitingToStart,
            Playing,
            End
        }

        [SerializeField] InputReaderSO _inputReaderSO;

        [SerializeField] ToolRecipeManagerSO _levelToolRecipeManagerSO;

        private float _waitingToStartTimer;
        private float _gameplayTimer;
        private float _customerSpawnerTimer;
        private GameState _gameState;

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

        [Header("Listening To")]
        [SerializeField]
        private VoidEventChannelSO _inMenuEvent;
        [SerializeField]
        private VoidEventChannelSO _outMenuEvent;

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
        }

        private void OnDisable()
        {
            _inMenuEvent.OnEventRaised -= ToMenuMode;
            _outMenuEvent.OnEventRaised -= ToGameplayMode;
        }

        private void Start()
        {
            _gameState = GameState.WaitingToStart;
            _inputReaderSO.DisableAllInput();
        }

        private void Update()
        {
            switch (_gameState) {
                case GameState.WaitingToStart:
                    // esperar countdown to start
                    _waitingToStartTimer -= Time.deltaTime;
                    if (_waitingToStartTimer < 0f) {
                        _gameState = GameState.Playing;
                        _inputReaderSO.EnableGameplayInput();
                    }

                    _waitingToStartTimerEvent.RaiseEvent(_waitingToStartTimer);
                    break;
                case GameState.Playing:
                    // timer juego
                    _gameplayTimer -= Time.deltaTime;
                    if (_gameplayTimer < 0f) {
                        _gameState = GameState.End;
                        _inputReaderSO.DisableAllInput();
                    }

                    // timer npcs
                    _customerSpawnerTimer -= Time.deltaTime;
                    if (_customerSpawnerTimer < 0f) {
                        _customerSpawnerTimer = _customerSpawnerTimerMax;

                        SpawnNewCustomer();
                    }

                    _gameplayTimerNormalizedEvent.RaiseEvent(GetTimerNormalized(_gameplayTimer, _gameplayTimerMax));
                    break;
                case GameState.End:
                    // mostrar puntuacion
                    break;
                default:
                    Debug.LogWarning($"{_gameState} is not implemented");
                    break;
            }

            
        }

        private float GetTimerNormalized(float timer, float timerMax)
        {
            return timer / timerMax;
        }

        private void ToMenuMode()
        {
            _inputReaderSO.EnableMenuInput();
        }

        private void ToGameplayMode()
        {
            _inputReaderSO.EnableGameplayInput();
        }

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
    }
}
