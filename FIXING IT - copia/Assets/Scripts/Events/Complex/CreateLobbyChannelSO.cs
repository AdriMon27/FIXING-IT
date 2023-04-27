using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/Complex/Create Lobby Channel")]
public class CreateLobbyChannelSO : DescriptionBaseSO
{
    public UnityAction<string, bool> OnEventRaised;

    public void RaiseEvent(string lobbyName, bool isPrivate)
    {
        if (OnEventRaised != null) {
            OnEventRaised.Invoke(lobbyName, isPrivate);
        }
        else {
            Debug.LogWarning($"{errorMessage} with parameters {lobbyName} and {isPrivate}");
        }
    }
}
