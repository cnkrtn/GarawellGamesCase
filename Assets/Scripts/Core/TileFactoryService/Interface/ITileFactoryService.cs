using System.Threading.Tasks;
using Tile;
using UnityEngine;

namespace Core.TileFactoryService.Interface
{
    public interface ITileFactoryService
    {
        Task Inject(TileCatalog catalog);
        
        TileCatalog Catalog { get; } 
        GameObject Spawn(TileId id, Transform parent);
        void Despawn(GameObject tile);
    }

}