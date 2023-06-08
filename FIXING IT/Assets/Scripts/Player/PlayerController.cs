using FixingIt.ActorComponents;
using FixingIt.Counters;
using FixingIt.InputSystem;
using FixingIt.RoomObjects;
using Unity.Netcode;
using UnityEngine;

namespace FixingIt.PlayerGame
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : NetworkBehaviour, IRoomObjectParent
    {
        [SerializeField] InputReaderSO _inputReaderSO;

        [SerializeField] private Transform _holdingPoint;

        [Header("Player Comps")]
        [SerializeField] private PlayerAnimationComp _animationComp;
        [SerializeField] private AudioComponent _audioComp;

        [Header("Player Stats")]
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _rotateSpeed = 10f;
        [SerializeField] private float _interactDistance = 2f;
        [SerializeField] private LayerMask _countersLayerMask;

        private Rigidbody _rb;
        private RoomObject _roomObject;
        private Vector2 _direction;

        private Outline _currentOutline;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        private void OnEnable()
        {
            // se maneja dentro de las funciones para evitar una condición de carrera
            //if (OwnerClientId == NetworkManager.LocalClientId)
            //    Debug.Log("ofnesoinfie");
            //if (!IsOwner) {
            //    return;
            //}

            _inputReaderSO.MoveEvent += SetPlayerDirection;
            _inputReaderSO.InteractEvent += HandleInteraction;
            _inputReaderSO.AlternateInteractEvent += HandleAlternateInteraction;
        }

        private void OnDisable()
        {
            // si se deja ocurría una condición de carrera que bloqueaba el personaje
            //if (!IsOwner) {
            //    return;
            //}

            _inputReaderSO.MoveEvent -= SetPlayerDirection;
            _inputReaderSO.InteractEvent -= HandleInteraction;
            _inputReaderSO.AlternateInteractEvent -= HandleAlternateInteraction;
        }

        private void FixedUpdate()
        {
            if (!IsOwner) {
                return;
            }

            HandleMovement();
        }

        private void Update()
        {
            if (!IsOwner) {
                return;
            }

            HandleRotation();
            HandleSelectionOutline();
        }

        private void HandleSelectionOutline()
        {
            Vector3 rayOrigin = transform.position;
            Vector3 rayDir = transform.forward;

            if (Physics.Raycast(rayOrigin, rayDir, out RaycastHit rayHit, _interactDistance, _countersLayerMask)) {
                if (rayHit.transform.TryGetComponent(out Outline outline)) {
                    if (_currentOutline == outline) {
                        return;
                    }

                    if (_currentOutline != null) {
                        _currentOutline.enabled = false;
                    }

                    _currentOutline = outline;
                    _currentOutline.enabled = true;
                }
                else {
                    if (_currentOutline == null) { 
                        return;
                    }

                    _currentOutline.enabled = false;
                    _currentOutline = null;
                }
            }
            else {
                if (_currentOutline == null) { 
                    return;
                }

                _currentOutline.enabled = false;
                _currentOutline = null;
            }
        }

        private void HandleMovement()
        {
            Vector3 velocity = new Vector3(_direction.x, 0f, _direction.y);
            velocity *= _moveSpeed;

            _rb.velocity = velocity;

            bool isMoving = velocity != Vector3.zero;
            _animationComp.SetIsWalking(isMoving);

            if (isMoving) {
                _audioComp.PlaySound();
            }
            else {
                _audioComp.StopSound();
            }
        }

        private void HandleRotation()
        {
            Vector3 desiredRotation = new Vector3(_direction.x, 0f, _direction.y);

            transform.forward = Vector3.Slerp(transform.forward, desiredRotation, Time.deltaTime * _rotateSpeed);
        }

        #region InputActions
        private void SetPlayerDirection(Vector2 directionInput)
        {
            if (!IsOwner) {
                return;
            }

            _direction = directionInput;
        }

        private void HandleInteraction()
        {
            if (!IsOwner) {
                return;
            }

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
            if (!IsOwner) {
                return;
            }

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
