using System.Numerics;

namespace Data
{
    /// <summary>
    /// Implementation of IBall used in the Logic layer.
    /// </summary>
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
        }
    }
}
