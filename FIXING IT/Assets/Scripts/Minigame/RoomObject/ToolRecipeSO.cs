using UnityEngine;

namespace FixingIt.Minigame.RoomObject
{
    [CreateAssetMenu()]
    public class ToolRecipeSO : ScriptableObject
    {
        [SerializeField] private RoomObjectSO[] _pieces;
        [SerializeField] private RoomObjectSO _output;

        public RoomObjectSO[] Pieces => _pieces;
        public RoomObjectSO Output => _output;
    }
}
