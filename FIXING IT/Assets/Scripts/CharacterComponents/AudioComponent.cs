using UnityEngine;

namespace FixingIt.CharacterComponents
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioComponent : MonoBehaviour
    {
        private AudioSource _audioSource;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public void PlaySound()
        {
            if (_audioSource.isPlaying)
                return;

            _audioSource.PlayOneShot(_audioSource.clip);
        }

        public void StopSound()
        {
            _audioSource.Stop();
        }
    }
}
