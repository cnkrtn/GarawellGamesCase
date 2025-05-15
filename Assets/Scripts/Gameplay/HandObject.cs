using System;
using System.Linq;
using Core.GridHighlightService.Interface;
using Core.TileFactoryService.Interface;
using Core.HandService.Interface;
using Tile;
using UnityEngine;

public class HandObject : MonoBehaviour
{
    [SerializeField] private Transform[] slotAnchors;

    private IHandService _handService;
    private ITileFactoryService _tileFactoryService;
    private IGridHighlightService _gridHighlightService;
    private ShapeData[] _currentShapes;
    private int _tilesRemaining;

    void Awake()
    {
        _handService = ReferenceLocator.Instance.HandService;
        _tileFactoryService = ReferenceLocator.Instance.TileFactoryService;
        _gridHighlightService = ReferenceLocator.Instance.GridHighlightService;
    }

    void OnEnable() => EventService.TilePlaced += OnTilePlaced;
    void OnDisable() => EventService.TilePlaced -= OnTilePlaced;


    public void DealNewHand()
    {
        _tilesRemaining = slotAnchors.Length;
        _currentShapes = _handService.DealHand(_tilesRemaining);

        for (int i = 0; i < _currentShapes.Length; i++)
        {
            var go = _tileFactoryService.Spawn(_currentShapes[i].tileID, slotAnchors[i]);
            if (go.TryGetComponent<TileDrag>(out var td))
                td.Init(slotAnchors[i]);


            Color tileColor = _gridHighlightService.PlacedColor;


            foreach (var sr in go.GetComponentsInChildren<SpriteRenderer>())
            {
                sr.color = tileColor;
            }
        }
    }

    public void ClearHand()
    {
        foreach (var slot in slotAnchors)
        {
            if (slot.childCount > 2)
            {
                var go = slot.GetChild(2).gameObject;
                ReferenceLocator.Instance.TileFactoryService.Despawn(go);
            }
        }

        _tilesRemaining = 0;
        _currentShapes = new ShapeData[0];
    }

    private void OnTilePlaced(TileDrag placed)
    {
        if (!placed.TryGetComponent(out SOHolder so)) return;
        var playedShape = so.shape;

        _tileFactoryService.Despawn(placed.gameObject);


        var list = _currentShapes.ToList();
        if (!list.Remove(playedShape))
          //  Debug.LogWarning($"Tried to remove {playedShape.name} but it wasn't in the hand!");
        _currentShapes = list.ToArray();

        _tilesRemaining--;
        if (_tilesRemaining > 0)
        {
            bool canPlace = _handService.AnyCanPlace(_currentShapes);
           // Debug.Log($"[GameCheck] TilesRemaining={_tilesRemaining}, CanPlace={canPlace}");
            if (!canPlace)
            {
              //  Debug.Log("Game Over: no remaining hand-shapes fit on the board");
                EventService.GameOver?.Invoke();
            }
        }
        else
        {
            DealNewHand();
        }
    }
}