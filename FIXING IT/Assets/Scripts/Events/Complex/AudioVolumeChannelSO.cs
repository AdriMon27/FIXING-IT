using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/Complex/Audio Volume Channel")]
public class AudioVolumeChannelSO : DescriptionBaseSO, IMyEventSO<AudioNormalVolumes>
{
    public UnityAction<AudioNormalVolumes> OnEventRaised { get; set; }

    public void RaiseEvent(AudioNormalVolumes audioVolumes)
    {
        if (OnEventRaised != null) {
            OnEventRaised.Invoke(audioVolumes);
        }
        else {
            Debug.LogWarning($"{errorMessage} with parameterss {audioVolumes}");
        }
    }
}
