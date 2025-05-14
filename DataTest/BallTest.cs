using System.Numerics;
using Data;

namespace DataTest;

[TestClass]
public class BallTest
{
    [TestMethod]
    public void Move_UpdatesPosition_AccordingToVelocityAndDeltaTime()
    {
        var ball = new Ball
        {
            PositionX = 0,
            PositionY = 0,
            Velocity = new Vector2(1, 2),
            Color = "Red"
        };
        
        double deltaTime = 1.5;
        ball.Move(deltaTime);
        
        Assert.AreEqual(0 + 1 * deltaTime, ball.PositionX, 0.0001);
        Assert.AreEqual(0 + 2 * deltaTime, ball.PositionY, 0.0001);
    }
    
    [TestMethod]
    public void Move_NoDeltaTime_PositionRemainsTheSame()
    {
        var ball = new Ball
        {
            PositionX = 4,
            PositionY = 20,
            Velocity = new Vector2(1, 1),
            Color = "Red"
        };
        
        double deltaTime = 0;
        ball.Move(deltaTime);
        
        Assert.AreEqual(4, ball.PositionX, 0.0001);
        Assert.AreEqual(20, ball.PositionY, 0.0001);
    }

    [TestMethod]
    public void Ball_GettersReturnsCorrectValues()
    {
        var ball = new Ball
        {
            Id = 1,
            PositionX = 123,
            PositionY = 321,
            Velocity = new Vector2(1.5f, 2.1f),
            BallRadius = 2.0,
            Color = "Red"
        };
        ball.UpdateMass();
        double expectedMass = Math.Pow(2.0, 3);
        
        Assert.AreEqual(1, ball.Id);
        Assert.AreEqual(123, ball.PositionX);
        Assert.AreEqual(321, ball.PositionY);
        Assert.AreEqual(1.5f, ball.Velocity.X, 0.0001);
        Assert.AreEqual(2.1f, ball.Velocity.Y, 0.0001);
        Assert.AreEqual(2.0, ball.BallRadius, 0.0001);
        Assert.AreEqual(expectedMass, ball.Mass, 0.0001);
    }
}