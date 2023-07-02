using ProgramadorCastellano.Events;
using UnityEngine;

namespace FixingIt.Events
{
    [CreateAssetMenu(menuName = "Events/Complex/Screen Settings Channel")]
    public class ScreenSettingsChannelSO : BaseEventChannelSO<int, bool>
    {
        public new void RaiseEvent(int screenResolutionIndex, bool isFullScreen)
        {
            base.RaiseEvent(screenResolutionIndex, isFullScreen);
        }
    }
}