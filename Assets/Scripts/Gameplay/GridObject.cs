using Core.GridService.Data;
using Core.TileFactoryService.Service;
using Tile.Core.IGridSizeService.Service;
using UnityEngine;

namespace Grid
{
    [DefaultExecutionOrder(-1000)]
    public class GridObject : MonoBehaviour
    {
        [Header("Grid Size in POINTS (nodes)")]
        private int pointsX;
        private int pointsY;

        [Header("Screen Padding (world units)")]
        [SerializeField] private float horizontalPadding = 1f;
        [SerializeField] private float verticalPadding   = 1f;

        [Header("Manual Offset (world units)")]
        [Tooltip("Drag X to shift grid left/right, Y to shift up/down")]
        [SerializeField] private Vector2 manualOffset = Vector2.zero;

        [Header("Visual Scale & Prefabs")]
        [SerializeField] private float visualScale = 0.8f;
        [SerializeField] private GameObject pointPrefab;
        [SerializeField] private GameObject edgePrefab;
        [SerializeField] private Transform pointsParent;
        [SerializeField] private Transform edgesParent;
        [SerializeField] private GridHighlighter highlighter;
        [SerializeField] private TileCatalog tileCatalog;

        private async void Awake()
        {
            
            await ReferenceLocator.Instance.GridSizeService.Inject();
            (pointsX, pointsY) = ReferenceLocator.Instance.GridSizeService.GetGridSize();
            // 1) cell counts
            int cellsX = Mathf.Max(1, pointsX - 1);
            int cellsY = Mathf.Max(1, pointsY - 1);

            // 2) camera bounds
            var cam   = Camera.main;
            float camH = cam.orthographicSize * 2f;
            float camW = camH * cam.aspect;
            Vector3 camCenter = cam.transform.position;

            // 3) world‐space camera rect
            float worldLeft   = camCenter.x - camW  / 2f;
            float worldRight  = camCenter.x + camW  / 2f;
            float worldBottom = camCenter.y - camH  / 2f;
            float worldTop    = camCenter.y + camH  / 2f;

            // 4) inner rect after padding
            float innerLeft   = worldLeft   + horizontalPadding;
            float innerRight  = worldRight  - horizontalPadding;
            float innerBottom = worldBottom + verticalPadding;
            float innerTop    = worldTop    - verticalPadding;
            float availW      = innerRight  - innerLeft;
            float availH      = innerTop    - innerBottom;

            // 5) compute spacing
            float spacing = Mathf.Min(availW / cellsX,
                                      availH / cellsY);

            // 6) grid span
            float gridW = cellsX * spacing;
            float gridH = cellsY * spacing;

            // 7) center the grid in the padded rect
            float marginX = (availW - gridW) * 0.5f;
            float marginY = (availH - gridH) * 0.5f;

            // 8) bottom‐left origin before manual tweak
            Vector3 origin = new Vector3(
                innerLeft   + marginX,
                innerBottom + marginY,
                0f
            );

            // 9) apply your manual offset
            origin += new Vector3(manualOffset.x,
                                  manualOffset.y,
                                  0f);

            // 10) inject into the grid service
            var cfg = new GridSettings {
                width       = cellsX,
                height      = cellsY,
                spacing     = spacing,
                origin      = origin,
                visualScale = visualScale
            };
            var prefabs = new GridPrefabs {
                pointPrefab  = pointPrefab,
                edgePrefab   = edgePrefab,
                pointsParent = pointsParent,
                edgesParent  = edgesParent
            };

            await ReferenceLocator.Instance.GridService.Inject(prefabs, cfg);
            await ReferenceLocator.Instance.TileFactoryService.Inject(tileCatalog);
            await ReferenceLocator.Instance.GridHighlightService.Inject(highlighter);
            await ReferenceLocator.Instance.HandService.Inject(tileCatalog);

            enabled = false;
        }
    }
}
