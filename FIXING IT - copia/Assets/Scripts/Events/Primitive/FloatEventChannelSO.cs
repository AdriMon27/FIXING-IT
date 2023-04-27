using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/Primitive/Float Event Channel")]
public class FloatEventChannelSO : DescriptionBaseSO
{
    public UnityAction<float> OnEventRaised;

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
