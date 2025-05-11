using UnityEngine;
using System.Collections.Generic;



public class GridBuilder : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject pointPrefab;
    [SerializeField] private GameObject edgePrefab;

    [Header("Settings")]
    [SerializeField] private float   spacing    = 1f;
    [Tooltip("World‐space origin of the grid (bottom‐left corner).")]
    [SerializeField] private Vector2 gridOrigin = Vector2.zero;

    [SerializeField] private float gridVisualSizeMultiplier;
    [Header("Hierarchy Parents (Optional)")]
    [SerializeField] private Transform pointsParent;
    [SerializeField] private Transform edgesParent;

    // backing fields
    private Point[,]     _points;
    private List<Edge>   _edges;

    // runtime‐exposed properties
    public Point[,] Points => _points;
    public GridLogic Logic    { get; private set; }
    public float     Spacing  => spacing;
    public int       GridSize => _points.GetLength(0);

    private void Awake()
    {
        // initialize storage before building
        _points = new Point[5, 5];
        _edges  = new List<Edge>();
    }

    private void Start()
    {
        // move the builder to the desired origin in world‐space
        transform.position = new Vector3(gridOrigin.x, gridOrigin.y, 0f);

        BuildGrid();
        Logic = new GridLogic(_points, _edges);
        pointsParent.localScale = Vector3.one * gridVisualSizeMultiplier;
        edgesParent .localScale = Vector3.one * gridVisualSizeMultiplier;
    }

    private void BuildGrid()
    {
        // create points
        for (int y = 0; y < 5; y++)
        {
            for (int x = 0; x < 5; x++)
            {
                Vector2 localPos = new Vector2(x * spacing, y * spacing);
                Vector3 worldPos = transform.TransformPoint(localPos);

                // 1) instantiate the visual
                var pointGO = Instantiate(pointPrefab, worldPos, Quaternion.identity, pointsParent ?? transform);
                pointGO.name = $"Point_{x}_{y}";

                // 2) grab its SpriteRenderer
                var sr = pointGO.GetComponentInChildren<SpriteRenderer>();
                if (sr == null)
                    Debug.LogError("Point prefab missing a SpriteRenderer!", pointGO);

                // 3) create the data‐Point *and* assign its Renderer
                var pt = new Point(x, y, worldPos);
                pt.Renderer = sr;

                // 4) store it
                _points[x, y] = pt;
            }
        }


        // create edges
        for (int y = 0; y < 5; y++)
        {
            for (int x = 0; x < 5; x++)
            {
                var current = _points[x, y];

                if (x < 4)
                    CreateEdge(current, _points[x + 1, y]);

                if (y < 4)
                    CreateEdge(current, _points[x, y + 1]);
            }
        }
    }

    private void CreateEdge(Point a, Point b)
    {
        var edge = new Edge(a, b);
        _edges.Add(edge);

        // mid‐point in world‐space
        Vector3 mid = (a.WorldPos + b.WorldPos) * 0.5f;
        var edgeGO = Instantiate(edgePrefab, mid, Quaternion.identity, edgesParent ?? transform);
        edgeGO.name = $"Edge_{a.X}_{a.Y}_to_{b.X}_{b.Y}";

        // rotate if vertical
        if (Mathf.Abs(a.X - b.X) < 0.1f)
            edgeGO.transform.rotation = Quaternion.Euler(0, 0, 90);

        // cache its SpriteRenderer for highlight tints
        edge.Renderer = edgeGO.GetComponentInChildren<SpriteRenderer>();  
    }
}


public class Point
{
    public int X, Y;
    public Vector2 WorldPos;
    public SpriteRenderer Renderer; 
    public Point(int x, int y, Vector2 worldPos)
    {
        X = x;
        Y = y;
        WorldPos = worldPos;
        
    }
}

public class Edge
{
    public Point A, B;
    public bool IsFilled;
    public SpriteRenderer Renderer; 
    public Edge(Point a, Point b)
    {
        A = a;
        B = b;
        IsFilled = false;
    }

    public Vector2 GetCenter() => (A.WorldPos + B.WorldPos) / 2f;
}
