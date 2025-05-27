using System.Collections.Concurrent;
using System.Globalization;
using System.IO;
using System.Text;

namespace Data
{
    public class DiagnosticLogger : IDiagnosticLogger
    {
        private readonly string _logFilePath;
        private readonly BlockingCollection<string> _logQueue = new(new ConcurrentQueue<string>());
        private readonly Task _processingTask;
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private const int MaxRetries = 3;
        private const int DelayBetweenRetriesMs = 100;

        public DiagnosticLogger(string logDirectory, string fileNamePrefix = "log")
        {
            string timestamp = DateTime.Now.ToString("yyyyMMdd-HHmmss");
            _logFilePath = Path.Combine(logDirectory, $"{fileNamePrefix}-{timestamp}.log");
            
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }
            _processingTask = Task.Run(ProcessLogQueue, _cancellationTokenSource.Token);
        }

        private async Task ProcessLogQueue()
        {
            try
            {
                foreach (var logEntry in _logQueue.GetConsumingEnumerable(_cancellationTokenSource.Token))
                {
                    await WriteLogEntryToFileAsync(logEntry);
                }
            }
            catch (OperationCanceledException)
            {
                
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Critical error in DiagnosticLogger processing task: {ex}");
            }
        }

        private async Task WriteLogEntryToFileAsync(string logEntry)
        {
            for (int i = 0; i < MaxRetries; i++)
            {
                try
                {
                    using (var stream = new FileStream(_logFilePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite, bufferSize: 4096, useAsync: true))
                    using (var writer = new StreamWriter(stream, Encoding.ASCII))
                    {
                        await writer.WriteLineAsync(logEntry).ConfigureAwait(false);
                    }
                    return;
                }
                catch (IOException) when (i < MaxRetries - 1)
                {
                    await Task.Delay(DelayBetweenRetriesMs).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Failed to write log entry after multiple retries: {ex.Message}. Entry: {logEntry}");
                    return;
                }
            }
             Console.Error.WriteLine($"Failed to write log entry due to IOException after {MaxRetries} retries. Entry: {logEntry}");
        }

        private void QueueLogEntry(string eventType, string details)
        {
            if (_cancellationTokenSource.IsCancellationRequested) return;
            
            string timestamp = DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture); // ISO 8601 format
            string logEntry = $"{timestamp} [{eventType}] {details}";
            
            if (!_logQueue.TryAdd(logEntry))
            {
                Console.Error.WriteLine($"Failed to add log entry to queue: {logEntry}");
            }
        }

        public Task LogBallStateAsync(IBall ball, string eventType)
        {
            string details = $"BallID: {ball.Id}, PosX: {ball.PositionX:F2}, PosY: {ball.PositionY:F2}, VelX: {ball.Velocity.X:F2}, VelY: {ball.Velocity.Y:F2}, Radius: {ball.BallRadius:F2}, Color: {ball.Color}";
            QueueLogEntry(eventType, details);
            return Task.CompletedTask;
        }

        public Task LogCollisionAsync(IBall ball1, IBall ball2)
        {
            string details = $"Collision between BallID: {ball1.Id} (PosX: {ball1.PositionX:F2}, PosY: {ball1.PositionY:F2}) and BallID: {ball2.Id} (PosX: {ball2.PositionX:F2}, PosY: {ball2.PositionY:F2})";
            QueueLogEntry("BallCollision", details);
            return Task.CompletedTask;
        }

        public Task LogWallCollisionAsync(IBall ball, string wallSide)
        {
            string details = $"BallID: {ball.Id} hit {wallSide} wall. New PosX: {ball.PositionX:F2}, PosY: {ball.PositionY:F2}, New VelX: {ball.Velocity.X:F2}, VelY: {ball.Velocity.Y:F2}";
            QueueLogEntry("WallCollision", details);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _logQueue.CompleteAdding();
            try
            {
                _processingTask.Wait(TimeSpan.FromSeconds(5));
            }
            catch (AggregateException ae)
            {
                ae.Handle(ex => ex is TaskCanceledException || ex is OperationCanceledException);
            }
            catch (TaskCanceledException)
            {
                
            }
            _cancellationTokenSource.Dispose();
            _logQueue.Dispose();
        }
    }
}

