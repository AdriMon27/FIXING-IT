using System;
using UnityEngine;

namespace ProgramadorCastellano.MyFuncs
{
    [CreateAssetMenu(menuName = "Events/Funcs/Get String Func")]
    public class StringFuncSO : DescriptionBaseSO, IMyFuncSO<string>
    {
        private Func<string> OnFuncRaised;

        public bool TrySetOnFuncRaised(Func<string> newFunc)
        {
            if (OnFuncRaised != null)
            {
                return false;
            }

            OnFuncRaised = newFunc;
            return true;
        }

        public string RaiseFunc()
        {
            if (OnFuncRaised != null)
            {
                return OnFuncRaised.Invoke();
            }
            else
            {
                Debug.LogWarning($"{errorMessage}");
                return null;
            }
        }

        public void ClearOnFuncRaised()
        {
            OnFuncRaised = null;
        }
    }
}