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
       
        yield return new WaitUntil(() =>
            ReferenceLocator.Instance.GridService.GridWidth > 0 &&
            ReferenceLocator.Instance.GridService.GridHeight > 0
        );

      
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
      //  Debug.Log($"[LevelManager] Loading level {idx + 1}: {data.name}");


        _highlightSvc.ClearAllSquares();


        int w = _gridSvc.GridWidth;
        int h = _gridSvc.GridHeight;


        for (int y = 0; y <= h; y++)
        for (int x = 0; x <= w; x++)
        {
            var p = _gridSvc.GetPoint(x, y);
            p.IsFilledColor = false;

            if (p.Renderer)
                p.Renderer.color = _highlightSvc.NormalColor;
        }


        foreach (var e in _gridSvc.AllEdges)
        {
            e.IsFilled = false;
            if (e.Renderer)
                e.Renderer.color = _highlightSvc.NormalColor;
        }


        _highlightSvc.ClearPoints();
        _highlightSvc.ClearEdges();


        _scoreSvc.Reset();


        if (handObject != null)
            handObject.ClearHand();


        handObject.DealNewHand();


        loader.SetLevelData(data);
        loader.ApplyLevel();
    }
}