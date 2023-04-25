using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Code made to clean unnecesary managers in MainMenu
/// </summary>
public class MainMenuCleanUp : MonoBehaviour
{
    private void Awake()
    {
        if (NetworkManager.Singleton != null) {
            Destroy(NetworkManager.Singleton.gameObject);
        }
    }
}
