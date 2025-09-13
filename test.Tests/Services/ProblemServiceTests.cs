using Microsoft.Extensions.Options;
using Moq;
using MongoDB.Driver;
using test.Configuration;
using test.Models;
using test.Services;

namespace test.Tests.Services
{
    public class ProblemServiceTests
    {
        private readonly Mock<IMongoCollection<Problem>> _mockProblemsCollection;
        private readonly Mock<IMongoCollection<Category>> _mockCategoriesCollection;
        private readonly Mock<MongoDbService> _mockMongoDbService;
        private readonly Mock<MongoDbSettings> _mockSettings;
        private readonly ProblemService _problemService;

        public ProblemServiceTests()
        {
            _mockProblemsCollection = new Mock<IMongoCollection<Problem>>();
            _mockCategoriesCollection = new Mock<IMongoCollection<Category>>();
            _mockSettings = new Mock<MongoDbSettings>();
            _mockMongoDbService = new Mock<MongoDbService>(_mockSettings.Object);
            
            _mockMongoDbService.Setup(x => x.Database.GetCollection<Problem>(It.IsAny<string>()))
                .Returns(_mockProblemsCollection.Object);
            _mockMongoDbService.Setup(x => x.Database.GetCollection<Category>(It.IsAny<string>()))
                .Returns(_mockCategoriesCollection.Object);

            _problemService = new ProblemService(_mockMongoDbService.Object, _mockSettings.Object);
        }

