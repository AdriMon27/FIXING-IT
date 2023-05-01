using FixingIt.InputSystem;
using ProgramadorCastellano.MyEvents;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FixingIt.UI.CharacterSelection
{
    public class UICharacterSelectionManager : MonoBehaviour
    {
        [SerializeField] private InputReaderSO _inputReaderSO;

        [SerializeField] private Button _readyButton;
        [SerializeField] private Button _mainMenuButton;

        [Header("Panels")]
        [SerializeField]
        private UILeaveToMenuPanel _leaveToMainMenuPanel;

        [Header("Broadcasting To")]
        [SerializeField]
        private VoidEventChannelSO _readyButtonEvent;

        [Header("Listening To")]
        [SerializeField]
        private VoidEventChannelSO _cancelLeaveToMainMenuEvent;

        private void Start()
        {
            _readyButton.onClick.AddListener(ReadyButtonAction);
            _mainMenuButton.onClick.AddListener(MainMenuButtonAction);

            HideLeaveToMainMenuPopup();
        }

        private void OnEnable()
        {
            _inputReaderSO.MenuCancelEvent += HideLeaveToMainMenuPopup;

            _cancelLeaveToMainMenuEvent.OnEventRaised += HideLeaveToMainMenuPopup;
        }

        private void OnDisable()
        {
            _inputReaderSO.MenuCancelEvent -= HideLeaveToMainMenuPopup;

            _cancelLeaveToMainMenuEvent.OnEventRaised -= HideLeaveToMainMenuPopup;
        }

        private void ReadyButtonAction()
        {
            _readyButtonEvent.RaiseEvent();
        }

        private void MainMenuButtonAction()
        {
            _leaveToMainMenuPanel.gameObject.SetActive(true);
            EventSystem.current.SetSelectedGameObject(_leaveToMainMenuPanel.FirstSelected);
        }

        private void HideLeaveToMainMenuPopup()
        {
            _leaveToMainMenuPanel.gameObject.SetActive(false);
            EventSystem.current.SetSelectedGameObject(GetFirstSelected());
        }

        private GameObject GetFirstSelected()
        {
            return _readyButton.gameObject;
        }
    }
}
