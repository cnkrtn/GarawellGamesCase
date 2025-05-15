using System.Collections.Generic;
using System.Threading.Tasks;
using Core.AddressableService.Interface;
using Core.AudioService.Keys;
using Core.AudioService.Service;
using UnityEngine;

namespace Core.AudioService.Interface
{
    public class AudioService : IAudioService
    {
        private IAddressableService _addressableService;
        private AudioSource _audioSource1;
        private AudioSource _audioSource11;
        private AudioSource _audioSource12;
        private AudioSource _musicSource;

        private bool _isSfxMuted;
        private bool _isMusicMuted;
        private readonly List<string> _playingAudios = new();
        private int _playingAudioCount;
        private float _volumeStartValue = 0.45f;

        public Task Inject(
            AudioSource audioSource1,
            AudioSource audioSource105,
            AudioSource audioSource11,
            AudioSource musicSource
        )
        {
            _audioSource1       = audioSource1;
            _audioSource12      = audioSource105;
            _audioSource11      = audioSource11;
            _musicSource        = musicSource;
            _addressableService = ReferenceLocator.Instance.AddressableService;
            return Task.CompletedTask;
        }

        public void SetSfxMute(bool mute)
        {
            _isSfxMuted = mute;
            float vol = mute ? 0f : 0.4f;
            _audioSource1.volume  = vol;
            _audioSource11.volume = vol;
            _audioSource12.volume = vol;
        }

        public void SetMusicMute(bool mute)
        {
            _isMusicMuted       = mute;
            _musicSource.volume = mute ? 0f : 0.1f;
        }

        public void PlayAudioClip(AudioClip clip)
        {
            if (_isSfxMuted) return;
            _audioSource1.PlayOneShot(clip, 0.8f);
        }

        public void StopMusic()
        {
            _musicSource.Stop();
        }

        public async void PlayAudio(string key)
        {
            if (_isSfxMuted || _playingAudios.Contains(key)) return;

            var clip = await _addressableService.LoadAudioClip(key);
            _playingAudios.Add(key);
            _playingAudioCount++;

            float volume = AudioKeys.KEY_ALL_AUDIO[key].VolumeOverride != 0
                ? AudioKeys.KEY_ALL_AUDIO[key].VolumeOverride
                : AudioKeys.KEY_ALL_AUDIO[key].Countable
                    ? (_volumeStartValue - _playingAudioCount * 0.05f)
                    : _volumeStartValue;

            if (AudioKeys.KEY_ALL_AUDIO[key].RandomPitch)
            {
                int r = Random.Range(0, 3);
                var src = r == 0 ? _audioSource1 : r == 1 ? _audioSource12 : _audioSource11;
                src.PlayOneShot(clip, volume);
            }
            else
            {
                _audioSource1.PlayOneShot(clip, volume);
            }

            await Task.Delay(50);
            _playingAudios.Remove(key);
            await Task.Delay(500);
            _playingAudioCount--;
        }

        public async void PlayMusic(string key, int durationMs)
        {
            if (_isMusicMuted) return;

            await Task.Delay(durationMs);
            var clip = await _addressableService.LoadAudioClip(key);

            _musicSource.clip = clip;
            _musicSource.loop = true;
            _musicSource.Play();
        }
    }
}
