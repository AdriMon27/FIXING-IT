using ProgramadorCastellano.MyFuncs;
using UnityEngine;

namespace FixingIt.Funcs
{
    [CreateAssetMenu(menuName = "Events/Funcs/Complex/Int -> PlayerData Func")]
    public class IntPlayerdataFuncSO : BaseFuncSO<int, PlayerData>
    {
        public new PlayerData RaiseFunc(int playerIndex)
        {
            return base.RaiseFunc(playerIndex);
        }
    }
}