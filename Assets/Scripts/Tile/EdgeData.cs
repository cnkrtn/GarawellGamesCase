using UnityEngine;

[System.Serializable]
public struct EdgeData
{
    public Vector2Int pointA;
    public Vector2Int pointB;
  

    public EdgeData(Vector2Int a, Vector2Int b, Sprite sprite = null)
    {
        pointA = a;
        pointB = b;
    }

   
}