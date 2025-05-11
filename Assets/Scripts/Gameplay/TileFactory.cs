using System.Collections.Generic;
using Shape;
using UnityEngine;

public class TileFactory : MonoBehaviour
{
    [SerializeField] private TileCatalog _catalog;
    private readonly Dictionary<TileId, Queue<GameObject>> _pool = new();

    public GameObject Spawn(TileId id, Vector2Int gridPos)
    {
        if (!_pool.TryGetValue(id, out var q)) q = _pool[id] = new Queue<GameObject>();

        GameObject go = q.Count > 0 ? q.Dequeue()
            : Instantiate(_catalog.Get(id));

        go.transform.position = GridToWorld(gridPos);
        go.SetActive(true);
        return go;
    }

    public void Despawn(TileId id, GameObject go)
    {
        go.SetActive(false);
        _pool[id].Enqueue(go);
    }

    Vector3 GridToWorld(Vector2Int p) => new(p.x, p.y, 0);
}