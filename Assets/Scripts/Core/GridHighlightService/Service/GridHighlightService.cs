

using System.Collections.Generic;
using System.Threading.Tasks;
using Core.GridService.Data;
using Core.GridHighlightService.Interface;
using Grid;
using UnityEngine;

namespace Core.GridHighlightService.Service
{
    public class GridHighlightService : IGridHighlightService
    {
        private GridHighlighter _gridHighlighter;

        public Task Inject(GridHighlighter highlighter)
        {
            _gridHighlighter = highlighter;
            return Task.CompletedTask;
        }

        public void FlashEdges(IEnumerable<Edge> edges)    => _gridHighlighter.FlashEdges(edges);
        public void ClearEdges()                          => _gridHighlighter.ClearEdges();
        public void FlashPoints(IEnumerable<Point> pts)   => _gridHighlighter.FlashPoints(pts);
        public void ClearPoints()                         => _gridHighlighter.ClearPoints();
        
        public void ShowSquareVisual(Point o)              => _gridHighlighter.ShowSquareVisual(o);
        public void HideSquareVisual(Point o)              => _gridHighlighter.HideSquareVisual(o);

        public void ClearAllSquares() => _gridHighlighter.ClearAllSquares();

        public void BurstSquaresSequential(IEnumerable<Point> origins, float interval = 0.1f)
            => _gridHighlighter.BurstSquaresSequential(origins, interval);

        public Color HighlightColor => _gridHighlighter.ValidColor;
        public Color PlacedColor   => _gridHighlighter.PlacedColor;
        public Color NormalColor   => _gridHighlighter.NormalColor;
        
        
    }
}