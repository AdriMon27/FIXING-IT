using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/Screen Settings Channel")]
public class ScreenSettingsChannelSO : DescriptionBaseSO
{
    public UnityAction<int, bool> OnEventRaised;

    public void RaiseEvent(int screenResolutionIndex, bool isFullScreen)
    {
        if (OnEventRaised != null) {
            OnEventRaised.Invoke(screenResolutionIndex, isFullScreen);
        }
        else {
            Debug.LogError($"{errorMessage} with parameters {screenResolutionIndex} and {isFullScreen}");
        }
    }
}
