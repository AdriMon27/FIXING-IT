using FixingIt.InputSystem;
using UnityEngine;

namespace FixingIt.PlayerGame
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] InputReaderSO _inputReaderSO;

        [Header("Player Stats")]
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _rotateSpeed = 10f;

        private Vector2 _direction;

        private void OnEnable()
        {
            _inputReaderSO.MoveEvent += SetPlayerDirection;
        }

        private void OnDisable()
        {
            _inputReaderSO.MoveEvent -= SetPlayerDirection;
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
        }

        private void HandleRotation()
        {
            Vector3 desiredRotation = new Vector3(_direction.x, 0f, _direction.y);

            transform.forward = Vector3.Slerp(transform.forward, desiredRotation, Time.deltaTime * _rotateSpeed);
        }

        private void SetPlayerDirection(Vector2 directionInput)
        {
            _direction = directionInput;
        }
    }
}
