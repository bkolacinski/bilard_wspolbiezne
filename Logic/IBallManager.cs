using Data;

namespace Logic
{
    public interface IBallManager
    {
        IBall CreateBall();
        
        bool RemoveBall(int id);
        
        void UpdateBalls(double deltaTime);
        
        List<IBall> GetBalls();
    }
}
