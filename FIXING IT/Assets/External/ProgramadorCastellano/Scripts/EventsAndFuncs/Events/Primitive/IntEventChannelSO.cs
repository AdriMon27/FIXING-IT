using ProgramadorCastellano.Base;
using UnityEngine;
using UnityEngine.Events;

namespace ProgramadorCastellano.MyEvents
{
    [CreateAssetMenu(menuName = "Events/Primitive/Int Event Channel")]
    public class IntEventChannelSO : DescriptionBaseSO, IMyEventSO<int>
    {
        public UnityAction<int> OnEventRaised { get; set; }

        public void RaiseEvent(int arg0)
        {
            if (OnEventRaised != null) {
                OnEventRaised.Invoke(arg0);
            }
            else {
                Debug.LogWarning($"{errorMessage} with parameter {arg0}");
            }
        }
    }
}
