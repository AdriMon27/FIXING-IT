using UnityEngine;

public class ChangeSceneBridge : MonoBehaviour
{
    [SerializeField] private GameSceneSO _lobbySelectionScene;
    [SerializeField] private GameSceneSO _mainMenuScene;

    [Header("Broadcasting To")]
    [SerializeField]
    private LoadSceneChannelSO _loadSceneChannel;

    [Header("Listening To")]
    [SerializeField]
    private VoidEventChannelSO _toLobbySelectionEvent;
    [SerializeField]
    private VoidEventChannelSO _toMainMenuScreenEvent;

    private void OnEnable()
    {
        _toLobbySelectionEvent.OnEventRaised += LoadLobbySelection;
        _toMainMenuScreenEvent.OnEventRaised += LoadMainMenuScreen;
    }
    private void OnDisable()
    {
        _toLobbySelectionEvent.OnEventRaised -= LoadLobbySelection;
        _toMainMenuScreenEvent.OnEventRaised += LoadMainMenuScreen;
    }

    private void LoadLobbySelection()
    {
        _loadSceneChannel.RaiseEvent(_lobbySelectionScene);
    }

    private void LoadMainMenuScreen()
    {
        _loadSceneChannel.RaiseEvent(_mainMenuScene);
    }
}
