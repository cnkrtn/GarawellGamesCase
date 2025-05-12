using System.Threading.Tasks;


namespace Core.GameService.Interface
{
    public interface IGameService
    {
        Task Inject();
        public bool Fading { get; }
        public bool GameReady { get; }
        public bool LoadingScreen { get; }
        public bool OnTransition { get; }
        void SetLoadingScreen(bool value);
        void SetOnUI(bool value);
        bool OnUI { get; }
        void StartGame();
        void ReturnGame();
       
    }
}