using FixingIt.ActorComponents;
using FixingIt.RoomObjects.Logic;
using FixingIt.RoomObjects.SO;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

namespace FixingIt.Customer
{
    public class CustomerController : NetworkBehaviour, IRoomObjectParent
    {
        // could have be done with a state machine but this case is too simple
        private enum ClientState
        {
            Waiting,
            GoingToCounter,
            LeavingCounter
        }

        [SerializeField] Transform _holdingPoint;

        private NavMeshAgent _agent;
        private ClientState _clientState;

        private RoomObject _roomObject;

        private Transform _startTransform;
        private IRoomObjectParent _parentToLeaveBrokenObject;

        [Header("Customer Comps")]
        [SerializeField]
        private AudioComponent _audioComp;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();

            _clientState = ClientState.Waiting;
        }

        private void Start()
        {
            if (!IsServer)
                return;

            GoToCounter();
        }

        private void Update()
        {
            if (!IsServer)
                return;

            switch (_clientState)
            {
                case ClientState.Waiting:
                    break;
                case ClientState.GoingToCounter:
                    _audioComp.PlaySound();
                    if (_agent.remainingDistance < _agent.stoppingDistance)
                    {
                        _clientState = ClientState.Waiting;
                        _roomObject.SetRoomObjectParent(_parentToLeaveBrokenObject);

                        _audioComp.StopSound();
                    }
                    break;
                case ClientState.LeavingCounter:
                    _audioComp.PlaySound();
                    if (_agent.remainingDistance < _agent.stoppingDistance)
                    {
                        // si da tiempo cambiarlo por un sistema de pooling
                        Destroy(gameObject);
                    }
                    break;
                default:
                    Debug.LogWarning($"{_clientState} is not implemented");
                    break;
            }
        }

        private void GoToCounter()
        {
            Debug.Log(_parentToLeaveBrokenObject != null);
            _agent.SetDestination(_parentToLeaveBrokenObject.transform.position);
            _clientState = ClientState.GoingToCounter;
        }

        public void LeaveCounter()
        {
            _agent.SetDestination(_startTransform.position);
            _clientState = ClientState.LeavingCounter;
        }

        public void InitCustomer(Transform startTransform, IRoomObjectParent parentToLeaveBrokenObject, RoomObjectSO _objectToFixSO)
        {
            _startTransform = startTransform;
            _parentToLeaveBrokenObject = parentToLeaveBrokenObject;

            transform.position = _startTransform.position;
            transform.rotation = _startTransform.rotation;

            //RoomObject.SpawnRoomObject(_objectToFixSO, this);
        }

        #region RoomObjectParent
        public Transform GetRoomObjectTransform()
        {
            return _holdingPoint;
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

        public NetworkObject GetNetworkObject()
        {
            return NetworkObject;
        }
        #endregion
    }
}
