using FixingIt.Counters;
using FixingIt.InputSystem;
using UnityEngine;

namespace FixingIt.PlayerGame
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] InputReaderSO _inputReaderSO;

        [Header("Player Comps")]
        [SerializeField] private PlayerAnimationComp _animationComp;

        [Header("Player Stats")]
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _rotateSpeed = 10f;
        [SerializeField] private float _interactDistance = 2f;
        [SerializeField] private LayerMask _countersLayerMask;

        private Vector2 _direction;

        private void OnEnable()
        {
            _inputReaderSO.MoveEvent += SetPlayerDirection;
            _inputReaderSO.InteractEvent += HandleInteraction;
        }

        private void OnDisable()
        {
            _inputReaderSO.MoveEvent -= SetPlayerDirection;
            _inputReaderSO.InteractEvent -= HandleInteraction;
        }

        private void Update()
        {
            HandleMovement();
            HandleRotation();
        }

        private void HandleMovement()
        {
            Vector3 velocity = new Vector3(_direction.x, 0f, _direction.y);
            velocity *= _moveSpeed;

            transform.position += velocity * Time.deltaTime;

            _animationComp.SetIsWalking(velocity != Vector3.zero);
        }

        private void HandleRotation()
        {
            Vector3 desiredRotation = new Vector3(_direction.x, 0f, _direction.y);

            transform.forward = Vector3.Slerp(transform.forward, desiredRotation, Time.deltaTime * _rotateSpeed);
        }

        private void HandleInteraction()
        {
            Vector3 rayOrigin = transform.position; // origin in floor
            Vector3 rayDir = transform.forward;

            if (Physics.Raycast(rayOrigin, rayDir, out RaycastHit rayHit, _interactDistance, _countersLayerMask)) {
                if (rayHit.transform.TryGetComponent(out BaseCounter baseCounter)) {
                    baseCounter.Interact();
                }
            }

            Debug.DrawRay(rayOrigin, rayDir * _interactDistance, Color.red, 5f);
        }

        private void SetPlayerDirection(Vector2 directionInput)
        {
            _direction = directionInput;
        }
    }
}
