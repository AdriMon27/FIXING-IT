using ProgramadorCastellano.Base;
using UnityEngine;
using UnityEngine.Events;

namespace ProgramadorCastellano.Events
{
    /// <summary>
    /// Base Scriptable Object class for creating our personals EventChannelSO.
    /// Don´t forget to add CreateAssetMenu.
    /// If you want to change the names in the parameters you can make a new RaiseEvent() ant call base.RaiseEvent()
    /// </summary>
    /// <typeparam name="T">Struct,List,Dict... that the event channel will use</typeparam>
    public abstract class BaseEventChannelSO<T> : DescriptionBaseSO, IMyEventSO<T>
    {
        public UnityAction<T> OnEventRaised { get; set; }

        public void RaiseEvent(T eventArg)
        {
            if (OnEventRaised != null) {
                OnEventRaised.Invoke(eventArg);
            }
            else {
                Debug.LogWarning($"{errorMessage} with parameter {eventArg}");
            }
        }
    }

    public abstract class BaseEventChannelSO<T0,T1> : DescriptionBaseSO, IMyEventSO<T0, T1>
    {
        public UnityAction<T0,T1> OnEventRaised { get; set; }

        public void RaiseEvent(T0 eventArg0, T1 eventArg1)
        {
            if (OnEventRaised != null) {
                OnEventRaised.Invoke(eventArg0, eventArg1);
            }
            else {
                Debug.LogWarning($"{errorMessage} with parameters {eventArg0} and {eventArg1}");
            }
        }
    }
}