using System.Collections;
using System.Linq;
using Core.GridHighlightService.Interface;
using Core.GridService.Data;
using Core.GridService.Interface;
using Core.SceneLoaderService.Keys;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] private SpriteRenderer boardBackground;

    private LevelData _levelData;
    private IGridService _grid;
    private IGridHighlightService _highlight;

    public void SetLevelData(LevelData data) => _levelData = data;

    void Awake()
    {
        _grid      = ReferenceLocator.Instance.GridService;
        _highlight = ReferenceLocator.Instance.GridHighlightService;

        // subscribe to the service event
        ReferenceLocator.Instance.SceneLoaderService.SceneLoaded += OnServiceSceneLoaded;
    }

    void OnDestroy()
    {
        ReferenceLocator.Instance.SceneLoaderService.SceneLoaded -= OnServiceSceneLoaded;
    }


    private IEnumerator Start()
    {
        // same guard: wait for grid
        yield return new WaitUntil(() => ReferenceLocator.Instance.GridService.GridSize > 0);

        if (_levelData != null)
            ApplyLevel();
    }
    private void OnServiceSceneLoaded(string sceneName)
    {
        if (sceneName != SceneKeys.KEY_GAME_START_SCENE) return;

        // clear old square visuals
        _highlight.ClearAllSquares();

        // re-apply current level
        if (_levelData != null)
            ApplyLevel();
    }
    public void ApplyLevel()
    {
        int size = _grid.GridSize;

        // 1) Clear any existing fill‐state in the grid data
        foreach (var p in Enumerable.Range(0, size)
                     .SelectMany(x => Enumerable.Range(0, size)
                         .Select(y => _grid.GetPoint(x, y))))
        {
            p.IsFilledColor = false;
            if (p.Renderer) p.Renderer.color = _highlight.NormalColor;
        }

        foreach (var e in _grid.AllEdges)
        {
            e.IsFilled = false;
            if (e.Renderer) e.Renderer.color = _highlight.NormalColor;
        }

        // 2) For each closed cell, mark its corners & edges as filled—and color them immediately
        foreach (var cell in _levelData.closedCells)
        {
            int cx = cell.x, cy = cell.y;
            if (cx < 0 || cy < 0 || cx >= size - 1 || cy >= size - 1)
            {
                Debug.LogWarning($"LevelLoader: cell {cell} out of range, skipping.");
                continue;
            }

            // corner points
            var bl = _grid.GetPoint(cx,   cy);
            var br = _grid.GetPoint(cx+1, cy);
            var tl = _grid.GetPoint(cx,   cy+1);
            var tr = _grid.GetPoint(cx+1, cy+1);

            foreach (var pt in new[]{ bl, br, tl, tr })
            {
                pt.IsFilledColor = true;
                if (pt.Renderer)
                    pt.Renderer.color = _highlight.PlacedColor;
            }

            // edges
            var edges = new[]
            {
                _grid.GetEdge(bl, br),
                _grid.GetEdge(tl, tr),
                _grid.GetEdge(bl, tl),
                _grid.GetEdge(br, tr)
            };
            foreach (var e in edges)
            {
                if (e == null) continue;
                e.IsFilled = true;
                if (e.Renderer)
                    e.Renderer.color = _highlight.PlacedColor;
            }
        }

        // 3) Finally, place the 1×1 square visuals on every closed cell
        foreach (var cell in _levelData.closedCells)
        {
            var bl = _grid.GetPoint(cell.x, cell.y);
            _highlight.ShowSquareVisual(bl);
        }
    }


}
