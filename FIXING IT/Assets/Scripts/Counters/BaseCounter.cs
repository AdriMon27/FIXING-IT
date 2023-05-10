using UnityEngine;

namespace FixingIt.Counters
{
    public abstract class BaseCounter : MonoBehaviour
    {
        public abstract void Interact();
        public abstract void AlternateInteract();
    }
}
