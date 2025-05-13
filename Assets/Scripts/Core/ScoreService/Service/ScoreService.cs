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
        private int _exp;
        private const int _levelExperienceCap = 1000;
        private const int _experiencePerLine = 100;


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
            // 0) Shape‐placement bonus: 1 pt for each unique corner in the shape
            int uniqueCorners = tile.Shape.edges
                .SelectMany(e => new[] { e.pointA, e.pointB })
                .Distinct()
                .Count();

            _score += uniqueCorners;
            Debug.Log($"[ScoreService] Placed shape ({uniqueCorners} corners) → +{uniqueCorners} pts (total {_score})");
            EventService.ScoreUpdated?.Invoke(_score);

            // 1) Get exactly the edges you just filled
            var placedEdges = _gridService.GetEdges(tile.Shape, tile.OriginCell);
            int size = _gridService.GridSize;
            var completedCorners = new HashSet<Point>();

            // 2) For each edge, test its two neighboring unit squares
            foreach (var e in placedEdges)
            {
                int dx = e.B.X - e.A.X;
                int dy = e.B.Y - e.A.Y;

                var candidates = new List<Vector2Int>();
                if (dx != 0) // horizontal edge
                {
                    candidates.Add(new Vector2Int(e.A.X, e.A.Y)); // above
                    candidates.Add(new Vector2Int(e.A.X, e.A.Y - 1)); // below
                }
                else if (dy != 0) // vertical edge
                {
                    candidates.Add(new Vector2Int(e.A.X, e.A.Y)); // right
                    candidates.Add(new Vector2Int(e.A.X - 1, e.A.Y)); // left
                }

                foreach (var c in candidates)
                    TryAddCompleted(c.x, c.y, size, completedCorners);
            }

            // 3) Square‐completion bonus: 10× combo per square
            if (completedCorners.Count > 0)
            {
                _combo += completedCorners.Count;

                // only emit for second combo and above
                if (_combo > 1)
                    EventService.ComboUpdated?.Invoke(_combo);

                int gained = completedCorners.Count * _combo * 10;
                _score += gained;
                EventService.ScoreUpdated?.Invoke(_score);

                foreach (var corner in completedCorners)
                {
                    _gridHighlightService.ShowSquareVisual(corner);
                    EventService.SquareCompleted?.Invoke(corner, gained);
                }
                   

                ClearFullLines(size);
            }
            else if (_combo > 0)
            {
                Debug.Log($"[ScoreService] Combo reset from {_combo} to 0");
                _combo = 0;
                // no ComboUpdated here so UI won’t display a “×0”
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

        private void ClearSquareConditional(Point o)
        {
            // 1) corner points
            Point bl = _gridService.GetPoint(o.X, o.Y);
            Point br = _gridService.GetPoint(o.X + 1, o.Y);
            Point tl = _gridService.GetPoint(o.X, o.Y + 1);
            Point tr = _gridService.GetPoint(o.X + 1, o.Y + 1);

            var edges = new[]
            {
                _gridService.GetEdge(bl, br),
                _gridService.GetEdge(tl, tr),
                _gridService.GetEdge(bl, tl),
                _gridService.GetEdge(br, tr)
            };

            // --- Clear edges conditionally ---
            foreach (var e in edges)
            {
                if (e == null) continue;

                // find neighboring squares of this edge
                var neighborOrigins = new List<Vector2Int>();
                int dx = e.B.X - e.A.X, dy = e.B.Y - e.A.Y;
                if (dx != 0) // horizontal
                {
                    neighborOrigins.Add(new Vector2Int(e.A.X, e.A.Y));
                    neighborOrigins.Add(new Vector2Int(e.A.X, e.A.Y - 1));
                }
                else // vertical
                {
                    neighborOrigins.Add(new Vector2Int(e.A.X, e.A.Y));
                    neighborOrigins.Add(new Vector2Int(e.A.X - 1, e.A.Y));
                }

                bool usedElsewhere = false;
                foreach (var nc in neighborOrigins)
                {
                    if ((nc.x == o.X && nc.y == o.Y) // skip current
                        || nc.x < 0 || nc.y < 0
                        || nc.x >= _gridService.GridSize - 1
                        || nc.y >= _gridService.GridSize - 1)
                        continue;

                    if (IsSquareComplete(_gridService.GetPoint(nc.x, nc.y)))
                    {
                        usedElsewhere = true;
                        break;
                    }
                }

                if (!usedElsewhere)
                {
                    // clear this edge
                    e.IsFilled = false;
                    e.A.IsFilledColor = false;
                    e.B.IsFilledColor = false;
                    if (e.Renderer)
                        e.Renderer.color = _gridHighlightService.NormalColor;
                }
            }

            // --- Clear corner points conditionally ---
            var points = new[] { bl, br, tl, tr };
            foreach (var p in points)
            {
                if (p == null) continue;

                // check all squares that reference this point
                var squareOrigins = new[]
                {
                    new Vector2Int(p.X, p.Y),
                    new Vector2Int(p.X - 1, p.Y),
                    new Vector2Int(p.X, p.Y - 1),
                    new Vector2Int(p.X - 1, p.Y - 1),
                };

                bool usedByOther = false;
                foreach (var so in squareOrigins)
                {
                    if ((so.x == o.X && so.y == o.Y)
                        || so.x < 0 || so.y < 0
                        || so.x >= _gridService.GridSize - 1
                        || so.y >= _gridService.GridSize - 1)
                        continue;

                    if (IsSquareComplete(_gridService.GetPoint(so.x, so.y)))
                    {
                        usedByOther = true;
                        break;
                    }
                }

                if (!usedByOther)
                {
                    p.IsFilledColor = false;
                    if (p.Renderer)
                        p.Renderer.color = _gridHighlightService.NormalColor;
                }
            }
        }

        public void Reset()
        {
            _score = 0;
            _combo = 0;
            _exp = 0;
            EventService.ScoreUpdated?.Invoke(_score);
            EventService.ComboUpdated?.Invoke(_combo);
            EventService.ExpUpdated?.Invoke(_exp);
        }


        private void ClearFullLines(int size)
        {
            // rows
            for (int y = 0; y < size - 1; y++)
            {
                bool full = true;
                for (int x = 0; x < size - 1; x++)
                    if (!IsSquareComplete(_gridService.GetPoint(x, y)))
                    {
                        full = false;
                        break;
                    }

                if (!full) continue;

                // 0) give 100 points + notify UI
                _score += 100;
                EventService.ScoreUpdated?.Invoke(_score);
                EventService.LineCleared?.Invoke();

                GrantExp(_experiencePerLine);
                // 1) collect the points for this row
                var origins = new List<Point>();
                for (int x = 0; x < size - 1; x++)
                    origins.Add(_gridService.GetPoint(x, y));

                // 2) play the burst tween
                _gridHighlightService.BurstSquaresSequential(origins, 0.1f);

                // 3) hide & clear them logically
                foreach (var origin in origins)
                {
                    _gridHighlightService.HideSquareVisual(origin);
                    ClearSquareConditional(origin);
                }
            }

            // columns (same pattern)
            for (int x = 0; x < size - 1; x++)
            {
                bool full = true;
                for (int y = 0; y < size - 1; y++)
                    if (!IsSquareComplete(_gridService.GetPoint(x, y)))
                    {
                        full = false;
                        break;
                    }

                if (!full) continue;

                // 0) give 100 points + notify UI
                _score += 100;
                EventService.ScoreUpdated?.Invoke(_score);
                EventService.LineCleared?.Invoke();
                GrantExp(_experiencePerLine);
                var origins = new List<Point>();
                for (int y = 0; y < size - 1; y++)
                    origins.Add(_gridService.GetPoint(x, y));

                _gridHighlightService.BurstSquaresSequential(origins, 0.1f);

                foreach (var origin in origins)
                {
                    _gridHighlightService.HideSquareVisual(origin);
                    ClearSquareConditional(origin);
                }
            }
        }

        private void GrantExp(int amount)
        {
            _exp += amount;
            if (_exp >= _levelExperienceCap)
            {
                _exp = _levelExperienceCap;
                EventService.ExpUpdated?.Invoke(_exp);
                EventService.LevelFinished?.Invoke();
            }
            else
            {
                EventService.ExpUpdated?.Invoke(_exp);
            }
        }

        public void Dispose()
        {
            EventService.TilePlaced -= OnTilePlaced;
        }
    }
}