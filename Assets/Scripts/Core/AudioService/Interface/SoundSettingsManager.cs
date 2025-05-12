using UnityEngine;
using UnityEngine.Audio;

namespace Core.AudioService
{
    public static class SoundSettingsManager
    {
        private const string MusicVolumeKey = "MusicVolume";
        private const string SFXVolumeKey = "SFXVolume";
        private const string MusicMutedKey = "MusicMuted";
        private const string SFXMutedKey = "SFXMuted";

        private static float _musicVolume = 1f; 
        private static float _sfxVolume = 1f;   
        private static bool _musicMuteToggle;        
        private static bool _sfxMuteToggle;          

        private static AudioMixer _audioMixer;

        public static void Initialize(AudioMixer mixer)
        {
            _audioMixer = mixer;
            LoadSettings();
            ApplyVolumeSettings();
        }

        public static void LoadSettings()
        {
            _musicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 1f);
            _sfxVolume = PlayerPrefs.GetFloat(SFXVolumeKey, 1f);
            _musicMuteToggle = PlayerPrefs.GetInt(MusicMutedKey, 0) == 1;
            _sfxMuteToggle = PlayerPrefs.GetInt(SFXMutedKey, 0) == 1;

            // Debug.Log($"Loaded => MusicVol:{_musicVolume}, SFXVol:{_sfxVolume}, " +
            //           $"MusicMuted:{_musicMuted}, SFXMuted:{_sfxMuted}");
        }

        public static float GetMusicVolume() => _musicVolume;
        public static float GetSFXVolume() => _sfxVolume;
        public static bool IsMusicMuted() => _musicMuteToggle;
        public static bool IsSFXMuted() => _sfxMuteToggle;

        // -----------------------
        // SLIDER SETTERS
        // -----------------------
        public static void SetMusicVolume(float volume)
        {
            _musicVolume = Mathf.Clamp01(volume);
            PlayerPrefs.SetFloat(MusicVolumeKey, _musicVolume);
            PlayerPrefs.Save();
            ApplyVolumeSettings();
        }

        public static void SetSFXVolume(float volume)
        {
            _sfxVolume = Mathf.Clamp01(volume);
            PlayerPrefs.SetFloat(SFXVolumeKey, _sfxVolume);
            PlayerPrefs.Save();
            ApplyVolumeSettings();
        }

        // -----------------------
        // TOGGLE SETTERS
        // -----------------------
        public static void ToggleMusicMute(bool mute)
        {
            _musicMuteToggle = mute;
            PlayerPrefs.SetInt(MusicMutedKey, mute ? 1 : 0);
            PlayerPrefs.Save();
            ApplyVolumeSettings();
        }

        public static void ToggleSFXMute(bool mute)
        {
            _sfxMuteToggle = mute;
            PlayerPrefs.SetInt(SFXMutedKey, mute ? 1 : 0);
            PlayerPrefs.Save();
            ApplyVolumeSettings();
        }

        // -----------------------
        // APPLY TO MIXER
        // -----------------------
        public static void ApplyVolumeSettings()
        {
            if (_audioMixer == null) return;

            // If muted => finalVolume = 0, else => sliderVolume
            // Then convert finalVolume => dB
            float finalMusicValue = _musicMuteToggle ? 0f : _musicVolume;
            float finalSFXValue = _sfxMuteToggle ? 0f : _sfxVolume;

            // Use logarithmic scale for the mixer
            float musicDB = Mathf.Approximately(finalMusicValue, 0f) 
                ? -80f 
                : Mathf.Log10(finalMusicValue) * 20;
            float sfxDB = Mathf.Approximately(finalSFXValue, 0f) 
                ? -80f 
                : Mathf.Log10(finalSFXValue) * 20;

           // Debug.Log($"Applying => Music dB:{musicDB}, SFX dB:{sfxDB}");

            _audioMixer.SetFloat("MusicVolume", musicDB);
            _audioMixer.SetFloat("SFXVolume", sfxDB);
        }
    }
}
