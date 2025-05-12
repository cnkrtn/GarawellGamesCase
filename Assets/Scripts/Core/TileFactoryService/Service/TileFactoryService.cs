// Core/TileFactoryService/Service/TileFactoryService.cs

using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Core.TileFactoryService.Interface;
using Tile;

namespace Core.TileFactoryService.Service
{
    public class TileFactoryService : ITileFactoryService
    {
        private TileCatalog _catalog;
        private readonly Dictionary<TileId, Queue<GameObject>> _pool = new();
        private readonly Dictionary<GameObject, TileId> _goToId = new();
        public TileCatalog Catalog => _catalog;

        public Task Inject(TileCatalog catalog)
        {
            _catalog = catalog;
            foreach (var entry in _catalog.entries)
                _pool[entry.id] = new Queue<GameObject>();
            return Task.CompletedTask;
        }

        public GameObject Spawn(TileId id, Transform parent)
        {
            GameObject go = _pool[id].Count > 0
                ? _pool[id].Dequeue()
                : Object.Instantiate(_catalog.Get(id));

            go.transform.SetParent(parent, false);
            // reset so it always sits correctly
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale    = Vector3.one*.1f;

            go.SetActive(true);
            _goToId[go] = id;
            return go;
        }

        public void Despawn(GameObject tile)
        {
            if (!_goToId.TryGetValue(tile, out var id))
            {
                Debug.LogWarning($"[TileFactory] Despawn called on untracked GameObject {tile.name}");
                // just ignore untracked tiles (no destroy)
                return;
            }

            _goToId.Remove(tile);

            // reset before pooling
            tile.transform.SetParent(null, false);
            tile.transform.localPosition = Vector3.zero;
            tile.transform.localRotation = Quaternion.identity;
            tile.transform.localScale    = Vector3.one*.1f;

            tile.SetActive(false);
            _pool[id].Enqueue(tile);
        }
    }
}
