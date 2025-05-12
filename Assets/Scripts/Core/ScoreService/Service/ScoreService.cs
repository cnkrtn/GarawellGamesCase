// Core/ScoreService/Service/ScoreService.cs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.GridService.Data;
using Core.GridService.Interface;
using Core.GridHighlightService.Interface;
using Tile; // for EventService & TileDrag
using UnityEngine;

namespace Core.ScoreService.Service
{
    public class ScoreService : IScoreService, IDisposable
    {
        private IGridService _gridService;
        private IGridHighlightService _gridHighlightService;
        private int _score;
        private int _combo;

        public int CurrentScore => _score;
        public int CurrentCombo => _combo;

        public Task Inject()
        {
            _gridService = ReferenceLocator.Instance.GridService;
            _gridHighlightService = ReferenceLocator.Instance.GridHighlightService;
            EventService.TilePlaced += OnTilePlaced;
            Debug.Log("[ScoreService] Subscribed to TilePlaced");
            return Task.CompletedTask;
        }

        private void OnTilePlaced(TileDrag tile)
        {
            // 1) Get exactly the edges you just filled
            var placedEdges = _gridService.GetEdges(tile.Shape, tile.OriginCell);
            int size = _gridService.GridSize;
            var completedCorners = new HashSet<Point>();

            // 2) For each edge, test its two neighboring unit squares
            foreach (var e in placedEdges)
            {
                int dx = e.B.X - e.A.X;
                int dy = e.B.Y - e.A.Y;

                // Build the two candidate corner coords (bottom-left of each square)
                List<Vector2Int> candidates = new List<Vector2Int>();

                if (dx != 0) // horizontal edge
                {
                    // square above: same x,y
                    candidates.Add(new Vector2Int(e.A.X, e.A.Y));
                    // square below: y-1
                    candidates.Add(new Vector2Int(e.A.X, e.A.Y - 1));
                }
                else if (dy != 0) // vertical edge
                {
                    // square to right: x,y
                    candidates.Add(new Vector2Int(e.A.X, e.A.Y));
                    // square to left: x-1,y
                    candidates.Add(new Vector2Int(e.A.X - 1, e.A.Y));
                }

                // Try each candidate
                foreach (var c in candidates)
                    TryAddCompleted(c.x, c.y, size, completedCorners);
            }

            // 3) Score & combo
            if (completedCorners.Count > 0)
            {
                _combo += completedCorners.Count;
                int gained = completedCorners.Count * _combo;
                _score += gained;
                Debug.Log($"[ScoreService] Combo +{completedCorners.Count} → {_combo}; +{gained} pts → {_score}");

                foreach (var corner in completedCorners)
                    _gridHighlightService.ShowSquareVisual(corner);

                ClearFullLines(size);
            }
            else if (_combo > 0)
            {
                Debug.Log($"[ScoreService] Combo reset from {_combo} to 0");
                _combo = 0;
            }
        }

        /// <summary>
        /// Bounds-check (cx,cy) in [0..size-2], fetch its Point, then add
        /// if that 1×1 is fully filled.
        /// </summary>
        private void TryAddCompleted(int cx, int cy, int size, HashSet<Point> set)
        {
            if (cx < 0 || cy < 0 || cx >= size - 1 || cy >= size - 1)
                return;

            var corner = _gridService.GetPoint(cx, cy);
            if (IsSquareComplete(corner))
                set.Add(corner);
        }


        private bool IsSquareComplete(Point o)
        {
            var right = _gridService.GetPoint(o.X + 1, o.Y);
            var up = _gridService.GetPoint(o.X, o.Y + 1);
            var upRight = _gridService.GetPoint(o.X + 1, o.Y + 1);

            var edges = new[]
            {
                _gridService.GetEdge(o, right),
                _gridService.GetEdge(up, upRight),
                _gridService.GetEdge(o, up),
                _gridService.GetEdge(right, upRight),
            };

            return edges.All(e => e != null && e.IsFilled);
        }

        private void ClearSquare(Point o)
        {
            Point bottomLeft  = _gridService.GetPoint(o.X,     o.Y);
            Point bottomRight = _gridService.GetPoint(o.X + 1, o.Y);
            Point    topLeft  = _gridService.GetPoint(o.X,     o.Y + 1);
            Point   topRight  = _gridService.GetPoint(o.X + 1, o.Y + 1);

            var edges = new[]
            {
                _gridService.GetEdge(o,       bottomRight),
                _gridService.GetEdge(topLeft, topRight),
                _gridService.GetEdge(o,       topLeft),
                _gridService.GetEdge(bottomRight, topRight)
            };
            foreach (var e in edges)
            {
                if (e == null) continue;
                e.IsFilled        = false;
                e.A.IsFilledColor = false;
                e.B.IsFilledColor = false;
                if (e.Renderer)
                    e.Renderer.color = _gridHighlightService.NormalColor;
            }
            // 3) **Now also clear the corner points themselves**
            var points = new[] { bottomLeft, bottomRight, topLeft, topRight };
            foreach (var p in points)
            {
                p.IsFilledColor = false;
                if (p.Renderer)
                    p.Renderer.color = _gridHighlightService.NormalColor;
            }
        }

        private void ClearFullLines(int size)
        {
            // rows
            for (int y = 0; y < size - 1; y++)
            {
                bool full = true;
                for (int x = 0; x < size - 1; x++)
                {
                    if (!IsSquareComplete(_gridService.GetPoint(x, y)))
                    {
                        full = false;
                        break;
                    }
                }

                if (full)
                {
                    Debug.Log($"[ScoreService] Clearing full row {y}");
                    for (int x = 0; x < size - 1; x++)
                    {
                        var corner = _gridService.GetPoint(x, y);
                        _gridHighlightService.HideSquareVisual(corner);
                        ClearSquare(corner);
                    }
                }
            }

            // columns
            for (int x = 0; x < size - 1; x++)
            {
                bool full = true;
                for (int y = 0; y < size - 1; y++)
                {
                    if (!IsSquareComplete(_gridService.GetPoint(x, y)))
                    {
                        full = false;
                        break;
                    }
                }

                if (full)
                {
                    Debug.Log($"[ScoreService] Clearing full column {x}");
                    for (int y = 0; y < size - 1; y++)
                    {
                        var corner = _gridService.GetPoint(x, y);
                        _gridHighlightService.HideSquareVisual(corner);
                        ClearSquare(corner);
                    }
                }
            }
        }

        public void Dispose()
        {
            EventService.TilePlaced -= OnTilePlaced;
        }
    }
}