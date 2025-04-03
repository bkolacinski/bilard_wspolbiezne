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
        public void RemoveBall_BallIsRemoved()
        {
            _ballManager.CreateBall();
            Assert.AreEqual(_ballManager.GetBalls().Count, 1);
            Assert.AreEqual(_ballManager.RemoveBall(1), true);
            Assert.AreEqual(_ballManager.GetBalls().Count, 0);
            Assert.AreEqual(_ballManager.RemoveBall(0), false);
        }

        [TestMethod]
        public void CreateBall_BallWithinBounds()
        {
            IBall ball = _ballManager.CreateBall();
            
            Assert.IsNotNull(ball);
            Assert.IsTrue(ball.PositionX >= _ballRadius);
            Assert.IsTrue(ball.PositionX <= _tableWidth - _ballRadius);
            Assert.IsTrue(ball.PositionY >= _ballRadius);
            Assert.IsTrue(ball.PositionY <= _tableHeight - _ballRadius);
        }

        [TestMethod]
        public void UpdateBalls_BounceOnLeftWall()
        {
            IBall ball = _ballManager.CreateBall();
            ball.PositionX = _ballRadius;
            ball.Velocity = new Vector2(-20.0f, 0.0f);
            
            _ballManager.UpdateBalls(1.0);
            
            Assert.IsTrue(ball.PositionX >= _ballRadius);
            Assert.AreEqual(ball.Velocity.X, 20.0f, 0.0001f);
        }
        
        [TestMethod]
        public void UpdateBalls_BounceOnRightWall()
        {
            IBall ball = _ballManager.CreateBall();
            ball.PositionX = _tableWidth - _ballRadius + 1;
            ball.Velocity = new Vector2(20.0f, 0.0f);
            
            _ballManager.UpdateBalls(1.0);
            
            Assert.IsTrue(ball.PositionX <= _tableWidth - _ballRadius);
            Assert.AreEqual(-20.0f, ball.Velocity.X, 0.0001f);
        }
        
        [TestMethod]
        public void UpdateBalls_BounceOnTopWall()
        {
            IBall ball = _ballManager.CreateBall();
            ball.PositionY = _ballRadius;
            ball.Velocity = new Vector2(0.0f, -20.0f);
            
            _ballManager.UpdateBalls(1.0);
            
            Assert.IsTrue(ball.PositionY >= _ballRadius);
            Assert.AreEqual(20.0f, ball.Velocity.Y, 0.0001f);
        }
        
        [TestMethod]
        public void UpdateBalls_BounceOnBottomWall()
        {
            IBall ball = _ballManager.CreateBall();
            ball.PositionY = _tableHeight - _ballRadius + 1;
            ball.Velocity = new Vector2(0.0f, 20.0f);
            
            _ballManager.UpdateBalls(1.0);
            
            Assert.IsTrue(ball.PositionY <= _tableHeight - _ballRadius);
            Assert.AreEqual(-20.0f, ball.Velocity.Y, 0.0001f);
        }
        
        [TestMethod]
        public void GetBalls_ReturnsAllCreatedBalls()
        {
            for (int i = 0; i < 10; i++)
            {
                _ballManager.CreateBall();
            }
            
            var balls = _ballManager.GetBalls();
            
            Assert.AreEqual(10, balls.Count, "GetBalls did not return the correct number of balls");
        }
    }
}
