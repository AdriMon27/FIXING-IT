using Unity.Netcode;
using UnityEngine;

public class MainMenuCleanUp : MonoBehaviour
{
    // Code made to clean unnecesary network managers in MainMenu
    // All of them are loaded again at LobbySelection
    private void Awake()
    {
        if (NetworkManager.Singleton != null) {
            Destroy(NetworkManager.Singleton.gameObject);
        }

        if (NetworkSceneLoader.Instance != null) {
            Destroy(NetworkSceneLoader.Instance.gameObject);
        }

        if (FixingGameMultiplayer.Instance != null) {
            Destroy(FixingGameMultiplayer.Instance.gameObject);
        }
    }
}
