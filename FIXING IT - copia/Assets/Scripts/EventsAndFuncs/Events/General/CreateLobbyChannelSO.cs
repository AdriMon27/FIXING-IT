using ProgramadorCastellano.Events;
using UnityEngine;

namespace FixingIt.Events
{
    [CreateAssetMenu(menuName = "Events/Complex/Create Lobby Channel")]
    public class CreateLobbyChannelSO : BaseEventChannelSO<string, bool>
    {
        public new void RaiseEvent(string lobbyName, bool isPrivate)
        {
            base.RaiseEvent(lobbyName, isPrivate);
        }
    }
}