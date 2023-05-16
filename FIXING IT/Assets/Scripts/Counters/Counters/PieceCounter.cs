using FixingIt.RoomObjects;
using UnityEngine;

namespace FixingIt.Counters
{
    public class PieceCounter : BaseCounter
    {
        [SerializeField] private RoomObjectSO _roomObjectSO;

        [Header("Components")]
        [SerializeField] private PieceCounterVisualComp _pieceCounterVisualComp;

        private void Start()
        {
            _pieceCounterVisualComp.SetSprite(_roomObjectSO.Sprite);
        }

        public override void AlternateInteract(IRoomObjectParent roomObjectParent)
        {
            return;
        }

        public override void Interact(IRoomObjectParent roomObjectParent)
        {
            if (roomObjectParent == null)
                return;

            if (roomObjectParent.HasRoomObject())
                return;

            RoomObject.SpawnRoomObject(_roomObjectSO, roomObjectParent);
        }
    }
}
