using System.Collections;
using Core.ScoreService.Service;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Tile.UI
{
    public class UIManager : MonoBehaviour
    {
        [Header("Score & Combo Displays")] [SerializeField]
        private TextMeshProUGUI scoreText;

        [SerializeField] private GameObject scoreTextPop;

        [SerializeField] private TextMeshProUGUI comboTextPop;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI coinText;
        [SerializeField] private TextMeshProUGUI lineClearText;
        [SerializeField] private Slider expSlider;
        [Header("Game Over Panel")] 
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private GameObject levelCompletePanel;
        [SerializeField] private HandObject _handObject;
        
        
        private IScoreService _scoreService;
        private int _lastScore;
        private Coroutine _countRoutine;

        void Awake()
        {
            _scoreService = ReferenceLocator.Instance.ScoreService;
            gameOverPanel.SetActive(false);
        }

        void OnEnable()
        {
            EventService.ScoreUpdated += OnScoreUpdated;
            EventService.ComboUpdated += OnComboUpdated;
            EventService.GameOver += OnGameOver;
            EventService.LineCleared    += OnLineCleared;
            EventService.ExpUpdated += OnExpUpdated;
            EventService.LevelFinished += OnLevelFinished;
        }

        void OnDisable()
        {
            EventService.ScoreUpdated -= OnScoreUpdated;
            EventService.ComboUpdated -= OnComboUpdated;
            EventService.GameOver -= OnGameOver;
            EventService.LineCleared    -= OnLineCleared;
            EventService.ExpUpdated -= OnExpUpdated;
            EventService.LevelFinished -= OnLevelFinished;
        }


        private Tween _scoreTween;

        private void OnScoreUpdated(int newScore)
        {
            int oldScore = _lastScore;
            _lastScore = newScore;

            // kill any in‐flight tween
            _scoreTween?.Kill();

            // compute a duration based on how big the change is (min 0.3s, max 1s)
            float duration = Mathf.Clamp((newScore - oldScore) * 0.02f, 0.3f, 1f);

            // build the sequence
            _scoreTween = DOTween.Sequence()
                // 1) scale up over the first half
                .Append(scoreText.transform.DOScale(1.5f, duration * 0.5f).SetEase(Ease.OutBack))
                // 2) simultaneously tween the score number
                .Join(DOTween.To(() => oldScore, x => scoreText.text = x.ToString(), newScore, duration)
                    .SetEase(Ease.Linear))
                // 3) scale back down over 0.2s
                .Append(scoreText.transform.DOScale(1f, 0.2f).SetEase(Ease.InBack))
                // clear reference when done
                .OnComplete(() => _scoreTween = null);
        }

        private void OnLineCleared()
        {
           // lineClearText.text = bonusText;
            // simple fade & scale pop
//            lineClearText.transform.localScale = Vector3.zero;
//            lineClearText.gameObject.SetActive(true);
            // DOTween.Sequence()
            //     .Append(lineClearText.transform.DOScale(1.2f, 0.2f).SetEase(Ease.OutBack))
            //     .AppendInterval(0.6f)
            //     .Append(lineClearText.transform.DOScale(0f, 0.2f).SetEase(Ease.InBack))
            //     .OnComplete(() => lineClearText.gameObject.SetActive(false));
        }

        private void ShowScorePopup(int delta)
        {
            // if (delta <= 0) return;
            // scoreTextPop.text = $"+{delta}";
            // // TODO: trigger your pop animation here
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
            // comboTextPop.text = newCombo > 1 ? $"×{newCombo}" : string.Empty;
            // TODO: trigger combo pop animation
        }

        private void OnGameOver()
        {
            gameOverPanel.SetActive(true);
        }
    }
}