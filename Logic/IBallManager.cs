using Data;

namespace Logic
{
    public interface IBallManager
    {
        Task<IBall> CreateBall();
        
        Task<bool> RemoveBall(int id);
        
        Task UpdateBalls(double deltaTime);
        
        Task<List<IBall>> GetBalls();
        
        Task RemoveAllBalls();
    }
}
