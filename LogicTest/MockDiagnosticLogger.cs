using Data;
using System.Diagnostics;

namespace LogicTest;

public class MockDiagnosticLogger : IDiagnosticLogger
{
    private readonly object _logLock = new(); 
    public List<string> LoggedMessages { get; } = new();
    public int DisposeCallCount { get; private set; }

    public Task LogBallStateAsync(IBall ball, string eventType)
    {
        string message = $"BallState: ID={ball.Id}, Event={eventType}";
        lock (_logLock)
        {
            LoggedMessages.Add(message);
        }
        Debug.WriteLine($"MockLogger: Added: {{{message}}} (LoggedMessages Count: {LoggedMessages.Count})");
        return Task.CompletedTask;
    }

    public Task LogCollisionAsync(IBall ball1, IBall ball2)
    {
        string message = $"Collision: Ball1_ID={ball1.Id}, Ball2_ID={ball2.Id}";
        lock (_logLock)
        {
            LoggedMessages.Add(message);
        }
        Debug.WriteLine($"MockLogger: Added: {{{message}}} (LoggedMessages Count: {LoggedMessages.Count})");
        return Task.CompletedTask;
    }

    public Task LogWallCollisionAsync(IBall ball, string wallSide)
    {
        string message = $"WallCollision: ID={ball.Id}, Wall={wallSide}";
        lock (_logLock)
        {
            LoggedMessages.Add(message);
        }
        Debug.WriteLine($"MockLogger: Added: {{{message}}} (LoggedMessages Count: {LoggedMessages.Count})");
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        DisposeCallCount++;
        Debug.WriteLine($"MockLogger: Dispose called. Call count: {DisposeCallCount}");
    }
        
    public void ClearMessages()
    {
        lock (_logLock)
        {
            LoggedMessages.Clear();
        }
        Debug.WriteLine($"MockLogger: Messages cleared via ClearMessages().");
    }
}