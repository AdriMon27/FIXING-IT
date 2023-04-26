using UnityEngine;
using UnityEngine.EventSystems;

public class UIPopUpPanel : MonoBehaviour
{
    public enum PopUpMode
    {
        CreateLobby,
        JoinByCode,
        Error
    }

    [SerializeField] private GameObject _blurImage;
    [SerializeField] private UICreateLobbyPanel _createLobbyPanel;
    [SerializeField] private UIJoinByCodePanel _joinByCodePanel;
    [SerializeField] private UILobbyErrorPanel _errorPanel;

    public void ShowPopUp(PopUpMode popUpMode, string errorMsg = "No error")
    {
        _blurImage.SetActive(true);

        switch (popUpMode) {
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
                _errorPanel.SetErrorMsg(errorMsg);
                EventSystem.current.SetSelectedGameObject(_errorPanel.FirstSelected);
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
    }
}
