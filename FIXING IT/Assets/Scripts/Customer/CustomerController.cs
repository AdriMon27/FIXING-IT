using FixingIt.RoomObjects;
using ProgramadorCastellano.Funcs;
using UnityEngine;
using UnityEngine.AI;

namespace FixingIt.Customer
{
    public class CustomerController : MonoBehaviour, IRoomObjectParent
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

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();

            _clientState = ClientState.Waiting;
        }

        private void Start()
        {
            GoToCounter();
        }

        private void Update()
        {
            switch (_clientState)
            {
                case ClientState.Waiting:
                    break;
                case ClientState.GoingToCounter:
                    if (_agent.remainingDistance < _agent.stoppingDistance)
                    {
                        _clientState = ClientState.Waiting;
                        _roomObject.SetRoomObjectParent(_parentToLeaveBrokenObject);
                        //Invoke(nameof(LeaveCounter), 5f);
                    }
                    break;
                case ClientState.LeavingCounter:
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

            RoomObject.SpawnRoomObject(_objectToFixSO, this);
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
        #endregion
    }
}
