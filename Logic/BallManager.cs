using System.Numerics;
using Data;


namespace Logic
{
    public class BallManager : IBallManager
    {
        private readonly List<IBall> _balls = new();
        private readonly Random _random = new();
        private readonly SemaphoreSlim _ballsSemaphore = new(1, 1);
        private readonly double _tableWidth;
        private readonly double _tableHeight;
        private readonly double _ballRadius;
        private static int _nextBallId;

        public BallManager(double tableWidth, double tableHeight, double ballRadius)
        {
            _tableWidth = tableWidth;
            _tableHeight = tableHeight;
            _ballRadius = ballRadius;
        }

        public async Task<IBall> CreateBall()
        {
            double initialRadius = _ballRadius * (0.8 + _random.NextDouble() * 0.4);
            double positionX = initialRadius + _random.NextDouble() * (_tableWidth - 2 * initialRadius);
            double positionY = initialRadius + _random.NextDouble() * (_tableHeight - 2 * initialRadius);

            const double velocityConst = 100.0;
            
            float velocityX = (float)(_random.NextDouble() * 2 * velocityConst - velocityConst);
            float velocityY = (float)(_random.NextDouble() * 2 * velocityConst - velocityConst);

            var colorBytes = new byte[3];
            _random.NextBytes(colorBytes);
            string randomColor = $"#{BitConverter.ToString(colorBytes).Replace("-", "")}";
            
            IBall ball = new Ball
            {
                Id = Interlocked.Increment(ref _nextBallId),
                PositionX = positionX,
                PositionY = positionY,
                BallRadius = initialRadius,
                Velocity = new Vector2(velocityX, velocityY),
                Color = randomColor,
            };

            await _ballsSemaphore.WaitAsync();
            try
            {
                _balls.Add(ball);
            }
            finally
            {
                _ballsSemaphore.Release();
            }
            return ball;
        }

        public async Task<bool> RemoveBall(int id)
        {
            await _ballsSemaphore.WaitAsync();
            try
            {
                var ballToRemove = _balls.Find(ball => ball.Id == id);
                if (ballToRemove != null)
                {
                    _balls.Remove(ballToRemove);
                    return true;
                }
                return false;
            }
            finally
            {
                _ballsSemaphore.Release();
            }
        }

        public async Task UpdateBalls(double deltaTime)
        {
            await _ballsSemaphore.WaitAsync();
            try
            {
                if (!_balls.Any()) return;
                
                List<IBall> currentBalls = new List<IBall>(_balls);
                foreach (var ball in currentBalls)
                {
                    ball.Move(deltaTime);
                }

                await HandleBallCollisions(currentBalls);

                foreach (var ball in currentBalls)
                {
                    double currentBallRadius = ball.BallRadius;

                    if (ball.PositionX < currentBallRadius)
                    {
                        ball.PositionX = currentBallRadius;
                        ball.Velocity = ball.Velocity with { X = -ball.Velocity.X };
                    }
                    else if (ball.PositionX > _tableWidth - currentBallRadius)
                    {
                        ball.PositionX = _tableWidth - currentBallRadius;
                        ball.Velocity = ball.Velocity with { X = -ball.Velocity.X };
                    }

                    if (ball.PositionY < currentBallRadius)
                    {
                        ball.PositionY = currentBallRadius;
                        ball.Velocity = ball.Velocity with { Y = -ball.Velocity.Y };
                    }
                    else if (ball.PositionY > _tableHeight - currentBallRadius)
                    {
                        ball.PositionY = _tableHeight - currentBallRadius;
                        ball.Velocity = ball.Velocity with { Y = -ball.Velocity.Y };
                    }
                }
            }
            finally
            {
                _ballsSemaphore.Release();
            }
        }

        private readonly struct CollisionCalculationResult
        {
            public readonly IBall Ball1;
            public readonly IBall Ball2;
            public readonly Vector2 NewVelocity1;
            public readonly Vector2 NewVelocity2;

            public CollisionCalculationResult(IBall b1, IBall b2, Vector2 v1, Vector2 v2)
            {
                Ball1 = b1;
                Ball2 = b2;
                NewVelocity1 = v1;
                NewVelocity2 = v2;
            }
        }

