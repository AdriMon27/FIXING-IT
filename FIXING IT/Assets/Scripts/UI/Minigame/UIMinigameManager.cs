using FixingIt.InputSystem;
using ProgramadorCastellano.Events;
using UnityEngine;

namespace FixingIt.UI.Minigame
{
    public class UIMinigameManager : MonoBehaviour
    {
        [SerializeField] InputReaderSO _inputReaderSO;

        [Header("Panels")]
        [SerializeField] private UIManualPanel _toolRecipesPanel;

        [Header("Broadcasting To")]
        [SerializeField]
        private VoidEventChannelSO _inMenuEvent;
        [SerializeField]
        private VoidEventChannelSO _outMenuEvent;

        [Header("Listening To")]
        [SerializeField]
        private VoidEventChannelSO _manualCounterUsedEvent;

        private void OnEnable()
        {
            _inputReaderSO.MenuCancelEvent += GoBack;

            _manualCounterUsedEvent.OnEventRaised += ShowManualPanel;
        }

        private void OnDisable()
        {
            _inputReaderSO.MenuCancelEvent -= GoBack;

            _manualCounterUsedEvent.OnEventRaised -= ShowManualPanel;
        }

        private void Start()
        {
            _toolRecipesPanel.Hide();
        }

        private void GoBack()
        {
            // hide all panels
            _toolRecipesPanel.Hide();

            // send event
            _outMenuEvent.RaiseEvent();
        }

        private void ShowManualPanel()
        {
            _toolRecipesPanel.Show();
            _inMenuEvent.RaiseEvent();
        }
    }
}
