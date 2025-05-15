using System.Threading.Tasks;
using Core.AudioService.Keys;
using Core.AudioService.Service;
using Core.SceneLoaderService.Interface;
using Core.SceneLoaderService.Keys;
using UnityEngine;
using UnityEngine.UI;

namespace Tile
{
    public class LevelCompletedPanel : MonoBehaviour
    {
        [Header("UI References")] [SerializeField]
        private Button nextButton;

        [SerializeField] private Button backButton;
        [SerializeField] private GameObject panelToHide;
        [SerializeField] private LevelManager levelManager;

        private ISceneLoaderService _sceneLoaderService;
        private IAudioService _audioService;

        private void Awake()
        {
            _sceneLoaderService = ReferenceLocator.Instance.SceneLoaderService;
            _audioService = ReferenceLocator.Instance.AudioService;

            nextButton.onClick.AddListener(OnNextClicked);
            backButton.onClick.AddListener(OnBackClicked);
        }

        private async void OnBackClicked()
        {
            _audioService.PlayAudio(AudioKeys.KEY_CLICK_SOUND);
            await _sceneLoaderService.LoadScene(SceneKeys.KEY_MAIN_MENU_SCENE);
        }

        private void OnNextClicked()
        {
            if (panelToHide != null)
                panelToHide.SetActive(false);
            _audioService.PlayAudio(AudioKeys.KEY_CLICK_SOUND);

            levelManager.NextLevel();
        }
    }
}