using Data;
using Model;

namespace ModelTest
{
    [TestClass]
    public class BallModelTest
    {
        [TestMethod]
        public void Constructor_InitializesPropertiesCorrectly()
        {
            int expectedId = 123;
            double expectedPositionX = 12.3;
            double expectedPositionY = 123.0;
            double expectedRadius = 7.5;
            
            TestBall testBall = new TestBall
            {
                Id = expectedId,
                PositionX = expectedPositionX,
                PositionY = expectedPositionY,
                BallRadius = expectedRadius
            };
            
            BallModel ballModel = new BallModel(testBall);
            
            Assert.AreEqual(expectedId, ballModel.Id);
            Assert.AreEqual(expectedPositionX, ballModel.PositionX);
            Assert.AreEqual(expectedPositionY, ballModel.PositionY);
            Assert.AreEqual(expectedRadius, ballModel.BallRadius);
        }
        
        [TestMethod]
        public void Diameter_ReturnsTwiceTheRadius()
        {
            double radius = 12.3;
            TestBall testBall = new TestBall { BallRadius = radius };
            BallModel ballModel = new BallModel(testBall);
            
            double diameter = ballModel.Diameter;
            
            Assert.AreEqual(radius * 2, diameter);
        }
        
        [TestMethod]
        public void Update_UpdatesPositionProperties()
        {
            TestBall initialBall = new TestBall { PositionX = 10, PositionY = 20 };
            BallModel ballModel = new BallModel(initialBall);
            
            TestBall updatedBall = new TestBall { PositionX = 30, PositionY = 40 };
            
            ballModel.Update(updatedBall);
            
            Assert.AreEqual(updatedBall.PositionX, ballModel.PositionX);
            Assert.AreEqual(updatedBall.PositionY, ballModel.PositionY);
        }
        
        private class TestBall : IBall
        {
            public int Id { get; set; }
            public double PositionX { get; set; }
            public double PositionY { get; set; }
            public double BallRadius { get; set; }
            public System.Numerics.Vector2 Velocity { get; set; }
            
            public double Mass => 1.0;
            
            public void Move(double deltaTime)
            {
                // No implementation needed for tests
            }
        }
    }
}