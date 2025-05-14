using UnityEngine;
using System.Collections.Generic;
using Tile;
using UnityEngine.Serialization;

[CreateAssetMenu]
public class ShapeData : ScriptableObject
{
    public TileId    tileID;
    public bool isLarge;
    public List<EdgeData> edges;         // your existing list of pointA/pointB pairs
    [Min(0)] public int weight = 1;
    [Tooltip("Which grid-point (in shape-local coordinates) acts as the anchor for placement")]
    public Vector2Int anchorPoint = new Vector2Int(0,0);
}
