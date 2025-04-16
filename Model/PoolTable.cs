using Logic;
using Data;
using System.Collections.Generic;
using System.Linq;

namespace Model
{
    public class PoolTable
    {
        public double Width { get; }
        public double Height { get; }
        public double BallRadius { get; }
        public IBallManager BallManager { get; }

        public PoolTable(double width, double height, double ballRadius, IBallManager ballManager)
        {
            Width = width;
            Height = height;
            BallRadius = ballRadius;
            BallManager = ballManager;
        }

        public void Update(double deltaTime)
        {
            BallManager.UpdateBalls(deltaTime);
        }
        
        public List<BallModel> GetBallModels()
        {
            return BallManager.GetBalls()
                .Select(ball => new BallModel(ball))
                .ToList();
        }

        public IBall AddBall() => BallManager.CreateBall();
    }
}