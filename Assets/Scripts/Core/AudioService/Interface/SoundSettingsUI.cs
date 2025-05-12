using Core.AudioService;
using UnityEngine;
using UnityEngine.UI;

public class SoundSettingsUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Toggle musicToggle;
    [SerializeField] private Toggle sfxToggle;

    private void Start()
    {
        // Load the volume + toggle from SoundSettingsManager
        float savedMusicVolume = SoundSettingsManager.GetMusicVolume();
        float savedSFXVolume = SoundSettingsManager.GetSFXVolume();

        musicSlider.value = savedMusicVolume; // Start from 0..1
        sfxSlider.value = savedSFXVolume;

        musicToggle.isOn = !SoundSettingsManager.IsMusicMuted();
        sfxToggle.isOn = !SoundSettingsManager.IsSFXMuted();

        // --- SLIDER EVENT ---
        musicSlider.onValueChanged.AddListener(volume =>
        {
            SoundSettingsManager.SetMusicVolume(volume);
           
        });
        
        sfxSlider.onValueChanged.AddListener(volume =>
        {
            SoundSettingsManager.SetSFXVolume(volume);
           
        });

        // --- TOGGLE EVENT ---
        musicToggle.onValueChanged.AddListener(isOn =>
        {
            SoundSettingsManager.ToggleMusicMute(!isOn);
           
        });
        sfxToggle.onValueChanged.AddListener(isOn =>
        {
            SoundSettingsManager.ToggleSFXMute(!isOn);
        });
    }
}