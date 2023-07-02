using FixingIt.ActorComponents;
using FixingIt.Events;
using FixingIt.Funcs;
using FixingIt.RoomObjects.Logic;
using FixingIt.RoomObjects.SO;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace FixingIt.Counters
{
    public class ToolCreatorCounter : BaseCounter
    {
        [SerializeField] private Transform[] _piecesTransform;

        [Header("Components")]
        [SerializeField] private AudioComponent _audioComp;
        [SerializeField] private ParticleSystem _particleSystem;

        [Header("Broadcasting To")]
        [SerializeField]
        private RoomObjectParentChannelSO _confusedRoomObjectParentEvent;
        //[SerializeField]
        //private VoidEventChannelSO _toolCreatedEvent;

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
            //RoomObjectSO toolCreated = _toolRecipeManagerSO.TryRecipe(_piecesSO);

            //if (toolCreated == null) {
            //    ClearPieces();
            //    _confusedRoomObjectParentEvent.RaiseEvent(roomObjectParent);
            //}
            //else {
            //    RoomObject.SpawnRoomObject(toolCreated, this);
            //    ClearPieces();
            //    _audioComp.PlaySound();
            //    _particleSystem.Play();
            //}

            TryToCraftToolServerRpc(roomObjectParent.GetNetworkObject());
        }

        [ServerRpc(RequireOwnership = false)]
        private void TryToCraftToolServerRpc(NetworkObjectReference roomObjectParentNORef)
        {
            // the Server checks the recipe just in case a client is cheating
            RoomObjectSO toolCreated = _toolRecipeManagerSO.TryRecipe(_piecesSO);

            if (toolCreated == null) {
                CraftFailedClientRpc(roomObjectParentNORef);
            }
            else {
                RoomObject.SpawnRoomObject(toolCreated, this);
                //int toolCreatedSOIndex = _getRoomObjectSOIndexFunc.RaiseFunc(toolCreated);
                CraftAchievedClientRpc(roomObjectParentNORef);
            }
        }

        [ClientRpc]
        private void CraftFailedClientRpc(NetworkObjectReference roomObjectParentNORef)
        {
            roomObjectParentNORef.TryGet(out NetworkObject roomObjectParentNO);
            IRoomObjectParent roomObjectParent = roomObjectParentNO.GetComponent<IRoomObjectParent>();

            ClearPieces();
            _confusedRoomObjectParentEvent.RaiseEvent(roomObjectParent);
        }

        [ClientRpc]
        private void CraftAchievedClientRpc(NetworkObjectReference roomObjectParentNORef)
        {
            ClearPieces();
            _audioComp.PlaySound();
            _particleSystem.Play();
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

            ReceivePieceServerRpc(roomObjectParent.GetNetworkObject());
        }

        [ServerRpc(RequireOwnership = false)]
        private void ReceivePieceServerRpc(NetworkObjectReference roomObjectParentNORef)
        {
            ReceivePieceClientRpc(roomObjectParentNORef);
        }

        [ClientRpc]
        private void ReceivePieceClientRpc(NetworkObjectReference roomObjectParentNORef)
        {
            roomObjectParentNORef.TryGet(out NetworkObject roomObjectParentNO);
            IRoomObjectParent roomObjectParent = roomObjectParentNO.GetComponent<IRoomObjectParent>();

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
