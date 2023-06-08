using ProgramadorCastellano.Base;
using System;
using UnityEngine;

namespace ProgramadorCastellano.Funcs
{
    /// <summary>
    /// Base Scriptable Object class for creating our personals FuncSO
    /// Don´t forget to add CreateAssetMenu
    /// If you want to change the names in the parameters you can make a new RaiseFunc() ant call base.RaiseEvent()
    /// </summary>
    /// <typeparam name="TResult">Struct,List,Dict... that the Func will return</typeparam>
    public abstract class BaseFuncSO<TResult> : DescriptionBaseSO, IMyFuncSO<TResult>
    {
        private Func<TResult> OnFuncRaised;

        /// <summary>
        /// Set the Func to null
        /// </summary>
        public void ClearOnFuncRaised()
        {
            OnFuncRaised = null;
        }

        /// <summary>
        /// Invokes the Func
        /// </summary>
        /// <returns>The <typeparamref name="TResult"/> output object if OnFuncRaised is not null or default if the Func is null</returns>
        public TResult RaiseFunc()
        {
            if (OnFuncRaised != null) {
                return OnFuncRaised.Invoke();
            }
            else {
                Debug.LogWarning($"{errorMessage}");
                return default;
            }
        }

        /// <summary>
        /// Tries to set a new Func to the internal Func
        /// </summary>
        /// <param name="newFunc"></param>
        /// <returns>If it was possible to set the Func, if the Func was null</returns>
        public bool TrySetOnFuncRaised(Func<TResult> newFunc)
        {
            if (OnFuncRaised != null) {
                return false;
            }

            OnFuncRaised = newFunc;
            return true;
        }
    }

    public abstract class BaseFuncSO<T0, TResult> : DescriptionBaseSO, IMyFuncSO<T0, TResult>
    {
        private Func<T0, TResult> OnFuncRaised;

        /// <summary>
        /// Set the Func to null
        /// </summary>
        public void ClearOnFuncRaised()
        {
            OnFuncRaised = null;
        }

        /// <summary>
        /// Tries to set a new Func to the internal Func
        /// </summary>
        /// <param name="arg0">First argument of the func</param>
        /// <returns>The <typeparamref name="TResult"/> output object if OnFuncRaised is not null or default if the Func is null</returns>
        public TResult RaiseFunc(T0 arg0)
        {
            if (OnFuncRaised != null) {
                return OnFuncRaised.Invoke(arg0);
            }
            else {
                Debug.LogWarning($"{errorMessage} with parameter {arg0}");
                return default;
            }
        }

        /// <summary>
        /// Tries to set a new Func to the internal Func
        /// </summary>
        /// <param name="newFunc"></param>
        /// <returns>If it was possible to set the Func, if the Func was null</returns>
        public bool TrySetOnFuncRaised(Func<T0, TResult> newFunc)
        {
            if (OnFuncRaised != null) {
                return false;
            }

            OnFuncRaised = newFunc;
            return true;
        }
    }

    public abstract class BaseFuncSO<T0, T1, TResult> : DescriptionBaseSO, IMyFuncSO<T0, T1, TResult>
    {
        private Func<T0, T1, TResult> OnFuncRaised;

        /// <summary>
        /// Set the Func to null
        /// </summary>
        public void ClearOnFuncRaised()
        {
            OnFuncRaised = null;
        }

        /// <summary>
        /// Tries to set a new Func to the internal Func
        /// </summary>
        /// <param name="arg0">First argument of the func</param>
        /// <param name="arg1">Second argument of the func</param>
        /// <returns>The <typeparamref name="TResult"/> output object if OnFuncRaised is not null or default if the Func is null</returns>
        public TResult RaiseFunc(T0 arg0, T1 arg1)
        {
            if (OnFuncRaised != null) {
                return OnFuncRaised.Invoke(arg0, arg1);
            }
            else {
                Debug.LogWarning($"{errorMessage} with parameters {arg0} and {arg1}");
                return default;
            }
        }

        /// <summary>
        /// Tries to set a new Func to the internal Func
        /// </summary>
        /// <param name="newFunc"></param>
        /// <returns>If it was possible to set the Func, if the Func was null</returns>
        public bool TrySetOnFuncRaised(Func<T0, T1, TResult> newFunc)
        {
            if (OnFuncRaised != null) {
                return false;
            }

            OnFuncRaised = newFunc;
            return true;
        }
    }
}
