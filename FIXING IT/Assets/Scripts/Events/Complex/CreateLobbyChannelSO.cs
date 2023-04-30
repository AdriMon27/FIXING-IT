using ProgramadorCastellano.MyEvents;
using UnityEngine;

[CreateAssetMenu(menuName = "Events/Complex/Create Lobby Channel")]
public class CreateLobbyChannelSO : BaseComplexEventChannelSO<string, bool>
{
    //public UnityAction<string, bool> OnEventRaised { get; set; }

    //public void RaiseEvent(string lobbyName, bool isPrivate)
    //{
    //    if (OnEventRaised != null) {
    //        OnEventRaised.Invoke(lobbyName, isPrivate);
    //    }
    //    else {
    //        Debug.LogWarning($"{errorMessage} with parameters {lobbyName} and {isPrivate}");
    //    }
    //}
    public new void RaiseEvent(string lobbyName, bool isPrivate)
    {
        base.RaiseEvent(lobbyName, isPrivate);
    }
}
