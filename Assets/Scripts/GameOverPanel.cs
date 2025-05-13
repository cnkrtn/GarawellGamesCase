using Core.SceneLoaderService.Interface;
using Core.SceneLoaderService.Keys;
using UnityEngine;
using UnityEngine.UI;

namespace Tile
{
    public class GameOverPanel : MonoBehaviour
    {
        [Header("UI References")] [SerializeField]
        private Button retryButton;

        [SerializeField] private Button backButton;
        [SerializeField] private GameObject panelToHide; // the win panel
        [SerializeField] private LevelManager levelManager;

        private ISceneLoaderService _sceneLoaderService;

        private void Awake()
        {
            _sceneLoaderService = ReferenceLocator.Instance.SceneLoaderService;

            retryButton.onClick.AddListener(OnNextClicked);
            backButton.onClick.AddListener(OnBackClicked);
        }

        private async void OnBackClicked()
        {
            await _sceneLoaderService.LoadScene(SceneKeys.KEY_MAIN_MENU_SCENE);
        }

        private void OnNextClicked()
        {
            // hide the win panel
            if (panelToHide != null)
                panelToHide.SetActive(false);

            // advance level (wraps automatically)
            levelManager.ReloadCurrentLevel();
        }
    }
}