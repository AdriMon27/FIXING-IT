using FixingIt.Minigame.RoomObject;
using System.Collections.Generic;
using UnityEngine;

namespace FixingIt.Counters
{
    public class ToolCreatorCounter : BaseCounter
    {
        [SerializeField] private ToolRecipeManagerSO _toolRecipeManagerSO;
        [SerializeField] private Transform[] _piecesTransform;
        private List<RoomObjectSO> _piecesSO;

        private void Awake()
        {
            _piecesSO = new List<RoomObjectSO>();
        }

        public override void AlternateInteract(IRoomObjectParent roomObjectParent)
        {
            if (HasRoomObject())
                return;

            TryToCraftTool();
        }

        public override void Interact(IRoomObjectParent roomObjectParent)
        {
            if (roomObjectParent == null)
                return;

            if (HasRoomObject()) {
                TryToGiveRoomObject(roomObjectParent);
            }
            else {
                TryToReceivePiece(roomObjectParent);
            }
        }

        private void TryToCraftTool()
        {
            RoomObjectSO toolCreated = _toolRecipeManagerSO.TryRecipe(_piecesSO);

            if (toolCreated == null) {
                ClearPieces();
            }
            else {
                RoomObject.SpawnRoomObject(toolCreated, this);
                ClearPieces();
            }
        }

        private void ClearPieces()
        {
            // delete all pieces visual
            foreach (Transform pieceTransform in _piecesTransform)
            {
                for (int i = pieceTransform.childCount - 1; i >= 0; i--)
                {
                    // Get the child transform
                    Transform childTransform = pieceTransform.GetChild(i);

                    // Destroy the child transform
                    DestroyImmediate(childTransform.gameObject);
                }
            }

            // delete all pieces logic
            _piecesSO.Clear();
        }

        private void TryToGiveRoomObject(IRoomObjectParent roomObjectParent)
        {
            if (roomObjectParent.HasRoomObject())
                return;

            GetRoomObject().SetRoomObjectParent(roomObjectParent);
        }

        private void TryToReceivePiece(IRoomObjectParent roomObjectParent)
        {
            if (!roomObjectParent.HasRoomObject())
                return;

            if (roomObjectParent.GetRoomObject().RoomObjectSO.Type != RoomObjectSO.RoomObjectType.Piece)
                return;

            if (_piecesSO.Count >= _piecesTransform.Length)
                return;

            // receive piece
            RoomObject pieceToSet = roomObjectParent.GetRoomObject();

            // set piece inside _pieces
            _piecesSO.Add(pieceToSet.RoomObjectSO);

            // set visual
            GameObject visualPiece = pieceToSet.RoomObjectVisualPrefab;

            Instantiate(visualPiece, _piecesTransform[_piecesSO.Count - 1]);

            // clear piece from roomObjectParent
            // roomObjectParent.ClearRoomObject() is managed by pieceToSet.Broke
            pieceToSet.Broke(); 
        }
    }
}
