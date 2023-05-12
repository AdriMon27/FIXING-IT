using UnityEngine;

namespace FixingIt.Minigame.RoomObject
{
    public class RoomObject : MonoBehaviour
    {
        [SerializeField] private RoomObjectSO _roomObjectSO;
        [SerializeField] private int _numberOfUses = 1;
        
        private IRoomObjectParent _roomObjectParent;

        public RoomObjectSO RoomObjectSO => _roomObjectSO;
        public GameObject RoomObjectVisualPrefab => transform.GetChild(0).gameObject;

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
            transform.localRotation = Quaternion.identity;
        }

        // TODO: cambiar por sistema de pooling como extra
        public void Broke()
        {
            Destroy(gameObject);
        }

        #region Static
        // TODO: cambiar por sistema de pooling como extra
        public static RoomObject SpawnRoomObject(RoomObjectSO roomObjectSO, IRoomObjectParent roomObjectParent)
        {
            GameObject roomObjectGO = Instantiate(roomObjectSO.RoomObjectPrefab);

            RoomObject roomObject = roomObjectGO.GetComponent<RoomObject>();
            roomObject.SetRoomObjectParent(roomObjectParent);

            return roomObject;
        }
        #endregion
    }
}
