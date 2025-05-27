using System.Numerics;
using Logic;
using Data;

namespace LogicTest;

[TestClass]
public sealed class BallManagerTest
{
    private double _tableWidth;
    private double _tableHeight;
    private double _ballRadius;
    private IBallManager _ballManager = null!;
    private MockDiagnosticLogger _mockLogger = null!;

    [TestInitialize]
    public void Initialize()
    {
        _tableWidth = 300;
        _tableHeight = 200;
        _ballRadius = 10;
        _mockLogger = new MockDiagnosticLogger();
        _ballManager = new BallManager(_tableWidth, _tableHeight, _ballRadius, _mockLogger);
    }

    [TestMethod]
    public async Task RemoveBall_BallIsRemoved_AndLogged()
    {
        IBall ball = await _ballManager.CreateBall();
        Assert.AreEqual(1, (await _ballManager.GetBalls()).Count);
        int ballId = ball.Id;
        _mockLogger.LoggedMessages.Clear();

        Assert.IsTrue(await _ballManager.RemoveBall(ballId));
        Assert.AreEqual(0, (await _ballManager.GetBalls()).Count);
        Assert.IsFalse(await _ballManager.RemoveBall(ballId)); 
        
        Assert.IsTrue(_mockLogger.LoggedMessages.Any(m => m.Contains($"BallState: ID={ballId}, Event=Removed")));
    }

    [TestMethod]
    public async Task CreateBall_BallWithinBounds_AndLogged()
    {
        _mockLogger.LoggedMessages.Clear();
        IBall ball = await _ballManager.CreateBall();

        Assert.IsNotNull(ball);
        Assert.IsTrue(ball.PositionX >= ball.BallRadius);
        Assert.IsTrue(ball.PositionX <= _tableWidth - ball.BallRadius);
        Assert.IsTrue(ball.PositionY >= ball.BallRadius);
        Assert.IsTrue(ball.PositionY <= _tableHeight - ball.BallRadius);
        Assert.IsTrue(_mockLogger.LoggedMessages.Any(m => m.Contains($"BallState: ID={ball.Id}, Event=Created")));
    }

    [TestMethod]
    public async Task UpdateBalls_BounceOnLeftWall_AndLogged()
    {
        IBall ball = await _ballManager.CreateBall();
        ball.PositionX = ball.BallRadius - 1;
        ball.Velocity = new Vector2(-20.0f, 0.0f);
        _mockLogger.LoggedMessages.Clear();

        await _ballManager.UpdateBalls(0.1);

        Assert.IsTrue(ball.PositionX >= ball.BallRadius - 0.001);
        Assert.AreEqual(20.0f, ball.Velocity.X, 0.0001f);
        Assert.IsTrue(_mockLogger.LoggedMessages.Any(m => m.Contains($"WallCollision: ID={ball.Id}, Wall=Left")));
    }

    [TestMethod]
    public async Task UpdateBalls_BounceOnRightWall_AndLogged()
    {
        IBall ball = await _ballManager.CreateBall();
        ball.PositionX = _tableWidth - ball.BallRadius + 1;
        ball.Velocity = new Vector2(20.0f, 0.0f);
        _mockLogger.LoggedMessages.Clear();

        await _ballManager.UpdateBalls(0.1);

        Assert.IsTrue(ball.PositionX <= _tableWidth - ball.BallRadius + 0.001);
        Assert.AreEqual(-20.0f, ball.Velocity.X, 0.0001f);
        Assert.IsTrue(_mockLogger.LoggedMessages.Any(m => m.Contains($"WallCollision: ID={ball.Id}, Wall=Right")));
    }

    [TestMethod]
    public async Task UpdateBalls_BounceOnBottomWall_AndLogged()
    {
        IBall ball = await _ballManager.CreateBall();
        ball.PositionY = _tableHeight - ball.BallRadius + 1;
        ball.Velocity = new Vector2(0.0f, 20.0f);
        _mockLogger.LoggedMessages.Clear();

        await _ballManager.UpdateBalls(0.1);

        Assert.IsTrue(ball.PositionY <= _tableHeight - ball.BallRadius + 0.001);
        Assert.AreEqual(-20.0f, ball.Velocity.Y, 0.0001f);
        Assert.IsTrue(_mockLogger.LoggedMessages.Any(m => m.Contains($"WallCollision: ID={ball.Id}, Wall=Bottom")));
    }

    [TestMethod]
    public async Task UpdateBalls_BounceOnTopWall_AndLogged()
    {
        IBall ball = await _ballManager.CreateBall();
        ball.PositionY = ball.BallRadius - 1;
        ball.Velocity = new Vector2(0.0f, -20.0f);
        _mockLogger.LoggedMessages.Clear();

        await _ballManager.UpdateBalls(0.1);

        Assert.IsTrue(ball.PositionY >= ball.BallRadius - 0.001);
        Assert.AreEqual(20.0f, ball.Velocity.Y, 0.0001f);
        Assert.IsTrue(_mockLogger.LoggedMessages.Any(m => m.Contains($"WallCollision: ID={ball.Id}, Wall=Top")));
    }

