using System.Numerics;

namespace Data
{
    public class Ball : IBall
    {
        public int Id { get; set; }

        public double PositionX { get; set; }

        public double PositionY { get; set; }

        public Vector2 Velocity { get; set; }

        private double _ballRadius;

        public double BallRadius
        {
            get => _ballRadius;
            set
            {
                _ballRadius = value;
                UpdateMass();
            }
        }

        public double Mass { get; private set; }

        public required string Color { get; set; }
        
        public void UpdateMass()
        {
            Mass = Math.Pow(BallRadius, 3);
        }

        public void Move(double deltaTime)
        {
            PositionX += Velocity.X * deltaTime;
            PositionY += Velocity.Y * deltaTime;
        }
    }
}