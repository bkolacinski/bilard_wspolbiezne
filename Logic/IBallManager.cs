using Data;

namespace Logic
{
    /// <summary>
    /// Defines an interface to be used to manage ball entities.
    /// </summary>
    public interface IBallManager
    {
        /// <summary>
        /// Creates a new ball.
        /// </summary>
        /// <returns>Abstract IBall.</returns>
        IBall CreateBall();
        
        /// <summary>
        /// Removes ball with given id.
        /// </summary>
        /// <param name="id">The id of ball to remove.</param>
        /// <returns>true when ball was successfully deleted, false otherwise.</returns>
        bool RemoveBall(int id);
        
        /// <summary>
        /// Updates all balls based on provided elapsed time.
        /// </summary>
        /// <param name="deltaTime">Elapsed time interval.</param>
        void UpdateBalls(double deltaTime);
        
        /// <summary>
        /// Returns all managed balls.
        /// </summary>
        /// <returns>List of all IBall instances.</returns>
        List<IBall> GetBalls();
    }
}
