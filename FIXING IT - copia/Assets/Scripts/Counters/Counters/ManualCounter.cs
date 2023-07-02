using FixingIt.RoomObjects.Logic;
using ProgramadorCastellano.Events;
using System.Collections;
using UnityEngine;

namespace FixingIt.Counters
{
    public class ManualCounter : BaseCounter
    {
        private delegate void Function();

        [Header("Broadcasting To")]
        [SerializeField] private VoidEventChannelSO _manualCounterUsedEvent;

        public override void AlternateInteract(IRoomObjectParent roomObjectParent)
        {
            ManualCounterUsed();
        }

        public override void Interact(IRoomObjectParent roomObjectParent)
        {
            StartCoroutine(InvokeNextFrame(ManualCounterUsed)); // necessary wait to avoid errors
            return;
        }

        private void ManualCounterUsed()
        {
            _manualCounterUsedEvent.RaiseEvent();
        }

        // This function could be exernalised but it is only used here so it is not necessary
        private IEnumerator InvokeNextFrame(Function function)
        {
            yield return null;
            function();
        }
    }
}
