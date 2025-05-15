using System.Threading.Tasks;

namespace Tile.Core.IGridSizeService.Interface
{
    public interface IGridSizeService
    {
        Task Inject();                               
        void   SetGridSize(int pointsX, int pointsY);
        (int pointsX, int pointsY) GetGridSize();
    }
}