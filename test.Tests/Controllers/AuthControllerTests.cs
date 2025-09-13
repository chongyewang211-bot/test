using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using test.Controllers;
using test.Models;
using test.Services;
using System.Security.Cryptography;
using System.Text;

namespace test.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly AuthController _authController;

        public AuthControllerTests()
        {
            _mockUserService = new Mock<IUserService>();
            _mockConfiguration = new Mock<IConfiguration>();
            _authController = new AuthController(_mockUserService.Object, _mockConfiguration.Object);
        }

        [Fact]
        public async Task Login_WithValidCredentials_ShouldReturnToken()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Username = "testuser",
                Password = "testpassword"
            };

            var user = new User
            {
                Id = "123",
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = HashPassword("testpassword"),
                IsActive = true
            };

            _mockUserService.Setup(x => x.GetUserByUsernameAsync(loginRequest.Username))
                .ReturnsAsync(user);
            _mockUserService.Setup(x => x.ValidatePasswordAsync(loginRequest.Password, user.PasswordHash))
                .ReturnsAsync(true);

            _mockConfiguration.Setup(x => x["Jwt:Key"]).Returns("YourSuperSecretKeyThatIsAtLeast32CharactersLong!");
            _mockConfiguration.Setup(x => x["Jwt:Issuer"]).Returns("TestIssuer");
            _mockConfiguration.Setup(x => x["Jwt:Audience"]).Returns("TestAudience");

            // Act
            var result = await _authController.Login(loginRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<LoginResponse>(okResult.Value);
            Assert.NotNull(response.Token);
            Assert.Equal(user.Username, response.Username);
            Assert.Equal(user.Email, response.Email);
        }

        [Fact]
        public async Task Login_WithInvalidUsername_ShouldReturnUnauthorized()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Username = "nonexistent",
                Password = "password"
            };

            _mockUserService.Setup(x => x.GetUserByUsernameAsync(loginRequest.Username))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _authController.Login(loginRequest);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            var response = Assert.IsType<object>(unauthorizedResult.Value);
            Assert.NotNull(response);
        }

        [Fact]
        public async Task Login_WithInvalidPassword_ShouldReturnUnauthorized()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Username = "testuser",
                Password = "wrongpassword"
            };

            var user = new User
            {
                Id = "123",
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = HashPassword("correctpassword"),
                IsActive = true
            };

            _mockUserService.Setup(x => x.GetUserByUsernameAsync(loginRequest.Username))
                .ReturnsAsync(user);
            _mockUserService.Setup(x => x.ValidatePasswordAsync(loginRequest.Password, user.PasswordHash))
                .ReturnsAsync(false);

            // Act
            var result = await _authController.Login(loginRequest);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            var response = Assert.IsType<object>(unauthorizedResult.Value);
            Assert.NotNull(response);
        }

        [Fact]
        public async Task Login_WithInactiveUser_ShouldReturnUnauthorized()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Username = "inactiveuser",
                Password = "password"
            };

            var user = new User
            {
                Id = "123",
                Username = "inactiveuser",
                Email = "inactive@example.com",
                PasswordHash = HashPassword("password"),
                IsActive = false // Inactive user
            };

            _mockUserService.Setup(x => x.GetUserByUsernameAsync(loginRequest.Username))
                .ReturnsAsync(user);

            // Act
            var result = await _authController.Login(loginRequest);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            var response = Assert.IsType<object>(unauthorizedResult.Value);
            Assert.NotNull(response);
        }

        [Fact]
        public async Task Login_WithNullLoginRequest_ShouldReturnBadRequest()
        {
            // Act
            var result = await _authController.Login(null!);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequestResult.Value);
        }

        [Fact]
        public async Task Login_WithEmptyUsername_ShouldReturnBadRequest()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Username = "",
                Password = "password"
            };

            // Act
            var result = await _authController.Login(loginRequest);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequestResult.Value);
        }

        [Fact]
        public async Task Login_WithEmptyPassword_ShouldReturnBadRequest()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Username = "testuser",
                Password = ""
            };

            // Act
            var result = await _authController.Login(loginRequest);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequestResult.Value);
        }

        [Fact]
        public async Task GetUserList_ShouldReturnAllUsers()
        {
            // Arrange
            var users = new List<User>
            {
                new User { Id = "1", Username = "user1", Email = "user1@example.com", IsActive = true },
                new User { Id = "2", Username = "user2", Email = "user2@example.com", IsActive = true }
            };

            _mockUserService.Setup(x => x.GetAllUsersAsync())
                .ReturnsAsync(users);

            // Act
            var result = await _authController.GetUserList();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<List<User>>(okResult.Value);
            Assert.Equal(2, response.Count);
        }

        [Fact]
        public async Task RecreateTestUser_ShouldCreateTestUser()
        {
            // Arrange
            _mockUserService.Setup(x => x.UserExistsAsync("test"))
                .ReturnsAsync(false);
            _mockUserService.Setup(x => x.CreateUserAsync("test", "test@example.com", "test"))
                .ReturnsAsync(new User
                {
                    Id = "123",
                    Username = "test",
                    Email = "test@example.com",
                    IsActive = true
                });

            // Act
            var result = await _authController.RecreateTestUser();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);

            _mockUserService.Verify(x => x.CreateUserAsync("test", "test@example.com", "test"), Times.Once);
        }

        [Fact]
        public async Task RecreateTestUser_WhenTestUserExists_ShouldStillCreateNewUser()
        {
            // Arrange
            _mockUserService.Setup(x => x.UserExistsAsync("test"))
                .ReturnsAsync(true);
            _mockUserService.Setup(x => x.CreateUserAsync("test", "test@example.com", "test"))
                .ReturnsAsync(new User
                {
                    Id = "123",
                    Username = "test",
                    Email = "test@example.com",
                    IsActive = true
                });

            // Act
            var result = await _authController.RecreateTestUser();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);

            _mockUserService.Verify(x => x.CreateUserAsync("test", "test@example.com", "test"), Times.Once);
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToHexString(hashedBytes);
        }
    }
}
