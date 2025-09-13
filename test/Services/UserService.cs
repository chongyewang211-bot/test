using System.Security.Cryptography;
using System.Text;
using MongoDB.Driver;
using test.Configuration;
using test.Models;

namespace test.Services
{
    public class UserService : IUserService
    {
        private readonly IMongoCollection<User> _users;

        public UserService(MongoDbService mongoDbService, MongoDbSettings settings)
        {
            _users = mongoDbService.Database.GetCollection<User>(settings.UsersCollectionName);
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _users.Find(u => u.Username == username && u.IsActive).FirstOrDefaultAsync();
        }

        public async Task<bool> ValidatePasswordAsync(string password, string passwordHash)
        {
            var hashedPassword = HashPassword(password);
            return hashedPassword == passwordHash;
        }

        public async Task<User> CreateUserAsync(string username, string email, string password)
        {
            var user = new User
            {
                Username = username,
                Email = email,
                PasswordHash = HashPassword(password),
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _users.InsertOneAsync(user);
            return user;
        }

        public async Task<bool> UserExistsAsync(string username)
        {
            var user = await _users.Find(u => u.Username == username).FirstOrDefaultAsync();
            return user != null;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _users.Find(u => u.IsActive).ToListAsync();
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}