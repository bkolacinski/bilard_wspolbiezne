using System.Numerics;
using Logic;
using Data;

namespace LogicTest
{
    [TestClass]
    public sealed class BallManagerTest
    {
        private double _tableWidth;
        private double _tableHeight;
        private double _ballRadius;
        private IBallManager _ballManager = null!;

        [TestInitialize]
        public void Initialize()
        {
            _tableWidth = 300;
            _tableHeight = 200;
            _ballRadius = 10;
            _ballManager = new BallManager(_tableWidth, _tableHeight, _ballRadius);
        }

        [TestMethod]
        public async Task RemoveBall_BallIsRemoved()
        {
            await _ballManager.CreateBall();
            Assert.AreEqual(1, (await _ballManager.GetBalls()).Count);
            int ballId = (await _ballManager.GetBalls())[0].Id;
            Assert.IsTrue(await _ballManager.RemoveBall(ballId)); 
            Assert.AreEqual(0, (await _ballManager.GetBalls()).Count);
            Assert.IsFalse(await _ballManager.RemoveBall(ballId));
        }

        [TestMethod]
        public async Task CreateBall_BallWithinBounds()
        {
            IBall ball = await _ballManager.CreateBall();

            Assert.IsNotNull(ball);
            Assert.IsTrue(ball.PositionX >= ball.BallRadius);
            Assert.IsTrue(ball.PositionX <= _tableWidth - ball.BallRadius);
            Assert.IsTrue(ball.PositionY >= ball.BallRadius);
            Assert.IsTrue(ball.PositionY <= _tableHeight - ball.BallRadius);
        }

        [TestMethod]
        public async Task UpdateBalls_BounceOnLeftWall()
        {
            IBall ball = await _ballManager.CreateBall();
            ball.PositionX = ball.BallRadius;
            ball.Velocity = new Vector2(-20.0f, 0.0f);

            await _ballManager.UpdateBalls(1.0);

            Assert.IsTrue(ball.PositionX >= ball.BallRadius);
            Assert.AreEqual(ball.Velocity.X, 20.0f, 0.0001f);
        }

        [TestMethod]
        public async Task UpdateBalls_BounceOnRightWall()
        {
            IBall ball = await _ballManager.CreateBall();
            ball.PositionX = _tableWidth - ball.BallRadius + 1;
            ball.Velocity = new Vector2(20.0f, 0.0f);

            await _ballManager.UpdateBalls(1.0);

            Assert.IsTrue(ball.PositionX <= _tableWidth - ball.BallRadius);
            Assert.AreEqual(-20.0f, ball.Velocity.X, 0.0001f);
        }

        [TestMethod]
        public async Task UpdateBalls_BounceOnBottomWall()
        {
            IBall ball = await _ballManager.CreateBall();
            ball.PositionY = _tableHeight - ball.BallRadius + 1;
            ball.Velocity = new Vector2(0.0f, 20.0f);

            await _ballManager.UpdateBalls(1.0);

            Assert.IsTrue(ball.PositionY <= _tableHeight - ball.BallRadius);
            Assert.AreEqual(-20.0f, ball.Velocity.Y, 0.0001f);
        }

        [TestMethod]
        public async Task UpdateBalls_BounceOnTopWall()
        {
            IBall ball = await _ballManager.CreateBall();
            ball.PositionY = ball.BallRadius;
            ball.Velocity = new Vector2(0.0f, -20.0f);

            await _ballManager.UpdateBalls(1.0);

            Assert.IsTrue(ball.PositionY >= ball.BallRadius);
            Assert.AreEqual(20.0f, ball.Velocity.Y, 0.0001f);
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
        public async Task RemoveAllBalls_ClearsBalls()
        {
            for (int i = 0; i < 5; i++)
            {
                await _ballManager.CreateBall();
            }

            Assert.AreEqual(5, (await _ballManager.GetBalls()).Count);
            
            await _ballManager.RemoveAllBalls();
            
            Assert.AreEqual(0, (await _ballManager.GetBalls()).Count);
        }

        [TestMethod]
        public async Task UpdateBalls_BallCollision()
        {
            // Arrange
            IBall ball1 = await _ballManager.CreateBall();
            IBall ball2 = await _ballManager.CreateBall();

            // Position balls for collision
            ball1.PositionX = 100;
            ball1.PositionY = 100;
            ball1.BallRadius = 10;
            ball1.Velocity = new Vector2(20.0f, 0.0f);

            ball2.PositionX = 120;
            ball2.PositionY = 100;
            ball2.BallRadius = 10;
            ball2.Velocity = new Vector2(-20.0f, 0.0f);

            Vector2 originalVelocity1 = ball1.Velocity;
            Vector2 originalVelocity2 = ball2.Velocity;

            // Act
            await _ballManager.UpdateBalls(0.1);

            // Assert
            Assert.AreNotEqual(originalVelocity1, ball1.Velocity, "Ball1's velocity should change after collision");
            Assert.AreNotEqual(originalVelocity2, ball2.Velocity, "Ball2's velocity should change after collision");
        }

        [TestMethod]
        public async Task CreateBall_HasCorrectProperties()
        {
            // Act
            IBall ball = await _ballManager.CreateBall();

            // Assert
            Assert.IsTrue(ball.Id > 0, "Ball should have a positive ID");
            Assert.IsTrue(ball.BallRadius >= _ballRadius * 0.8 && ball.BallRadius <= _ballRadius * 1.2,
                $"Ball radius should be within range of base radius ({_ballRadius})");
            Assert.IsTrue(ball.Mass > 0, "Ball should have positive mass");
        }

        [TestMethod]
        public async Task UpdateBalls_MovesWithDeltaTime()
        {
            // Arrange
            IBall ball = await _ballManager.CreateBall();
            ball.PositionX = 100;
            ball.PositionY = 100;
            ball.Velocity = new Vector2(10.0f, 20.0f);
            double initialX = ball.PositionX;
            double initialY = ball.PositionY;
            double deltaTime = 0.5;

            // Act
            await _ballManager.UpdateBalls(deltaTime);

            // Assert
            Assert.AreEqual(initialX + ball.Velocity.X * deltaTime, ball.PositionX, 0.001,
                "Ball X position should be updated according to velocity and delta time");
            Assert.AreEqual(initialY + ball.Velocity.Y * deltaTime, ball.PositionY, 0.001,
                "Ball Y position should be updated according to velocity and delta time");
        }

        [TestMethod]
        public async Task UpdateBalls_EmptyBallsListDoesNothing()
        {
            // Arrange
            await _ballManager.RemoveAllBalls();
            Assert.AreEqual(0, (await _ballManager.GetBalls()).Count);

            // Act - This should not throw an exception
            await _ballManager.UpdateBalls(1.0);

            // Assert
            Assert.AreEqual(0, (await _ballManager.GetBalls()).Count);
        }
    }
}