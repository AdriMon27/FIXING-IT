using ProgramadorCastellano.Events;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace FixingIt.Events
{
    [CreateAssetMenu(menuName = "Events/Complex/Lobbies Channel")]
    public class LobbiesChannelSO : BaseEventChannelSO<List<Lobby>>
    {
    }
}