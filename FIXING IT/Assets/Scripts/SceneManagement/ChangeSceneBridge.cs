using UnityEngine;

public class ChangeSceneBridge : MonoBehaviour
{
    [SerializeField] private GameSceneSO _lobbySelectionScene;
    [SerializeField] private GameSceneSO _mainMenuScene;
    [SerializeField] private GameSceneSO _characterSelectionScene;

    [Header("Broadcasting To")]
    [SerializeField]
    private LoadSceneChannelSO _loadSceneChannel;
    [SerializeField]
    private LoadSceneChannelSO _loadNetworkSceneChannel;

    [Header("Listening To")]
    [SerializeField]
    private VoidEventChannelSO _toLobbySelectionEvent;
    [SerializeField]
    private VoidEventChannelSO _toMainMenuScreenEvent;
    [SerializeField]
    private VoidEventChannelSO _hostStartedEvent;

    private void OnEnable()
    {
        _toLobbySelectionEvent.OnEventRaised += LoadLobbySelection;
        _toMainMenuScreenEvent.OnEventRaised += LoadMainMenuScreen;

        _hostStartedEvent.OnEventRaised += LoadCharacterSelection;
    }
    private void OnDisable()
    {
        _toLobbySelectionEvent.OnEventRaised -= LoadLobbySelection;
        _toMainMenuScreenEvent.OnEventRaised += LoadMainMenuScreen;

        _hostStartedEvent.OnEventRaised -= LoadCharacterSelection;
    }

    private void LoadLobbySelection()
    {
        _loadSceneChannel.RaiseEvent(_lobbySelectionScene);
    }

    private void LoadMainMenuScreen()
    {
        _loadSceneChannel.RaiseEvent(_mainMenuScene);
    }

    private void LoadCharacterSelection()
    {
        _loadNetworkSceneChannel.RaiseEvent(_characterSelectionScene);
    }
}
