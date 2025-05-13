using System;
using System.Threading.Tasks;

namespace Core.SceneLoaderService.Interface
{
    public interface ISceneLoaderService
    {
        /// <summary>
        /// Called once at startup to initialize the service.
        /// </summary>
        Task Inject();

        /// <summary>
        /// Loads a scene as the primary (single) active scene.
        /// Fires <see cref="SceneLoaded"/> when done.
        /// </summary>
        Task LoadScene(string sceneName);

        /// <summary>
        /// Loads a scene additively on top of whatever’s already loaded.
        /// Fires <see cref="SceneLoaded"/> when done.
        /// </summary>
        Task LoadAdditiveScene(string sceneName);

        /// <summary>
        /// Unloads an additively loaded scene.
        /// </summary>
        Task UnloadAdditiveScene(string sceneName);

        /// <summary>
        /// The key of the most recently loaded primary scene.
        /// </summary>
        string CurrentSceneName { get; }

        /// <summary>
        /// Raised whenever a scene finishes loading via LoadScene or LoadAdditiveScene.
        /// The parameter is the scene key that was just loaded.
        /// </summary>
        event Action<string> SceneLoaded;
    }
}