using FixingIt.RoomObjects.SO;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using FixingIt.Funcs;
using System.Collections.Generic;
using FixingIt.Events.Network;
using System;

namespace FixingIt.RoomObjects.Logic
{
    public class ToFixRoomObject : RoomObject
    {
        [SerializeField] private ToFixRoomObjectVisualComp _toFixRoomObjectVisualComp;
        [SerializeField] private RoomObjectSO[] _toolsToBeFixedSO;
        private NetworkList<bool> _toolsUsedSO = new NetworkList<bool>();

        //public bool IsFixed => _toolsUsedSO.All(valor => valor);
        public bool IsFixed => AllToolsUsed();

        [Header("Broadcasting To")]
        [SerializeField]
        private NetworkObjectReferenceEventSO _tableCounterNORefEvent;

        [Header("Invoking Func")]
        [SerializeField]
        private RoomObjectSOIntFuncSO _getRoomObjectSOIndexFunc;
        [SerializeField]
        private IntRoomObjectSOFuncSO _getRoomObjectSOFromIndexFunc;

        protected override void Awake()
        {
            base.Awake();
            //_toolsUsedSO = new bool[_toolsToBeFixedSO.Length];
        }

        public override void OnNetworkSpawn()
        {
            if (IsServer) { 
                for (int i = 0; i < _toolsToBeFixedSO.Length; i++) {
                    _toolsUsedSO.Add(false);
                }

                //TryToFixClientRpc(false, default);
            }
            _toolsUsedSO.OnListChanged += UpdateVisual;
            UpdateVisual(default);

            base.OnNetworkSpawn();
        }

        private void UpdateVisual(NetworkListEvent<bool> changeEvent)
        {
            _toFixRoomObjectVisualComp.UpdateTFROVisual(_toolsToBeFixedSO, GetLocalCopyToolsUsed());
        }

        private List<bool> GetLocalCopyToolsUsed()
        {
            List<bool> localToolsUsedSO = new List<bool>();
            for (int i = 0; i < _toolsUsedSO.Count; i++)
            {
                localToolsUsedSO.Add(_toolsUsedSO[i]);
            }

            string a = name + "localToolsUsed copy: ";
            foreach (bool localTool in localToolsUsedSO)
            {
                a += localTool + ";";
            }

            Debug.Log(a);

            return localToolsUsedSO;
        }

        public void TryToFix(RoomObjectSO toolUsed, RoomObject toolGO, NetworkObjectReference tableCounterNORef)
        {
            int toolUsedIndex = _getRoomObjectSOIndexFunc.RaiseFunc(toolUsed);

            TryToFixServerRpc(toolUsedIndex, toolGO.NetworkObject,tableCounterNORef);
        }

        [ServerRpc(RequireOwnership = false)]
        private void TryToFixServerRpc(int roomObjectSOIndex, NetworkObjectReference toolGameObjectNORef, NetworkObjectReference tableCounterNORef)
        {
            bool objectFixed = false;
            bool toolBeenUsed = false;

            if (IsFixed) {
                Debug.Log("cannot fix a object that is already fixed");
                return;
            }

            RoomObjectSO toolUsed = _getRoomObjectSOFromIndexFunc.RaiseFunc(roomObjectSOIndex);

            for (int i = 0; i < _toolsToBeFixedSO.Length; i++)
            {
                if (_toolsUsedSO[i])
                    continue;

                if (toolUsed == _toolsToBeFixedSO[i])
                {
                    _toolsUsedSO[i] = true;
                    toolBeenUsed = true;
                    break;
                }
            }

            if (toolBeenUsed) {
                toolGameObjectNORef.TryGet(out NetworkObject toolGameObjectNO);
                RoomObject tool = toolGameObjectNO.GetComponent<RoomObject>();

                tool.Use();
            }

            if (IsFixed) {
                objectFixed = true;
            }

            ObjectFixedClientRpc(objectFixed, tableCounterNORef);
        }

        [ClientRpc]
        private void ObjectFixedClientRpc(bool objectFixed, NetworkObjectReference tableCounterNORef)
        {
            if (objectFixed) {
                _tableCounterNORefEvent.RaiseEvent(tableCounterNORef);
            }
        }

        private bool AllToolsUsed()
        {
            foreach (bool tool in _toolsUsedSO) {
                if (!tool)
                    return false;
            }

            return true;
        }
    }
}
