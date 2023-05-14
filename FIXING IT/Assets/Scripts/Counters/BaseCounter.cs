using FixingIt.Minigame.RoomObject;
using UnityEngine;

namespace FixingIt.Counters
{
    public abstract class BaseCounter : MonoBehaviour, IRoomObjectParent
    {
        [SerializeField] private Transform _topPoint;

        private RoomObject _roomObject;

        public abstract void Interact(IRoomObjectParent roomObjectParent);
        public abstract void AlternateInteract(IRoomObjectParent roomObjectParent);

        public Transform GetRoomObjectTransform()
        {
            return _topPoint;
        }

        public RoomObject GetRoomObject()
        {
            return _roomObject;
        }

        public void SetRoomObject(RoomObject roomObject)
        {
            _roomObject = roomObject;
        }

        public bool HasRoomObject()
        {
            return _roomObject != null;
        }

        public void ClearRoomObject()
        {
            _roomObject = null;
        }
    }
}
