using UnityEngine.Events;

namespace ProgramadorCastellano.MyEvents
{
    /// <summary>
    /// Interface that all of my EventSO with no parameters will use
    /// </summary>
    internal interface IMyEventSO
    {
        public UnityAction OnEventRaised { get; set; }
        public void RaiseEvent();
    }

    /// <summary>
    /// Generic interface that all of my EventSO with 1 parameter will use
    /// </summary>
    /// <typeparam name="T">Type of the var that the UnityAction will invoke</typeparam>
    internal interface IMyEventSO<T>
    {
        public UnityAction<T> OnEventRaised { get; set; }
        public void RaiseEvent(T eventArg);
    }

    /// <summary>
    /// Generic interface that allof my EventSO with 2 parameters will use
    /// </summary>
    /// <typeparam name="T0"></typeparam>
    /// <typeparam name="T1"></typeparam>
    internal interface IMyEventSO<T0, T1>
    {
        public UnityAction<T0, T1> OnEventRaised { get; set; }
        public void RaiseEvent(T0 eventArg0, T1 eventArg1);
    }
}