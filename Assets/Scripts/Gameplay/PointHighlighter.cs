using UnityEngine;
using Grid;

public class PointClickHighlighter : MonoBehaviour
{
    [SerializeField] private GridBuilder     gridBuilder;
    [SerializeField] private GridHighlighter highlighter;
    [SerializeField] private Camera          cam;  // if null, will use Camera.main

    private bool       _dragging;
    private Vector2Int _last = new Vector2Int(-1, -1);

    void Start()
    {
        if (cam == null) cam = Camera.main;
    }

    void Update()
    {
        // Begin dragging on mouse‐down
        if (Input.GetMouseButtonDown(0))
            _dragging = true;

        // End dragging on mouse‐up
        if (Input.GetMouseButtonUp(0))
        {
            _dragging = false;
            highlighter.ClearPoints();
            _last = new Vector2Int(-1, -1);
        }

        // While held, update the highlight
        if (_dragging)
            TryHighlightPoint();
    }

    private void TryHighlightPoint()
    {
        // 1) get world‐pos via your MouseWorld() helper
        Vector3 world = MouseWorld();

        // 2) convert to nearest grid corner
        Vector3 local = world - gridBuilder.transform.position;
        float   s     = gridBuilder.Spacing;
        int     cx    = Mathf.RoundToInt(local.x / s);
        int     cy    = Mathf.RoundToInt(local.y / s);

        var cell = new Vector2Int(cx, cy);
        if (cell == _last) return;
        _last = cell;

        // 3) clear old, flash new
        highlighter.ClearPoints();
        highlighter.FlashPoint(cx, cy, gridBuilder);
    }

    // reuse your existing MouseWorld() method
    private Vector3 MouseWorld()
    {
        var m = Input.mousePosition;
        m.z = -cam.transform.position.z;
        return cam.ScreenToWorldPoint(m);
    }
}