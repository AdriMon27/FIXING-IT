using TMPro;
using UnityEngine;

namespace FixingIt.UI.Minigame
{
    public class UIEndGamePanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _endGameText;

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
