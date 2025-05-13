using Core.GridService.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Grid;

namespace Core.GridHighlightService.Interface
{
    public interface IGridHighlightService
    {
        Task Inject(GridHighlighter highlighter);

        void FlashEdges(IEnumerable<Edge> edges);
        public void FlashPoints(IEnumerable<Point> points);
        void ClearEdges();
        void ClearPoints();
        void ClearAllSquares();
        void BurstSquaresSequential(IEnumerable<Point> origins, float interval = 0.1f);
        void ShowSquareVisual(Point origin);
        void HideSquareVisual(Point origin);
        Color HighlightColor { get; }
        Color PlacedColor { get; }
        Color NormalColor { get; }
        
        
    }
}