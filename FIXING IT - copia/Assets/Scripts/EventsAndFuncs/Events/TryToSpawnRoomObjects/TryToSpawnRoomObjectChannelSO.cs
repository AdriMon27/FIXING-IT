using FixingIt.RoomObjects.SO;
using ProgramadorCastellano.Events;
using Unity.Netcode;
using UnityEngine;

namespace FixingIt.Events
{
    [CreateAssetMenu(menuName = "Events/Complex/Try To Spawn RoomObject Channel")]
    public class TryToSpawnRoomObjectChannelSO : BaseEventChannelSO<RoomObjectSO, NetworkObjectReference>
    {
    }
}
