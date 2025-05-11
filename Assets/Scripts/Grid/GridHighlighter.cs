using System.Collections.Generic;
using UnityEngine;
using Grid;   // Edge

namespace Grid
{
    public class GridHighlighter : MonoBehaviour
    {
        [SerializeField] private Color validColor  = Color.green;
        [SerializeField] private Color normalColor = Color.white;

        // we now keep the Edge itself so we can consult IsFilled
        private readonly List<Edge>           _activeEdges  = new();
        private readonly List<SpriteRenderer> _activePoints = new();

        public Color ValidColor => validColor;          // needed by TileDrag

        /* ------------ edges ------------ */

        public void FlashEdges(IEnumerable<Edge> edges)
        {
            ClearEdges();                               // clear previous flash

            foreach (var e in edges)
            {
                if (e?.Renderer == null) continue;
                e.Renderer.color = validColor;
                _activeEdges.Add(e);                    // remember the edge
            }
        }

        public void ClearEdges()
        {
            foreach (var e in _activeEdges)
            {
                if (e?.Renderer == null) continue;

                // reset colour **only if** the edge is still empty
                if (!e.IsFilled)
                    e.Renderer.color = normalColor;
            }
            _activeEdges.Clear();
        }

        /* ------------ points (unchanged) ------------ */

        public void FlashPoint(int x, int y, GridBuilder builder)
        {
            ClearPoints();

            if (x < 0 || x >= builder.GridSize ||
                y < 0 || y >= builder.GridSize)
                return;

            var p = builder.Points[x, y];
            if (p?.Renderer == null) return;

            p.Renderer.color = validColor;
            _activePoints.Add(p.Renderer);
        }

        public void ClearPoints()
        {
            foreach (var r in _activePoints)
                if (r != null) r.color = normalColor;
            _activePoints.Clear();
        }
    }
}
