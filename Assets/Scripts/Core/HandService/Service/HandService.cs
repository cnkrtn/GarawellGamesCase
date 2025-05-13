using System;
using Core.GridService.Interface;
using Core.HandService.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.TileFactoryService.Interface;
using UnityEngine;
using Random = UnityEngine.Random;


namespace Core.HandService.Service
{
    public class HandService : IHandService
    {
        private ShapeData[] _allShapes;
        private IGridService _grid;

        private ITileFactoryService _tileFactoryService;

        /* --------------------------------------------------------- */
        /*  Inject: load catalog & cache grid                        */
        /* --------------------------------------------------------- */
        public Task Inject(TileCatalog catalog)
        {
            _allShapes = catalog.entries
                .Select(e => e.shape)
                .ToArray();

            _grid = ReferenceLocator.Instance.GridService;

            return Task.CompletedTask;
        }

        /* --------------------------------------------------------- */
        /*  DealHand                                                 */
        /* --------------------------------------------------------- */
        public ShapeData[] DealHand(int handSize)
        {
            if (_allShapes == null || _allShapes.Length == 0)
                return Array.Empty<ShapeData>();

            // 1) compute the total weight of *all* shapes
            int totalWeight = _allShapes.Sum(s => Mathf.Max(1, s.weight));

            var hand = new ShapeData[handSize];

            // 2) for each slot, draw until the picked shape fits
            for (int i = 0; i < handSize; i++)
            {
                ShapeData pick = null;
                // keep sampling until CanPlaceShape succeeds
                do
                {
                    int r = Random.Range(0, totalWeight);
                    int cum = 0;
                    foreach (var s in _allShapes)
                    {
                        cum += Mathf.Max(1, s.weight);
                        if (r < cum)
                        {
                            pick = s;
                            break;
                        }
                    }
                    // if pick is null (shouldn’t happen) or it doesn’t fit, loop again
                } while (pick == null || !_grid.CanPlaceShape(pick));

                hand[i] = pick;
            }

            return hand;
        }


        public bool AnyCanPlace(IEnumerable<ShapeData> shapes)
        {
            foreach (var s in shapes)
                if (s != null && _grid.CanPlaceShape(s))
                    return true;
            return false;
        }
    }
}