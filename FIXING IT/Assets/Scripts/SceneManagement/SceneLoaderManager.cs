using Unity.Netcode;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class SceneLoaderManager : NetworkBehaviour
{
    // filled after loaded one scene
    private GameSceneSO _sceneToLoad;
    private GameSceneSO _currentSceneLoaded;
    private bool _isSceneToLoadNetwork = false;
    private bool _isCurrentSceneNetwork = false;

    private float _necessaryWaitSeconds = 0.1f;

    [Header("Listening To")]
    [SerializeField]
    private LoadSceneChannelSO _loadSceneChannel;
    [SerializeField]
    private LoadSceneChannelSO _loadNetworkSceneChannel;
    [SerializeField]
    private VoidEventChannelSO _exitGameEvent;

    [Header("Broadcasting To")]
    [SerializeField] VoidEventChannelSO _startSceneLoadingEvent;
    [SerializeField] VoidEventChannelSO _sceneLoadedEvent;

    [Header("Setting Func")]
    [SerializeField]
    private StringFuncSO _getCurrentSceneNameFunc;

    private void Awake()
    {
        _getCurrentSceneNameFunc.TrySetOnFuncRaised(() => _currentSceneLoaded.name);
    }

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

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.SceneManager.OnSceneEvent += SceneLoaderManager_OnNetworkSceneEvent;
        }
        Debug.Log($"OnNetworkSpawn and IsServer: {IsServer}");

        base.OnNetworkSpawn();
    }

    #region Local Load
    private void LoadScene(GameSceneSO sceneToLoad)
    {
        // send event
        _startSceneLoadingEvent.RaiseEvent();

        _sceneToLoad = sceneToLoad;
        _isSceneToLoadNetwork = false;

        UnloadPreviousScene();
        LoadNewScene();
    }

    private void LoadNewScene()
    {
        // load scene
        var loadingOperationHandle = _sceneToLoad.SceneReference.LoadSceneAsync(LoadSceneMode.Additive);
        loadingOperationHandle.Completed += OnNewSceneLoaded;

        _isCurrentSceneNetwork = false;
    }

    private void OnNewSceneLoaded(AsyncOperationHandle<SceneInstance> obj)
    {
        _currentSceneLoaded = _sceneToLoad;

        Scene s = obj.Result.Scene;
        SceneManager.SetActiveScene(s);

        //LightProbes.TetrahedralizeAsync(); //not necessary

        // send event
        _sceneLoadedEvent.RaiseEvent();
    }
    #endregion

    #region Network Load
    private void LoadNetworkScene(GameSceneSO sceneToLoad)
    {
        // send event
        _startSceneLoadingEvent.RaiseEvent();

        _sceneToLoad = sceneToLoad;
        _isSceneToLoadNetwork = true;

        UnloadPreviousScene();
        
        // si hemos descargado una local
        if (!_isCurrentSceneNetwork) {
            Invoke(nameof(LoadNewNetworkScene), _necessaryWaitSeconds);  // necessary wait to prevent errors
        }
        // else => lo maneja SceneLoaderManager_OnNewtworkSceneEvent
    }

    /// <summary>
    /// Function that should be called after all the clients have unload async the previous networkScene
    /// </summary>
    private void LoadNewNetworkScene()
    {
        if (!IsServer)
            return;

        // load scene
        string sceneName = _sceneToLoad.name;

        var status = NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        if (status != SceneEventProgressStatus.Started) {
            Debug.LogWarning($"Failed to load {sceneName} with a {nameof(SceneEventProgressStatus)}: {status}");
        }

        _currentSceneLoaded = _sceneToLoad;
        _isCurrentSceneNetwork = true;
    }
    #endregion

    #region Unload
    /// <summary>
    /// Se podría hacer Coroutine para añadir efecto de pantalla de carga
    /// </summary>
    private void UnloadPreviousScene()
    {
        // null when we are entering in the Main Menu
        if (_currentSceneLoaded == null) {
            return;
        }

        // check if it is managed by SceneManager or by NetworkSceneManager
        if (!_isCurrentSceneNetwork) {
            UnloadPreviousSceneLocal();
        }
        else {
            UnloadPreviousSceneNetwork();
        }
    }

    private void UnloadPreviousSceneLocal()
    {
        _currentSceneLoaded.SceneReference.UnLoadScene();
    }
    private void UnloadPreviousSceneNetwork()
    {
        // TODO: mirar esto bien
        if (!IsServer)
            return;

        Scene sceneToUnload = SceneManager.GetSceneByName(_currentSceneLoaded.name);
        if (sceneToUnload.IsValid()) {
            NetworkManager.Singleton.SceneManager.UnloadScene(sceneToUnload);
        }
        else { 
            Debug.LogError($"Escena con nombre {_currentSceneLoaded.name} no existe ahora mismo");
        }
    }
    #endregion

    private void ExitGame()
    {
        Debug.Log("Quitting Application!");
        Application.Quit();
    }


    private void SceneLoaderManager_OnNetworkSceneEvent(SceneEvent sceneEvent)
    {
        var clientOrServer = sceneEvent.ClientId == NetworkManager.ServerClientId ? "server" : "client";
        switch (sceneEvent.SceneEventType)
        {
            // locally cases
            case SceneEventType.LoadComplete:
                {
                    // We want to handle this for only the server-side
                    /*if (sceneEvent.ClientId == NetworkManager.ServerClientId)
                    {
                        // *** IMPORTANT ***
                        // Keep track of the loaded scene, you need this to unload it
                        m_LoadedScene = sceneEvent.Scene;
                    }*/
                    SceneManager.SetActiveScene(sceneEvent.Scene);

                    Debug.Log($"Loaded the {sceneEvent.SceneName} scene on {clientOrServer}-({sceneEvent.ClientId}).");

                    break;
                }
            case SceneEventType.UnloadComplete:
                {
                    Debug.Log($"Unloaded the {sceneEvent.SceneName} scene on {clientOrServer}-({sceneEvent.ClientId}).");

                    break;
                }
            // when server && all clients
            case SceneEventType.LoadEventCompleted:
                {
                    Debug.Log($"Load event completed for the following client identifiers:({sceneEvent.ClientsThatCompleted})");
                    if (sceneEvent.ClientsThatTimedOut.Count > 0) {
                        Debug.LogWarning($"Load event timed out for the following client identifiers:({sceneEvent.ClientsThatTimedOut})");
                    }

                    // send event
                    _sceneLoadedEvent.RaiseEvent();
                    break;
                }
            case SceneEventType.UnloadEventCompleted:
                {
                    Debug.Log($"Unload event completed for the following client identifiers:({sceneEvent.ClientsThatCompleted})");
                    if (sceneEvent.ClientsThatTimedOut.Count > 0) {
                        Debug.LogWarning($"Unload event timed out for the following client identifiers:({sceneEvent.ClientsThatTimedOut})");
                    }

                    if (_isSceneToLoadNetwork) {
                        Invoke(nameof(LoadNewNetworkScene), _necessaryWaitSeconds);  // necessary wait to prevent errors
                    }
                    // else -> managed by its function
                    break;
                }
        }
    }
}
