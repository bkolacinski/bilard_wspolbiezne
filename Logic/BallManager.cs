using System.Diagnostics;
using System.Numerics;
using Data;

namespace Logic
{
    public class BallManager : IBallManager
    {
        private readonly List<IBall> _balls = new List<IBall>();
        private readonly Random _random = new Random();
        private readonly double _tableWidth;
        private readonly double _tableHeight;
        private readonly double _ballRadius;

        public BallManager(double tableWidth, double tableHeight, double ballRadius)
        {
            _tableWidth = tableWidth;
            _tableHeight = tableHeight;
            _ballRadius = ballRadius;
        }

        public IBall CreateBall()
        {
            double positionX = _ballRadius + _random.NextDouble() * (_tableWidth - 2 * _ballRadius);
            double positionY = _ballRadius + _random.NextDouble() * (_tableHeight - 2 * _ballRadius);
            
            float velocityX = (float) ((_random.NextDouble() * 100) - 50);
            float velocityY = (float) ((_random.NextDouble() * 100) - 50);

            IBall ball = new Ball
            {
                Id = _balls.Count + 1,
                PositionX = positionX,
                PositionY = positionY,
                BallRadius = _ballRadius,
                Velocity = new Vector2(velocityX, velocityY)
            };
            
            _balls.Add(ball);
            return ball;
        }

        public bool RemoveBall(int id)
        {
            var ballToRemove = _balls.Find(ball => ball.Id == id);
            if (ballToRemove != null)
            {
                _balls.Remove(ballToRemove);
                return true;
            }
            return false;
        }

        public void UpdateBalls(double deltaTime)
        {
            foreach (var ball in _balls)
            {
                ball.Move(deltaTime);
                Debug.WriteLine($"BallManager.UpdateBalls: Ball {ball.Id} moved to ({ball.PositionX:F2},{ball.PositionY:F2})");

                // Left collision
                if (ball.PositionX < _ballRadius)
                {
                    ball.PositionX = _ballRadius;
                    ball.Velocity = ball.Velocity with { X = -ball.Velocity.X };
                }
                // Right collision
                else if (ball.PositionX > _tableWidth - _ballRadius)
                {
                    ball.PositionX = _tableWidth - _ballRadius;
                    ball.Velocity = ball.Velocity with { X = -ball.Velocity.X };
                }

                // Top collision
                if (ball.PositionY < _ballRadius)
                {
                    ball.PositionY = _ballRadius;
                    ball.Velocity = ball.Velocity with { Y = -ball.Velocity.Y };
                }
                // Bottom collision
                else if (ball.PositionY > _tableHeight - _ballRadius)
                {
                    ball.PositionY = _tableHeight - _ballRadius;
                    ball.Velocity = ball.Velocity with { Y = -ball.Velocity.Y };
                }
            }
        }

        public List<IBall> GetBalls()
        {
            return _balls;
        }
    }
}

