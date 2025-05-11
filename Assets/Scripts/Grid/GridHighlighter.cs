using System.Collections.Generic;
using UnityEngine;
using Grid;   // Edge, Point

namespace Grid
{
    public class GridHighlighter : MonoBehaviour
    {
        [SerializeField] private Color validColor   = Color.green;  // highlight
        [SerializeField] private Color placedColor  = Color.blue;   // permanent
        [SerializeField] private Color normalColor  = Color.white;  // default

        // we keep the Edge so we know when it's filled, and the Point so we know its fill-state
        private readonly List<Edge>  _activeEdges   = new();
        private readonly List<Point> _activePoints  = new();

        public Color ValidColor  => validColor;
        public Color PlacedColor => placedColor;

        /// <summary>
        /// Tint this shape’s edges **and** their endpoints with the “valid” colour.
        /// </summary>
        public void FlashEdges(IEnumerable<Edge> edges)
        {
            ClearEdges();
            ClearPoints();

            foreach (var e in edges)
            {
                if (e?.Renderer == null) continue;

                // edge
                e.Renderer.color = validColor;
                _activeEdges.Add(e);

                // endpoint A
                if (e.A?.Renderer != null)
                {
                    e.A.Renderer.color = validColor;
                    _activePoints.Add(e.A);
                }

                // endpoint B
                if (e.B?.Renderer != null)
                {
                    e.B.Renderer.color = validColor;
                    _activePoints.Add(e.B);
                }
            }
        }

        /// <summary>
        /// Reset only those edges you flashed:
        /// – if it’s now “filled”, stamp it with placedColor;
        /// – otherwise revert to normalColor.
        /// </summary>
        public void ClearEdges()
        {
            foreach (var e in _activeEdges)
            {
                if (e?.Renderer == null) continue;
                e.Renderer.color = e.IsFilled 
                    ? placedColor 
                    : normalColor;
            }
            _activeEdges.Clear();
        }

        /// <summary>
        /// Reset only those points you flashed:
        /// – if it’s already been placed (IsFilledColor), revert to placedColor;
        /// – otherwise revert to normalColor.
        /// </summary>
        public void ClearPoints()
        {
            foreach (var p in _activePoints)
            {
                if (p?.Renderer == null) continue;
                p.Renderer.color = p.IsFilledColor
                    ? placedColor
                    : normalColor;
            }
            _activePoints.Clear();
        }
    }
}
