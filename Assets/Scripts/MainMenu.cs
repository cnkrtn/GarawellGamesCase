using Core.AudioService.Keys;
using Core.AudioService.Service;
using Core.SceneLoaderService.Interface;
using Core.SceneLoaderService.Keys;
using Tile.Core.IGridSizeService.Interface;
using TMPro;

namespace Tile
{
    using UnityEngine;
    using UnityEngine.UI;
   

    public class MainMenu : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Button startButton;
        [SerializeField] private Button settingsButton;

        [SerializeField] private Button sizePanelButton;
        [Header("Input Fields")]
     
        [SerializeField] private TMP_InputField widthField;
        [SerializeField] private TMP_InputField heightField;
        [Header("Panels")]
        [SerializeField] private GameObject sizePanel;
        [SerializeField] private GameObject settingsPanel;

        [SerializeField] private GameObject _warningTextObject;
        private ISceneLoaderService _sceneLoaderService;
        private IGridSizeService _gridSizeService;
        private IAudioService _audioService;
        private void Awake()
        {
            // grab your scene‐loader service
            _gridSizeService = ReferenceLocator.Instance.GridSizeService;
            _sceneLoaderService = ReferenceLocator.Instance.SceneLoaderService;
            _audioService = ReferenceLocator.Instance.AudioService;
            
            // hide on start
            if (_warningTextObject != null)
                _warningTextObject.SetActive(false);
        }


        private void Start()
        {
            // wire up the button
            startButton.onClick.AddListener(OnStartButtonClicked);
            sizePanelButton.onClick.AddListener(OnSizePanelButtonClicked);
            settingsButton.onClick.AddListener(OnSettingsButtonClicked);
        }

        private void OnSettingsButtonClicked()
        {
            _audioService.PlayAudio(AudioKeys.KEY_CLICK_SOUND);
            sizePanel.SetActive(false);
            settingsPanel.SetActive(true);
        }

        private void OnSizePanelButtonClicked()
        {
            _audioService.PlayAudio(AudioKeys.KEY_CLICK_SOUND);
            settingsPanel.SetActive(false);
            sizePanel.SetActive(true);
        }

        public async void OnStartButtonClicked()
        {
            
            _audioService.PlayAudio(AudioKeys.KEY_CLICK_SOUND);
            // 1) parse
            if (int.TryParse(widthField.text, out var px) &&
                int.TryParse(heightField.text, out var py))
            {
                // 2) validate
                if (px >= 3 && px <= 8 && py >= 3 && py <= 8)
                {
                    // valid → hide panel, store settings, load game
                    if (_warningTextObject != null)
                        _warningTextObject.SetActive(false);

                    _gridSizeService.SetGridSize(px, py);
                    await _sceneLoaderService.LoadScene(SceneKeys.KEY_GAME_START_SCENE);
                }
                else
                {
                    // invalid → show panel
                    if (_warningTextObject != null)
                        _warningTextObject.SetActive(true);
                }
            }
            else
            {
                // parse failed → show panel
                if (_warningTextObject != null)
                    _warningTextObject.SetActive(true);
            }
        }
    }
}