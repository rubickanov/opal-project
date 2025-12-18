using UnityEngine;

namespace Rubickanov.Opal.Presentation
{
    public class SoundManager : MonoBehaviour
    {
        [SerializeField] private AudioSource _audioSource;

        [Header("Sounds")]
        [SerializeField] private AudioClip _flip;
        [SerializeField] private AudioClip _match;
        [SerializeField] private AudioClip _noMatch;
        [SerializeField] private AudioClip _gameWin;

        public void PlayFlip(float volume = 1.0f) => Play(_flip, volume);
        public void PlayMatch(float volume = 1.0f) => Play(_match, volume);
        public void PlayNoMatch(float volume = 1.0f) => Play(_noMatch, volume);
        public void PlayGameWin(float volume = 1.0f) => Play(_gameWin, volume);

        private void Play(AudioClip clip, float volume = 1.0f)
        {
            if (clip != null && _audioSource != null)
            {
                _audioSource.PlayOneShot(clip, volume);
            }
        }
    }
}
