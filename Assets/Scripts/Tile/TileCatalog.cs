using System;
using System.Collections.Generic;
using System.Linq;
using Tile;
using UnityEngine;

[CreateAssetMenu(menuName = "Tiles/TileCatalog")]
public class TileCatalog : ScriptableObject
{
    public List<Entry> entries;

    [Serializable]
    public struct Entry
    {
        public TileId id;
        public GameObject prefab;
        public ShapeData shape;
    }

    private Dictionary<TileId, GameObject> _map;
    public GameObject Get(TileId id)
    {
        _map ??= entries.ToDictionary(e => e.id, e => e.prefab);
        return _map[id];
    }
}