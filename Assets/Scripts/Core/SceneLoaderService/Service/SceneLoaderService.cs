using System.Threading.Tasks;
using Core.SceneLoaderService.Interface;
using UnityEngine;
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

        public event System.Action<string> SceneLoaded;

        public async Task LoadAdditiveScene(string sceneName)
        {
            var loadingTask = Addressables.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            await loadingTask.Task;
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
            _additiveSceneInstance = loadingTask.Result;

            // ← fire the event
            SceneLoaded?.Invoke(sceneName);
        }

        public async Task LoadScene(string sceneName)
        {
            if (_currentSceneName != null)
            {
                var unloadingTask = Addressables.UnloadSceneAsync(_sceneInstance);
                await unloadingTask.Task;
            }

            var loadingTask = Addressables.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            await loadingTask.Task;
            _currentSceneName = sceneName;
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(_currentSceneName));
            _sceneInstance = loadingTask.Result;

            // ← fire the event
            SceneLoaded?.Invoke(sceneName);
        }

        public async Task UnloadAdditiveScene(string sceneName)
        {
            // look up the scene by name
            var scene = SceneManager.GetSceneByName(sceneName);

            // if it isn't loaded or isn't valid, just return
            if (!scene.IsValid() || !scene.isLoaded)
            {
                Debug.LogWarning($"UnloadAdditiveScene: scene '{sceneName}' not loaded, skipping unload.");
                return;
            }

            // unload it (no need to SetActiveScene manually)
            var unloadOp = SceneManager.UnloadSceneAsync(scene);
            while (!unloadOp.isDone)
                await Task.Yield();
        }



    }
}
