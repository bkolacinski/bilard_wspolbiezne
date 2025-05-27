using Logic;
using Model;
using Data;
using System.IO;

namespace ViewModel
{
    public static class AppHelper
    {
        public static PoolTableViewModel CreateMainViewModel()
        {
            double tableWidth = 800;
            double tableHeight = 400;
            double ballRadius = 10;
            
            string logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DiagnosticLogs");
            IDiagnosticLogger diagnosticLogger = new DiagnosticLogger(logDirectory, "BallActivity");

            IBallManager ballManager = new BallManager(tableWidth, tableHeight, ballRadius, diagnosticLogger);
            var poolTable = new PoolTable(tableWidth, tableHeight, ballRadius, ballManager);
            return new PoolTableViewModel(poolTable);
        }
    }
}
