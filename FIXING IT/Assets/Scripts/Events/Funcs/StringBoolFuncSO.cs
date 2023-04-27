using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Events/Funcs/String -> Bool Func")]
public class StringBoolFuncSO : DescriptionBaseSO
{
    private Func<string, bool> OnFuncRaised;

    public bool TrySetOnFuncRaised(Func<string, bool> newFunc)
    {
        if (OnFuncRaised != null) {
            return false;
        }

        OnFuncRaised = newFunc;
        return true;
    }

    public bool RaiseFunc(string arg0)
    {
        if (OnFuncRaised != null) {
            return OnFuncRaised.Invoke(arg0);
        }
        else {
            Debug.LogWarning($"{errorMessage} with parameter {arg0}");
            return false;
        }
    }
}