        // Kopia stanu piłki, przydatna przy kolizjach
        private readonly struct BallSnapshot
        {
            public readonly Vector2 Position;
            public readonly Vector2 Velocity;
            public readonly float Mass;
            public readonly IBall OriginalBallRef;

            public BallSnapshot(IBall ball)
            {
                Position = new Vector2((float)ball.PositionX, (float)ball.PositionY);
                Velocity = ball.Velocity;
                Mass = (float)ball.Mass;
                OriginalBallRef = ball;
            }
        }

        private async Task HandleBallCollisions(List<IBall> ballsToProcess)
        {
            var collisionTasks = new List<Task<CollisionCalculationResult?>>();
            var processedPairs = new HashSet<Tuple<int, int>>();

            for (int i = 0; i < ballsToProcess.Count; i++)
            {
                for (int j = i + 1; j < ballsToProcess.Count; j++)
                {
                    IBall ball1 = ballsToProcess[i];
                    IBall ball2 = ballsToProcess[j];

                    Vector2 pos1 = new Vector2((float)ball1.PositionX, (float)ball1.PositionY);
                    Vector2 pos2 = new Vector2((float)ball2.PositionX, (float)ball2.PositionY);
                    
                    float distanceSquared = (pos2 - pos1).LengthSquared();
                    float totalRadius = (float)(ball1.BallRadius + ball2.BallRadius);

                    if (distanceSquared <= totalRadius * totalRadius)
                    {
                        if (Vector2.Dot(ball1.Velocity - ball2.Velocity, pos1 - pos2) < 0) 
                        {
                            var pairKey = ball1.Id < ball2.Id
                                ? Tuple.Create(ball1.Id, ball2.Id)
                                : Tuple.Create(ball2.Id, ball1.Id);
                            if (processedPairs.Add(pairKey))
                            {
                                var ball1Snapshot = new BallSnapshot(ball1);
                                var ball2Snapshot = new BallSnapshot(ball2);
                                collisionTasks.Add(
                                    Task.Run(() => CalculateNewVelocities(ball1Snapshot, ball2Snapshot)));
                            }
                        }
                    }
                }
            }

            var collisionResults = collisionTasks.Any() 
                ? await Task.WhenAll(collisionTasks)
                : Array.Empty<CollisionCalculationResult?>();
                
            var newVelocitiesMap = new Dictionary<int, Vector2>();
            foreach (var result in collisionResults)
            {
                if (result.HasValue)
                {
                    var res = result.Value;
                    newVelocitiesMap[res.Ball1.Id] = res.NewVelocity1;
                    newVelocitiesMap[res.Ball2.Id] = res.NewVelocity2;
                }
            }
            
            foreach (var ball in ballsToProcess)
            {
                if (newVelocitiesMap.TryGetValue(ball.Id, out var newVel))
                {
                    ball.Velocity = newVel;
                }
            }
        }

        private CollisionCalculationResult? CalculateNewVelocities(BallSnapshot b1, BallSnapshot b2)
        {
            Vector2 pos1 = b1.Position;
            Vector2 pos2 = b2.Position;
            Vector2 vel1 = b1.Velocity;
            Vector2 vel2 = b2.Velocity;
            float m1 = b1.Mass;
            float m2 = b2.Mass;

            Vector2 posDiff = pos1 - pos2;
            float distSq = posDiff.LengthSquared();
            if (distSq == 0) return null;

            Vector2 velDiff = vel1 - vel2;
            float dotProduct = Vector2.Dot(velDiff, posDiff);
    
            float scalar1 = 2 * m2 / (m1 + m2) * (dotProduct / distSq);
            float scalar2 = 2 * m1 / (m1 + m2) * (dotProduct / distSq);
    
            Vector2 newVel1 = vel1 - scalar1 * posDiff;
            Vector2 newVel2 = vel2 + scalar2 * posDiff;

            return new CollisionCalculationResult(b1.OriginalBallRef, b2.OriginalBallRef, newVel1, newVel2);
        }

        public async Task<List<IBall>> GetBalls()
        {
            await _ballsSemaphore.WaitAsync();
            try
            {
                return new List<IBall>(_balls);
            }
            finally
            {
                _ballsSemaphore.Release();
            }
        }
        
        public async Task RemoveAllBalls()
        {
            await _ballsSemaphore.WaitAsync();
            try
            {
                _balls.Clear();
            }
            finally
            {
                _ballsSemaphore.Release();
            }
        }
    }
}