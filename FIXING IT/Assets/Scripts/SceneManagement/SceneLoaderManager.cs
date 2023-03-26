using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class SceneLoaderManager : MonoBehaviour
{
    [Header("Listening To")]
    [SerializeField]
    private LoadSceneChannelSO _loadSceneChannel;
    [SerializeField]
    private VoidEventChannelSO _exitGameEvent;

    // filled after loaded one scene
    private GameSceneSO _sceneToLoad;
    private GameSceneSO _currentSceneLoaded;

    private void OnEnable()
    {
        _loadSceneChannel.OnEventRaised += LoadScene;
        _exitGameEvent.OnEventRaised += ExitGame;
    }

    private void OnDisable()
    {
        _loadSceneChannel.OnEventRaised -= LoadScene;
        _exitGameEvent.OnEventRaised -= ExitGame;
    }

    private void LoadScene(GameSceneSO sceneToLoad)
    {
        _sceneToLoad = sceneToLoad;

        UnloadPreviousScene();
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

        LoadNewScene();
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
    }

    private void ExitGame()
    {
        Debug.Log("Quitting Application!");
        Application.Quit();
    }
}
