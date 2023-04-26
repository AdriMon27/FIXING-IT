using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/Primitive/String Event Channel")]
public class StringEventChannelSO : DescriptionBaseSO
{
    public UnityAction<string> OnEventRaised;

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
