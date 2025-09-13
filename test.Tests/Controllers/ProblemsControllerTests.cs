using Microsoft.AspNetCore.Mvc;
using Moq;
using test.Controllers;
using test.Models;
using test.Services;

namespace test.Tests.Controllers
{
    public class ProblemsControllerTests
    {
        private readonly Mock<IProblemService> _mockProblemService;
        private readonly ProblemsController _problemsController;

        public ProblemsControllerTests()
        {
            _mockProblemService = new Mock<IProblemService>();
            _problemsController = new ProblemsController(_mockProblemService.Object);
        }

        [Fact]
        public async Task GetProblems_WithValidCategory_ShouldReturnProblems()
        {
            // Arrange
            var category = "Frontend Development";
            var problems = new List<Problem>
            {
                new Problem
                {
                    Id = "1",
                    Title = "Test Problem 1",
                    Category = category,
                    Difficulty = "Easy",
                    IsActive = true
                },
                new Problem
                {
                    Id = "2",
                    Title = "Test Problem 2",
                    Category = category,
                    Difficulty = "Medium",
                    IsActive = true
                }
            };

            _mockProblemService.Setup(x => x.GetProblemsByCategoryAsync(category))
                .ReturnsAsync(problems);

            // Act
            var result = await _problemsController.GetProblems(category);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<List<Problem>>(okResult.Value);
            Assert.Equal(2, response.Count);
            Assert.All(response, problem => Assert.Equal(category, problem.Category));
        }

        [Fact]
        public async Task GetProblems_WithAllCategory_ShouldReturnAllProblems()
        {
            // Arrange
            var problems = new List<Problem>
            {
                new Problem { Id = "1", Title = "Problem 1", IsActive = true },
                new Problem { Id = "2", Title = "Problem 2", IsActive = true }
            };

            _mockProblemService.Setup(x => x.GetProblemsByCategoryAsync("all"))
                .ReturnsAsync(problems);

            // Act
            var result = await _problemsController.GetProblems("all");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<List<Problem>>(okResult.Value);
            Assert.Equal(2, response.Count);
        }

        [Fact]
        public async Task GetProblems_WithEmptyCategory_ShouldReturnAllProblems()
        {
            // Arrange
            var problems = new List<Problem>
            {
                new Problem { Id = "1", Title = "Problem 1", IsActive = true }
            };

            _mockProblemService.Setup(x => x.GetProblemsByCategoryAsync("all"))
                .ReturnsAsync(problems);

            // Act
            var result = await _problemsController.GetProblems("");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<List<Problem>>(okResult.Value);
            Assert.Single(response);
        }

