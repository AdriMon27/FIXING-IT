using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu()]
public class GameSceneSO : DescriptionBaseSO
{
    [SerializeField] private AssetReference _sceneReference;
    public AssetReference SceneReference => _sceneReference;
}
