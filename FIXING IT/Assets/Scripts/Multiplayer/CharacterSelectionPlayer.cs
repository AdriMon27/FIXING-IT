using ProgramadorCastellano.MyEvents;
using ProgramadorCastellano.MyFuncs;
using UnityEngine;

namespace FixingIt.Multiplayer
{
    public class CharacterSelectionPlayer : MonoBehaviour
    {
        [SerializeField] private int _playerIndex;

        [Header("Listening To")]
        [SerializeField]
        private VoidEventChannelSO _playerDataNetworkListChangedEvent;

        [Header("Invoking Func")]
        [SerializeField]
        private IntBoolFuncSO _isPlayerIndexConnected;

        private void Start()
        {
            _playerDataNetworkListChangedEvent.OnEventRaised += UpdateCSPlayer;

            UpdateCSPlayer();
        }

        private void OnDestroy()
        {
            _playerDataNetworkListChangedEvent.OnEventRaised -= UpdateCSPlayer;
        }

        private void UpdateCSPlayer()
        {
            if (_isPlayerIndexConnected.RaiseFunc(_playerIndex))
            {
                Show();
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