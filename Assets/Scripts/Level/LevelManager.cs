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
        // wait until the grid is injected and has valid dimensions
        yield return new WaitUntil(() =>
            ReferenceLocator.Instance.GridService.GridWidth > 0 &&
            ReferenceLocator.Instance.GridService.GridHeight > 0
        );

        // now it’s safe to load
        LoadLevel(0);
    }

    void OnDestroy()
    {
        EventService.LevelFinished -= OnLevelFinished;
    }

    private void OnLevelFinished()
    {
        NextLevel();
    }

    /// <summary>
    /// Advance to the next level (wrapping at the end),
    /// resets score/XP, clears hand, and applies the new LevelData.
    /// </summary>
    public void NextLevel()
    {
        // _currentIndex = (_currentIndex + 1) % levels.Count;
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

        // A) Hide every 1×1 square visual
        _highlightSvc.ClearAllSquares();

        // B) Wipe out the grid’s fill‐states
        int w = _gridSvc.GridWidth;
        int h = _gridSvc.GridHeight;

        // points are (w+1)×(h+1)
        for (int y = 0; y <= h; y++)
        for (int x = 0; x <= w; x++)
        {
            var p = _gridSvc.GetPoint(x, y);
            p.IsFilledColor = false;
            // also reset its actual color so nothing lingers
            if (p.Renderer)
                p.Renderer.color = _highlightSvc.NormalColor;
        }

        // edges
        foreach (var e in _gridSvc.AllEdges)
        {
            e.IsFilled = false;
            if (e.Renderer)
                e.Renderer.color = _highlightSvc.NormalColor;
        }

        // C) Clear any leftover highlights
        _highlightSvc.ClearPoints();
        _highlightSvc.ClearEdges();

        // D) Reset score & combo & exp
        _scoreSvc.Reset();

        // E) Clear out the old hand *completely*
        if (handObject != null)
            handObject.ClearHand();

        // F) Deal a fresh new hand
        handObject.DealNewHand();

        // G) Finally, load and apply your LevelData
        loader.SetLevelData(data);
        loader.ApplyLevel();
    }
}