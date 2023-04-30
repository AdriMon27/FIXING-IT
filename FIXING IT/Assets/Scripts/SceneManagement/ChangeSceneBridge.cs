using ProgramadorCastellano.MyEvents;
using UnityEngine;

namespace FixingIt.SceneManagement
{
    public class ChangeSceneBridge : MonoBehaviour
    {
        [SerializeField] private GameSceneSO _lobbySelectionScene;
        [SerializeField] private GameSceneSO _mainMenuScene;
        [SerializeField] private GameSceneSO _characterSelectionScene;
        [SerializeField] private GameSceneSO _minigameScene;

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
        [SerializeField]
        private VoidEventChannelSO _allPlayersReadyEvent;
        [SerializeField]
        private VoidEventChannelSO _networkToMainMenuEvent;

        private void OnEnable()
        {
            _toLobbySelectionEvent.OnEventRaised += LoadLobbySelection;
            _toMainMenuScreenEvent.OnEventRaised += LoadMainMenuScreen;

            _hostStartedEvent.OnEventRaised += LoadCharacterSelection;

            _allPlayersReadyEvent.OnEventRaised += LoadMinigame;

            _networkToMainMenuEvent.OnEventRaised += LoadMainMenuScreen;
        }
        private void OnDisable()
        {
            _toLobbySelectionEvent.OnEventRaised -= LoadLobbySelection;
            _toMainMenuScreenEvent.OnEventRaised += LoadMainMenuScreen;

            _hostStartedEvent.OnEventRaised -= LoadCharacterSelection;

            _allPlayersReadyEvent.OnEventRaised -= LoadMinigame;

            _networkToMainMenuEvent.OnEventRaised -= LoadMainMenuScreen;
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

        private void LoadMinigame()
        {
            _loadNetworkSceneChannel.RaiseEvent(_minigameScene);
        }
    }
}