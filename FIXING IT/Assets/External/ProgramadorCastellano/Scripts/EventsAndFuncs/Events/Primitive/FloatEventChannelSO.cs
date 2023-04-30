using ProgramadorCastellano.Base;
using UnityEngine;
using UnityEngine.Events;

namespace ProgramadorCastellano.MyEvents
{
    [CreateAssetMenu(menuName = "Events/Primitive/Float Event Channel")]
    public class FloatEventChannelSO : DescriptionBaseSO, IMyEventSO<float>
    {
        public UnityAction<float> OnEventRaised { get; set; }

        public void RaiseEvent(float arg0)
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