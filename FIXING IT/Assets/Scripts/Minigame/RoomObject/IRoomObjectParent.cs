using UnityEngine;

namespace FixingIt.Minigame.RoomObject
{
    public interface IRoomObjectParent
    {
        public Transform GetRoomObjectTransform();
        public RoomObject GetRoomObject();
        public void SetRoomObject(RoomObject roomObject);
        public bool HasRoomObject();
        public void ClearRoomObject();
    }
}
