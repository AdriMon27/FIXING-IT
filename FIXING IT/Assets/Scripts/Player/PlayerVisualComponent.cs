using UnityEngine;

namespace FixingIt.PlayerGame
{
    public class PlayerVisualComponent : MonoBehaviour
    {
        [SerializeField] private MeshRenderer _headMeshRenderer;
        [SerializeField] private MeshRenderer _bodyMeshRenderer;

        private Material _material;

        private void Awake()
        {
            _material = new Material(_headMeshRenderer.material);

            _headMeshRenderer.material = _material;
            _bodyMeshRenderer.material = _material;
        }

        public void SetPlayerColor(Color color)
        {
            _material.color = color;
        }
    }
}
