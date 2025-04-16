using Logic;
using Data;
using Model;
using System.Numerics;

namespace ModelTest
{
    [TestClass]
    public class PoolTableTest
    {
        [TestMethod]
        public void Constructor_InitializesPropertiesCorrectly()
        {
            double expectedWidth = 800;
            double expectedHeight = 400;
            double expectedBallRadius = 12;
            MockBallManager mockBallManager = new MockBallManager();
            
            PoolTable poolTable = new PoolTable(expectedWidth, expectedHeight, expectedBallRadius, mockBallManager);
            
            Assert.AreEqual(expectedWidth, poolTable.Width);
            Assert.AreEqual(expectedHeight, poolTable.Height);
            Assert.AreEqual(expectedBallRadius, poolTable.BallRadius);
            Assert.AreEqual(mockBallManager, poolTable.BallManager);
        }
        
        [TestMethod]
        public void Update_CallsBallManagerUpdateBalls()
        {
            double deltaTime = 0.1;
            MockBallManager mockBallManager = new MockBallManager();
            PoolTable poolTable = new PoolTable(800, 600, 15, mockBallManager);
            
            poolTable.Update(deltaTime);
            
            Assert.AreEqual(deltaTime, mockBallManager.LastUpdateDeltaTime);
            Assert.AreEqual(1, mockBallManager.UpdateBallsCallCount);
        }
        
        [TestMethod]
        public void GetBallModels_ReturnsCorrectModels()
        {
            MockBallManager mockBallManager = new MockBallManager();
            mockBallManager.MockBalls.Add(new MockBall { Id = 1, PositionX = 100, PositionY = 200, BallRadius = 12 });
            mockBallManager.MockBalls.Add(new MockBall { Id = 2, PositionX = 300, PositionY = 400, BallRadius = 12 });
            
            PoolTable poolTable = new PoolTable(800, 400, 12, mockBallManager);
            
            var ballModels = poolTable.GetBallModels();
            
            Assert.AreEqual(2, ballModels.Count);
            
            Assert.AreEqual(1, ballModels[0].Id);
            Assert.AreEqual(100, ballModels[0].PositionX);
            Assert.AreEqual(200, ballModels[0].PositionY);
            Assert.AreEqual(12, ballModels[0].BallRadius);
            
            Assert.AreEqual(2, ballModels[1].Id);
            Assert.AreEqual(300, ballModels[1].PositionX);
            Assert.AreEqual(400, ballModels[1].PositionY);
            Assert.AreEqual(12, ballModels[1].BallRadius);
        }
        
        [TestMethod]
        public void AddBall_DelegatesToBallManager()
        {
            MockBallManager mockBallManager = new MockBallManager();
            PoolTable poolTable = new PoolTable(800, 400, 12, mockBallManager);
            
            MockBall expectedBall = new MockBall { Id = 123 };
            mockBallManager.NextBallToCreate = expectedBall;
            
            IBall result = poolTable.AddBall();
            
            Assert.AreEqual(1, mockBallManager.CreateBallCallCount);
            Assert.AreEqual(expectedBall, result);
        }
        
        // Mock implementation of IBallManager
        private class MockBallManager : IBallManager
        {
            public List<IBall> MockBalls { get; } = new List<IBall>();
            public int UpdateBallsCallCount { get; private set; } = 0;
            public int CreateBallCallCount { get; private set; } = 0;
            public int RemoveBallCallCount { get; private set; } = 0;
            public double LastUpdateDeltaTime { get; private set; }
            public MockBall NextBallToCreate { get; set; }
    
            public IBall CreateBall()
            {
                CreateBallCallCount++;
                return NextBallToCreate;
            }
    
            public List<IBall> GetBalls()
            {
                return MockBalls.Cast<IBall>().ToList();
            }
    
            public void UpdateBalls(double deltaTime)
            {
                UpdateBallsCallCount++;
                LastUpdateDeltaTime = deltaTime;
            }
            
            public bool RemoveBall(int id)
            {
                RemoveBallCallCount++;
                IBall ballToRemove = MockBalls.FirstOrDefault(b => b.Id == id);
                if (ballToRemove != null)
                {
                    return MockBalls.Remove(ballToRemove);
                }
                return false;
            }
        }
        
        private class MockBall : IBall
        {
            public int Id { get; set; }
            public double PositionX { get; set; }
            public double PositionY { get; set; }
            public double BallRadius { get; set; }
            public Vector2 Velocity { get; set; }
            public double Mass => 1.0;
            
            public void Move(double deltaTime)
            {
                // No implementation needed for tests
            }
        }
    }
}