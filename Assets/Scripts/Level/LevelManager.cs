using System.Collections;
using System.Collections.Generic;
using Core.GridHighlightService.Interface;
using Core.GridService.Data;
using Core.GridService.Interface;
using Core.ScoreService.Service;
using Tile;
using UnityEngine;
using UnityEngine.Serialization;

public class LevelManager : MonoBehaviour
{
    [Header("Assign all levels in play order")] [SerializeField]
    private List<LevelData> levels;

    [FormerlySerializedAs("_loader")] [SerializeField]
    private LevelLoader loader;

    [SerializeField] private HandObject handObject;

    private int _currentIndex = 0;
    private ScoreService _scoreSvc;
    private IGridService _gridSvc;
    private IGridHighlightService _highlightSvc;
    public LevelData CurrentLevelData => levels[_currentIndex];

    void Awake()
    {
        _scoreSvc = ReferenceLocator.Instance.ScoreService as ScoreService;
        _gridSvc = ReferenceLocator.Instance.GridService;
        _highlightSvc = ReferenceLocator.Instance.GridHighlightService;
        EventService.LevelFinished += OnLevelFinished;
    }

    private IEnumerator Start()
    {
        // wait until the grid is injected and built
        yield return new WaitUntil(() => ReferenceLocator.Instance.GridService.GridSize > 0);

        // now it’s safe to load
        LoadLevel(0);
    }

    void OnDestroy() => EventService.LevelFinished -= OnLevelFinished;

    private void OnLevelFinished()
    {
        // same as NextLevel() below:
        NextLevel();
    }

    /// <summary>
    /// Advance to the next level (wrapping at the end),
    /// resets score/XP, and applies the new LevelData.
    /// </summary>
    public void NextLevel()
    {
        _currentIndex = (_currentIndex + 1) % levels.Count;
        LoadLevel(_currentIndex);
    }

    public void ReloadCurrentLevel()
    {
        LoadLevel(_currentIndex);
    }

    private void LoadLevel(int idx)
    {
        var data = levels[idx];
        Debug.Log($"[LevelManager] Loading level {idx + 1}: {data.name}");

        // A) Clear all square markers
        _highlightSvc.ClearAllSquares();

        // B) Clear grid data
        int size = _gridSvc.GridSize;
        for (int y = 0; y < size; y++)
        for (int x = 0; x < size; x++)
            _gridSvc.GetPoint(x, y).IsFilledColor = false;

        foreach (var e in _gridSvc.AllEdges)
            e.IsFilled = false;

        // C) Re-paint via highlight service
        _highlightSvc.ClearPoints();
        _highlightSvc.ClearEdges();

        // D) Reset score & XP
        _scoreSvc.Reset();

        // E) Re-deal the hand

        if (handObject != null)
            handObject.ClearHand();
        handObject.DealNewHand();

        // F) Load the new layout
        loader.SetLevelData(data);
        loader.ApplyLevel();
    }
}