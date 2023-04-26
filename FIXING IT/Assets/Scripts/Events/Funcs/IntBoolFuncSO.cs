using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Events/Funcs/Int -> Bool Func")]
public class IntBoolFuncSO : DescriptionBaseSO
{
    private Func<int, bool> OnFuncRaised;

    public bool TrySetOnFuncRaised(Func<int, bool> newFunc)
    {
        if (OnFuncRaised != null) {
            return false;
        }

        OnFuncRaised = newFunc;
        return true;
    }

    public bool RaiseFunc(int arg0)
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
