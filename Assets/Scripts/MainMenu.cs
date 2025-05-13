using Core.SceneLoaderService.Keys;

namespace Tile
{
    using UnityEngine;
    using UnityEngine.UI;
    using Core.SceneLoaderService.Interface; // adjust namespace as needed

    public class MainMenu : MonoBehaviour
    {
        [Header("UI References")] [SerializeField]
        private Button startButton;

        private ISceneLoaderService _sceneLoaderService;

        private void Awake()
        {
            // grab your scene‐loader service
            _sceneLoaderService = ReferenceLocator.Instance.SceneLoaderService;
        }


        private void Start()
        {
            // wire up the button
            startButton.onClick.AddListener(OnStartButtonClicked);
        }

        private async void OnStartButtonClicked()
        {
            // disable to prevent double-tap
            startButton.interactable = false;

            // load your game scene
            await _sceneLoaderService.LoadScene(SceneKeys.KEY_GAME_START_SCENE);
           
            
        }
    }
}