using System;
using System.Linq;
using Core.TileFactoryService.Interface;
using Core.HandService.Interface;
using Tile;
using UnityEngine;

public class HandObject : MonoBehaviour
{
    [SerializeField] private Transform[] slotAnchors;

    private IHandService        _handService;
    private ITileFactoryService _tileFactoryService;
    private int _tilesRemaining;

    void Awake()
    {
        var loc = ReferenceLocator.Instance;
        _handService        = loc.HandService;
        _tileFactoryService = loc.TileFactoryService;
    }

    void OnEnable()  => EventService.TilePlaced += OnTilePlaced;
    void OnDisable() => EventService.TilePlaced -= OnTilePlaced;

    void Start() => DealNewHand();

    public void DealNewHand()
    {
        _tilesRemaining = slotAnchors.Length;
        var shapes = _handService.DealHand(_tilesRemaining);

        for (int i = 0; i < shapes.Length; i++)
        {
            var go = _tileFactoryService.Spawn(shapes[i].tileID, slotAnchors[i]);
            if (go.TryGetComponent<TileDrag>(out var tileDrag))
            {
                tileDrag.Init(slotAnchors[i]);
            }
            else
            {
                Debug.LogError($"[HandObject] Spawned tile is missing TileDrag component on {go.name}");
            }

        }
    }

    private void OnTilePlaced(TileDrag placed)
    {
        _tileFactoryService.Despawn(placed.gameObject);

        if (--_tilesRemaining <= 0)
            DealNewHand();
    }

 
}