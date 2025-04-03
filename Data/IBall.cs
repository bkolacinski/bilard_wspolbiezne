using System.Numerics;

namespace Data
{
    /// <summary>
    /// Represents a ball entity in the Data layer.
    /// This is an abstract representation.
    /// </summary>
    public interface IBall
    {
        /// <summary>
        /// Gets or sets the identifier for the ball.
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Gets or sets the X coordinate of the ball's position.
        /// </summary>
        public double PositionX { get; set; }
        
        /// <summary>
        /// Gets or sets the Y coordinate of the ball's position.
        /// </summary>
        public double PositionY { get; set; }
        
        /// <summary>
        /// Gets or sets the velocity of the ball.
        /// </summary>
        public Vector2 Velocity { get; set; }
        
        /// <summary>
        /// Gets or sets the radius of the ball.
        /// </summary>
        public double BallRadius { get; set; }
        
        /// <summary>
        /// Moves the ball based on the elapsed deltaTime.
        /// </summary>
        /// <param name="deltaTime">The time interval used for moving the ball.</param>
        public void Move(double deltaTime);
    }
}

