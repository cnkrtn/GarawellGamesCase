// Core/HandService/Interface/IHandService.cs

using System.Collections.Generic;
using System.Threading.Tasks;


namespace Core.HandService.Interface
{
    public interface IHandService
    {
        /// <summary>Boot-time setup.  No parameters; pulls what it needs from ReferenceLocator.</summary>
        Task Inject(TileCatalog catalog);

        /// <summary>Return a playable, weighted hand of ShapeData.</summary>
        ShapeData[] DealHand(int handSize);

        public bool AnyCanPlace(IEnumerable<ShapeData> shapes);
    }
}