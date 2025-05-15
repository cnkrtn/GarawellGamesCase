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

        ReferenceLocator.Instance.SceneLoaderService.SceneLoaded += OnServiceSceneLoaded;
    }

    void OnDestroy()
    {
        ReferenceLocator.Instance.SceneLoaderService.SceneLoaded -= OnServiceSceneLoaded;
    }

    private IEnumerator Start()
    {
        // wait until grid is built
        yield return new WaitUntil(() => ReferenceLocator.Instance.GridService.GridWidth > 0);

        if (_levelData != null)
            ApplyLevel();
    }

    private void OnServiceSceneLoaded(string sceneName)
    {
        if (sceneName != SceneKeys.KEY_GAME_START_SCENE) return;

        _highlight.ClearAllSquares();
        if (_levelData != null)
            ApplyLevel();
    }

    public void ApplyLevel()
    {
        

        int w = _grid.GridWidth;
        int h = _grid.GridHeight;

        // 2) clear all points
        for (int y = 0; y <= h; y++)
        for (int x = 0; x <= w; x++)
        {
            var pt = _grid.GetPoint(x, y);
            pt.IsFilledColor = false;
            if (pt.Renderer)
                pt.Renderer.color = _highlight.NormalColor;
        }

        // 3) clear all edges
        foreach (var e in _grid.AllEdges)
        {
            e.IsFilled = false;
            if (e.Renderer)
                e.Renderer.color = _highlight.NormalColor;
        }

        // 4) fill preset cells
        foreach (var cell in _levelData.closedCells)
        {
            int cx = cell.x, cy = cell.y;
            // skip out of range
            if (cx < 0 || cy < 0 || cx >= w || cy >= h)
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

        // 5) show square visuals
        foreach (var cell in _levelData.closedCells)
        {
            int cx = cell.x, cy = cell.y;
            if (cx < 0 || cy < 0 || cx >= w || cy >= h)
                continue;

            var bl = _grid.GetPoint(cx, cy);
            _highlight.ShowSquareVisual(bl);
        }
    }
}
