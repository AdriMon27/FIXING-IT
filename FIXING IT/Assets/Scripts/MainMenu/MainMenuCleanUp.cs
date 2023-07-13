using FixingIt.GameLobby;
using FixingIt.Multiplayer;
using FixingIt.SceneManagement.Logic;
using FixingIt.SceneManagement.ScriptableObjects;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FixingIt.MainMenu
{
    public class MainMenuCleanUp : MonoBehaviour
    {
        [SerializeField]
        private GameSceneSO[] _networkScenes; 
        // Code made to clean unnecesary network managers in MainMenu
        // All of them are loaded again at LobbySelection
        private void Awake()
        {
            // unload unnecesary scenes
            for (int i = 0; i < SceneManager.sceneCount; i++) {
                foreach (GameSceneSO netScene in _networkScenes) {
                    //Debug.LogWarning(netScene.name + " " + SceneManager.GetSceneAt(i));
                    if (netScene.name == SceneManager.GetSceneAt(i).name) {
                        SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(i));
                        //Debug.LogWarning(i + SceneManager.GetSceneAt(i).name);
                        //i--;
                    }
                }
            }

            // destroy unnecesary managers
            if (NetworkManager.Singleton != null)
            {
                Destroy(NetworkManager.Singleton.gameObject);
            }

            if (NetworkSceneLoader.Instance != null)
            {
                Destroy(NetworkSceneLoader.Instance.gameObject);
            }

            if (LobbyManager.Instance != null)
            {
                Destroy(LobbyManager.Instance.gameObject);
            }

            if (FixingGameMultiplayer.Instance != null)
            {
                Destroy(FixingGameMultiplayer.Instance.gameObject);
            }
        }
    }
}