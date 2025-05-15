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


        public Task Inject(TileCatalog catalog)
        {
            _allShapes = catalog.entries
                .Select(e => e.shape)
                .ToArray();

            _grid = ReferenceLocator.Instance.GridService;

            return Task.CompletedTask;
        }


        public ShapeData[] DealHand(int handSize)
        {
            if (_allShapes == null || _allShapes.Length == 0)
                return Array.Empty<ShapeData>();

            var hand = new ShapeData[handSize];
            bool largePicked = false;

            for (int i = 0; i < handSize; i++)
            {
                var candidates = largePicked
                    ? _allShapes.Where(s => !s.isLarge).ToArray()
                    : _allShapes;


                if (candidates.Length == 0)
                    break;


                int totalWeight = candidates.Sum(s => Mathf.Max(1, s.weight));

                ShapeData pick = null;

                do
                {
                    int r = Random.Range(0, totalWeight);
                    int cum = 0;
                    foreach (var s in candidates)
                    {
                        cum += Mathf.Max(1, s.weight);
                        if (r < cum)
                        {
                            pick = s;
                            break;
                        }
                    }
                } while (pick == null || !_grid.CanPlaceShape(pick));

                hand[i] = pick;


                if (pick.isLarge)
                    largePicked = true;
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