using ProgramadorCastellano.MyEvents;
using UnityEngine;

namespace FixingIt.Events
{
    public struct AudioNormalVolumes
    {
        public float GeneralVolume;
        public float MusicVolume;
        public float SFXVolume;

        public AudioNormalVolumes(float general, float music, float sfx)
        {
            GeneralVolume = general;
            MusicVolume = music;
            SFXVolume = sfx;
        }

        public override string ToString()
        {
            return $"GeneralVolume: {GeneralVolume}. MusicVolume: {MusicVolume}. SFXVolume: {SFXVolume}";
        }
    }

    [CreateAssetMenu(menuName = "Events/Complex/Audio Volume Channel")]
    public class AudioVolumeChannelSO : BaseComplexEventChannelSO<AudioNormalVolumes>
    {
    }
}