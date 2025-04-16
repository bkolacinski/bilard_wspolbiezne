using Data;

namespace Model
{
    public class BallModel
    {
        public int Id { get; private set; }
        public double PositionX { get; private set; }
        public double PositionY { get; private set; }
        public double BallRadius { get; private set; }
        public double Diameter => BallRadius * 2;

        public BallModel(IBall ball)
        {
            Id = ball.Id;
            PositionX = ball.PositionX;
            PositionY = ball.PositionY;
            BallRadius = ball.BallRadius;
        }
        
        public void Update(IBall ball)
        {
            PositionX = ball.PositionX;
            PositionY = ball.PositionY;
        }
    }
}