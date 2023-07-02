using UnityEngine;

namespace FixingIt.RoomObjects.SO
{
    [CreateAssetMenu()]
    public class RoomObjectSO : ScriptableObject
    {
        public enum RoomObjectType
        {
            Piece,
            Tool,
            ObjectToFix
        }

        [SerializeField] private string _objectName;
        [SerializeField] private RoomObjectType _type;
        [SerializeField] private GameObject _roomObjetPrefab;
        [SerializeField] private Sprite _sprite;

        public string ObjectName => _objectName;
        public RoomObjectType Type => _type;
        public GameObject RoomObjectPrefab => _roomObjetPrefab;
        public Sprite Sprite => _sprite;
    }
}
