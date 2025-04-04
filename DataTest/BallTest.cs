﻿using System.Numerics;
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
            Velocity = new Vector2(1, 2)
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
            Velocity = new Vector2(1, 1)
        };
        
        double deltaTime = 0;
        ball.Move(deltaTime);
        
        Assert.AreEqual(4, ball.PositionX, 0.0001);
        Assert.AreEqual(20, ball.PositionY, 0.0001);
    }

    [TestMethod]
    public void Ball_GettersReturnsCorrectValues()
    {
        var Ball = new Ball
        {
            Id = 1,
            PositionX = 123,
            PositionY = 321,
            Velocity = new Vector2(1.5f, 2.1f),
            BallRadius = 2.0
        };
        
        int BallId = Ball.Id;
        double BallRadius = Ball.BallRadius;
        
        Assert.AreEqual(1, BallId, 0.0001);
        Assert.AreEqual(2.0, BallRadius, 0.0001);
    }
}