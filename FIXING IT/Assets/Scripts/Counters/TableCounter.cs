using FixingIt.Minigame.RoomObject;
using UnityEngine;

namespace FixingIt.Counters
{
    public class TableCounter : BaseCounter
    {
        //[SerializeField] private RoomObjectSO _testRoomObjectSO;

        //private void Start()
        //{
        //    var go = Instantiate(_testRoomObjectSO.RoomObjectPrefab);

        //    go.GetComponent<RoomObject>().SetRoomObjectParent(this);
        //}

        public override void AlternateInteract()
        {
            return;
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
                // try to receive roomObject from roomObjectParent
                TryToReceiveRoomObject(roomObjectParent);
            }

            Debug.Log($"{roomObjectParent} has interacted with {name}, topPoint at {GetRoomObjectTransform().position} and RoomObject: {GetRoomObject()}");
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
