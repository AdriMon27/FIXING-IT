using FixingIt.Funcs;
using ProgramadorCastellano.MyEvents;
using ProgramadorCastellano.MyFuncs;
using UnityEngine;

namespace FixingIt.CharacterSelection
{
    public class CharacterSelectionPlayer : MonoBehaviour
    {
        [SerializeField] private int _playerIndex;
        [SerializeField] private GameObject _readyGameObject;
        [SerializeField] private PlayerVisualComponent _playerVisualComp;

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

        private void Start()
        {
            _playerDataNetworkListChangedEvent.OnEventRaised += UpdateCSPlayer;
            _clientReadyChangedEvent.OnEventRaised += UpdateCSPlayer;

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

                // show color
                Color playerColor = _getPlayerColorFunc.RaiseFunc(playerData.ColorId);
                _playerVisualComp.SetPlayerColor(playerColor);
            }
            else
            {
                Hide();
            }
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