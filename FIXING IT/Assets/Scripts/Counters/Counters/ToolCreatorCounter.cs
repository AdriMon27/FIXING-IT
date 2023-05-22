using FixingIt.Events;
using FixingIt.Funcs;
using FixingIt.RoomObjects;
using ProgramadorCastellano.Events;
using System.Collections.Generic;
using UnityEngine;

namespace FixingIt.Counters
{
    public class ToolCreatorCounter : BaseCounter
    {
        [SerializeField] private Transform[] _piecesTransform;

        [Header("Broadcasting To")]
        [SerializeField]
        private RoomObjectParentChannelSO _confusedRoomObjectParentEvent;
        [SerializeField]
        private VoidEventChannelSO _toolCreatedEvent;

        [Header("Invoking Func")]
        [SerializeField]
        private ToolRecipeManagerFuncSO _getLevelToolRecipeManagerSOFunc;
        
        private ToolRecipeManagerSO _toolRecipeManagerSO;
        private List<RoomObjectSO> _piecesSO;

        private void Awake()
        {
            _piecesSO = new List<RoomObjectSO>();
        }

        private void Start()
        {
            _toolRecipeManagerSO = _getLevelToolRecipeManagerSOFunc.RaiseFunc();
        }

        public override void AlternateInteract(IRoomObjectParent roomObjectParent)
        {
            if (HasRoomObject())
                return;

            TryToCraftTool(roomObjectParent);
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

        private void TryToCraftTool(IRoomObjectParent roomObjectParent)
        {
            RoomObjectSO toolCreated = _toolRecipeManagerSO.TryRecipe(_piecesSO);

            if (toolCreated == null) {
                ClearPieces();
                _confusedRoomObjectParentEvent.RaiseEvent(roomObjectParent);
            }
            else {
                RoomObject.SpawnRoomObject(toolCreated, this);
                ClearPieces();
                _toolCreatedEvent.RaiseEvent();
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

            if (roomObjectParent.GetRoomObject().RoomObjectSO.Type != RoomObjectSO.RoomObjectType.Piece) {
                _confusedRoomObjectParentEvent.RaiseEvent(roomObjectParent);
                return;
            }

            if (_piecesSO.Count >= _piecesTransform.Length) {
                _confusedRoomObjectParentEvent.RaiseEvent(roomObjectParent);
                return;
            }

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
