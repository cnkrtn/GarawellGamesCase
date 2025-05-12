using System;
using System.Collections.Generic;
using Core.GridService.Data;
using Core.GridService.Interface;
using UnityEngine;

namespace Grid
{
    public class GridHighlighter : MonoBehaviour
    {
        [Header("Highlight Colours")] [SerializeField]
        private Color validColor = Color.green;

        [SerializeField] private Color placedColor = Color.blue;
        [SerializeField] private Color normalColor = Color.white;

        [Header("Square Visual Prefab & Spacing")] [SerializeField]
        private GameObject squarePrefab;

        [SerializeField] private Transform squareParent; // parent for square visuals
        [SerializeField] private float cellSpacing = 1f;

        private readonly List<Edge> _activeEdges = new List<Edge>();
        private readonly List<Point> _activePoints = new List<Point>();
        private readonly Queue<GameObject> _squarePool = new Queue<GameObject>();
        private readonly Dictionary<Point, GameObject> _squareVisuals = new Dictionary<Point, GameObject>();

        public Color ValidColor => validColor;
        public Color PlacedColor => placedColor;
        public Color NormalColor => normalColor;

        private IGridService _gridService;
        private void Awake()
        {
            _gridService = ReferenceLocator.Instance.GridService;
        }

        public void FlashEdges(IEnumerable<Edge> edges)
        {
            ClearEdges();
            ClearPoints();
            foreach (var e in edges)
            {
                if (e == null || e.IsFilled || e.Renderer == null) continue;
                e.Renderer.color = validColor;
                _activeEdges.Add(e);
                if (e.A?.Renderer != null)
                {
                    e.A.Renderer.color = validColor;
                    _activePoints.Add(e.A);
                }

                if (e.B?.Renderer != null)
                {
                    e.B.Renderer.color = validColor;
                    _activePoints.Add(e.B);
                }
            }
        }

        public void ClearEdges()
        {
            foreach (var e in _activeEdges)
            {
                if (e?.Renderer == null) continue;
                e.Renderer.color = e.IsFilled ? placedColor : normalColor;
            }

            _activeEdges.Clear();
        }

        public void FlashPoints(IEnumerable<Point> points)
        {
            ClearPoints();
            foreach (var p in points)
            {
                if (p == null || p.IsFilledColor || p.Renderer == null) continue;
                p.Renderer.color = validColor;
                _activePoints.Add(p);
            }
        }

        public void ClearPoints()
        {
            foreach (var p in _activePoints)
            {
                if (p?.Renderer == null) continue;
                p.Renderer.color = p.IsFilledColor ? placedColor : normalColor;
            }

            _activePoints.Clear();
        }

        /// <summary>
        /// Show a square marker at the given cell origin using the squareParent.
        /// Positions in world-space to avoid local offset issues.
        /// </summary>
        public void ShowSquareVisual(Point origin)
        {
            if (_squareVisuals.ContainsKey(origin))
                return;

            // 1) get marker
            GameObject marker;
            if (_squarePool.Count > 0)
                marker = _squarePool.Dequeue();
            else
                marker = Instantiate(squarePrefab);

            // 2) compute true world‐space center of that cell
            float spacing      = _gridService.Spacing;
            Vector3 gridOrigin = _gridService.Origin;    
            float cx = (origin.X + 0.5f) * spacing;
            float cy = (origin.Y + 0.5f) * spacing;
            Vector3 worldCenter = gridOrigin + new Vector3(cx, cy, 0f);

            // 3) set position, rotation, scale, parent
            marker.transform.position = worldCenter;
            marker.transform.rotation = Quaternion.identity;
            marker.transform.localScale = Vector3.one * spacing;  // or multiply by visualScale if you like
            marker.transform.SetParent(squareParent, true);
            marker.SetActive(true);

            _squareVisuals[origin] = marker;
        }


        /// <summary>
        /// Hide (and pool) the square marker at the given origin.
        /// </summary>
        public void HideSquareVisual(Point origin)
        {
            if (!_squareVisuals.TryGetValue(origin, out var marker))
                return;

            _squareVisuals.Remove(origin);
            marker.SetActive(false);
            _squarePool.Enqueue(marker);
        }
    }
}