using ProgramadorCastellano.Events;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FixingIt.UI.Minigame
{
    public class UIEndGamePanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _endGameText;
        [SerializeField] private Button _toMainMenuBtn;

        [Header("Broadcasting To")]
        [SerializeField]
        private VoidEventChannelSO _leaveGameToMainMenuEvent;

        private void Start()
        {
            _toMainMenuBtn.onClick.AddListener(() => _leaveGameToMainMenuEvent.RaiseEvent());
        }

        public void Show(int numberOfObjectsFixed)
        {
            gameObject.SetActive(true);
            _endGameText.text = $"You have fixed and returned {numberOfObjectsFixed} objects!";
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
