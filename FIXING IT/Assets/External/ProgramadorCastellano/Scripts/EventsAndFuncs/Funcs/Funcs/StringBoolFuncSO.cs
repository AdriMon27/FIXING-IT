using System;
using UnityEngine;

namespace ProgramadorCastellano.MyFuncs
{
    [CreateAssetMenu(menuName = "Events/Funcs/String -> Bool Func")]
    public class StringBoolFuncSO : DescriptionBaseSO, IMyFuncSO<string, bool>
    {
        private Func<string, bool> OnFuncRaised;

        public bool TrySetOnFuncRaised(Func<string, bool> newFunc)
        {
            if (OnFuncRaised != null)
            {
                return false;
            }

            OnFuncRaised = newFunc;
            return true;
        }

        public bool RaiseFunc(string arg0)
        {
            if (OnFuncRaised != null)
            {
                return OnFuncRaised.Invoke(arg0);
            }
            else
            {
                Debug.LogWarning($"{errorMessage} with parameter {arg0}");
                return false;
            }
        }

        public void ClearOnFuncRaised()
        {
            OnFuncRaised = null;
        }
    }
}