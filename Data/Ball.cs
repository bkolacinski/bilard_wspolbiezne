using System.Numerics;

namespace Data
{
    public class Ball : IBall
    {
        public int Id { get; set; }
        
        public double PositionX { get; set; }
        
        public double PositionY { get; set; }
        
        public Vector2 Velocity { get; set; }
        
        public double BallRadius { get; set; }
        
        public void Move(double deltaTime)
        {
            PositionX += Velocity.X * deltaTime;
            PositionY += Velocity.Y * deltaTime;
            System.Diagnostics.Debug.WriteLine($"Ball {Id} moved to: {PositionX:F2}, {PositionY:F2}");
        }
    }
}
