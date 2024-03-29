using FixingIt.ActorComponents;
using FixingIt.Events;
using FixingIt.Events.Network;
using FixingIt.RoomObjects.Logic;
using FixingIt.RoomObjects.SO;
using System;
using Unity.Netcode;
using UnityEngine;

namespace FixingIt.Counters
{
    public class TableCounter : BaseCounter
    {
        [Header("Components")]
        [SerializeField] private AudioComponent _objectFixedAudioComp;
        //[SerializeField] private AudioComponent _objectFixingAudioComp;

        [Header("Broadcasting To")]
        [SerializeField]
        private RoomObjectParentChannelSO _confusedRoomObjectParentEvent;

        [Header("Listening To")]
        [SerializeField]
        private NetworkObjectReferenceEventSO _tableCounterNORefEvent;

        private void OnEnable()
        {
            _tableCounterNORefEvent.OnEventRaised += IsObjectFixedInThisTableCounter;
        }

        private void OnDisable()
        {
            _tableCounterNORefEvent.OnEventRaised -= IsObjectFixedInThisTableCounter;
        }

        private void IsObjectFixedInThisTableCounter(NetworkObjectReference tableCounterNORef)
        {
            tableCounterNORef.TryGet(out NetworkObject tableCounterNO);
            TableCounter tableCounter = tableCounterNO.GetComponent<TableCounter>();

            if (tableCounter == null)
                return;

            if (tableCounter != this)
                return;

            _objectFixedAudioComp.PlaySound();
        }

        public override void AlternateInteract(IRoomObjectParent roomObjectParent)
        {
            if (roomObjectParent == null)
                return;

            if (!HasRoomObject()
                || GetRoomObject().RoomObjectSO.Type != RoomObjectSO.RoomObjectType.ObjectToFix) {
                
                _confusedRoomObjectParentEvent.RaiseEvent(roomObjectParent);
                return;
            }

            if (!roomObjectParent.HasRoomObject()
                || roomObjectParent.GetRoomObject().RoomObjectSO.Type == RoomObjectSO.RoomObjectType.ObjectToFix) {
                
                _confusedRoomObjectParentEvent.RaiseEvent(roomObjectParent);
                return;
            }

            if (GetRoomObject() is not ToFixRoomObject) {
                Debug.LogError($"{GetRoomObject().RoomObjectSO} is of type {RoomObjectSO.RoomObjectType.ObjectToFix} " +
                    $"but the prefab does not inheret from {typeof(ToFixRoomObject)}");
                return;
            }

            // check if roomObjectParent.RoomObject is inside necesaryTools to fix object
            ToFixRoomObject toFixRoomObject = (GetRoomObject() as ToFixRoomObject);
            toFixRoomObject.TryToFix(roomObjectParent.GetRoomObject().RoomObjectSO, roomObjectParent.GetRoomObject(), GetNetworkObject());
            //if (toFixRoomObject.TryToFix(roomObjectParent.GetRoomObject().RoomObjectSO, out bool toolBeenUsed)) {

            //    _objectFixedAudioComp.PlaySound();
            //    //_objectFixedEvent.RaiseEvent();
            //}
            //else {
            //    //_objectFixingAudioComp.PlaySound();
            //    //_objectFixingEvent.RaiseEvent();
            //}

            //if (toolBeenUsed) { 
            //    roomObjectParent.GetRoomObject().Use();
            //}
        }
        public override void Interact(IRoomObjectParent roomObjectParent)
        {
            // this case should not happened but just in case
            if (roomObjectParent == null)
                return;

            if (HasRoomObject()) {
                TryToGiveRoomObject(roomObjectParent);
            }
            else {
                TryToReceiveRoomObject(roomObjectParent);
            }
        }

        private void TryToGiveRoomObject(IRoomObjectParent roomObjectParent)
        {
            // we cannot assign it
            if (roomObjectParent.HasRoomObject())
                return;

            // assign object to new parent
            GetRoomObject().SetRoomObjectParent(roomObjectParent);
        }
        private void TryToReceiveRoomObject(IRoomObjectParent roomObjectParent)
        {
            // nothing to receive
            if (!roomObjectParent.HasRoomObject())
                return;

            // receive room object
            roomObjectParent.GetRoomObject().SetRoomObjectParent(this);
        }
    }
}
