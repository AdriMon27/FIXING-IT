using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkSceneLoader : NetworkBehaviour
{
    public static NetworkSceneLoader Instance { get; private set; }

    private float _necessaryWaitSeconds = 0.1f;

    [Header("Broadcasting To")]
    [SerializeField]
    private VoidEventChannelSO _sceneLoadedEvent;
    [SerializeField]
    private VoidEventChannelSO _unloadedNetworkSceneEventCompleted;

    [Header("Setting Func")]
    [SerializeField]
    private StringBoolFuncSO _loadNewNetworkSceneByNameFunc;
    [SerializeField]
    private StringBoolFuncSO _unloadNetworkSceneByNameFunc;

    private void Awake()
    {
        if (Instance != null && Instance != this) {
            Destroy(this);
        }
        else {
            Instance = this;
        }
        DontDestroyOnLoad(this);

        _loadNewNetworkSceneByNameFunc.TrySetOnFuncRaised(LoadNewNetworkScene);
        _unloadNetworkSceneByNameFunc.TrySetOnFuncRaised(UnloadNetworkScene);
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer) {
            NetworkManager.Singleton.SceneManager.OnSceneEvent += NetworkSceneLoader_OnNetworkSceneEvent;
        }
        Debug.Log($"OnNetworkSpawn and IsServer: {IsServer}");

        base.OnNetworkSpawn();
    }

    #region Load
    /// <summary>
    /// Function that should be called after all the clients have unload async the previous networkScene.
    /// Loads async a new NetworkScene
    /// </summary>
    /// <param name="sceneName">Name of the scene to load</param>
    /// <returns>If the Scene was loaded && You are server</returns>
    /// <exception cref="Exception">It was not possible to load the scene due to NetworkErrors</exception>
    private bool LoadNewNetworkScene(string sceneName)
    {
        if (!IsServer)
            return false;

        var status = NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        if (status != SceneEventProgressStatus.Started) {
            string errorMsg = $"Failed to load {sceneName} with a {nameof(SceneEventProgressStatus)}: {status}";

            Debug.LogWarning(errorMsg);
            throw new Exception(errorMsg);
        }

        return true;
    }
    #endregion

    #region Unload
    /// <summary>
    /// Unloads a NetworkScene
    /// </summary>
    /// <param name="sceneName">Name of the scene to unload</param>
    /// <returns>If the Scene was unloaded && You are server</returns>
    /// <exception cref="Exception">It was not possible to unload the scene due to nameScene error</exception>
    private bool UnloadNetworkScene(string sceneName)
    {
        // TODO: mirar esto bien
        if (!IsServer)
            return false;

        Scene sceneToUnload = SceneManager.GetSceneByName(sceneName);
        if (!sceneToUnload.IsValid()) {
            string errorMsg = $"Escena con nombre {sceneName} no existe ahora mismo";

            Debug.LogWarning(errorMsg);
            throw new Exception(errorMsg);
        }

        NetworkManager.Singleton.SceneManager.UnloadScene(sceneToUnload);
        return true;
    }
    #endregion

    private void NetworkSceneLoader_OnNetworkSceneEvent(SceneEvent sceneEvent)
    {
        var clientOrServer = sceneEvent.ClientId == NetworkManager.ServerClientId ? "server" : "client";
        switch (sceneEvent.SceneEventType)
        {
            // locally cases
            case SceneEventType.LoadComplete:
                {
                    //SceneManager.SetActiveScene(sceneEvent.Scene);

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

                    // in theory this code is unreachable for clients but just in case
                    if (IsServer) { 
                        _unloadedNetworkSceneEventCompleted.RaiseEvent();
                    }

                    break;
                }
        }
    }
}
