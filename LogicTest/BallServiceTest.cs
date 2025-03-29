using System.Numerics;
using Logic;
using Data;

namespace LogicTest
{
    [TestClass]
    public sealed class BallServiceTest
    {
        private double _tableWidth;
        private double _tableHeight;
        private double _ballRadius;
        private BallService _ballService = null!;

        [TestInitialize]
        public void Initialize()
        {
            _tableWidth = 300;
            _tableHeight = 200;
            _ballRadius = 10;
            _ballService = new BallService(_tableWidth, _tableHeight, _ballRadius);
        }

        [TestMethod]
        public void CreateBall_BallWithinBounds()
        {
            Ball ball = _ballService.CreateBall();
            
            Assert.IsNotNull(ball);
            Assert.IsTrue(ball.PositionX >= _ballRadius);
            Assert.IsTrue(ball.PositionX <= _tableWidth - _ballRadius);
            Assert.IsTrue(ball.PositionY >= _ballRadius);
            Assert.IsTrue(ball.PositionY <= _tableHeight - _ballRadius);
        }

        [TestMethod]
        public void UpdateBalls_BounceOnLeftWall()
        {
            Ball ball = _ballService.CreateBall();
            ball.PositionX = _ballRadius;
            ball.Velocity = new Vector2(-20.0f, 0.0f);
            
            _ballService.UpdateBalls(1.0);
            
            Assert.IsTrue(ball.PositionX >= _ballRadius);
            Assert.AreEqual(ball.Velocity.X, 20.0f, 0.0001f);
        }
        
        [TestMethod]
        public void UpdateBalls_BounceOnRightWall()
        {
            Ball ball = _ballService.CreateBall();
            ball.PositionX = _tableWidth - _ballRadius + 1;
            ball.Velocity = new Vector2(20.0f, 0.0f);
            
            _ballService.UpdateBalls(1.0);
            
            Assert.IsTrue(ball.PositionX <= _tableWidth - _ballRadius);
            Assert.AreEqual(-20.0f, ball.Velocity.X, 0.0001f);
        }
        
        [TestMethod]
        public void UpdateBalls_BounceOnTopWall()
        {
            Ball ball = _ballService.CreateBall();
            ball.PositionY = _ballRadius;
            ball.Velocity = new Vector2(0.0f, -20.0f);
            
            _ballService.UpdateBalls(1.0);
            
            Assert.IsTrue(ball.PositionY >= _ballRadius);
            Assert.AreEqual(20.0f, ball.Velocity.Y, 0.0001f);
        }
        
        [TestMethod]
        public void UpdateBalls_BounceOnBottomWall()
        {
            Ball ball = _ballService.CreateBall();
            ball.PositionY = _tableHeight - _ballRadius + 1;
            ball.Velocity = new Vector2(0.0f, 20.0f);
            
            _ballService.UpdateBalls(1.0);
            
            Assert.IsTrue(ball.PositionY <= _tableHeight - _ballRadius);
            Assert.AreEqual(-20.0f, ball.Velocity.Y, 0.0001f);
        }
        
        [TestMethod]
        public void GetBalls_ReturnsAllCreatedBalls()
        {
            for (int i = 0; i < 10; i++)
            {
                _ballService.CreateBall();
            }
            
            var balls = _ballService.GetBalls();
            
            Assert.AreEqual(10, balls.Count, "GetBalls did not return the correct number of balls");
        }
    }
}
