using UnityEngine;

[System.Serializable]
public struct ShapeEdges
{
    public Vector2Int pointA; 
    public Vector2Int pointB; 

    public ShapeEdges(Vector2Int a, Vector2Int b)
    {
        pointA = a;
        pointB = b;
    }
}