using System;
using System.Threading.Tasks;

namespace Core.SceneLoaderService.Interface
{
    public interface ISceneLoaderService
    {
       
        Task Inject();

       
        Task LoadScene(string sceneName);

       
        Task LoadAdditiveScene(string sceneName);

        
        Task UnloadAdditiveScene(string sceneName);

      
        string CurrentSceneName { get; }

       
        event Action<string> SceneLoaded;
    }
}