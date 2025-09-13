using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using System.Text.Json;
using test.Models;

namespace test.Tests.Integration
{
    public class ApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public ApiIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task Login_WithValidCredentials_ShouldReturnToken()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Username = "test",
                Password = "test"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

            // Assert
            response.EnsureSuccessStatusCode();
            var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
            Assert.NotNull(loginResponse);
            Assert.NotEmpty(loginResponse.Token);
            Assert.Equal("test", loginResponse.Username);
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ShouldReturnUnauthorized()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Username = "invalid",
                Password = "invalid"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetProblems_WithoutAuthentication_ShouldReturnUnauthorized()
        {
            // Act
            var response = await _client.GetAsync("/api/problems");

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetCategories_WithoutAuthentication_ShouldReturnUnauthorized()
        {
            // Act
            var response = await _client.GetAsync("/api/problems/categories");

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetProblems_WithAuthentication_ShouldReturnProblems()
        {
            // Arrange - First login to get token
            var loginRequest = new LoginRequest
            {
                Username = "test",
                Password = "test"
            };

            var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
            var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();
            
            // Add authorization header
            _client.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResult!.Token);

            // Act
            var response = await _client.GetAsync("/api/problems");

            // Assert
            response.EnsureSuccessStatusCode();
            var problems = await response.Content.ReadFromJsonAsync<List<Problem>>();
            Assert.NotNull(problems);
            Assert.NotEmpty(problems);
        }

        [Fact]
        public async Task GetCategories_WithAuthentication_ShouldReturnCategories()
        {
            // Arrange - First login to get token
            var loginRequest = new LoginRequest
            {
                Username = "test",
                Password = "test"
            };

            var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
            var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();
            
            // Add authorization header
            _client.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResult!.Token);

            // Act
            var response = await _client.GetAsync("/api/problems/categories");

            // Assert
            response.EnsureSuccessStatusCode();
            var categories = await response.Content.ReadFromJsonAsync<List<Category>>();
            Assert.NotNull(categories);
            Assert.NotEmpty(categories);
        }

        [Fact]
        public async Task GetProblems_WithCategoryFilter_ShouldReturnFilteredProblems()
        {
            // Arrange - First login to get token
            var loginRequest = new LoginRequest
            {
                Username = "test",
                Password = "test"
            };

            var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
            var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();
            
            // Add authorization header
            _client.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResult!.Token);

            // Act
            var response = await _client.GetAsync("/api/problems?category=Frontend%20Development");

            // Assert
            response.EnsureSuccessStatusCode();
            var problems = await response.Content.ReadFromJsonAsync<List<Problem>>();
            Assert.NotNull(problems);
            
            // All returned problems should be in the specified category
            Assert.All(problems, problem => 
                Assert.Equal("Frontend Development", problem.Category));
        }

        [Fact]
        public async Task CreateTestProblem_WithAuthentication_ShouldCreateProblem()
        {
            // Arrange - First login to get token
            var loginRequest = new LoginRequest
            {
                Username = "test",
                Password = "test"
            };

            var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
            var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();
            
            // Add authorization header
            _client.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResult!.Token);

            // Act
            var response = await _client.PostAsync("/api/problems/test-create", null);

            // Assert
            response.EnsureSuccessStatusCode();
            var createdProblem = await response.Content.ReadFromJsonAsync<Problem>();
            Assert.NotNull(createdProblem);
            Assert.Contains("Test Problem", createdProblem.Title);
            Assert.Equal("Easy", createdProblem.Difficulty);
            Assert.Equal("Frontend Development", createdProblem.Category);
        }

        [Fact]
        public async Task CreateProblem_WithAuthentication_ShouldCreateProblemWithAutoIncrement()
        {
            // Arrange - First login to get token
            var loginRequest = new LoginRequest
            {
                Username = "test",
                Password = "test"
            };

            var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
            var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();
            
            // Add authorization header
            _client.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResult!.Token);

            var newProblem = new Problem
            {
                Title = "Integration Test Problem",
                Description = "Created during integration test",
                Difficulty = "Medium",
                Category = "Backend Development",
                Tags = new List<string> { "Integration", "Test" },
                AcceptanceRate = 75,
                Likes = 0
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/problems", newProblem);

            // Assert
            response.EnsureSuccessStatusCode();
            var createdProblem = await response.Content.ReadFromJsonAsync<Problem>();
            Assert.NotNull(createdProblem);
            Assert.Equal("Integration Test Problem", createdProblem.Title);
            Assert.Equal("Medium", createdProblem.Difficulty);
            Assert.Equal("Backend Development", createdProblem.Category);
            Assert.True(createdProblem.ProblemNumber > 0); // Should have auto-assigned number
            Assert.True(createdProblem.IsActive);
        }

        [Fact]
        public async Task HealthCheck_ShouldReturnOk()
        {
            // Act
            var response = await _client.GetAsync("/health");

            // Assert
            response.EnsureSuccessStatusCode();
        }
    }
}
