using FixingIt.RoomObjects;
using UnityEngine;

namespace FixingIt.Counters
{
    public class TableCounter : BaseCounter
    {
        public override void AlternateInteract(IRoomObjectParent roomObjectParent)
        {
            if (roomObjectParent == null)
                return;

            if (!HasRoomObject()
                || GetRoomObject().RoomObjectSO.Type != RoomObjectSO.RoomObjectType.ObjectToFix) {
                // lanzar evento jugador confuso

                return;
            }

            if (!roomObjectParent.HasRoomObject()
                || roomObjectParent.GetRoomObject().RoomObjectSO.Type == RoomObjectSO.RoomObjectType.ObjectToFix) {
                // lanzar evento jugador confuso

                return;
            }

            if (GetRoomObject() is not ToFixRoomObject) {
                Debug.LogError($"{GetRoomObject().RoomObjectSO} is of type {RoomObjectSO.RoomObjectType.ObjectToFix} " +
                    $"but the prefab does not inheret from {typeof(ToFixRoomObject)}");
                return;
            }

            // check if roomObjectParent.RoomObject is inside necesaryTools to fix object
            ToFixRoomObject toFixRoomObject = (GetRoomObject() as ToFixRoomObject);
            if (toFixRoomObject.TryToFix(roomObjectParent.GetRoomObject().RoomObjectSO, out bool toolBeenUsed)) {
                // send event object fixed

                Debug.Log("object fixed");
            }
            else {
                // send event fixing
                Debug.Log("Trying to fix object");
            }

            if (toolBeenUsed) { 
                roomObjectParent.GetRoomObject().Use();
            }
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