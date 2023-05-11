using UnityEngine;

namespace FixingIt.Minigame.RoomObject
{
    public class RoomObject : MonoBehaviour
    {
        [SerializeField] private RoomObjectSO _roomObjectSO;
        [SerializeField] private int _numberOfUses = 1;
        
        private IRoomObjectParent _roomObjectParent;

        public void SetRoomObjectParent(IRoomObjectParent newRoomObjectParent)
        {
            // clear parent info
            _roomObjectParent?.ClearRoomObject();

            // set new parent
            _roomObjectParent = newRoomObjectParent;

            if (newRoomObjectParent.HasRoomObject()) {
                Debug.LogError("IRoomObjectParent already has a RoomObject");
            }

            newRoomObjectParent.SetRoomObject(this);

            // set transform
            transform.parent = newRoomObjectParent.GetRoomObjectTransform();
            transform.localPosition = Vector3.zero;
        }
    }
}
