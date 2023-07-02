using Unity.Netcode;
using UnityEngine;

namespace FixingIt.PlayerGame
{
    [RequireComponent(typeof(Animator))]
    public class PlayerAnimationComp : NetworkBehaviour
    {
        private const string IS_WALKING = "IsWalking";

        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void SetIsWalking(bool isWalking)
        {
            if (!IsOwner) {
                return;
            }

            _animator.SetBool(IS_WALKING, isWalking);
        }
    }
}
