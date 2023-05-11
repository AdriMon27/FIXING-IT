using FixingIt.Minigame.RoomObject;
using UnityEngine;

namespace FixingIt.Counters
{
    public class PieceCounter : BaseCounter
    {
        [SerializeField] private RoomObjectSO _roomObjectSO;

        public override void AlternateInteract()
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
