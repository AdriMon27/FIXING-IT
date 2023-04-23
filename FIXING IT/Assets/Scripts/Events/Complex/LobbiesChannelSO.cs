using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/Complex/Lobbies Channel")]
public class LobbiesChannelSO : DescriptionBaseSO
{
    public UnityAction<List<Lobby>> OnEventRaised;

    public void RaiseEvent(List<Lobby> lobbies)
    {
        if (OnEventRaised != null) {
            OnEventRaised.Invoke(lobbies);
        }
        else {
            Debug.LogError($"{errorMessage} with parameter {lobbies}");
        }
    }
}
