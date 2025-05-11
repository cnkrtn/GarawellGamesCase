using UnityEngine;
using Grid;
using Tile;

public class TileDrag : MonoBehaviour
{
    [Header("Hold Settings")]
    [SerializeField] private float   holdScaleMultiplier = 1.4f;
  

    /* ---- injected references ---- */
    private GridBuilder  _builder;
    private GridLogic    _grid;
    private GridHighlighter _hi;
    private Camera       _cam;
    private Transform    _homeSlot;
    private TileFactory  _factory;
    private TileId       _tileId;

    /* ---- shape data ---- */
    private ShapeData _shape;

    /* ---- drag state ---- */
    private bool       _dragging;
    private Vector3    _grabOffset;
    private Vector3    _idleScale;
    private Vector3    _holdScale;

    /* ------------------------------------------------------------ */
    /*  public initialiser                                           */
    /* ------------------------------------------------------------ */

    public void Init(GridBuilder   builder,
                     GridHighlighter hi,
                     Camera         cam,
                     Transform      homeSlot,
                     TileFactory    factory,
                     TileId         tileId)
    {
        _builder  = builder;
        _grid     = builder.Logic;
        _hi       = hi;
        _cam      = cam;
        _homeSlot = homeSlot;
        _factory  = factory;
        _tileId   = tileId;

        var holder = GetComponentInChildren<SOHolder>();
        if (holder == null) Debug.LogError("Missing SOHolder!", this);
        _shape = holder.shape;

        _idleScale = transform.localScale;
        _holdScale = _idleScale * holdScaleMultiplier;
    }

    /* ------------------------------------------------------------ */
    /*  mouse handlers                                               */
    /* ------------------------------------------------------------ */

    private void OnMouseDown()
    {
        _dragging           = true;
        transform.localScale = _holdScale;

        transform.SetParent(null, true);

        // snap to anchor-marker of the slot
        Vector3 anchor = _homeSlot.GetChild(0).position;
        transform.position = anchor;

        _grabOffset = transform.position - MouseWorld();

        HighlightAtPivot();
    }

    private void OnMouseDrag()
    {
        if (!_dragging) return;

        transform.position = MouseWorld() + _grabOffset;
        HighlightAtPivot();
    }

    private void OnMouseUp()
    {
        if (!_dragging) return;
        _dragging = false;

        Vector2Int pivotCell  = WorldToCell(transform.position);
        Vector2Int originCell = pivotCell - _shape.anchorPoint;

        var edges = GetEdges(originCell);

        /* ---------- successful drop ---------- */
        if (edges != null && CanPlace(edges))
        {
            foreach (var e in edges)
            {
                e.IsFilled = true;
                e.A.IsFilledColor = true;
                e.B.IsFilledColor = true;
                
                if (e.Renderer) e.Renderer.color = _hi.ValidColor;  // permanent tint
            }

            _hi.ClearEdges();  
            _hi.ClearPoints();;// clears list but keeps tinted filled edges
            _factory.Despawn(_tileId, gameObject);  // return to pool
            return;
        }

        /* ---------- invalid: reset ---------- */
        _hi.ClearEdges();
        _hi.ClearPoints();
        transform.SetParent(_homeSlot, false);
        transform.localPosition = Vector3.zero;
        transform.localScale    = _idleScale;
    }

    /* ------------------------------------------------------------ */
    /*  helpers                                                      */
    /* ------------------------------------------------------------ */

    private void HighlightAtPivot()
    {
        // 0) clear any previous highlight
        _hi.ClearEdges();
        _hi.ClearPoints();

        // 1) compute the candidate edges
        Vector2Int anchorCell = WorldToCell(transform.position);
        Vector2Int originCell = anchorCell - _shape.anchorPoint;
        var edges = GetEdges(originCell);

        // 2) only flash on valid
        if (edges != null && CanPlace(edges))
            _hi.FlashEdges(edges);
    }


    private Vector3 MouseWorld()
    {
        Vector3 m = Input.mousePosition;
        m.z = -_cam.transform.position.z;
        return _cam.ScreenToWorldPoint(m);
    }

    private Vector2Int WorldToCell(Vector3 world)
    {
        Vector3 local = world - _builder.transform.position;
        float   s     = _builder.Spacing;                    // spacing already = 0.8
        return new Vector2Int(
            Mathf.FloorToInt(local.x / s + 0.5f),
            Mathf.FloorToInt(local.y / s + 0.5f)
        );
    }

    private Vector3 CellToWorld(Vector2Int cell)
    {
        float s = _builder.Spacing;
        return _builder.transform.position + new Vector3(cell.x * s, cell.y * s, 0f);
    }

    private bool CanPlace(Edge[] edges)
    {
        foreach (var e in edges)
            if (e.IsFilled) return false;
        return true;
    }

    private Edge[] GetEdges(Vector2Int origin)
    {
        if (_shape?.edges == null) return null;

        var list = new Edge[_shape.edges.Count];
        for (int i = 0; i < list.Length; i++)
        {
            var ed = _shape.edges[i];
            var a  = origin + ed.pointA;
            var b  = origin + ed.pointB;

            if (a.x < 0 || a.x >= _builder.GridSize ||
                a.y < 0 || a.y >= _builder.GridSize ||
                b.x < 0 || b.x >= _builder.GridSize ||
                b.y < 0 || b.y >= _builder.GridSize)
                return null;

            var pA = _grid.GetPointAt(a.x, a.y);
            var pB = _grid.GetPointAt(b.x, b.y);
            var re = _grid.GetEdge(pA, pB);
            if (re == null) return null;

            list[i] = re;
        }
        return list;
    }
}
