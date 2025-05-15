using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.AudioService.Keys;
using Core.AudioService.Service;
using Core.GridService.Data;
using Core.GridService.Interface;
using Core.GridHighlightService.Interface;
using Tile;
using UnityEngine;

namespace Core.ScoreService.Service
{
    public class ScoreService : IScoreService, IDisposable
    {
        private IGridService _gridService;
        private IGridHighlightService _gridHighlightService;
        private IAudioService _audioService;
        private int _score;
        private int _combo;
        private int _exp;

        private const int _levelExperienceCap = 500;
        private const int _experiencePerLine = 100;

        public int CurrentScore => _score;
        public int CurrentCombo => _combo;

        public Task Inject()
        {
            _gridService = ReferenceLocator.Instance.GridService;
            _gridHighlightService = ReferenceLocator.Instance.GridHighlightService;
            _audioService = ReferenceLocator.Instance.AudioService;
            EventService.TilePlaced += OnTilePlaced;
            // Debug.Log("[ScoreService] Subscribed to TilePlaced");
            return Task.CompletedTask;
        }

        private void OnTilePlaced(TileDrag tile)
        {
            int uniqueCorners = tile.Shape.edges
                .SelectMany(e => new[] { e.pointA, e.pointB })
                .Distinct()
                .Count();

            _score += uniqueCorners;
            EventService.ScoreUpdated?.Invoke(_score);


            var placedEdges = _gridService.GetEdges(tile.Shape, tile.OriginCell);
            var completedCorners = new HashSet<Point>();

            foreach (var e in placedEdges)
            {
                int dx = e.B.X - e.A.X, dy = e.B.Y - e.A.Y;
                var candidates = new List<Vector2Int>();

                if (dx != 0)
                {
                    candidates.Add(new Vector2Int(e.A.X, e.A.Y));
                    candidates.Add(new Vector2Int(e.A.X, e.A.Y - 1));
                }
                else
                {
                    candidates.Add(new Vector2Int(e.A.X, e.A.Y));
                    candidates.Add(new Vector2Int(e.A.X - 1, e.A.Y));
                }

                foreach (var c in candidates)
                    TryAddCompleted(c.x, c.y, completedCorners);
            }


            if (completedCorners.Count > 0)
            {
                _combo += completedCorners.Count;
                if (_combo > 1)
                    EventService.ComboUpdated?.Invoke(_combo);

                int perSquarePts = completedCorners.Count * _combo * 10;
                _score += perSquarePts;
                EventService.ScoreUpdated?.Invoke(_score);

                foreach (var corner in completedCorners)
                {
                    _gridHighlightService.ShowSquareVisual(corner);
                    EventService.SquareCompleted?.Invoke(corner, perSquarePts);
                }

                ClearFullLines();
            }
            else if (_combo > 0)
            {
                _combo = 0;
            }
        }

        private void TryAddCompleted(int cx, int cy, HashSet<Point> set)
        {
            int w = _gridService.GridWidth;
            int h = _gridService.GridHeight;

            if (cx < 0 || cy < 0 || cx >= w || cy >= h)
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

        private void ClearFullLines()
        {
            int w = _gridService.GridWidth;
            int h = _gridService.GridHeight;

            // 1) rows
            for (int y = 0; y < h; y++)
            {
                if (!Enumerable.Range(0, w)
                        .All(x => IsSquareComplete(_gridService.GetPoint(x, y))))
                    continue;

                _score += 100;
                EventService.ScoreUpdated?.Invoke(_score);
                EventService.LineCleared?.Invoke();
                GrantExp(_experiencePerLine);

                var origins = Enumerable.Range(0, w)
                    .Select(x => _gridService.GetPoint(x, y))
                    .ToList();

                EventService.SquareCompleted?.Invoke(origins[0], 100);

                _gridHighlightService.BurstSquaresSequential(origins, 0.1f);
                foreach (var origin in origins)
                {
                    _audioService.PlayAudio(AudioKeys.KEY_LINE_CLEARED);
                    _gridHighlightService.HideSquareVisual(origin);
                    ClearSquareConditional(origin);
                }
            }

            // 2) columns
            for (int x = 0; x < w; x++)
            {
                if (!Enumerable.Range(0, h)
                        .All(y => IsSquareComplete(_gridService.GetPoint(x, y))))
                    continue;

                _score += 100;
                EventService.ScoreUpdated?.Invoke(_score);
                EventService.LineCleared?.Invoke();
                GrantExp(_experiencePerLine);

                var origins = Enumerable.Range(0, h)
                    .Select(y => _gridService.GetPoint(x, y))
                    .ToList();

                EventService.SquareCompleted?.Invoke(origins[0], 100);

                _gridHighlightService.BurstSquaresSequential(origins, 0.1f);
                foreach (var origin in origins)
                {
                    _gridHighlightService.HideSquareVisual(origin);
                    ClearSquareConditional(origin);
                }
            }

            int pointsX = w + 1;
            int pointsY = h + 1;
            for (int y = 0; y < pointsY; y++)
            for (int x = 0; x < pointsX; x++)
            {
                var p = _gridService.GetPoint(x, y);
                if (p.Renderer != null)
                    p.Renderer.color = p.IsFilledColor
                        ? _gridHighlightService.PlacedColor
                        : _gridHighlightService.NormalColor;
            }


            foreach (var e in _gridService.AllEdges)
            {
                if (e.Renderer != null)
                    e.Renderer.color = e.IsFilled
                        ? _gridHighlightService.PlacedColor
                        : _gridHighlightService.NormalColor;
            }
        }


        private void ClearSquareConditional(Point o)
        {
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


            foreach (var e in edges)
            {
                if (e == null) continue;


                var neighborOrigins = new List<Vector2Int>();
                int dx = e.B.X - e.A.X, dy = e.B.Y - e.A.Y;
                if (dx != 0)
                {
                    neighborOrigins.Add(new Vector2Int(e.A.X, e.A.Y));
                    neighborOrigins.Add(new Vector2Int(e.A.X, e.A.Y - 1));
                }
                else
                {
                    neighborOrigins.Add(new Vector2Int(e.A.X, e.A.Y));
                    neighborOrigins.Add(new Vector2Int(e.A.X - 1, e.A.Y));
                }

                bool usedElsewhere = false;
                foreach (var nc in neighborOrigins)
                {
                    if ((nc.x == o.X && nc.y == o.Y)
                        || nc.x < 0 || nc.y < 0
                        || nc.x >= _gridService.GridWidth
                        || nc.y >= _gridService.GridHeight)
                        continue;

                    if (IsSquareComplete(_gridService.GetPoint(nc.x, nc.y)))
                    {
                        usedElsewhere = true;
                        break;
                    }
                }


                if (!usedElsewhere)
                {
                    e.IsFilled = false;
                    e.A.IsFilledColor = false;
                    e.B.IsFilledColor = false;
                    if (e.Renderer)
                        e.Renderer.color = _gridHighlightService.NormalColor;
                }
            }


            var points = new[] { bl, br, tl, tr };
            foreach (var p in points)
            {
                if (p == null) continue;


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
                        || so.x >= _gridService.GridWidth
                        || so.y >= _gridService.GridHeight)
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

        public void Reset()
        {
            _score = 0;
            _combo = 0;
            _exp = 0;
            EventService.ScoreUpdated?.Invoke(_score);
            EventService.ComboUpdated?.Invoke(_combo);
            EventService.ExpUpdated?.Invoke(_exp);
        }

        public void Dispose()
        {
            EventService.TilePlaced -= OnTilePlaced;
        }
    }
}