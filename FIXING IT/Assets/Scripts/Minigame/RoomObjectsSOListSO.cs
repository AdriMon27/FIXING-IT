using FixingIt.RoomObjects.SO;
using System.Collections.Generic;
using UnityEngine;

namespace FixingIt.Minigame
{
    //[CreateAssetMenu()]
    public class RoomObjectsSOListSO : ScriptableObject
    {
        [SerializeField] private List<RoomObjectSO> _roomObjectsSO;
        public List<RoomObjectSO> RoomObjectsSO => _roomObjectsSO;
    }
}
