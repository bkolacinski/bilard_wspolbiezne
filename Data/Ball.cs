using System.Numerics;

namespace Data
{
    public class Ball : IBall
    {
        private readonly object _lock = new();

        public int Id { get; set; }

        private double _positionX;
        public double PositionX
        {
            get { lock (_lock) { return _positionX; } }
            set { lock (_lock) { _positionX = value; } }
        }

        private double _positionY;
        public double PositionY
        {
            get { lock (_lock) { return _positionY; } }
            set { lock (_lock) { _positionY = value; } }
        }

        private Vector2 _velocity;
        public Vector2 Velocity
        {
            get { lock (_lock) { return _velocity; } }
            set { lock (_lock) { _velocity = value; } }
        }

        private double _ballRadius;
        public double BallRadius
        {
            get { lock (_lock) { return _ballRadius; } }
            set
            {
                lock (_lock)
                {
                    _ballRadius = value;
                    UpdateMass();
                }
            }
        }

        private double _mass;
        public double Mass
        {
            get { lock (_lock) { return _mass; } }
            private set { lock (_lock) { _mass = value; } }
        }

        private string _color;
        public required string Color 
        {
            get { return _color; }
            set { _color = value; }
        }
        
        private void UpdateMass()
        {
            _mass = Math.Pow(_ballRadius, 3);
        }

        public void Move(double deltaTime)
        {
            lock (_lock)
            {
                _positionX += _velocity.X * deltaTime;
                _positionY += _velocity.Y * deltaTime;
            }
        }
    }
}

