using Core.GridService.Data;
using Core.TileFactoryService.Service;
using UnityEngine;

namespace Grid
{
    public class GridObject : MonoBehaviour
    {
        [Header("Prefabs & Parents")]
        [SerializeField] private GameObject pointPrefab;
        [SerializeField] private GameObject edgePrefab;
        [SerializeField] private Transform  pointsParent;
        [SerializeField] private Transform  edgesParent;
        
        [Header("Grid Settings")]
        [SerializeField] private int    size           = 5;
        [SerializeField] private float  spacing        = 1f;
        [SerializeField] private Vector3 gridOrigin    = Vector2.zero;
        [SerializeField] private float  visualScale    = 0.8f;

        [SerializeField] private GridHighlighter highlighter;
        [SerializeField] private TileCatalog _tileCatalog;
        private async void Awake()
        {
            var cfg = new GridSettings {
                size        = size,
                spacing     = spacing,
                origin      = gridOrigin,
                visualScale = visualScale
            };

            var prefabs = new GridPrefabs {
                pointPrefab  = pointPrefab,
                edgePrefab   = edgePrefab,
                pointsParent = pointsParent,
                edgesParent  = edgesParent
            };

            await ReferenceLocator.Instance.GridService.Inject(prefabs, cfg);
           
            await ReferenceLocator.Instance.TileFactoryService.Inject(_tileCatalog);
            await ReferenceLocator.Instance.GridHighlightService.Inject(highlighter);
            await ReferenceLocator.Instance.HandService.Inject(_tileCatalog);
           
            // presenter done with its job
            enabled = false;
        }
    }

}