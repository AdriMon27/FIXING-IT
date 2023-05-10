using UnityEngine;

namespace FixingIt.PlayerGame
{
    [RequireComponent(typeof(Animator))]
    public class PlayerAnimationComp : MonoBehaviour
    {
        private const string IS_WALKING = "IsWalking";

        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void SetIsWalking(bool isWalking)
        {
            _animator.SetBool(IS_WALKING, isWalking);
        }
    }
}
