using FixingIt.Events;
using FixingIt.SceneManagement.ScriptableObjects;
using ProgramadorCastellano.Events;
using ProgramadorCastellano.Funcs;
using System;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace FixingIt.SceneManagement
{
    /// <summary>
    /// Persistent Manager, doesn´t need an Instance
    /// </summary>
    public class SceneLoaderManager : MonoBehaviour
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
        [SerializeField]
        private VoidEventChannelSO _unloadedNetworkEventCompleted;

        [Header("Broadcasting To")]
        [SerializeField] VoidEventChannelSO _startSceneLoadingEvent;
        [SerializeField] VoidEventChannelSO _sceneLoadedEvent;

        [Header("Invoking Func")]
        [SerializeField]
        private StringBoolFuncSO _loadNewNetworkSceneByNameFunc;
        [SerializeField]
        private StringBoolFuncSO _unloadNetworkSceneByNameFunc;

        [Header("Setting Func")]
        [SerializeField]
        private StringFuncSO _getCurrentSceneNameFunc;

        private void Awake()
        {
            //DontDestroyOnLoad(gameObject);

            _getCurrentSceneNameFunc.TrySetOnFuncRaised(() => _currentSceneLoaded.name);
        }

        private void OnEnable()
        {
            _loadSceneChannel.OnEventRaised += LoadScene;
            _loadNetworkSceneChannel.OnEventRaised += LoadNetworkScene;
            _exitGameEvent.OnEventRaised += ExitGame;

            _unloadedNetworkEventCompleted.OnEventRaised += SceneLoaderManager_UnloadedNetworkEventCompleted;
        }

        private void OnDisable()
        {
            _loadSceneChannel.OnEventRaised -= LoadScene;
            _loadNetworkSceneChannel.OnEventRaised -= LoadNetworkScene;
            _exitGameEvent.OnEventRaised -= ExitGame;

            _unloadedNetworkEventCompleted.OnEventRaised -= SceneLoaderManager_UnloadedNetworkEventCompleted;
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
            if (!_isCurrentSceneNetwork)
            {
                Invoke(nameof(LoadNewNetworkScene), _necessaryWaitSeconds);  // necessary wait to prevent errors
            }
            // else => lo maneja UnloadPreviousSceneNetwork
        }

        /// <summary>
        /// Function that should be called after all the clients have unload async the previous networkScene
        /// </summary>
        private void LoadNewNetworkScene()
        {
            // load scene
            string sceneName = _sceneToLoad.name;
            bool hasNetSceneLoaded = false;
            try
            {
                hasNetSceneLoaded = _loadNewNetworkSceneByNameFunc.RaiseFunc(sceneName);
            }
            catch (Exception e)
            {
                ManageNetworkSceneLoaderExceptions(e);
            }

            if (hasNetSceneLoaded)
                _currentSceneLoaded = _sceneToLoad;

            _isCurrentSceneNetwork = hasNetSceneLoaded;
        }
        #endregion

        #region Unload
        /// <summary>
        /// Se podría hacer Coroutine para añadir efecto de pantalla de carga
        /// </summary>
        private void UnloadPreviousScene()
        {
            // null when we are entering in the Main Menu
            if (_currentSceneLoaded == null)
            {
                return;
            }

            // check if it is managed by SceneManager or by NetworkSceneManager
            if (!_isCurrentSceneNetwork)
            {
                UnloadPreviousSceneLocal();
            }
            else
            {
                UnloadPreviousSceneNetwork();
            }
        }

        private void UnloadPreviousSceneLocal()
        {
            _currentSceneLoaded.SceneReference.UnLoadScene();
        }
        private void UnloadPreviousSceneNetwork()
        {
            string sceneName = _currentSceneLoaded.name;

            try
            {
                _unloadNetworkSceneByNameFunc.RaiseFunc(sceneName);
            }
            catch (Exception e)
            {
                ManageNetworkSceneLoaderExceptions(e);
            }
        }
        #endregion

        private void ExitGame()
        {
            Debug.Log("Quitting Application!");
            Application.Quit();
        }

        private void ManageNetworkSceneLoaderExceptions(Exception e)
        {
            Debug.LogError($"Error catched: {e.Message}");
        }

        private void SceneLoaderManager_UnloadedNetworkEventCompleted()
        {
            if (_isSceneToLoadNetwork)
            {
                Invoke(nameof(LoadNewNetworkScene), _necessaryWaitSeconds);  // necessary wait to prevent errors
            }
            // else -> managed by its function
        }
    }
}