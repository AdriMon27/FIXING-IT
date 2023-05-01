using ProgramadorCastellano.Events;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FixingIt.UI.CharacterSelection
{
    public class UIKickedPanel : MonoBehaviour
    {
        [SerializeField] Button _acceptButton;

        [Header("Listening To")]
        [SerializeField]
        private StringEventChannelSO _rejectedToServerEvent;

        [Header("Broadcasting To")]
        [SerializeField]
        private VoidEventChannelSO _leaveGameToMainMenuEvent;

        private void Start()
        {
            _rejectedToServerEvent.OnEventRaised += Show;

            _acceptButton.onClick.AddListener(() => _leaveGameToMainMenuEvent.RaiseEvent());

            Hide();
        }

        private void OnDestroy()
        {
            _rejectedToServerEvent.OnEventRaised -= Show;
        }

        private void Show(string rejectedReason)
        {
            EventSystem.current.SetSelectedGameObject(_acceptButton.gameObject);
            gameObject.SetActive(true);
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
