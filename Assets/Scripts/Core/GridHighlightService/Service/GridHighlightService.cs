// Core/GridHighlightService/Service/GridHighlightService.cs

using System.Collections.Generic;
using System.Threading.Tasks;
using Core.GridService.Data;
using Core.GridHighlightService.Interface;
using UnityEngine;
using Grid;

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

        public void FlashEdges(IEnumerable<Edge> edges) => _gridHighlighter.FlashEdges(edges);
        public void ClearEdges() => _gridHighlighter.ClearEdges();
        public void FlashPoints(IEnumerable<Point> points) => _gridHighlighter.FlashPoints(points);
        public void ClearPoints() => _gridHighlighter.ClearPoints();
        
        public void ShowSquareVisual(Point o) => _gridHighlighter.ShowSquareVisual(o);
        public void HideSquareVisual(Point o) => _gridHighlighter.HideSquareVisual(o);
        
        
        public Color HighlightColor => _gridHighlighter.ValidColor;
        public Color PlacedColor => _gridHighlighter.PlacedColor;
        public Color NormalColor => _gridHighlighter.NormalColor;
    }
}