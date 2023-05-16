using FixingIt.RoomObjects;
using UnityEngine;

namespace FixingIt.Counters
{
    public class CustomerCounter : BaseCounter
    {
        public override void AlternateInteract(IRoomObjectParent roomObjectParent)
        {
            return;
        }

        public override void Interact(IRoomObjectParent roomObjectParent)
        {
            if (roomObjectParent == null)
                return;

            if (HasRoomObject()) {  // customer has left an objectToFix
                TryToGiveObjectToFix(roomObjectParent);
            }
            else {
                // mirar si tenemos cliente asignado con objeto a arreglar

                // intentar entregar objeto arreglado

            }

            Debug.Log("interacting with a CustomerCounter");
        }

        private void TryToGiveObjectToFix(IRoomObjectParent roomObjectParent)
        {
            if (roomObjectParent.HasRoomObject())
                return;

            GetRoomObject().SetRoomObjectParent(roomObjectParent);
        }
    }
}
