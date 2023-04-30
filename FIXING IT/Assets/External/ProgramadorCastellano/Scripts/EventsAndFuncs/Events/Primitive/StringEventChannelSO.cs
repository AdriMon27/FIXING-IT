using ProgramadorCastellano.Base;
using UnityEngine;
using UnityEngine.Events;

namespace ProgramadorCastellano.MyEvents
{
    [CreateAssetMenu(menuName = "Events/Primitive/String Event Channel")]
    public class StringEventChannelSO : DescriptionBaseSO, IMyEventSO<string>
    {
        public UnityAction<string> OnEventRaised { get; set; }

        public void RaiseEvent(string arg0)
        {
            if (OnEventRaised != null) {
                OnEventRaised.Invoke(arg0);
            }
            else {
                Debug.LogWarning($"{errorMessage} with parameters {arg0}");
            }
        }
    }
}