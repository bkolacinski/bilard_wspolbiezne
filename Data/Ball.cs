using System.Numerics;

namespace Data
{
    public class Ball
    {
        public int Id { get; set; }
        public double PositionX { get; set; }
        public double PositionY { get; set; }
        public Vector2 Velocity { get; set; }
        public double Radius { get; set; }
        
        public void Move(double deltaTime)
        {
            PositionX += Velocity.X * deltaTime;
            PositionY += Velocity.Y * deltaTime;
        }
    }
}
