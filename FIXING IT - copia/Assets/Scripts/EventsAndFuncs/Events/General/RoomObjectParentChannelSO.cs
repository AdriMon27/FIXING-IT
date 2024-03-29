using FixingIt.RoomObjects.Logic;
using ProgramadorCastellano.Events;
using UnityEngine;

namespace FixingIt.Events
{
    [CreateAssetMenu(menuName = "Events/Complex/RoomObjectParent Channel")]
    public class RoomObjectParentChannelSO : BaseEventChannelSO<IRoomObjectParent>
    {
    }
}
