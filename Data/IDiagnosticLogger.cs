namespace Data
{
    public interface IDiagnosticLogger : IDisposable
    {
        Task LogBallStateAsync(IBall ball, string eventType);
        Task LogCollisionAsync(IBall ball1, IBall ball2);
        Task LogWallCollisionAsync(IBall ball, string wallSide);
    }
}

