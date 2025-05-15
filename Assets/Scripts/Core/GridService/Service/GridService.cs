using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using Core.GridService.Data;
using Core.GridService.Interface;
using Grid;

namespace Core.GridService.Service
{
    public class GridService : IGridService
    {
        private Point[,] _points;
        private List<Edge> _edges;
        private GridLogic _logic;
        private GridSettings _gridSettings;

        private bool _built;

        public IEnumerable<Edge> AllEdges => _edges;

        public float Spacing => _gridSettings.spacing;
        public Vector3 Origin => _gridSettings.origin;
        public int GridWidth => _gridSettings.width;
        public int GridHeight => _gridSettings.height;

        public async Task Inject(GridPrefabs prefabs, GridSettings cfg)
        {
            _gridSettings = cfg;

            _points = new Point[cfg.width + 1, cfg.height + 1];
            _edges = new List<Edge>();

            BuildPoints(prefabs);
            BuildEdges(prefabs);

            _logic = new GridLogic(_points, _edges);


            prefabs.pointsParent.localScale = Vector3.one * cfg.visualScale;
            prefabs.edgesParent.localScale = Vector3.one * cfg.visualScale;

            await Task.CompletedTask;
        }

        private void BuildPoints(GridPrefabs p)
        {
            for (int y = 0; y <= _gridSettings.height; y++)
            for (int x = 0; x <= _gridSettings.width; x++)
            {
                Vector3 world = _gridSettings.origin + new Vector3(x * _gridSettings.spacing,
                    y * _gridSettings.spacing,
                    0f);
                var go = Object.Instantiate(p.pointPrefab, world,
                    Quaternion.identity,
                    p.pointsParent);
                go.name = $"Point_{x}_{y}";

                go.transform.localScale =
                    go.transform.localScale * _gridSettings.spacing * (_gridSettings.visualScale / .9f);
                var sr = go.GetComponentInChildren<SpriteRenderer>();
                var pt = new Point(x, y, world) { Renderer = sr };
                _points[x, y] = pt;
            }
        }

        private void BuildEdges(GridPrefabs p)
        {
            for (int y = 0; y <= _gridSettings.height; y++)
            for (int x = 0; x <= _gridSettings.width; x++)
            {
                var cur = _points[x, y];
                if (x < _gridSettings.width) CreateEdge(cur, _points[x + 1, y], p);
                if (y < _gridSettings.height) CreateEdge(cur, _points[x, y + 1], p);
            }
        }

        private void CreateEdge(Point a, Point b, GridPrefabs p)
        {
            var e = new Edge(a, b);
            _edges.Add(e);

            Vector3 mid = (a.WorldPos + b.WorldPos) * 0.5f;
            var go = Object.Instantiate(p.edgePrefab, mid,
                Quaternion.identity,
                p.edgesParent);

            if (Mathf.Abs(a.X - b.X) < 0.1f)
                go.transform.rotation = Quaternion.Euler(0, 0, 90);
            go.transform.localScale =
                go.transform.localScale * _gridSettings.spacing * (-_gridSettings.visualScale / .9f);
            e.Renderer = go.GetComponentInChildren<SpriteRenderer>();
        }


        public Point GetPoint(int x, int y) => _points[x, y];
        public Edge GetEdge(Point a, Point b) => _logic.GetEdge(a, b);

        private bool InBounds(Vector2Int p) =>
            p.x >= 0 && p.y >= 0 &&
            p.x <= _gridSettings.width && p.y <= _gridSettings.height;

        public Edge[] GetEdges(ShapeData shape, Vector2Int origin)
        {
            var list = new Edge[shape.edges.Count];
            for (int i = 0; i < list.Length; i++)
            {
                var ed = shape.edges[i];
                Vector2Int a = origin + ed.pointA;
                Vector2Int b = origin + ed.pointB;

                if (!InBounds(a) || !InBounds(b))
                    return null;

                var pA = _logic.GetPointAt(a.x, a.y);
                var pB = _logic.GetPointAt(b.x, b.y);
                var e = _logic.GetEdge(pA, pB);
                if (e == null) return null;

                list[i] = e;
            }

            return list;
        }

        public bool CanPlaceShape(ShapeData shape)
        {
            for (int y = 0; y <= _gridSettings.height; y++)
            for (int x = 0; x <= _gridSettings.width; x++)
            {
                var origin = new Vector2Int(x, y) - shape.anchorPoint;
                var edges = GetEdges(shape, origin);
                if (edges == null) continue;
                if (edges.All(e => !e.IsFilled)) return true;
            }

            return false;
        }
    }
}