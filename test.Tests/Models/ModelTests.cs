using test.Models;

namespace test.Tests.Models
{
    public class ModelTests
    {
        [Fact]
        public void Problem_ShouldHaveCorrectProperties()
        {
            // Arrange & Act
            var problem = new Problem
            {
                Id = "123",
                ProblemNumber = 1,
                Title = "Test Problem",
                Description = "Test Description",
                Difficulty = "Easy",
                Category = "Frontend Development",
                Tags = new List<string> { "JavaScript", "HTML" },
                AcceptanceRate = 85,
                Likes = 150,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Assert
            Assert.Equal("123", problem.Id);
            Assert.Equal(1, problem.ProblemNumber);
            Assert.Equal("Test Problem", problem.Title);
            Assert.Equal("Test Description", problem.Description);
            Assert.Equal("Easy", problem.Difficulty);
            Assert.Equal("Frontend Development", problem.Category);
            Assert.Equal(2, problem.Tags.Count);
            Assert.Contains("JavaScript", problem.Tags);
            Assert.Contains("HTML", problem.Tags);
            Assert.Equal(85, problem.AcceptanceRate);
            Assert.Equal(150, problem.Likes);
            Assert.True(problem.IsActive);
            Assert.True(problem.CreatedAt <= DateTime.UtcNow);
            Assert.True(problem.UpdatedAt <= DateTime.UtcNow);
        }

        [Fact]
        public void Category_ShouldHaveCorrectProperties()
        {
            // Arrange & Act
            var category = new Category
            {
                Id = "456",
                Key = "frontend",
                Name = "Frontend Development",
                Icon = "💻",
                Color = "#4CAF50",
                ProblemCount = 25,
                EasyCount = 10,
                MediumCount = 10,
                HardCount = 5,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Assert
            Assert.Equal("456", category.Id);
            Assert.Equal("frontend", category.Key);
            Assert.Equal("Frontend Development", category.Name);
            Assert.Equal("💻", category.Icon);
            Assert.Equal("#4CAF50", category.Color);
            Assert.Equal(25, category.ProblemCount);
            Assert.Equal(10, category.EasyCount);
            Assert.Equal(10, category.MediumCount);
            Assert.Equal(5, category.HardCount);
            Assert.True(category.IsActive);
            Assert.True(category.CreatedAt <= DateTime.UtcNow);
            Assert.True(category.UpdatedAt <= DateTime.UtcNow);
        }

        [Fact]
        public void User_ShouldHaveCorrectProperties()
        {
            // Arrange & Act
            var user = new User
            {
                Id = "789",
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = "hashedpassword",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Assert
            Assert.Equal("789", user.Id);
            Assert.Equal("testuser", user.Username);
            Assert.Equal("test@example.com", user.Email);
            Assert.Equal("hashedpassword", user.PasswordHash);
            Assert.True(user.IsActive);
            Assert.True(user.CreatedAt <= DateTime.UtcNow);
            Assert.True(user.UpdatedAt <= DateTime.UtcNow);
        }

        [Fact]
        public void LoginRequest_ShouldHaveCorrectProperties()
        {
            // Arrange & Act
            var loginRequest = new LoginRequest
            {
                Username = "testuser",
                Password = "testpassword"
            };

            // Assert
            Assert.Equal("testuser", loginRequest.Username);
            Assert.Equal("testpassword", loginRequest.Password);
        }

        [Fact]
        public void LoginResponse_ShouldHaveCorrectProperties()
        {
            // Arrange & Act
            var loginResponse = new LoginResponse
            {
                Token = "jwt-token-string",
                Username = "testuser",
                Email = "test@example.com"
            };

            // Assert
            Assert.Equal("jwt-token-string", loginResponse.Token);
            Assert.Equal("testuser", loginResponse.Username);
            Assert.Equal("test@example.com", loginResponse.Email);
        }

        [Fact]
        public void Problem_ShouldHandleNullTags()
        {
            // Arrange & Act
            var problem = new Problem
            {
                Id = "123",
                ProblemNumber = 1,
                Title = "Test Problem",
                Tags = null
            };

            // Assert
            Assert.Null(problem.Tags);
        }

        [Fact]
        public void Problem_ShouldHandleEmptyTags()
        {
            // Arrange & Act
            var problem = new Problem
            {
                Id = "123",
                ProblemNumber = 1,
                Title = "Test Problem",
                Tags = new List<string>()
            };

            // Assert
            Assert.NotNull(problem.Tags);
            Assert.Empty(problem.Tags);
        }

        [Fact]
        public void Category_ShouldCalculateTotalCorrectly()
        {
            // Arrange & Act
            var category = new Category
            {
                EasyCount = 5,
                MediumCount = 3,
                HardCount = 2,
                ProblemCount = 10
            };

            // Assert
            var total = category.EasyCount + category.MediumCount + category.HardCount;
            Assert.Equal(10, total);
            Assert.Equal(category.ProblemCount, total);
        }

        [Fact]
        public void Problem_ShouldSetDefaultValues()
        {
            // Arrange & Act
            var problem = new Problem
            {
                Id = "123",
                ProblemNumber = 1,
                Title = "Test Problem"
            };

            // Assert
            Assert.Equal("123", problem.Id);
            Assert.Equal(1, problem.ProblemNumber);
            Assert.Equal("Test Problem", problem.Title);
            Assert.Null(problem.Description);
            Assert.Null(problem.Difficulty);
            Assert.Null(problem.Category);
            Assert.Null(problem.Tags);
            Assert.Equal(0, problem.AcceptanceRate);
            Assert.Equal(0, problem.Likes);
            Assert.False(problem.IsActive); // Default value
        }

        [Fact]
        public void Category_ShouldSetDefaultValues()
        {
            // Arrange & Act
            var category = new Category
            {
                Id = "456",
                Key = "test",
                Name = "Test Category"
            };

            // Assert
            Assert.Equal("456", category.Id);
            Assert.Equal("test", category.Key);
            Assert.Equal("Test Category", category.Name);
            Assert.Null(category.Icon);
            Assert.Null(category.Color);
            Assert.Equal(0, category.ProblemCount);
            Assert.Equal(0, category.EasyCount);
            Assert.Equal(0, category.MediumCount);
            Assert.Equal(0, category.HardCount);
            Assert.False(category.IsActive); // Default value
        }

        [Fact]
        public void User_ShouldSetDefaultValues()
        {
            // Arrange & Act
            var user = new User
            {
                Id = "789",
                Username = "testuser"
            };

            // Assert
            Assert.Equal("789", user.Id);
            Assert.Equal("testuser", user.Username);
            Assert.Null(user.Email);
            Assert.Null(user.PasswordHash);
            Assert.False(user.IsActive); // Default value
        }
    }
}
