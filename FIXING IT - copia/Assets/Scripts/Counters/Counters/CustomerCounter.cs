using FixingIt.Customer;
using FixingIt.Events;
using FixingIt.RoomObjects.Logic;
using Unity.Netcode;
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

            InteractServerRpc(roomObjectParent.GetNetworkObject());
            //if (HasRoomObject()) {  // customer has left an objectToFix
            //    TryToGiveObjectToFix(roomObjectParent);
            //}
            //else {
            //    // mirar si tenemos objeto a arreglar asignado
            //    if (_customerAssigned == null) // we are waiting for next customer
            //        return;

            //    // intentar entregar objeto arreglado
            //    TryToReceiveObjectFixed(roomObjectParent);
            //}
        }

        [ServerRpc(RequireOwnership = false)]
        private void InteractServerRpc(NetworkObjectReference roomObjecParentNORef)
        {
            roomObjecParentNORef.TryGet(out NetworkObject roomObjectParentNO);
            IRoomObjectParent roomObjectParent = roomObjectParentNO.GetComponent<IRoomObjectParent>();

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
                ConfusedClientRpc(_customerAssigned.GetNetworkObject());
                return;
            }

            ToFixRoomObject objectToFix = roomObject as ToFixRoomObject;
            if (!objectToFix.IsFixed) {
                ConfusedClientRpc(_customerAssigned.GetNetworkObject());
                return;
            }

            // give to object fixed to customer
            roomObjectParent.GetRoomObject().SetRoomObjectParent(_customerAssigned);
            _customerAssigned.LeaveCounter();

            NetworkObject customerNO = _customerAssigned.GetNetworkObject();

            _customerAssigned = null;
            
            // send event object given back to customer
            CustomerWithObjectFixedClientRpc(customerNO);
        }

        [ClientRpc]
        private void ConfusedClientRpc(NetworkObjectReference customerNORef)
        {
            customerNORef.TryGet(out NetworkObject customerNO);
            CustomerController customer = customerNO.GetComponent<CustomerController>();

            _confusedRoomObjectParentEvent.RaiseEvent(customer);
        }

        [ClientRpc]
        private void CustomerWithObjectFixedClientRpc(NetworkObjectReference customerNORef)
        {
            customerNORef.TryGet(out NetworkObject customerNO);
            CustomerController customer = customerNO.GetComponent<CustomerController>();

            _customerWithObjectFixedEvent.RaiseEvent(customer);
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
