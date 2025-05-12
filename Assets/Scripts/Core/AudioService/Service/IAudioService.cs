using System.Threading.Tasks;
using UnityEngine;

namespace Core.AudioService.Service
{
    public interface IAudioService
    {
        Task Inject(AudioSource audioSource1, AudioSource audioSource11, AudioSource audioSource12,
            AudioSource musicSource);
        void PlayAudioClip(AudioClip clip);
        void PlayAudio(string key);
        void PlayMusic(string key, int durationMs);
        void StopMusic();
    }
}