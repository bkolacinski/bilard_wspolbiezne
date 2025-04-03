using System.Numerics;
using Data;

namespace Logic
{
    /// <summary>
    /// Implementation of IBallManager used in Logic layer.
    /// </summary>
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

                if (ball.PositionX < _ballRadius)
                {
                    ball.PositionX = _ballRadius;
                    ball.Velocity = new Vector2(-ball.Velocity.X, ball.Velocity.Y);
                }
                else if (ball.PositionX > _ballRadius + _tableWidth)
                {
                    ball.PositionX = _tableWidth - _ballRadius;
                    ball.Velocity = new Vector2(-ball.Velocity.X, ball.Velocity.Y);
                }

                if (ball.PositionY < _ballRadius)
                {
                    ball.PositionY = _ballRadius;
                    ball.Velocity = new Vector2(ball.Velocity.X, -ball.Velocity.Y);
                }
                else if (ball.PositionY > _ballRadius + _tableHeight)
                {
                    ball.PositionY = _tableHeight - _ballRadius;
                    ball.Velocity = new Vector2(ball.Velocity.X, -ball.Velocity.Y);
                }
            }
        }

        public List<IBall> GetBalls()
        {
            return _balls;
        }
    }
}

