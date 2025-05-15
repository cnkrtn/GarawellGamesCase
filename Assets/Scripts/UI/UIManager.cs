using System.Collections;
using Core.AudioService.Keys;
using Core.AudioService.Service;
using Core.GridService.Data;
using Core.ScoreService.Service;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Tile.UI
{
    public class UIManager : MonoBehaviour
    {
        [Header("Score & Combo Displays")] [SerializeField]
        private TextMeshProUGUI scoreText;

        [SerializeField] private Canvas uiCanvas;
        [SerializeField] private Camera camera;
        [SerializeField] private GameObject scoreTextPop;

        [SerializeField] private TextMeshProUGUI comboTextPop;
        private float _comboPopScale = 4f;
        private float _popUpDuration = 1f;
        private float _popDownDuration = 0.15f;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI coinText;
        [SerializeField] private TextMeshProUGUI lineClearText;
        [SerializeField] private Slider expSlider;

        [Header("Game Over Panel")] [SerializeField]
        private GameObject gameOverPanel;

        [SerializeField] private GameObject levelCompletePanel;
        [SerializeField] private HandObject _handObject;


        private IScoreService _scoreService;
        private IAudioService _audioService;
        private int _lastScore;
        private Coroutine _countRoutine;

        void Awake()
        {
            _scoreService = ReferenceLocator.Instance.ScoreService;
            _audioService = ReferenceLocator.Instance.AudioService;
            gameOverPanel.SetActive(false);
        }

        void OnEnable()
        {
            EventService.ScoreUpdated += OnScoreUpdated;
            EventService.ComboUpdated += OnComboUpdated;
            EventService.GameOver += OnGameOver;
            EventService.LineCleared += OnLineCleared;
            EventService.ExpUpdated += OnExpUpdated;
            EventService.LevelFinished += OnLevelFinished;
            EventService.SquareCompleted += OnSquareCompleted;
        }

        private void OnSquareCompleted(Point origin, int points)
        {
            // 1) compute the world‐space center of that cell
            float s = ReferenceLocator.Instance.GridService.Spacing;
            Vector3 worldOrigin = ReferenceLocator.Instance.GridService.Origin;
            Vector3 worldCenter = worldOrigin + new Vector3((origin.X + .5f) * s,
                (origin.Y + .5f) * s,
                0);

            // 2) convert to canvas local coords
            Vector2 screenPt = camera.WorldToScreenPoint(worldCenter);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                uiCanvas.transform as RectTransform,
                screenPt,
                uiCanvas.worldCamera,
                out Vector2 localPt
            );

            // 3) spawn the prefab under the canvas
            var popup = Instantiate(scoreTextPop, uiCanvas.transform);

            // 4) set its anchored position if it has a RectTransform
            if (popup.TryGetComponent<RectTransform>(out var rt))
            {
                rt.anchoredPosition = localPt;
            }
            else
            {
                Debug.LogWarning("Score popup prefab has no RectTransform!");
            }

            // 4) set the text
            var tmp = popup.GetComponentInChildren<TextMeshProUGUI>();
            if (tmp != null) tmp.text = $"+{points}";

            // 6) let the popup animate & destroy itself (via its own script)
        }


        void OnDisable()
        {
            EventService.ScoreUpdated -= OnScoreUpdated;
            EventService.ComboUpdated -= OnComboUpdated;
            EventService.GameOver -= OnGameOver;
            EventService.LineCleared -= OnLineCleared;
            EventService.ExpUpdated -= OnExpUpdated;
            EventService.LevelFinished -= OnLevelFinished;
            EventService.SquareCompleted -= OnSquareCompleted;
        }


        private Tween _scoreTween;


        private void OnScoreUpdated(int newScore)
        {
            int oldScore = _lastScore;
            _lastScore = newScore;


            _scoreTween?.Kill();


            float duration = Mathf.Clamp((newScore - oldScore) * 0.02f, 0.3f, 1f);


            _scoreTween = DOTween.Sequence()
                .Append(scoreText.transform.DOScale(1.5f, duration * 0.5f).SetEase(Ease.OutBack))
                .Join(DOTween.To(() => oldScore, x => scoreText.text = x.ToString(), newScore, duration)
                    .SetEase(Ease.Linear))
                .Append(scoreText.transform.DOScale(1f, 0.2f).SetEase(Ease.InBack))
                .OnComplete(() => _scoreTween = null);
        }

        private void OnLineCleared()
        {
            _audioService.PlayAudio(AudioKeys.KEY_EXCELLENT);
            lineClearText.gameObject.SetActive(true);
            lineClearText.transform.localScale = Vector3.zero;

            lineClearText.transform
                .DOScale(_comboPopScale, _popUpDuration).SetEase(Ease.OutBack)
                .OnComplete(() =>
                    lineClearText.transform
                        .DOScale(0, _popUpDuration).SetEase(Ease.InBack)
                        .OnComplete(() => lineClearText.gameObject.SetActive(false))
                );
        }

        private void ShowScorePopup(int delta)
        {
            // if (delta <= 0) return;
            // scoreTextPop.text = $"+{delta}";
        }

        private void OnExpUpdated(int newExp)
        {
            // animate slider to new value
            expSlider.DOValue(newExp, 0.5f).SetEase(Ease.OutCubic);
        }

        private void OnLevelFinished()
        {
            // show level complete UI
            levelCompletePanel.SetActive(true);
        }

        private void OnComboUpdated(int newCombo)
        {
            Debug.Log("Yessss");
            if (newCombo < 2)
                return;
            _audioService.PlayAudio(AudioKeys.KEY_CoMBO);
            comboTextPop.text = " COMBO" + newCombo;
            comboTextPop.gameObject.SetActive(true);


            comboTextPop.transform
                .DOScale(_comboPopScale, _popUpDuration)
                .SetEase(Ease.OutBack)
                .OnComplete(() =>
                    comboTextPop.transform
                        .DOScale(0, _popDownDuration)
                        .SetEase(Ease.InBack)
                        .OnComplete(() =>
                            comboTextPop.gameObject.SetActive(false)
                        )
                );
        }

        private void OnGameOver()
        {
            gameOverPanel.SetActive(true);
        }
    }
}