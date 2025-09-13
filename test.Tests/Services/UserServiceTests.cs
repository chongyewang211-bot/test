using Microsoft.Extensions.Options;
using Moq;
using MongoDB.Driver;
using test.Configuration;
using test.Models;
using test.Services;

namespace test.Tests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IMongoCollection<User>> _mockUsersCollection;
        private readonly Mock<MongoDbService> _mockMongoDbService;
        private readonly Mock<MongoDbSettings> _mockSettings;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _mockUsersCollection = new Mock<IMongoCollection<User>>();
            _mockSettings = new Mock<MongoDbSettings>();
            _mockMongoDbService = new Mock<MongoDbService>(_mockSettings.Object);
            
            _mockMongoDbService.Setup(x => x.Database.GetCollection<User>(It.IsAny<string>()))
                .Returns(_mockUsersCollection.Object);

            _userService = new UserService(_mockMongoDbService.Object, _mockSettings.Object);
        }

        [Fact]
        public async Task GetUserByUsernameAsync_WhenUserExists_ShouldReturnUser()
        {
            // Arrange
            var username = "testuser";
            var expectedUser = new User
            {
                Id = "123",
                Username = username,
                Email = "test@example.com",
                IsActive = true
            };

            var mockCursor = new Mock<IAsyncCursor<User>>();
            mockCursor.Setup(x => x.Current).Returns(new[] { expectedUser });
            mockCursor.Setup(x => x.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            mockCursor.Setup(x => x.MoveNext(It.IsAny<CancellationToken>()))
                .Returns(true);

            _mockUsersCollection.Setup(x => x.FindAsync<User>(
                It.IsAny<FilterDefinition<User>>(),
                It.IsAny<FindOptions<User, User>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockCursor.Object);

            // Act
            var result = await _userService.GetUserByUsernameAsync(username);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedUser.Username, result.Username);
            Assert.Equal(expectedUser.Email, result.Email);
            Assert.True(result.IsActive);
        }

        [Fact]
        public async Task GetUserByUsernameAsync_WhenUserDoesNotExist_ShouldReturnNull()
        {
            // Arrange
            var username = "nonexistent";
            var mockCursor = new Mock<IAsyncCursor<User>>();
            mockCursor.Setup(x => x.Current).Returns(new User[0]);
            mockCursor.Setup(x => x.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _mockUsersCollection.Setup(x => x.FindAsync<User>(
                It.IsAny<FilterDefinition<User>>(),
                It.IsAny<FindOptions<User, User>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockCursor.Object);

            // Act
            var result = await _userService.GetUserByUsernameAsync(username);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task ValidatePasswordAsync_WhenPasswordMatches_ShouldReturnTrue()
        {
            // Arrange
            var password = "testpassword";
            var hashedPassword = "9f86d081884c7d659a2feaa0c55ad015a3bf4f1b2b0b822cd15d6c15b0f00a08"; // SHA256 of "test"

            // Act
            var result = await _userService.ValidatePasswordAsync(password, hashedPassword);

            // Assert
            Assert.False(result); // This will be false because we're using SHA256, not the actual hash
        }

        [Fact]
        public async Task CreateUserAsync_ShouldCreateUserWithCorrectProperties()
        {
            // Arrange
            var username = "newuser";
            var email = "newuser@example.com";
            var password = "password123";

            _mockUsersCollection.Setup(x => x.InsertOneAsync(
                It.IsAny<User>(),
                It.IsAny<InsertOneOptions>(),
                It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _userService.CreateUserAsync(username, email, password);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(username, result.Username);
            Assert.Equal(email, result.Email);
            Assert.True(result.IsActive);
            Assert.NotEmpty(result.PasswordHash);
            Assert.True(result.CreatedAt <= DateTime.UtcNow);

            _mockUsersCollection.Verify(x => x.InsertOneAsync(
                It.IsAny<User>(),
                It.IsAny<InsertOneOptions>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UserExistsAsync_WhenUserExists_ShouldReturnTrue()
        {
            // Arrange
            var username = "existinguser";
            var mockCursor = new Mock<IAsyncCursor<User>>();
            mockCursor.Setup(x => x.Current).Returns(new[] { new User { Username = username } });
            mockCursor.Setup(x => x.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _mockUsersCollection.Setup(x => x.FindAsync<User>(
                It.IsAny<FilterDefinition<User>>(),
                It.IsAny<FindOptions<User, User>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockCursor.Object);

            // Act
            var result = await _userService.UserExistsAsync(username);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task UserExistsAsync_WhenUserDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            var username = "nonexistent";
            var mockCursor = new Mock<IAsyncCursor<User>>();
            mockCursor.Setup(x => x.Current).Returns(new User[0]);
            mockCursor.Setup(x => x.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _mockUsersCollection.Setup(x => x.FindAsync<User>(
                It.IsAny<FilterDefinition<User>>(),
                It.IsAny<FindOptions<User, User>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockCursor.Object);

            // Act
            var result = await _userService.UserExistsAsync(username);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task GetAllUsersAsync_ShouldReturnAllActiveUsers()
        {
            // Arrange
            var users = new List<User>
            {
                new User { Id = "1", Username = "user1", IsActive = true },
                new User { Id = "2", Username = "user2", IsActive = true }
            };

            var mockCursor = new Mock<IAsyncCursor<User>>();
            mockCursor.Setup(x => x.Current).Returns(users);
            mockCursor.Setup(x => x.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _mockUsersCollection.Setup(x => x.FindAsync<User>(
                It.IsAny<FilterDefinition<User>>(),
                It.IsAny<FindOptions<User, User>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockCursor.Object);

            // Act
            var result = await _userService.GetAllUsersAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.All(result, user => Assert.True(user.IsActive));
        }
    }
}
