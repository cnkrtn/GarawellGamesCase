using System;
using System.Collections.Generic;
using Core.GridService.Data;
using Core.GridService.Interface;
using DG.Tweening;
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

        [SerializeField] private Transform squareParent;
        // [SerializeField] private float cellSpacing = 1f;

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

                // full‐alpha valid‐highlight
                e.Renderer.color = new Color(validColor.r, validColor.g, validColor.b, 1f);
                _activeEdges.Add(e);
                if (e.A?.Renderer != null)
                {
                    e.A.Renderer.color = new Color(validColor.r, validColor.g, validColor.b, 1f);
                    _activePoints.Add(e.A);
                }

                if (e.B?.Renderer != null)
                {
                    e.B.Renderer.color = new Color(validColor.r, validColor.g, validColor.b, 1f);
                    _activePoints.Add(e.B);
                }
            }
        }

        public void FlashPoints(IEnumerable<Point> points)
        {
            ClearPoints();
            foreach (var p in points)
            {
                if (p == null || p.IsFilledColor || p.Renderer == null) continue;

                p.Renderer.color = new Color(validColor.r, validColor.g, validColor.b, 1f);
                _activePoints.Add(p);
            }
        }

        public void ClearEdges()
        {
            foreach (var e in _activeEdges)
            {
                if (e?.Renderer == null) continue;

                // pick base color
                var baseCol = e.IsFilled ? placedColor : normalColor;
                // choose alpha
                float a = e.IsFilled ? 1f : (150f / 255f);
                e.Renderer.color = new Color(baseCol.r, baseCol.g, baseCol.b, a);
            }

            _activeEdges.Clear();
        }

        public void ClearPoints()
        {
            foreach (var p in _activePoints)
            {
                if (p?.Renderer == null) continue;

                var baseCol = p.IsFilledColor ? placedColor : normalColor;
                float a = p.IsFilledColor ? 1f : (150f / 255f);
                p.Renderer.color = new Color(baseCol.r, baseCol.g, baseCol.b, a);
            }

            _activePoints.Clear();
        }


        /// <summary>
        /// Show a square marker at the given cell origin.
        /// </summary>
        public void ShowSquareVisual(Point origin)
        {
            if (_squareVisuals.ContainsKey(origin))
                return;

            GameObject marker = _squarePool.Count > 0
                ? _squarePool.Dequeue()
                : Instantiate(squarePrefab);

            float spacing = _gridService.Spacing;
            Vector3 gridOrigin = _gridService.Origin;
            float cx = (origin.X + 0.5f) * spacing;
            float cy = (origin.Y + 0.5f) * spacing;
            Vector3 worldCenter = gridOrigin + new Vector3(cx, cy, 0f);

            marker.transform.position = worldCenter;
            marker.transform.rotation = Quaternion.identity;
            marker.transform.localScale = Vector3.one * spacing;
            marker.transform.SetParent(squareParent, true);

            if (marker.TryGetComponent(out SpriteRenderer renderer))
            {
                renderer.color = new Color(placedColor.r, placedColor.g, placedColor.b, 1f);
            }

            marker.SetActive(true);
            _squareVisuals[origin] = marker;
        }

        /// <summary>
        /// Burst‐and‐shrink one square immediately.
        /// </summary>
        public void HideSquareVisual(Point origin)
        {
            if (!_squareVisuals.TryGetValue(origin, out var marker))
                return;

            _squareVisuals.Remove(origin);

            marker.transform
                .DOScale(0f, 0.3f)
                .SetEase(Ease.InBack)
                .OnComplete(() =>
                {
                    marker.SetActive(false);
                    marker.transform.localScale = Vector3.one * _gridService.Spacing;
                    _squarePool.Enqueue(marker);
                });
        }
        public void ClearAllSquares()
        {
            // deactivate and pool every square
            foreach (var kv in _squareVisuals)
            {
                var marker = kv.Value;
                marker.SetActive(false);
                _squarePool.Enqueue(marker);
            }
            _squareVisuals.Clear();
        }

        /// <summary>
        /// Burst (shrink‐and‐hide) the given squares in sequence.
        /// </summary>
        public void BurstSquaresSequential(IEnumerable<Point> origins, float interval = 0.1f)
        {
            float spacing = _gridService.Spacing;
            float upDur = 0.05f; // time to scale *up*
            float downDur = 0.1f; // time to scale *down*

            var seq = DOTween.Sequence();
            foreach (var origin in origins)
            {
                if (!_squareVisuals.TryGetValue(origin, out var marker))
                    continue;

                _squareVisuals.Remove(origin);

                // 1) pop up to 120%
                seq.Append(marker.transform
                        .DOScale(spacing * 1.2f, upDur)
                        .SetEase(Ease.OutBack))
                    // 2) then shrink to zero
                    .Append(marker.transform
                        .DOScale(0f, downDur)
                        .SetEase(Ease.InBack))
                    // 3) once done, deactivate & pool
                    .AppendCallback(() =>
                    {
                        marker.SetActive(false);
                        marker.transform.localScale = Vector3.one * spacing;
                        _squarePool.Enqueue(marker);
                    })
                    // 4) pause before next
                    .AppendInterval(interval);
            }
        }
    }
}