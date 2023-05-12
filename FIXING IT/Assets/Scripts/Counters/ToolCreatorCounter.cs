using FixingIt.Minigame.RoomObject;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FixingIt.Counters
{
    public class ToolCreatorCounter : BaseCounter
    {
        [SerializeField] private Transform[] _piecesTransform;
        private List<RoomObjectSO> _piecesSO;

        private void Awake()
        {
            _piecesSO = new List<RoomObjectSO>();
        }

        public override void AlternateInteract()
        {
            Debug.Log("Trying to create a new tool");
        }

        public override void Interact(IRoomObjectParent roomObjectParent)
        {
            if (roomObjectParent == null)
                return;

            if (HasRoomObject()) {
                // try to give tool to roomObjectParent
            }
            else {
                TryToReceivePiece(roomObjectParent);
            }
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
            roomObjectParent.ClearRoomObject();
            pieceToSet.Broke();
        }
    }
}
