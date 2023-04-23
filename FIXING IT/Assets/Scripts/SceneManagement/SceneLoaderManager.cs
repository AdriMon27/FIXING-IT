using Unity.Netcode;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class SceneLoaderManager : MonoBehaviour
{
    // filled after loaded one scene
    private GameSceneSO _sceneToLoad;
    private GameSceneSO _currentSceneLoaded;

    [Header("Listening To")]
    [SerializeField]
    private LoadSceneChannelSO _loadSceneChannel;
    [SerializeField]
    private LoadSceneChannelSO _loadNetworkSceneChannel;
    [SerializeField]
    private VoidEventChannelSO _exitGameEvent;

    [Header("Broadcasting To")]
    [SerializeField] VoidEventChannelSO _sceneLoadedEvent;

    private void OnEnable()
    {
        _loadSceneChannel.OnEventRaised += LoadScene;
        _loadNetworkSceneChannel.OnEventRaised += LoadNetworkScene;
        _exitGameEvent.OnEventRaised += ExitGame;
    }

    private void OnDisable()
    {
        _loadSceneChannel.OnEventRaised -= LoadScene;
        _loadNetworkSceneChannel.OnEventRaised -= LoadNetworkScene;
        _exitGameEvent.OnEventRaised -= ExitGame;
    }

    private void LoadScene(GameSceneSO sceneToLoad)
    {
        _sceneToLoad = sceneToLoad;

        UnloadPreviousScene();
        LoadNewScene();
    }

    private void LoadNetworkScene(GameSceneSO sceneToLoad)
    {
        UnloadPreviousScene();

        string sceneName = sceneToLoad.name;

        var status = NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        if (status != SceneEventProgressStatus.Started) {
            Debug.LogWarning($"Failed to load {sceneName} with a {nameof(SceneEventProgressStatus)}: {status}");
        }

        _currentSceneLoaded = sceneToLoad;
    }

    /// <summary>
    /// Se podría hacer Coroutine para añadir efecto de pantalla de carga
    /// </summary>
    private void UnloadPreviousScene()
    {
        // null when we are entering in the Main Menu
        if (_currentSceneLoaded != null) {
            _currentSceneLoaded.SceneReference.UnLoadScene();
        }
    }

    private void LoadNewScene()
    {
        var loadingOperationHandle = _sceneToLoad.SceneReference.LoadSceneAsync(LoadSceneMode.Additive);
        loadingOperationHandle.Completed += OnNewSceneLoaded;
    }

    private void OnNewSceneLoaded(AsyncOperationHandle<SceneInstance> obj)
    {
        _currentSceneLoaded = _sceneToLoad;

        Scene s = obj.Result.Scene;
        SceneManager.SetActiveScene(s);

        //LightProbes.TetrahedralizeAsync(); //not necessary

        _sceneLoadedEvent.RaiseEvent();
    }

    private void ExitGame()
    {
        Debug.Log("Quitting Application!");
        Application.Quit();
    }
}
