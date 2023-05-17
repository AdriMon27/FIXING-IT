using ProgramadorCastellano.Events;
using UnityEngine;

namespace FixingIt.UI.Minigame
{
    public class UIMinigameManager : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private UIManualPanel _toolRecipesPanel;

        [Header("Broadcasting To")]
        [SerializeField]
        private VoidEventChannelSO _inMenuEvent;

        [Header("Listening To")]
        [SerializeField]
        private VoidEventChannelSO _manualCounterUsedEvent;

        private void OnEnable()
        {
            _manualCounterUsedEvent.OnEventRaised += ShowManualPanel;
        }

        private void OnDisable()
        {
            _manualCounterUsedEvent.OnEventRaised -= ShowManualPanel;
        }

        private void Start()
        {
            _toolRecipesPanel.Hide();
        }

        private void ShowManualPanel()
        {
            _toolRecipesPanel.Show();
            _inMenuEvent.RaiseEvent();
        }
    }
}
