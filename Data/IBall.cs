using System.Numerics;

namespace Data
{
    public interface IBall
    {
        public int Id { get; set; }
        
        public double PositionX { get; set; }
        
        public double PositionY { get; set; }
        
        public Vector2 Velocity { get; set; }
        
        public double BallRadius { get; set; }
        
        public void Move(double deltaTime);
    }
}

