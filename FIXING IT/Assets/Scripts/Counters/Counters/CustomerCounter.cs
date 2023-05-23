using FixingIt.Customer;
using FixingIt.Events;
using FixingIt.RoomObjects;
using UnityEngine;

namespace FixingIt.Counters
{
    public class CustomerCounter : BaseCounter
    {
        [Header("Broadcasting To")]
        [SerializeField]
        private RoomObjectParentChannelSO _confusedRoomObjectParentEvent;
        [SerializeField]
        private RoomObjectParentChannelSO _customerWithObjectFixedEvent;

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

            if (roomObject is not ToFixRoomObject) {
                _confusedRoomObjectParentEvent.RaiseEvent(_customerAssigned);
                return;
            }

            ToFixRoomObject objectToFix = roomObject as ToFixRoomObject;
            if (!objectToFix.IsFixed) {
                _confusedRoomObjectParentEvent.RaiseEvent(_customerAssigned);
                return;
            }

            // give to object fixed to customer
            roomObjectParent.GetRoomObject().SetRoomObjectParent(_customerAssigned);
            _customerAssigned.LeaveCounter();

            // send event object given back to customer
            _customerWithObjectFixedEvent.RaiseEvent(_customerAssigned);
            
            _customerAssigned = null;
        }

        public void SetCustomerAssigned(CustomerController customerAssigned)
        {
            _customerAssigned = customerAssigned;
        }

        public bool HasCustomerAssigned()
        {
            return _customerAssigned != null;
        }
    }
}
