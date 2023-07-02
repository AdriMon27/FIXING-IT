using UnityEngine;
using UnityEngine.Audio;

namespace FixingIt.Settings
{
    [CreateAssetMenu(menuName = "Audio/AudioMixerSO")]
    public class AudioMixerSO : ScriptableObject
    {
        [SerializeField] private AudioMixer _mixer;
        [SerializeField] private AudioMixerGroup _musicMixerGroup;
        [SerializeField] private AudioMixerGroup _sfxMixerGroup;

        public AudioMixer Mixer => _mixer;
        public AudioMixerGroup MusicMixerGroup => _musicMixerGroup;
        public AudioMixerGroup SFXMixerGroup => _sfxMixerGroup;
    }
}
