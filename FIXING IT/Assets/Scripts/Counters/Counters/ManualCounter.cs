using FixingIt.RoomObjects;
using ProgramadorCastellano.Events;
using UnityEngine;

namespace FixingIt.Counters
{
    public class ManualCounter : BaseCounter
    {
        [Header("Broadcasting To")]
        [SerializeField] private VoidEventChannelSO _manualCounterUsedEvent;

        public override void AlternateInteract(IRoomObjectParent roomObjectParent)
        {
            ManualCounterUsed();
        }

        public override void Interact(IRoomObjectParent roomObjectParent)
        {
            ManualCounterUsed();
        }

        private void ManualCounterUsed()
        {
            _manualCounterUsedEvent.RaiseEvent();
        }
    }
}
