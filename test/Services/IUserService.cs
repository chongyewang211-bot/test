using test.Models;

namespace test.Services
{
    public interface IUserService
    {
        Task<User?> GetUserByUsernameAsync(string username);
        Task<bool> ValidatePasswordAsync(string password, string hashedPassword);
        Task<User> CreateUserAsync(string username, string email, string password);
        Task<bool> UserExistsAsync(string username);
        Task<List<User>> GetAllUsersAsync();
    }
}
