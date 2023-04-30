using FixingIt.Events;
using UnityEngine;

namespace FixingIt.Settings
{
    public class ResolutionManager : MonoBehaviour
    {
        private int MAX_RESOLUTION => Screen.resolutions.Length - 1;

        [Header("Listening To")]
        [SerializeField]
        private ScreenSettingsChannelSO _screenSettingsChannel;

        [Header("Broadcasting To")]
        [SerializeField]
        private ScreenSettingsChannelSO _screenSettingsChanged;

        private void OnEnable()
        {
            _screenSettingsChannel.OnEventRaised += SetResolution;
        }
        private void OnDisable()
        {
            _screenSettingsChannel.OnEventRaised -= SetResolution;
        }

        private void SetResolution(int indexResolution, bool fullScreen)
        {
            // check if index resolution is not valid
            if (indexResolution >= Screen.resolutions.Length)
            {
                indexResolution = MAX_RESOLUTION;    // not valid -> put max resolution
            }

            Resolution resolution = Screen.resolutions[indexResolution];
            Screen.SetResolution(resolution.width, resolution.height, fullScreen);

            _screenSettingsChanged.RaiseEvent(indexResolution, fullScreen);
            //TODO: save resolution
        }
    }
}