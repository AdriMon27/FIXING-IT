using UnityEngine;
using UnityEngine.EventSystems;

namespace FixingIt.UI.LobbySelection
{
    public class UIPopUpPanel : MonoBehaviour
    {
        public enum PopUpMode
        {
            CreateLobby,
            JoinByCode,
            Error,
            LobbyState
        }

        [SerializeField] private GameObject _blurImage;
        [SerializeField] private UICreateLobbyPanel _createLobbyPanel;
        [SerializeField] private UIJoinByCodePanel _joinByCodePanel;
        [SerializeField] private UILobbyErrorPanel _errorPanel;
        [SerializeField] private UILobbyState _statePanel;

        public void ShowPopUp(PopUpMode popUpMode, string msg = "No error")
        {
            _blurImage.SetActive(true);
            HidePopUp();

            switch (popUpMode)
            {
                case PopUpMode.CreateLobby:
                    _createLobbyPanel.gameObject.SetActive(true);
                    EventSystem.current.SetSelectedGameObject(_createLobbyPanel.FirstSelected);
                    break;
                case PopUpMode.JoinByCode:
                    _joinByCodePanel.gameObject.SetActive(true);
                    EventSystem.current.SetSelectedGameObject(_joinByCodePanel.FirstSelected);
                    break;
                case PopUpMode.Error:
                    _errorPanel.gameObject.SetActive(true);
                    _errorPanel.SetErrorMsg(msg);
                    EventSystem.current.SetSelectedGameObject(_errorPanel.FirstSelected);
                    break;
                case PopUpMode.LobbyState:
                    _statePanel.gameObject.SetActive(true);
                    _statePanel.SetStateMessage(msg);
                    EventSystem.current.SetSelectedGameObject(null);
                    break;
                default:
                    Debug.LogWarning("PopUpMode not implemented");
                    break;
            }
        }

        public void HidePopUp()
        {
            _blurImage.SetActive(false);

            _createLobbyPanel.gameObject.SetActive(false);
            _joinByCodePanel.gameObject.SetActive(false);
            _errorPanel.gameObject.SetActive(false);
            _statePanel.gameObject.SetActive(false);
        }
    }
}