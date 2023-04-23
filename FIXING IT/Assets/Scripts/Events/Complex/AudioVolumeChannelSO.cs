using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/Complex/Audio Volume Channel")]
public class AudioVolumeChannelSO : DescriptionBaseSO
{
    public UnityAction<AudioNormalVolumes> OnEventRaised;

    public void RaiseEvent(AudioNormalVolumes audioVolumes)
    {
        if (OnEventRaised != null) {
            OnEventRaised.Invoke(audioVolumes);
        }
        else {
            Debug.LogError($"{errorMessage} with parameterss {audioVolumes}");
        }
    }
}
