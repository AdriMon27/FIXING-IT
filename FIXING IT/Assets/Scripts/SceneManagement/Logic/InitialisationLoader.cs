using FixingIt.Events;
using FixingIt.SceneManagement.ScriptableObjects;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace FixingIt.SceneManagement.Logic
{
    public class InitialisationLoader : MonoBehaviour
    {
        [SerializeField] GameSceneSO _managersScene;
        [SerializeField] GameSceneSO _mainMenuScene;

        [Header("Broadcasting To")]
        [SerializeField]
        private LoadSceneChannelSO _loadSceneChannel;

        private void Start()
        {
            _managersScene.SceneReference.LoadSceneAsync(LoadSceneMode.Additive).Completed += LoadMainMenu;

            //Cursor.lockState = CursorLockMode.Locked;
        }

        private void LoadMainMenu(AsyncOperationHandle<SceneInstance> obj)
        {
            _loadSceneChannel.RaiseEvent(_mainMenuScene);

            SceneManager.UnloadSceneAsync(0);   //delete itself after
        }
    }
}