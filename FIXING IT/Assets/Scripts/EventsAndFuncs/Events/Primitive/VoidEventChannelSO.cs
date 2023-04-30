using ProgramadorCastellano.MyEvents;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/Primitive/Void Event Channel")]
public class VoidEventChannelSO : DescriptionBaseSO, IMyEventSO
{
    public UnityAction OnEventRaised { get; set; }

    public void RaiseEvent()
    {
        if (OnEventRaised != null) {
            OnEventRaised.Invoke();
        }
        else {
            Debug.LogWarning(errorMessage);
        }
    }
}
