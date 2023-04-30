using System;

namespace ProgramadorCastellano.MyFuncs
{
    /// <summary>
    /// It is the baseInterface for all the IMyFuncSO
    /// </summary>
    internal interface IBaseMyFuncSO
    {
        void ClearOnFuncRaised();
    }

    /// <summary>
    /// Interface that all of MyFuncSO will use
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    internal interface IMyFuncSO<TResult> : IBaseMyFuncSO
    {
        public bool TrySetOnFuncRaised(Func<TResult> newFunc);

        public TResult RaiseFunc();
    }

    internal interface IMyFuncSO<T0, TResult> : IBaseMyFuncSO
    {
        public bool TrySetOnFuncRaised(Func<T0, TResult> newFunc);
        public TResult RaiseFunc(T0 funcArg);
    }
}