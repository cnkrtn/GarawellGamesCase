

using System.Collections.Generic;
using System.Threading.Tasks;
using Core.GridService.Data;
using UnityEngine;


namespace Core.GridService.Interface
{
    public interface IGridService
    {
        Task Inject(GridPrefabs prefabs, GridSettings settings);

        int GridWidth  { get; }
        int GridHeight { get; }
        float   Spacing   { get; }
        Vector3 Origin    { get; }
        IEnumerable<Edge> AllEdges { get; }
        Edge    GetEdge(Point a, Point b);
        Point   GetPoint(int x, int y);
        Edge[]  GetEdges(ShapeData shape, Vector2Int origin);
        bool    CanPlaceShape(ShapeData shape);
    }
}