using ProgramadorCastellano.Events;
using Unity.Netcode;
using UnityEngine;

namespace FixingIt.Events.Network
{
    [CreateAssetMenu(menuName = "Events/Complex/NetworkObjectReference Channel")]
    public class NetworkObjectReferenceEventSO : BaseEventChannelSO<NetworkObjectReference>
    {
    }
}
