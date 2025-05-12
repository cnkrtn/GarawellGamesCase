using System.Threading.Tasks;
using Core.GameService.Interface;
using Core.SceneLoaderService.Interface;
using Core.SceneLoaderService.Keys;


namespace Core.GameService.Service
{
    public class GameService : IGameService
    {
      
        private ISceneLoaderService _sceneLoaderService;

        private bool _onTransition;
        private bool _fading;
        private bool _gameReady;
        private bool _loadingScreen;
        private bool _onUI;

        public bool Fading => _fading;
        public bool GameReady => _gameReady;
        public bool LoadingScreen => _loadingScreen;
        public bool OnUI => _onUI;
        public bool OnTransition => _onTransition;

        public async void SetOnUI(bool value)
        {
            await Task.Delay(20);
            _onUI = value;
        }

        public Task Inject()
        {
           
            _sceneLoaderService = ReferenceLocator.Instance.SceneLoaderService;
            
            return Task.CompletedTask;
        }

        public void SetLoadingScreen(bool value)
        {
            _loadingScreen = value;
        }

        public async void StartGame()
        {
           
            await _sceneLoaderService.LoadScene(SceneKeys.KEY_GAME_START_SCENE);
            _gameReady = true;
            await Task.Delay(20);
           
        }

        public async void ReturnGame()
        {
            _onTransition = true;
            _gameReady = false;

           
            await _sceneLoaderService.LoadScene(SceneKeys.KEY_GAME_START_SCENE);

            _gameReady = true;
            await Task.Delay(50);
            _onTransition = false;
        }

       
    }
}
