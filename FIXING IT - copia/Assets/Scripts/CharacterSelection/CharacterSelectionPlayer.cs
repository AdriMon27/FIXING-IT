using FixingIt.Funcs;
using FixingIt.PlayerGame;
using ProgramadorCastellano.Events;
using ProgramadorCastellano.Funcs;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace FixingIt.CharacterSelection
{
    public class CharacterSelectionPlayer : MonoBehaviour
    {
        [SerializeField] private int _playerIndex;
        [SerializeField] private GameObject _readyGameObject;
        [SerializeField] private TextMeshPro _playerNameText;
        [SerializeField] private PlayerVisualComp _playerVisualComp;
        [SerializeField] private Button _kickButton;

        [Header("Broadcasting To")]
        [SerializeField]
        private ULongEventChannelSO _kickPlayerClientIdEvent;
        [SerializeField]
        private StringEventChannelSO _kickPlayerPlayerIdEvent;

        [Header("Listening To")]
        [SerializeField]
        private VoidEventChannelSO _playerDataNetworkListChangedEvent;
        [SerializeField]
        private VoidEventChannelSO _clientReadyChangedEvent;

        [Header("Invoking Func")]
        [SerializeField]
        private IntBoolFuncSO _isPlayerIndexConnectedFunc;
        [SerializeField]
        private ULongBoolFuncSO _isPlayerReadyFunc;
        [SerializeField]
        private IntPlayerdataFuncSO _getPlayerDataFromPlayerIndexFunc;
        [SerializeField]
        private IntColorFuncSO _getPlayerColorFunc;
        [SerializeField]
        private StringFuncSO _getPlayerNameFunc;

        private void Awake()
        {
            _kickButton.onClick.AddListener(KickPlayer);
        }

        private void Start()
        {
            _playerDataNetworkListChangedEvent.OnEventRaised += UpdateCSPlayer;
            _clientReadyChangedEvent.OnEventRaised += UpdateCSPlayer;

            // server is 0
            _kickButton.gameObject.SetActive(NetworkManager.Singleton.IsServer && _playerIndex != 0);

            UpdateCSPlayer();
        }

        private void OnDestroy()
        {
            _playerDataNetworkListChangedEvent.OnEventRaised -= UpdateCSPlayer;
            _clientReadyChangedEvent.OnEventRaised -= UpdateCSPlayer;
        }

        private void UpdateCSPlayer()
        {
            if (_isPlayerIndexConnectedFunc.RaiseFunc(_playerIndex))
            {
                Show();

                // show ready
                PlayerData playerData = _getPlayerDataFromPlayerIndexFunc.RaiseFunc(_playerIndex);
                bool isReady = _isPlayerReadyFunc.RaiseFunc(playerData.ClientId);
                _readyGameObject.SetActive(isReady);

                // show name
                _playerNameText.text = playerData.PlayerName.ToString();

                // show color
                Color playerColor = _getPlayerColorFunc.RaiseFunc(playerData.ColorId);
                _playerVisualComp.SetPlayerColor(playerColor);
            }
            else
            {
                Hide();
            }
        }

        private void KickPlayer()
        {
            PlayerData playerData = _getPlayerDataFromPlayerIndexFunc.RaiseFunc(_playerIndex);
            //_kickPlayerPlayerIdEvent.RaiseEvent(playerData.PlayerId.ToString());
            _kickPlayerClientIdEvent.RaiseEvent(playerData.ClientId);
        }

        private void Show()
        {
            gameObject.SetActive(true);
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}