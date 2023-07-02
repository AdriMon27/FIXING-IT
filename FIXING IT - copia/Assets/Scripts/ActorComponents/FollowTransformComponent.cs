using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FixingIt.ActorComponents
{
    public class FollowTransformComponent : MonoBehaviour
    {
        private Transform _targetTransform;

        public void SetTargetTransform(Transform newTarget)
        {
            _targetTransform = newTarget;
        }

        private void LateUpdate()
        {
            if (_targetTransform == null) {
                return;
            }

            transform.position = _targetTransform.position;
            transform.rotation = _targetTransform.rotation;
        }
    }
}
