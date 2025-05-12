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

        private readonly List<string> _playingAudios = new();
        private int _playingAudioCount;
        private float _volumeStartValue = 0.45f;

        public Task Inject(AudioSource audioSource1, AudioSource audioSource105, AudioSource audioSource11,
            AudioSource musicSource)
        {
            _audioSource1 = audioSource1;
            _audioSource12 = audioSource105;
            _audioSource11 = audioSource11;
            _musicSource = musicSource;
            _addressableService = ReferenceLocator.Instance.AddressableService;
            return Task.CompletedTask;
        }

        public void PlayAudioClip(AudioClip clip)
        {
            _audioSource1.PlayOneShot(clip, 0.8f);
        }

        public void StopMusic()
        {
            _musicSource.Stop();
        }

        public async void PlayAudio(string key)
        {
            if (SoundSettingsManager.GetSFXVolume() == 0) return; // Respect mute setting
            if (_playingAudios.Contains(key)) return;

            var audioClip = await _addressableService.LoadAudioClip(key);
            _playingAudios.Add(key);
            _playingAudioCount++;

            var volume = AudioKeys.KEY_ALL_AUDIO[key].VolumeOverride != 0
                ? AudioKeys.KEY_ALL_AUDIO[key].VolumeOverride
                : AudioKeys.KEY_ALL_AUDIO[key].Countable
                    ? SoundSettingsManager.GetSFXVolume() * (_volumeStartValue - _playingAudioCount * 0.05f)
                    : SoundSettingsManager.GetSFXVolume() * _volumeStartValue;

            if (AudioKeys.KEY_ALL_AUDIO[key].RandomPitch)
            {
                var randomVal = Random.Range(0, 3);
                switch (randomVal)
                {
                    case 0:
                        _audioSource1.PlayOneShot(audioClip, volume);
                        break;
                    case 1:
                        _audioSource12.PlayOneShot(audioClip, volume);
                        break;
                    case 2:
                        _audioSource11.PlayOneShot(audioClip, volume);
                        break;
                }
            }

            await Task.Delay(50);
            _playingAudios.Remove(key);
            await Task.Delay(500);
            _playingAudioCount--;
        }

        public async void PlayMusic(string key, int durationMs)
        {
           // Debug.Log($"🎵 PlayMusic Called with Key: {key}");

            // Optional: If you want a delay before starting the music:
            await Task.Delay(durationMs);

            // Load the audio clip from Addressables
            var audioClip = await _addressableService.LoadAudioClip(key);

            // Always overwrite the clip and play
            _musicSource.clip = audioClip;
            _musicSource.loop = true;
            _musicSource.Play();
           // Debug.Log("✅ Music started playing unconditionally.");

            // Apply volume settings from the mixer
            SoundSettingsManager.ApplyVolumeSettings();
        }
    }
}