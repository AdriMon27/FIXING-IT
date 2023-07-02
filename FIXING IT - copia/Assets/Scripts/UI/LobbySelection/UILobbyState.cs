using TMPro;
using UnityEngine;

namespace FixingIt.UI.LobbySelection
{
    public class UILobbyState : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _stateMessageText;

        public void SetStateMessage(string stateMsg)
        {
            _stateMessageText.text = stateMsg;
        }
    }
}
