using System;

namespace ProgramadorCastellano.MyFuncs
{
    /// <summary>
    /// This interface shouldn´t be used outside the namespace
    /// It is the baseInterface for all the IMyFuncSO
    /// </summary>
    public interface IBaseMyFuncSO
    {
        void ClearOnFuncRaised();
    }

    /// <summary>
    /// Interface that all of MyFuncSO will use
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public interface IMyFuncSO<TResult> : IBaseMyFuncSO
    {
        public bool TrySetOnFuncRaised(Func<TResult> newFunc);

        public TResult RaiseFunc();
    }

    public interface IMyFuncSO<T0, TResult> : IBaseMyFuncSO
    {
        public bool TrySetOnFuncRaised(Func<T0, TResult> newFunc);
        public TResult RaiseFunc(T0 funcArg);
    }
}