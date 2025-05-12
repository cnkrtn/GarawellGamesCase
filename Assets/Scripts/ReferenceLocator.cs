using Core.AddressableService.Interface;
using Core.AddressableService.Service;
using Core.AudioService;
using Core.AudioService.Interface;
using Core.AudioService.Keys;
using Core.AudioService.Service;
using Core.GameService.Interface;
using Core.GameService.Service;
using Core.GridService.Interface;
using Core.GridService.Service;
using Core.SceneLoaderService.Interface;
using Core.SceneLoaderService.Keys;
using Core.SceneLoaderService.Service;
using Core.GridHighlightService.Interface;
using Core.GridHighlightService.Service;
using Core.HandService.Interface;
using Core.HandService.Service;
using Core.ScoreService.Service;
using Core.TileFactoryService.Interface;
using Core.TileFactoryService.Service;

using UnityEngine;
using UnityEngine.Audio;

public class ReferenceLocator : MonoBehaviour
{
    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private AudioSource _audioSource1;
    [SerializeField] private AudioSource _audioSource11;
    [SerializeField] private AudioSource _audioSource12;
    [SerializeField] private AudioSource _musicSource;

    public static ReferenceLocator Instance;

    private IAudioService _audioService;
    private ISceneLoaderService _sceneLoaderService;
    private IGameService _gameService;
    private IAddressableService _addressableService;

    private IGridService _gridService;

    private IGridHighlightService _highlightService;
    private ITileFactoryService _tileFactoryService;

    private IHandService _handService;
    private IScoreService _scoreService;
    public IScoreService ScoreService => _scoreService;
    public IHandService HandService => _handService;
    public ITileFactoryService TileFactoryService => _tileFactoryService;
    public IGridHighlightService GridHighlightService => _highlightService;
    public IGridService GridService => _gridService;

    public IAudioService AudioService => _audioService;
    public ISceneLoaderService SceneLoaderService => _sceneLoaderService;
    public IAddressableService AddressableService => _addressableService;

    public IGameService GameService => _gameService;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        Instance = this;

        _audioService = new AudioService();
        _sceneLoaderService = new SceneLoaderService();
        _addressableService = new AddressableService();
        _gridService = new GridService();
        _highlightService = new GridHighlightService();
        _handService = new HandService();
        _tileFactoryService = new TileFactoryService();
        _gameService = new GameService();
        _scoreService = new ScoreService();
        //  SoundSettingsManager.Initialize(_audioMixer);

        Init();
    }

    private async void Init()
    {
        await _audioService.Inject(_audioSource1, _audioSource12, _audioSource11, _musicSource);
        await _addressableService.Inject();
        await _gameService.Inject();
        await _sceneLoaderService.Inject();
        await _scoreService.Inject();
        StartGame();
    }

    private async void StartGame()
    {
        await _sceneLoaderService.LoadScene(SceneKeys.KEY_GAME_START_SCENE);
        //  _audioService.PlayMusic(AudioKeys.KEY_MAIN_MUSIC, 1000);
    }

    private void Update()
    {
        if (_gameService.GameReady)
        {
            // Optional per-frame logic here (like calling Update() on other systems)
        }
    }
}