using UnityEngine;
using System.Collections.Generic;
using Tile;
using UnityEngine.Serialization;

[CreateAssetMenu]
public class ShapeData : ScriptableObject
{
    public TileId    tileID;
    public bool isLarge;
    public List<EdgeData> edges;         
    [Min(0)] public int weight = 1;
    [Tooltip("Anchor Point")]
    public Vector2Int anchorPoint = new Vector2Int(0,0);
}
