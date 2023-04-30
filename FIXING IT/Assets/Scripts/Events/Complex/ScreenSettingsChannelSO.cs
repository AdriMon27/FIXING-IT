using ProgramadorCastellano.MyEvents;
using UnityEngine;

[CreateAssetMenu(menuName = "Events/Complex/Screen Settings Channel")]
public class ScreenSettingsChannelSO : BaseComplexEventChannelSO<int, bool>
{
    //public UnityAction<int, bool> OnEventRaised { get; set; }

    //public void RaiseEvent(int screenResolutionIndex, bool isFullScreen)
    //{
    //    if (OnEventRaised != null) {
    //        OnEventRaised.Invoke(screenResolutionIndex, isFullScreen);
    //    }
    //    else {
    //        Debug.LogWarning($"{errorMessage} with parameters {screenResolutionIndex} and {isFullScreen}");
    //    }
    //}

    public new void RaiseEvent(int screenResolutionIndex, bool isFullScreen)
    {
        base.RaiseEvent(screenResolutionIndex, isFullScreen);
    }
}
