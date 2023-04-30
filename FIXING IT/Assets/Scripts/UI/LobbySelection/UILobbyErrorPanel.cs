using ProgramadorCastellano.MyEvents;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FixingIt.UI.LobbySelection
{
    public class UILobbyErrorPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _tmpError;
        [SerializeField] private Button _acceptErrorButton;

        [Header("Broadcasting To")]
        [SerializeField]
        private VoidEventChannelSO _acceptedLobbbyErrorEvent;

        public GameObject FirstSelected => _acceptErrorButton.gameObject;

        private void Start()
        {
            _acceptErrorButton.onClick.AddListener(AcceptError);
        }

        private void AcceptError()
        {
            _acceptedLobbbyErrorEvent.RaiseEvent();
        }

        public void SetErrorMsg(string errorMsg)
        {
            _tmpError.text = errorMsg;
        }
    }
}