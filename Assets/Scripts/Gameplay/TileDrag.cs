using System;
using System.Linq;
using UnityEngine;
using Grid;
using Core.TileFactoryService.Interface;
using Core.GridService.Interface;
using Core.GridHighlightService.Interface;

using Tile;

public class TileDrag : MonoBehaviour
{
    [Header("Hold Settings")]
    [SerializeField] float holdScaleMultiplier = 1.4f;

    // The grid cell where this tile's origin (bottom-left) lands
    public Vector2Int OriginCell { get; private set; }

    /* ── scene slot set by HandDealer ───────────────────────────── */
    private Transform _homeSlot;

    /* ── services ───────────────────────────────────────────────── */
    private ITileFactoryService   _tileFactoryService;
    private IGridService          _gridService;
    private IGridHighlightService _gridHighlightService;

    /* ── camera & shape data ────────────────────────────────────── */
    private Camera    _cam;
    private ShapeData _shape;
    public ShapeData Shape => _shape;
    /* ── drag state ─────────────────────────────────────────────── */
    bool    _dragging;
    Vector3 _grabOffset;
    Vector3 _idleScale;
    Vector3 _holdScale;

    /* ------------------------------------------------------------ */
    /*  Init: called by HandDealer after Spawn                      */
    /* ------------------------------------------------------------ */
    public void Init(Transform homeSlot)
    {
        _homeSlot = homeSlot;
        _idleScale = transform.localScale;
        _holdScale = _idleScale * holdScaleMultiplier;
        _shape = GetComponentInChildren<SOHolder>().shape;

        var loc = ReferenceLocator.Instance;
        _tileFactoryService   = loc.TileFactoryService;
        _gridService          = loc.GridService;
        _gridHighlightService = loc.GridHighlightService;
        _cam = Camera.main;
    }

    /* ------------------------------------------------------------ */
    /*  Mouse events                                                */
    /* ------------------------------------------------------------ */
    void OnMouseDown()
    {
        _dragging = true;
        transform.localScale = _holdScale;
        transform.SetParent(null, true);

        transform.position = _homeSlot.GetChild(0).position;
        _grabOffset = transform.position - MouseWorld();

        HighlightAtPivot();
    }

    void OnMouseDrag()
    {
        if (!_dragging) return;
        transform.position = MouseWorld() + _grabOffset;
        HighlightAtPivot();
    }

    void OnMouseUp()
    {
        if (!_dragging) return;
        _dragging = false;

        Vector2Int pivotCell = WorldToCell(transform.position);
        Vector2Int originCell = pivotCell - _shape.anchorPoint;

        // record the final origin for scoring
        OriginCell = originCell;

        var edges = _gridService.GetEdges(_shape, originCell);
        if (edges != null && edges.All(e => !e.IsFilled))     // SUCCESS
        {
            foreach (var e in edges)
            {
                e.IsFilled        = true;
                e.A.IsFilledColor = true;
                e.B.IsFilledColor = true;
                if (e.Renderer)
                    e.Renderer.color = _gridHighlightService.PlacedColor;
            }

            _gridHighlightService.ClearEdges();
            _gridHighlightService.ClearPoints();
            Debug.Log($"[TileDrag] Raising TilePlaced for origin {OriginCell.x},{OriginCell.y}");
            EventService.TilePlaced?.Invoke(this);
        }
        else // FAIL
        {
            _gridHighlightService.ClearEdges();
            _gridHighlightService.ClearPoints();
            transform.SetParent(_homeSlot, false);
            transform.localPosition = Vector3.zero;
            transform.localScale = _idleScale;
        }
    }

    /* ------------------------------------------------------------ */
    /*  Helpers                                                     */
    /* ------------------------------------------------------------ */
    void HighlightAtPivot()
    {
        _gridHighlightService.ClearEdges();
        _gridHighlightService.ClearPoints();

        Vector2Int pivotCell = WorldToCell(transform.position);
        Vector2Int origin = pivotCell - _shape.anchorPoint;

        var edges = _gridService.GetEdges(_shape, origin);
        if (edges != null && edges.All(e => !e.IsFilled))
            _gridHighlightService.FlashEdges(edges);
    }

    Vector3 MouseWorld()
    {
        var m = Input.mousePosition;
        m.z = -_cam.transform.position.z;
        return _cam.ScreenToWorldPoint(m);
    }

    Vector2Int WorldToCell(Vector3 world)
    {
        Vector3 local = world - _gridService.Origin;
        float s = _gridService.Spacing;
        return new Vector2Int(
            Mathf.FloorToInt(local.x / s + 0.5f),
            Mathf.FloorToInt(local.y / s + 0.5f)
        );
    }
}
