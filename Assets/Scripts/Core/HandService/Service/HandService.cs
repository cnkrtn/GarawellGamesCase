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

            // build weighted pool from *all* shapes (no playable filter here)
            var pool = new List<ShapeData>();
            foreach (var s in _allShapes)
            {
                int w = Mathf.Max(1, s.weight);
                for (int i = 0; i < w; i++) pool.Add(s);
            }

            var hand = new ShapeData[handSize];
            for (int i = 0; i < handSize; i++)
            {
                ShapeData pick;
                int attempts = 0;
                do
                {
                    pick = pool[Random.Range(0, pool.Count)];
                    attempts++;
                    if (attempts > pool.Count) break;  // fallback
                }
                while (!_grid.CanPlaceShape(pick));

                hand[i] = pick;
            }

            return hand;
        }

    }
}