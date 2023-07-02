using FixingIt.ActorComponents;
using FixingIt.Events;
using FixingIt.RoomObjects.SO;
using ProgramadorCastellano.Events;
using Unity.Netcode;
using UnityEngine;

namespace FixingIt.RoomObjects.Logic
{
    [RequireComponent(typeof(FollowTransformComponent))]
    public class RoomObject : NetworkBehaviour
    {
        private static TryToSpawnRoomObjectChannelSO _staticTryToSpawnRoomObjectEvent;
        public static Transform StaticInSceneTransform { get; private set; }

        [SerializeField] private RoomObjectSO _roomObjectSO;
        [SerializeField] private NetworkVariable<int> _numberOfUses = new NetworkVariable<int>(1);

        [Header("Components")]
        [SerializeField] private AudioComponent _roomObjectUsedAudioComp;
        [SerializeField] private AudioComponent _roomObjectBrokenAudioComp;
        private FollowTransformComponent _followTransformComp;

        [Header("Broadcasting To")]
        //[SerializeField]
        //private VoidEventChannelSO _roomObjectUsedEvent;
        [SerializeField]
        private VoidEventChannelSO _roomObjectBrokenAfterUseEvent;
        [SerializeField]
        private TryToSpawnRoomObjectChannelSO _tryToSpawnRoomObjectEvent;   // its value should be the same among all the roomObjects

        private IRoomObjectParent _roomObjectParent;

        public RoomObjectSO RoomObjectSO => _roomObjectSO;
        public GameObject RoomObjectVisualPrefab => transform.GetChild(0).gameObject;

        protected virtual void Awake()
        {
            _followTransformComp = GetComponent<FollowTransformComponent>();
        }

        private void Start()
        {
            if (_tryToSpawnRoomObjectEvent != null) { 
                _staticTryToSpawnRoomObjectEvent = _tryToSpawnRoomObjectEvent;
                StaticInSceneTransform = transform;
            }
        }

        public void SetRoomObjectParent(IRoomObjectParent newRoomObjectParent)
        {
            SetRoomObjectParentServerRpc(newRoomObjectParent.GetNetworkObject());
        }

        [ServerRpc(RequireOwnership = false)]
        private void SetRoomObjectParentServerRpc(NetworkObjectReference newRoomObjectParentNORef)
        {
            transform.parent = StaticInSceneTransform;
            SetRoomObjectParentClientRpc(newRoomObjectParentNORef);
        }

        [ClientRpc]
        private void SetRoomObjectParentClientRpc(NetworkObjectReference newRoomObjectParentNORef)
        {
            newRoomObjectParentNORef.TryGet(out NetworkObject newRoomObjectParentNO);
            IRoomObjectParent newRoomObjectParent = newRoomObjectParentNO.GetComponent<IRoomObjectParent>();

            // clear parent info
            _roomObjectParent?.ClearRoomObject();

            // set new parent
            _roomObjectParent = newRoomObjectParent;

            if (newRoomObjectParent.HasRoomObject())
            {
                Debug.LogError("IRoomObjectParent already has a RoomObject");
            }

            newRoomObjectParent.SetRoomObject(this);

            // set transform
            _followTransformComp.SetTargetTransform(newRoomObjectParent.GetRoomObjectTransform());
        }

        public void Use()
        {
            _numberOfUses.Value--;

            if (_numberOfUses.Value <= 0)
            {
                _roomObjectBrokenAfterUseEvent.RaiseEvent();

                //if (_roomObjectBrokenAudioComp == null) {
                //    Debug.LogWarning("Only Tools should be broken");
                //}
                //else {
                //    _roomObjectBrokenAudioComp.PlaySound();
                //}
                Broke();
            }
            else
            {
                if (_roomObjectUsedAudioComp == null)
                {
                    Debug.LogWarning("Only Tools should be used");
                }
                else
                {
                    _roomObjectUsedAudioComp.PlaySound(false);
                }
            }
        }

        // TODO: cambiar por sistema de pooling como extra
        public void Broke()
        {
            _roomObjectParent.ClearRoomObject();

            Destroy(gameObject);
        }

        #region Static
        // TODO: cambiar por sistema de pooling como extra
        public static void SpawnRoomObject(RoomObjectSO roomObjectSO, IRoomObjectParent roomObjectParent)
        {
            _staticTryToSpawnRoomObjectEvent.RaiseEvent(roomObjectSO, roomObjectParent.GetNetworkObject());
        }
        #endregion
    }
}
