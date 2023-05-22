using UnityEngine;

namespace FixingIt.Settings
{
    [CreateAssetMenu(menuName = "Audio/AudioClipsSO")]
    public class AudioClipsSO : ScriptableObject
    {
        [SerializeField] AudioClip _objectFixed;
        [SerializeField] AudioClip _objectFixing;
        [SerializeField] AudioClip _toolCreated;
        [SerializeField] AudioClip _pieceCounterUsed;
        [SerializeField] AudioClip _roomObjectUsed;
        [SerializeField] AudioClip _roomObjectBroken;

        public AudioClip ObjectFixed => _objectFixed;
        public AudioClip ObjectFixing => _objectFixing;
        public AudioClip ToolCreated => _toolCreated;
        public AudioClip PieceCounterUsed => _pieceCounterUsed;
        public AudioClip RoomObjectUsed => _roomObjectUsed;
        public AudioClip RoomObjectBroken => _roomObjectBroken;
    }
}
