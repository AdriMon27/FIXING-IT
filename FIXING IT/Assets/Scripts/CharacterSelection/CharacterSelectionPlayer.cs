using ProgramadorCastellano.MyEvents;
using ProgramadorCastellano.MyFuncs;
using UnityEngine;

namespace FixingIt.CharacterSelection
{
    public class CharacterSelectionPlayer : MonoBehaviour
    {
        [SerializeField] private int _playerIndex;
        [SerializeField] private GameObject _readyGameObject;

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

                //bool isReady = _isPlayerReadyFunc.RaiseFunc(_playerIndex);
                //_readyGameObject.SetActive(isReady);
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