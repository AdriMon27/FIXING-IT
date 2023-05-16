using System.Linq;
using UnityEngine;

namespace FixingIt.RoomObjects
{
    public class ToFixRoomObject : RoomObject
    {
        [SerializeField] private ToFixRoomObjectVisualComp _toFixRoomObjectVisualComp;
        [SerializeField] private RoomObjectSO[] _toolsToBeFixedSO;
        private bool[] _toolsUsedSO;

        public bool IsFixed => _toolsUsedSO.All(valor => valor);

        private void Awake()
        {
            _toolsUsedSO = new bool[_toolsToBeFixedSO.Length];
        }

        private void Start()
        {
            _toFixRoomObjectVisualComp.UpdateTFROVisual(_toolsToBeFixedSO, _toolsUsedSO);
        }

        //private void FixObject()
        //{
        //    // change visual?
        //    _toFixRoomObjectVisualComp.UpdateTFROVisual(_toolsToBeFixedSO, _toolsUsedSO);
        //    Debug.Log("FixObject");
        //}

        public bool TryToFix(RoomObjectSO toolUsed, out bool toolBeenUsed)
        {
            toolBeenUsed = false;

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
                    toolBeenUsed = true;
                    break;
                }
            }

            _toFixRoomObjectVisualComp.UpdateTFROVisual(_toolsToBeFixedSO, _toolsUsedSO);

            if (!IsFixed)
                return false;

            return true;
        }
    }
}