        [Fact]
        public async Task GetAllProblemsAsync_ShouldReturnAllActiveProblems()
        {
            // Arrange
            var problems = new List<Problem>
            {
                new Problem { Id = "1", Title = "Problem 1", IsActive = true },
                new Problem { Id = "2", Title = "Problem 2", IsActive = true }
            };

            var mockCursor = new Mock<IAsyncCursor<Problem>>();
            mockCursor.Setup(x => x.Current).Returns(problems);
            mockCursor.Setup(x => x.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _mockProblemsCollection.Setup(x => x.FindAsync<Problem>(
                It.IsAny<FilterDefinition<Problem>>(),
                It.IsAny<FindOptions<Problem, Problem>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockCursor.Object);

            // Act
            var result = await _problemService.GetAllProblemsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.All(result, problem => Assert.True(problem.IsActive));
        }

        [Fact]
        public async Task GetProblemsByCategoryAsync_ShouldReturnProblemsInCategory()
        {
            // Arrange
            var category = "Frontend Development";
            var problems = new List<Problem>
            {
                new Problem { Id = "1", Title = "Frontend Problem 1", Category = category, IsActive = true },
                new Problem { Id = "2", Title = "Frontend Problem 2", Category = category, IsActive = true }
            };

            var mockCursor = new Mock<IAsyncCursor<Problem>>();
            mockCursor.Setup(x => x.Current).Returns(problems);
            mockCursor.Setup(x => x.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _mockProblemsCollection.Setup(x => x.FindAsync<Problem>(
                It.IsAny<FilterDefinition<Problem>>(),
                It.IsAny<FindOptions<Problem, Problem>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockCursor.Object);

            // Act
            var result = await _problemService.GetProblemsByCategoryAsync(category);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.All(result, problem => Assert.Equal(category, problem.Category));
        }

        [Fact]
        public async Task GetProblemsByCategoryAsync_WithAllCategory_ShouldReturnAllProblems()
        {
            // Arrange
            var problems = new List<Problem>
            {
                new Problem { Id = "1", Title = "Problem 1", IsActive = true },
                new Problem { Id = "2", Title = "Problem 2", IsActive = true }
            };

            var mockCursor = new Mock<IAsyncCursor<Problem>>();
            mockCursor.Setup(x => x.Current).Returns(problems);
            mockCursor.Setup(x => x.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _mockProblemsCollection.Setup(x => x.FindAsync<Problem>(
                It.IsAny<FilterDefinition<Problem>>(),
                It.IsAny<FindOptions<Problem, Problem>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockCursor.Object);

            // Act
            var result = await _problemService.GetProblemsByCategoryAsync("all");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetProblemByIdAsync_WhenProblemExists_ShouldReturnProblem()
        {
            // Arrange
            var problemId = "123";
            var expectedProblem = new Problem
            {
                Id = problemId,
                Title = "Test Problem",
                IsActive = true
            };

            var mockCursor = new Mock<IAsyncCursor<Problem>>();
            mockCursor.Setup(x => x.Current).Returns(new[] { expectedProblem });
            mockCursor.Setup(x => x.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _mockProblemsCollection.Setup(x => x.FindAsync<Problem>(
                It.IsAny<FilterDefinition<Problem>>(),
                It.IsAny<FindOptions<Problem, Problem>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockCursor.Object);

            // Act
            var result = await _problemService.GetProblemByIdAsync(problemId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedProblem.Id, result.Id);
            Assert.Equal(expectedProblem.Title, result.Title);
        }

        [Fact]
        public async Task GetProblemByIdAsync_WhenProblemDoesNotExist_ShouldReturnNull()
        {
            // Arrange
            var problemId = "nonexistent";
            var mockCursor = new Mock<IAsyncCursor<Problem>>();
            mockCursor.Setup(x => x.Current).Returns(new Problem[0]);
            mockCursor.Setup(x => x.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _mockProblemsCollection.Setup(x => x.FindAsync<Problem>(
                It.IsAny<FilterDefinition<Problem>>(),
                It.IsAny<FindOptions<Problem, Problem>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockCursor.Object);

            // Act
            var result = await _problemService.GetProblemByIdAsync(problemId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateProblemAsync_ShouldAssignProblemNumberAndCreateProblem()
        {
            // Arrange
            var newProblem = new Problem
            {
                Title = "New Problem",
                Description = "Description",
                Difficulty = "Easy",
                Category = "Frontend Development",
                Tags = new List<string> { "Test" },
                AcceptanceRate = 80,
                Likes = 0
            };

            // Mock for getting next problem number
            var existingProblem = new Problem { ProblemNumber = 5 };
            var mockCursor = new Mock<IAsyncCursor<Problem>>();
            mockCursor.Setup(x => x.Current).Returns(new[] { existingProblem });
            mockCursor.Setup(x => x.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _mockProblemsCollection.Setup(x => x.FindAsync<Problem>(
                It.IsAny<FilterDefinition<Problem>>(),
                It.IsAny<FindOptions<Problem, Problem>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockCursor.Object);

            _mockProblemsCollection.Setup(x => x.InsertOneAsync(
                It.IsAny<Problem>(),
                It.IsAny<InsertOneOptions>(),
                It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _problemService.CreateProblemAsync(newProblem);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(6, result.ProblemNumber); // Should be next number after 5
            Assert.True(result.IsActive);
            Assert.True(result.CreatedAt <= DateTime.UtcNow);
            Assert.True(result.UpdatedAt <= DateTime.UtcNow);

            _mockProblemsCollection.Verify(x => x.InsertOneAsync(
                It.IsAny<Problem>(),
                It.IsAny<InsertOneOptions>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateProblemAsync_ShouldUpdateProblem()
        {
            // Arrange
            var problemId = "123";
            var updatedProblem = new Problem
            {
                Id = problemId,
                Title = "Updated Problem",
                IsActive = true
            };

            _mockProblemsCollection.Setup(x => x.ReplaceOneAsync(
                It.IsAny<FilterDefinition<Problem>>(),
                It.IsAny<Problem>(),
                It.IsAny<ReplaceOptions>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ReplaceOneResult.Acknowledged(1, 1, "123"));

            // Act
            var result = await _problemService.UpdateProblemAsync(problemId, updatedProblem);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.UpdatedAt <= DateTime.UtcNow);

            _mockProblemsCollection.Verify(x => x.ReplaceOneAsync(
                It.IsAny<FilterDefinition<Problem>>(),
                It.IsAny<Problem>(),
                It.IsAny<ReplaceOptions>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteProblemAsync_WhenProblemExists_ShouldReturnTrue()
        {
            // Arrange
            var problemId = "123";

            _mockProblemsCollection.Setup(x => x.DeleteOneAsync(
                It.IsAny<FilterDefinition<Problem>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(new DeleteResult.Acknowledged(1));

            // Act
            var result = await _problemService.DeleteProblemAsync(problemId);

            // Assert
            Assert.True(result);

            _mockProblemsCollection.Verify(x => x.DeleteOneAsync(
                It.IsAny<FilterDefinition<Problem>>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteProblemAsync_WhenProblemDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            var problemId = "nonexistent";

            _mockProblemsCollection.Setup(x => x.DeleteOneAsync(
                It.IsAny<FilterDefinition<Problem>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(new DeleteResult.Acknowledged(0));

            // Act
            var result = await _problemService.DeleteProblemAsync(problemId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task GetAllCategoriesAsync_ShouldReturnAllActiveCategories()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category { Key = "frontend", Name = "Frontend Development", IsActive = true },
                new Category { Key = "backend", Name = "Backend Development", IsActive = true }
            };

            var mockCursor = new Mock<IAsyncCursor<Category>>();
            mockCursor.Setup(x => x.Current).Returns(categories);
            mockCursor.Setup(x => x.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _mockCategoriesCollection.Setup(x => x.FindAsync<Category>(
                It.IsAny<FilterDefinition<Category>>(),
                It.IsAny<FindOptions<Category, Category>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockCursor.Object);

            // Act
            var result = await _problemService.GetAllCategoriesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.All(result, category => Assert.True(category.IsActive));
        }

        [Fact]
        public async Task CreateCategoryAsync_ShouldCreateCategory()
        {
            // Arrange
            var newCategory = new Category
            {
                Key = "test",
                Name = "Test Category",
                Icon = "🧪",
                Color = "#FF0000",
                ProblemCount = 0,
                EasyCount = 0,
                MediumCount = 0,
                HardCount = 0
            };

            _mockCategoriesCollection.Setup(x => x.InsertOneAsync(
                It.IsAny<Category>(),
                It.IsAny<InsertOneOptions>(),
                It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _problemService.CreateCategoryAsync(newCategory);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsActive);
            Assert.True(result.CreatedAt <= DateTime.UtcNow);

            _mockCategoriesCollection.Verify(x => x.InsertOneAsync(
                It.IsAny<Category>(),
                It.IsAny<InsertOneOptions>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
