using UnityEngine;

[System.Serializable]
public struct ShapeEdges
{
    public Vector2Int pointA; // Offset from anchor (e.g. (0,0))
    public Vector2Int pointB; // Offset from anchor (e.g. (1,0))

    public ShapeEdges(Vector2Int a, Vector2Int b)
    {
        pointA = a;
        pointB = b;
    }
}