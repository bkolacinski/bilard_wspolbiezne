using Logic;
using Model;

namespace ViewModel
{
    public static class AppHelper
    {
        public static PoolTableViewModel CreateMainViewModel()
        {
            double tableWidth = 800;
            double tableHeight = 400;
            double ballRadius = 10;

            IBallManager ballManager = new BallManager(tableWidth, tableHeight, ballRadius);
            var poolTable = new PoolTable(tableWidth, tableHeight, ballRadius, ballManager);
            return new PoolTableViewModel(poolTable);
        }
    }
}