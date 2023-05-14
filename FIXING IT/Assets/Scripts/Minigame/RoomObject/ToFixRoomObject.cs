using System.Linq;
using UnityEngine;

namespace FixingIt.Minigame.RoomObject
{
    public class ToFixRoomObject : RoomObject
    {
        [SerializeField] private RoomObjectSO[] _toolsToBeFixedSO;
        private bool[] _toolsUsedSO;

        public bool IsFixed => _toolsUsedSO.All(valor => valor);

        private void Awake()
        {
            _toolsUsedSO = new bool[_toolsToBeFixedSO.Length];
        }

        private void FixObject()
        {
            // change visual?

            Debug.Log("FixObject");
        }

        public bool TryToFix(RoomObjectSO toolUsed)
        {
            // should be check from outside but just in case
            if (IsFixed) {
                Debug.Log("cannot fix a object that is already fixed");
                return false;
            }

            for (int i = 0; i < _toolsToBeFixedSO.Length; i++) {
                if (_toolsUsedSO[i])
                    continue;

                if (toolUsed == _toolsToBeFixedSO[i]) {
                    _toolsUsedSO[i] = true;
                    break;
                }
            }

            if (!IsFixed)
                return false;

            FixObject();
            return true;
        }
    }
}
