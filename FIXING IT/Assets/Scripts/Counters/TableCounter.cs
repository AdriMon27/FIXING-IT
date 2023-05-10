using UnityEngine;

namespace FixingIt.Counters
{
    public class TableCounter : BaseCounter
    {
        public override void AlternateInteract()
        {
            return;
        }

        public override void Interact()
        {
            Debug.Log($"Interacted with {name}");
        }
    }
}
