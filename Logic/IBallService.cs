using Data;

namespace Logic
{
    public interface IBallService
    {
        Ball CreateBall();
        void UpdateBalls(double deltaTime);
        List<Ball> GetBalls();
    }
}
