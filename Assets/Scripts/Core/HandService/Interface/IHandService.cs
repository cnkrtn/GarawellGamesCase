using System.Collections.Generic;
using System.Threading.Tasks;


namespace Core.HandService.Interface
{
    public interface IHandService
    {
        Task Inject(TileCatalog catalog);


        ShapeData[] DealHand(int handSize);

        public bool AnyCanPlace(IEnumerable<ShapeData> shapes);
    }
}