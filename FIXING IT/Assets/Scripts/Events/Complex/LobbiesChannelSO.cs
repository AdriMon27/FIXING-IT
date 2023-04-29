using ProgramadorCastellano.MyEvents;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/Complex/Lobbies Channel")]
public class LobbiesChannelSO : DescriptionBaseSO, IMyEventSO<List<Lobby>>
{
    public UnityAction<List<Lobby>> OnEventRaised { get; set; }

    public void RaiseEvent(List<Lobby> lobbies)
    {
        if (OnEventRaised != null) {
            OnEventRaised.Invoke(lobbies);
        }
        else {
            Debug.LogWarning($"{errorMessage} with parameter {lobbies}");
        }
    }
}