        [Fact]
        public async Task GetProblem_WithValidId_ShouldReturnProblem()
        {
            // Arrange
            var problemId = "123";
            var problem = new Problem
            {
                Id = problemId,
                Title = "Test Problem",
                Description = "Test Description",
                Difficulty = "Easy",
                Category = "Frontend Development",
                IsActive = true
            };

            _mockProblemService.Setup(x => x.GetProblemByIdAsync(problemId))
                .ReturnsAsync(problem);

            // Act
            var result = await _problemsController.GetProblem(problemId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Problem>(okResult.Value);
            Assert.Equal(problemId, response.Id);
            Assert.Equal("Test Problem", response.Title);
        }

        [Fact]
        public async Task GetProblem_WithInvalidId_ShouldReturnNotFound()
        {
            // Arrange
            var problemId = "nonexistent";

            _mockProblemService.Setup(x => x.GetProblemByIdAsync(problemId))
                .ReturnsAsync((Problem?)null);

            // Act
            var result = await _problemsController.GetProblem(problemId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.NotNull(notFoundResult.Value);
        }

        [Fact]
        public async Task CreateProblem_WithValidProblem_ShouldCreateProblem()
        {
            // Arrange
            var newProblem = new Problem
            {
                Title = "New Problem",
                Description = "New Description",
                Difficulty = "Medium",
                Category = "Backend Development",
                Tags = new List<string> { "C#", "ASP.NET" },
                AcceptanceRate = 75,
                Likes = 0
            };

            var createdProblem = new Problem
            {
                Id = "123",
                ProblemNumber = 26,
                Title = "New Problem",
                Description = "New Description",
                Difficulty = "Medium",
                Category = "Backend Development",
                Tags = new List<string> { "C#", "ASP.NET" },
                AcceptanceRate = 75,
                Likes = 0,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _mockProblemService.Setup(x => x.CreateProblemAsync(It.IsAny<Problem>()))
                .ReturnsAsync(createdProblem);

            // Act
            var result = await _problemsController.CreateProblem(newProblem);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            var response = Assert.IsType<Problem>(createdAtActionResult.Value);
            Assert.Equal(26, response.ProblemNumber);
            Assert.Equal("New Problem", response.Title);
            Assert.True(response.IsActive);

            _mockProblemService.Verify(x => x.CreateProblemAsync(It.IsAny<Problem>()), Times.Once);
        }

        [Fact]
        public async Task CreateProblem_WithNullProblem_ShouldReturnBadRequest()
        {
            // Act
            var result = await _problemsController.CreateProblem(null!);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateProblem_WithValidIdAndProblem_ShouldUpdateProblem()
        {
            // Arrange
            var problemId = "123";
            var updatedProblem = new Problem
            {
                Id = problemId,
                Title = "Updated Problem",
                Description = "Updated Description",
                Difficulty = "Hard",
                Category = "Full Stack",
                IsActive = true
            };

            _mockProblemService.Setup(x => x.UpdateProblemAsync(problemId, It.IsAny<Problem>()))
                .ReturnsAsync(updatedProblem);

            // Act
            var result = await _problemsController.UpdateProblem(problemId, updatedProblem);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Problem>(okResult.Value);
            Assert.Equal(problemId, response.Id);
            Assert.Equal("Updated Problem", response.Title);

            _mockProblemService.Verify(x => x.UpdateProblemAsync(problemId, It.IsAny<Problem>()), Times.Once);
        }

        [Fact]
        public async Task UpdateProblem_WithNullProblem_ShouldReturnBadRequest()
        {
            // Arrange
            var problemId = "123";

            // Act
            var result = await _problemsController.UpdateProblem(problemId, null!);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequestResult.Value);
        }

        [Fact]
        public async Task DeleteProblem_WithValidId_ShouldDeleteProblem()
        {
            // Arrange
            var problemId = "123";

            _mockProblemService.Setup(x => x.DeleteProblemAsync(problemId))
                .ReturnsAsync(true);

            // Act
            var result = await _problemsController.DeleteProblem(problemId);

            // Assert
            var noContentResult = Assert.IsType<NoContentResult>(result);

            _mockProblemService.Verify(x => x.DeleteProblemAsync(problemId), Times.Once);
        }

        [Fact]
        public async Task DeleteProblem_WithInvalidId_ShouldReturnNotFound()
        {
            // Arrange
            var problemId = "nonexistent";

            _mockProblemService.Setup(x => x.DeleteProblemAsync(problemId))
                .ReturnsAsync(false);

            // Act
            var result = await _problemsController.DeleteProblem(problemId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.NotNull(notFoundResult.Value);
        }

        [Fact]
        public async Task GetCategories_ShouldReturnAllCategories()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category
                {
                    Key = "frontend",
                    Name = "Frontend Development",
                    Icon = "💻",
                    Color = "#4CAF50",
                    ProblemCount = 10,
                    EasyCount = 4,
                    MediumCount = 4,
                    HardCount = 2,
                    IsActive = true
                },
                new Category
                {
                    Key = "backend",
                    Name = "Backend Development",
                    Icon = "⚙️",
                    Color = "#2196F3",
                    ProblemCount = 8,
                    EasyCount = 3,
                    MediumCount = 3,
                    HardCount = 2,
                    IsActive = true
                }
            };

            _mockProblemService.Setup(x => x.GetAllCategoriesAsync())
                .ReturnsAsync(categories);

            // Act
            var result = await _problemsController.GetCategories();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<List<Category>>(okResult.Value);
            Assert.Equal(2, response.Count);
            Assert.All(response, category => Assert.True(category.IsActive));
        }

        [Fact]
        public async Task CreateCategory_WithValidCategory_ShouldCreateCategory()
        {
            // Arrange
            var newCategory = new Category
            {
                Key = "test",
                Name = "Test Category",
                Icon = "🧪",
                Color = "#FF9800",
                ProblemCount = 0,
                EasyCount = 0,
                MediumCount = 0,
                HardCount = 0
            };

            var createdCategory = new Category
            {
                Id = "123",
                Key = "test",
                Name = "Test Category",
                Icon = "🧪",
                Color = "#FF9800",
                ProblemCount = 0,
                EasyCount = 0,
                MediumCount = 0,
                HardCount = 0,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _mockProblemService.Setup(x => x.CreateCategoryAsync(It.IsAny<Category>()))
                .ReturnsAsync(createdCategory);

            // Act
            var result = await _problemsController.CreateCategory(newCategory);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            var response = Assert.IsType<Category>(createdAtActionResult.Value);
            Assert.Equal("test", response.Key);
            Assert.Equal("Test Category", response.Name);
            Assert.True(response.IsActive);

            _mockProblemService.Verify(x => x.CreateCategoryAsync(It.IsAny<Category>()), Times.Once);
        }

        [Fact]
        public async Task CreateTestProblem_ShouldCreateTestProblem()
        {
            // Arrange
            var testProblem = new Problem
            {
                Id = "123",
                ProblemNumber = 26,
                Title = "Test Problem " + DateTime.UtcNow.ToString("HH:mm:ss"),
                Description = "This is a test problem created at " + DateTime.UtcNow,
                Difficulty = "Easy",
                Category = "Frontend Development",
                Tags = new List<string> { "Test", "Auto-Increment" },
                AcceptanceRate = 100,
                Likes = 0,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _mockProblemService.Setup(x => x.CreateProblemAsync(It.IsAny<Problem>()))
                .ReturnsAsync(testProblem);

            // Act
            var result = await _problemsController.CreateTestProblem();

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            var response = Assert.IsType<Problem>(createdAtActionResult.Value);
            Assert.Equal(26, response.ProblemNumber);
            Assert.Contains("Test Problem", response.Title);
            Assert.Equal("Easy", response.Difficulty);
            Assert.Equal("Frontend Development", response.Category);

            _mockProblemService.Verify(x => x.CreateProblemAsync(It.IsAny<Problem>()), Times.Once);
        }
    }
}
