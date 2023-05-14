using FixingIt.Counters;
using FixingIt.InputSystem;
using FixingIt.Minigame.RoomObject;
using UnityEngine;

namespace FixingIt.PlayerGame
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour, IRoomObjectParent
    {
        [SerializeField] InputReaderSO _inputReaderSO;

        [SerializeField] private Transform _holdingPoint;

        [Header("Player Comps")]
        [SerializeField] private PlayerAnimationComp _animationComp;

        [Header("Player Stats")]
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _rotateSpeed = 10f;
        [SerializeField] private float _interactDistance = 2f;
        [SerializeField] private LayerMask _countersLayerMask;

        private Rigidbody _rb;
        private RoomObject _roomObject;
        private Vector2 _direction;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        private void OnEnable()
        {
            _inputReaderSO.MoveEvent += SetPlayerDirection;
            _inputReaderSO.InteractEvent += HandleInteraction;
            _inputReaderSO.AlternateInteractEvent += HandleAlternateInteraction;
        }

        private void OnDisable()
        {
            _inputReaderSO.MoveEvent -= SetPlayerDirection;
            _inputReaderSO.InteractEvent -= HandleInteraction;
            _inputReaderSO.AlternateInteractEvent -= HandleAlternateInteraction;
        }

        private void FixedUpdate()
        {
            HandleMovement();
        }

        private void Update()
        {
            HandleRotation();
        }

        private void HandleMovement()
        {
            Vector3 velocity = new Vector3(_direction.x, 0f, _direction.y);
            velocity *= _moveSpeed;

            //transform.position += velocity * Time.deltaTime;
            _rb.velocity = velocity;

            _animationComp.SetIsWalking(velocity != Vector3.zero);
        }

        private void HandleRotation()
        {
            Vector3 desiredRotation = new Vector3(_direction.x, 0f, _direction.y);

            transform.forward = Vector3.Slerp(transform.forward, desiredRotation, Time.deltaTime * _rotateSpeed);
        }

        #region InputActions
        private void SetPlayerDirection(Vector2 directionInput)
        {
            _direction = directionInput;
        }

        private void HandleInteraction()
        {
            Vector3 rayOrigin = transform.position;
            Vector3 rayDir = transform.forward;

            if (Physics.Raycast(rayOrigin, rayDir, out RaycastHit rayHit, _interactDistance, _countersLayerMask)) {
                if (rayHit.transform.TryGetComponent(out BaseCounter baseCounter)) {
                    baseCounter.Interact(this);
                }

                Debug.DrawRay(rayHit.point, rayDir * _interactDistance, Color.green, 5f);
            }

            Debug.DrawRay(rayOrigin, rayDir * _interactDistance, Color.red, 5f);
        }

        private void HandleAlternateInteraction()
        {
            Vector3 rayOrigin = transform.position;
            Vector3 rayDir = transform.forward;

            if (Physics.Raycast(rayOrigin, rayDir, out RaycastHit rayHit, _interactDistance, _countersLayerMask)) {
                if (rayHit.transform.TryGetComponent(out BaseCounter baseCounter)) {
                    baseCounter.AlternateInteract(this);
                }

                Debug.DrawRay(rayHit.point, rayDir * _interactDistance, Color.green, 5f);
            }

            Debug.DrawRay(rayOrigin, rayDir * _interactDistance, Color.blue, 5f);
        }
        #endregion

        #region IRoomObjectParent
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
