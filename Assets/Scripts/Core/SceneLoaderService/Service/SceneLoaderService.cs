using System.Threading.Tasks;
using Core.SceneLoaderService.Interface;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Core.SceneLoaderService.Service
{
    public class SceneLoaderService : ISceneLoaderService
    {
        private string _currentSceneName;

        private SceneInstance _sceneInstance;
        private SceneInstance _additiveSceneInstance;

        public string CurrentSceneName => _currentSceneName;
        
        public Task Inject()
        {
            return Task.CompletedTask;
        }

        public async Task LoadAdditiveScene(string sceneName)
        {
            var loadingTask = Addressables.LoadSceneAsync(sceneName,LoadSceneMode.Additive);
            await loadingTask.Task;
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
            _additiveSceneInstance = loadingTask.Result;
        }

        public async Task UnloadAdditiveScene(string sceneName)
        {
            var loadingTask = Addressables.UnloadSceneAsync(_additiveSceneInstance);
            await loadingTask.Task;
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(_currentSceneName));
        }

        public async Task LoadScene(string sceneName)
        {
            if (_currentSceneName != null)
            {
               var unloadingTask = Addressables.UnloadSceneAsync (_sceneInstance);
               await unloadingTask.Task;
               var loadingTask = Addressables.LoadSceneAsync(sceneName,LoadSceneMode.Additive);
               await loadingTask.Task;
               _currentSceneName = sceneName;
               SceneManager.SetActiveScene(SceneManager.GetSceneByName(_currentSceneName));
               _sceneInstance = loadingTask.Result;
            }
            else
            {
                var loadingTask = Addressables.LoadSceneAsync(sceneName,LoadSceneMode.Additive);
                await loadingTask.Task;
                _currentSceneName = sceneName;
                SceneManager.SetActiveScene(SceneManager.GetSceneByName(_currentSceneName));
                _sceneInstance = loadingTask.Result;
            }
        }
    }
}
