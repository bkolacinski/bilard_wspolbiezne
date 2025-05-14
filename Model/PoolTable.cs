using Logic;
using Data;

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

        public async Task Update(double deltaTime)
        {
            await BallManager.UpdateBalls(deltaTime);
        }
        
        public async Task<List<BallModel>> GetBallModels()
        {
            var balls = await BallManager.GetBalls(); // Added await
            return balls
                .Select(ball => new BallModel(ball))
                .ToList();
        }

        public async Task<IBall> AddBall() => await BallManager.CreateBall();
    }
}