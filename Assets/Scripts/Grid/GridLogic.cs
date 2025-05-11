using System.Collections.Generic;
using UnityEngine;

public class GridLogic
{
    private readonly Point[,] _points;
    private readonly List<Edge> _edges;
    private readonly Dictionary<(Point, Point), Edge> _edgeMap;

    public GridLogic(Point[,] points, List<Edge> edges)
    {
        _points = points;
        _edges = edges;
        _edgeMap = new();

        foreach (var edge in edges)
        {
            var key = GetEdgeKey(edge.A, edge.B);
            _edgeMap[key] = edge;
        }
    }

    private (Point, Point) GetEdgeKey(Point a, Point b)
    {
        // Ensure consistent key regardless of order
        return (a.X < b.X || a.Y < b.Y) ? (a, b) : (b, a);
    }

    public Edge GetEdge(Point a, Point b)
    {
        _edgeMap.TryGetValue(GetEdgeKey(a, b), out var edge);
        return edge;
    }

    public bool HasEdge(Point a, Point b) => _edgeMap.ContainsKey(GetEdgeKey(a, b));

    public bool IsEdgeFilled(Point a, Point b)
    {
        var edge = GetEdge(a, b);
        return edge != null && edge.IsFilled;
    }

    public void MarkEdgeFilled(Point a, Point b)
    {
        var edge = GetEdge(a, b);
        if (edge != null) edge.IsFilled = true;
    }

    public bool IsSquareClosed(int x, int y)
    {
        // Check if square (x,y) → (x+1,y+1) is enclosed
        if (x >= 4 || y >= 4) return false;

        var p00 = _points[x, y];
        var p10 = _points[x + 1, y];
        var p01 = _points[x, y + 1];
        var p11 = _points[x + 1, y + 1];

        return
            IsEdgeFilled(p00, p10) &&
            IsEdgeFilled(p10, p11) &&
            IsEdgeFilled(p11, p01) &&
            IsEdgeFilled(p01, p00);
    }

    public IEnumerable<Vector2Int> GetAllClosedSquares()
    {
        for (int y = 0; y < 4; y++)
        {
            for (int x = 0; x < 4; x++)
            {
                if (IsSquareClosed(x, y))
                    yield return new Vector2Int(x, y);
            }
        }
    }

    public Point GetPointAt(int x, int y) => _points[x, y];

    public IEnumerable<Edge> AllEdges => _edges;
}
