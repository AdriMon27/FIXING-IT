using FixingIt.Customer;
using FixingIt.RoomObjects;
using UnityEngine;

namespace FixingIt.Counters
{
    public class CustomerCounter : BaseCounter
    {
        private CustomerController _customerAssigned;

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
                // mirar si tenemos objeto a arreglar asignado
                if (_customerAssigned == null) // we are waiting for next customer
                    return;

                // intentar entregar objeto arreglado
                TryToReceiveObjectFixed(roomObjectParent);
            }

            Debug.Log("interacting with a CustomerCounter");
        }

        private void TryToGiveObjectToFix(IRoomObjectParent roomObjectParent)
        {
            if (roomObjectParent.HasRoomObject())
                return;

            GetRoomObject().SetRoomObjectParent(roomObjectParent);
        }
        private void TryToReceiveObjectFixed(IRoomObjectParent roomObjectParent)
        {
            RoomObject roomObject = roomObjectParent.GetRoomObject();

            if (roomObject is not ToFixRoomObject)
                return;

            ToFixRoomObject objectToFix = roomObject as ToFixRoomObject;
            if (!objectToFix.IsFixed)
                return;

            // give to object fixed to customer
            roomObjectParent.GetRoomObject().SetRoomObjectParent(_customerAssigned);
            _customerAssigned.LeaveCounter();
            _customerAssigned = null;

            // send event object given back to customer

        }

        public void SetCustomerAssigned(CustomerController customerAssigned)
        {
            _customerAssigned = customerAssigned;
        }
    }
}