    [TestMethod]
    public async Task GetBalls_ReturnsAllCreatedBalls()
    {
        for (int i = 0; i < 10; i++)
        {
            await _ballManager.CreateBall();
        }

        var balls = await _ballManager.GetBalls();

        Assert.AreEqual(10, balls.Count, "GetBalls did not return the correct number of balls");
    }

    [TestMethod]
    public async Task RemoveAllBalls_ClearsBalls_AndLogsRemoval()
    {
        for (int i = 0; i < 5; i++)
        {
            await _ballManager.CreateBall();
        }
        var ballsBeforeClear = await _ballManager.GetBalls();
        Assert.AreEqual(5, ballsBeforeClear.Count);
        _mockLogger.LoggedMessages.Clear();

        await _ballManager.RemoveAllBalls();

        Assert.AreEqual(0, (await _ballManager.GetBalls()).Count);
        foreach (var ball in ballsBeforeClear)
        {
            Assert.IsTrue(_mockLogger.LoggedMessages.Any(m => m.Contains($"BallState: ID={ball.Id}, Event=RemovedDueToClearAll")));
        }
        Assert.AreEqual(ballsBeforeClear.Count, _mockLogger.LoggedMessages.Count(m => m.Contains("Event=RemovedDueToClearAll")));
    }

    [TestMethod]
    public async Task UpdateBalls_BallCollision_AndLogged()
    {
        _mockLogger.LoggedMessages.Clear();
        IBall ball1 = await _ballManager.CreateBall();
        IBall ball2 = await _ballManager.CreateBall();
        _mockLogger.LoggedMessages.Clear();
        
        ball1.PositionX = 100;
        ball1.PositionY = 100;
        ball1.BallRadius = 10;
        ball1.Velocity = new Vector2(20.0f, 0.0f);

        ball2.PositionX = 115;
        ball2.PositionY = 100;
        ball2.BallRadius = 10;
        ball2.Velocity = new Vector2(-20.0f, 0.0f);

        Vector2 originalVelocity1 = ball1.Velocity;
        Vector2 originalVelocity2 = ball2.Velocity;
        
        await _ballManager.UpdateBalls(0.1);
        
        Assert.AreNotEqual(originalVelocity1.X, ball1.Velocity.X, 0.001f, "Ball1's X velocity should change after collision");
        Assert.AreNotEqual(originalVelocity2.X, ball2.Velocity.X, 0.001f, "Ball2's X velocity should change after collision");
        Assert.IsTrue(_mockLogger.LoggedMessages.Any(m => m.Contains($"Collision: Ball1_ID={ball1.Id}, Ball2_ID={ball2.Id}") || m.Contains($"Collision: Ball1_ID={ball2.Id}, Ball2_ID={ball1.Id}")));
    }

    [TestMethod]
    public async Task CreateBall_HasCorrectProperties()
    {
        IBall ball = await _ballManager.CreateBall();
        
        Assert.IsTrue(ball.Id > 0, "Ball should have a positive ID");
        Assert.IsTrue(ball.BallRadius >= _ballRadius * 0.8 && ball.BallRadius <= _ballRadius * 1.2,
            $"Ball radius should be within range of base radius ({_ballRadius})");
        Assert.IsTrue(ball.Mass > 0, "Ball should have positive mass");
    }

    [TestMethod]
    public async Task UpdateBalls_MovesWithDeltaTime()
    {
        IBall ball = await _ballManager.CreateBall();
        ball.PositionX = 100;
        ball.PositionY = 100;
        ball.Velocity = new Vector2(10.0f, 20.0f);
        double initialX = ball.PositionX;
        double initialY = ball.PositionY;
        double deltaTime = 0.5;
        
        await _ballManager.UpdateBalls(deltaTime);
        
        Assert.AreEqual(initialX + ball.Velocity.X * deltaTime, ball.PositionX, 0.001,
            "Ball X position should be updated according to velocity and delta time");
        Assert.AreEqual(initialY + ball.Velocity.Y * deltaTime, ball.PositionY, 0.001,
            "Ball Y position should be updated according to velocity and delta time");
    }

    [TestMethod]
    public async Task UpdateBalls_EmptyBallsListDoesNothing()
    {
        await _ballManager.RemoveAllBalls();
        Assert.AreEqual(0, (await _ballManager.GetBalls()).Count);
        
        await _ballManager.UpdateBalls(1.0);
        
        Assert.AreEqual(0, (await _ballManager.GetBalls()).Count);
    }
}