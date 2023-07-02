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
        [SerializeField] private UIEndGamePanel _endGamePanel;

        [Header("Broadcasting To")]
        [SerializeField]
        private VoidEventChannelSO _inMenuEvent;
        [SerializeField]
        private VoidEventChannelSO _outMenuEvent;

        [Header("Listening To")]
        [SerializeField]
        private VoidEventChannelSO _manualCounterUsedEvent;
        [SerializeField]
        private IntEventChannelSO _numberObjectsFixedEvent;

        private void OnEnable()
        {
            _inputReaderSO.MenuCancelEvent += GoBack;

            _manualCounterUsedEvent.OnEventRaised += ShowManualPanel;

            _numberObjectsFixedEvent.OnEventRaised += ShowEndGamePanel;
        }

        private void OnDisable()
        {
            _inputReaderSO.MenuCancelEvent -= GoBack;

            _manualCounterUsedEvent.OnEventRaised -= ShowManualPanel;

            _numberObjectsFixedEvent.OnEventRaised -= ShowEndGamePanel;
        }

        private void Start()
        {
            _toolRecipesPanel.Hide();
            _endGamePanel.Hide();
        }

        private void GoBack()
        {
            // hide all gameplay panels
            _toolRecipesPanel.Hide();

            // send event
            _outMenuEvent.RaiseEvent();
        }

        private void ShowManualPanel()
        {
            _toolRecipesPanel.Show();
            _inMenuEvent.RaiseEvent();
        }

        private void ShowEndGamePanel(int numberOfObjecsFixed)
        {
            _endGamePanel.Show(numberOfObjecsFixed);
        }
    }
}
