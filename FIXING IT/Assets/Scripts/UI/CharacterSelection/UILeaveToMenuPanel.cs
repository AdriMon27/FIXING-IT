using ProgramadorCastellano.MyEvents;
using UnityEngine;
using UnityEngine.UI;

namespace FixingIt.UI.CharacterSelection
{
    public class UILeaveToMenuPanel : MonoBehaviour
    {
        [SerializeField] private Button _leaveButton;
        [SerializeField] private Button _cancelButton;

        public GameObject FirstSelected => _cancelButton.gameObject;

        [Header("Broadcasting To")]
        [SerializeField]
        private VoidEventChannelSO _leaveToMainMenuEvent;
        [SerializeField]
        private VoidEventChannelSO _cancelLeaveToMainMenuEvent;

        private void Start()
        {
            _leaveButton.onClick.AddListener(() => _leaveToMainMenuEvent.RaiseEvent());
            _cancelButton.onClick.AddListener(() => _cancelLeaveToMainMenuEvent.RaiseEvent());
        }
    }
}
