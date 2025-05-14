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
        public async Task Update_CallsBallManagerUpdateBalls()
        {
            double deltaTime = 0.1;
            MockBallManager mockBallManager = new MockBallManager();
            PoolTable poolTable = new PoolTable(800, 600, 15, mockBallManager);
            
            await poolTable.Update(deltaTime);
            
            Assert.AreEqual(deltaTime, mockBallManager.LastUpdateDeltaTime);
            Assert.AreEqual(1, mockBallManager.UpdateBallsCallCount);
        }
        
        [TestMethod]
        public async Task GetBallModels_ReturnsCorrectModels()
        {
            MockBallManager mockBallManager = new MockBallManager();
            mockBallManager.MockBalls.Add(new MockBall { Id = 1, PositionX = 100, PositionY = 200, BallRadius = 12, 
                Color = "Red" });
            mockBallManager.MockBalls.Add(new MockBall { Id = 2, PositionX = 300, PositionY = 400, BallRadius = 12,
                Color = "Red" });
            
            PoolTable poolTable = new PoolTable(800, 400, 12, mockBallManager);
            
            var ballModels = await poolTable.GetBallModels(); // Added await
            
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
        public async Task AddBall_DelegatesToBallManager()
        {
            MockBallManager mockBallManager = new MockBallManager();
            PoolTable poolTable = new PoolTable(800, 400, 12, mockBallManager);
            
            MockBall expectedBall = new MockBall { Id = 123, Color = "Red" };
            mockBallManager.NextBallToCreate = expectedBall;
            
            IBall result = await poolTable.AddBall();
            
            Assert.AreEqual(1, mockBallManager.CreateBallCallCount);
            Assert.AreEqual(expectedBall, result);
        }
        
        private class MockBallManager : IBallManager
        {
            public List<IBall> MockBalls { get; } = new();
            public int UpdateBallsCallCount { get; private set; }
            public int CreateBallCallCount { get; private set; }
            public int RemoveBallCallCount { get; private set; }
            public int RemoveAllBallsCallCount { get; private set; }
            public double LastUpdateDeltaTime { get; private set; }
            public MockBall? NextBallToCreate { get; set; }
    
            public Task<IBall> CreateBall()
            {
                CreateBallCallCount++;
                return Task.FromResult<IBall>(NextBallToCreate ?? new MockBall {Color = "Red"});
            }
    
            public Task<List<IBall>> GetBalls()
            {
                return Task.FromResult(MockBalls.ToList());
            }

            public Task RemoveAllBalls()
            {
                RemoveAllBallsCallCount++;
                MockBalls.Clear();
                return Task.CompletedTask;
            }

            public Task UpdateBalls(double deltaTime)
            {
                UpdateBallsCallCount++;
                LastUpdateDeltaTime = deltaTime;
                return Task.CompletedTask;
            }
            
            public Task<bool> RemoveBall(int id)
            {
                RemoveBallCallCount++;
                IBall? ballToRemove = MockBalls.FirstOrDefault(b => b.Id == id);
                if (ballToRemove != null)
                {
                    return Task.FromResult(MockBalls.Remove(ballToRemove));
                }
                return Task.FromResult(false);
            }
        }
        
        [TestMethod]
        public async Task MockBallManager_RemoveAllBalls_ClearsAndCounts()
        {
            MockBallManager mockBallManager = new MockBallManager();
            mockBallManager.MockBalls.Add(new MockBall{Color = "Red"});
            
            await mockBallManager.RemoveAllBalls();

            Assert.AreEqual(0, mockBallManager.MockBalls.Count);
            Assert.AreEqual(1, mockBallManager.RemoveAllBallsCallCount);
        }

        [TestMethod]
        public void MockBallManager_ReturnsCorrectProperties()
        {
            MockBallManager mockBallManager = new MockBallManager();
            
            mockBallManager.RemoveAllBalls();
            Assert.AreEqual(0, mockBallManager.RemoveBallCallCount);
        }
        
        private class MockBall : IBall
        {
            public int Id { get; set; }
            public double PositionX { get; set; }
            public double PositionY { get; set; }
            public double BallRadius { get; set; }
            public Vector2 Velocity { get; set; }
            public required string Color { get; set; }
            public double Mass => 1.0;
            
            public void Move(double deltaTime)
            {
                // No implementation needed for tests
            }
        }
        
        [TestMethod]
        public void MockBall_ReturnsCorrectProperties()
        {
            MockBall mockBall = new MockBall
            {
                Id = 123, 
                PositionX = 100, 
                PositionY = 200, 
                BallRadius = 12,
                Velocity = new Vector2(1.0f, 2.0f), 
                Color = "Red"
            };
            
            mockBall.Move(0.1);
            
            Assert.AreEqual(123, mockBall.Id);
            Assert.AreEqual(100, mockBall.PositionX);
            Assert.AreEqual(200, mockBall.PositionY);
            Assert.AreEqual(12, mockBall.BallRadius);
            Assert.AreEqual(new Vector2(1.0f, 2.0f), mockBall.Velocity);
            Assert.AreEqual(1.0, mockBall.Mass);
        }
    }
}