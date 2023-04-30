using UnityEngine;
using UnityEngine.AddressableAssets;

namespace FixingIt.SceneManagement
{
    [CreateAssetMenu()]
    public class GameSceneSO : DescriptionBaseSO
    {
        [SerializeField] private AssetReference _sceneReference;
        public AssetReference SceneReference => _sceneReference;
    }
}