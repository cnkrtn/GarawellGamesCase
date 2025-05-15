using System.Threading.Tasks;

namespace Tile.Core.IGridSizeService.Service
{
    public class GridSizeService : Interface.IGridSizeService
    {
        private int _pointsX = 5;
        private int _pointsY = 5;

       
        public Task Inject()
        {
           
            return Task.CompletedTask;
        }

        public void SetGridSize(int pointsX, int pointsY)
        {
            _pointsX = pointsX;
            _pointsY = pointsY;
        }

        public (int pointsX, int pointsY) GetGridSize()
        {
            return (_pointsX, _pointsY);
        }
    }
}