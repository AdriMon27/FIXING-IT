using UnityEngine;

namespace FixingIt.Counters
{
    public class PieceCounterVisualComp : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;

        public void SetSprite(Sprite sprite)
        {
            _spriteRenderer.sprite = sprite;
        }
    }
}
