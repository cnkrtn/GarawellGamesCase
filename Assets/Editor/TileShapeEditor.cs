using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ShapeData))]
public class TileShapeEditor : Editor
{
    const int   GRID  = 5;    // 5 × 5 board
    const float PAD   = 8f;   // border px
    const float DOT   = 4f;   // node radius

    public override void OnInspectorGUI()
    {
        // Draw default fields (tileId + edges list)
        DrawDefaultInspector();

        // Reserve a square for the preview
        GUILayout.Space(8);
        float size = Mathf.Min(EditorGUIUtility.currentViewWidth - 16, 220);
        Rect rect  = GUILayoutUtility.GetRect(size, size);

        DrawPreview(rect);

        if (GUI.changed) Repaint();      // live update while typing
    }

    // ─── PREVIEW ────────────────────────────────────────────────────────────
    void DrawPreview(Rect rect)
    {
        var shape = (ShapeData)target;
        if (shape.edges == null || shape.edges.Count == 0) return;

        Handles.BeginGUI();

        // board background
        Handles.color = new Color(0,0,0, .05f);
        Handles.DrawSolidRectangleWithOutline(rect, new Color(0,0,0, .02f), Color.gray);

        float cell = (rect.width - PAD * 2) / (GRID - 1);
        float left = rect.x + PAD;
        float bot  = rect.yMax - PAD;

        // collect unique points for dot drawing
        var points = new System.Collections.Generic.HashSet<Vector2Int>();

        // draw edges
        Handles.color = Color.green;
        foreach (var e in shape.edges)
        {
            Vector2Int a = e.pointA;
            Vector2Int b = e.pointB;

            Vector2 guiA = new(left + a.x * cell, bot - a.y * cell);
            Vector2 guiB = new(left + b.x * cell, bot - b.y * cell);
            Handles.DrawLine(guiA, guiB, 3f);

            points.Add(a);
            points.Add(b);
        }

        // draw dots
        Handles.color = Color.black;
        foreach (var p in points)
        {
            Vector2 gui = new(left + p.x * cell, bot - p.y * cell);
            Handles.DrawSolidDisc(gui, Vector3.forward, DOT);
        }

        Handles.EndGUI();
    }
}
