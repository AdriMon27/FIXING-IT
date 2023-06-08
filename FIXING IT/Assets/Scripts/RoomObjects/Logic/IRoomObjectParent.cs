using Unity.Netcode;
using UnityEngine;

namespace FixingIt.RoomObjects.Logic
{
    public interface IRoomObjectParent
    {
        public Transform transform { get; }

        public Transform GetRoomObjectTransform();
        public RoomObject GetRoomObject();
        public void SetRoomObject(RoomObject roomObject);
        public bool HasRoomObject();
        public void ClearRoomObject();

        public NetworkObject GetNetworkObject();
    }
}
