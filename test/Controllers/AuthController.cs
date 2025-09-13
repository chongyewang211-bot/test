using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using test.Models;
using test.Services;

namespace test.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;

        public AuthController(IUserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }

        [HttpPost("recreate-test-user")]
        public async Task<ActionResult> RecreateTestUser()
        {
            try
            {
                // Create test user with known password
                var testUser = await _userService.CreateUserAsync("test", "test@example.com", "test");
                var testHash = HashPassword("test");
                
                return Ok(new { 
                    message = "Test user recreated", 
                    username = testUser.Username,
                    email = testUser.Email,
                    passwordHash = testUser.PasswordHash,
                    expectedHash = testHash,
                    hashMatch = testUser.PasswordHash == testHash
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error recreating test user", error = ex.Message });
            }
        }

        [HttpPost("test-user")]
        public async Task<ActionResult> TestUser([FromBody] LoginRequest request)
        {
            try
            {
                var user = await _userService.GetUserByUsernameAsync(request.Username);
                if (user == null)
                {
                    return Ok(new { 
                        message = "User not found", 
                        username = request.Username,
                        usersInDb = await GetUserList()
                    });
                }

                var isValidPassword = await _userService.ValidatePasswordAsync(request.Password, user.PasswordHash);
                var hashedInput = HashPassword(request.Password);
                
                return Ok(new { 
                    message = "User found", 
                    username = user.Username,
                    email = user.Email,
                    isActive = user.IsActive,
                    storedHash = user.PasswordHash,
                    inputHash = hashedInput,
                    passwordValid = isValidPassword
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error testing user", error = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userService.GetUserByUsernameAsync(request.Username);
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }

            var isValidPassword = await _userService.ValidatePasswordAsync(request.Password, user.PasswordHash);
            if (!isValidPassword)
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }

            var token = GenerateJwtToken(user);
            var expiresAt = DateTime.UtcNow.AddHours(24);

            return Ok(new LoginResponse
            {
                Token = token,
                Username = user.Username,
                Email = user.Email,
                ExpiresAt = expiresAt
            });
        }

        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"] ?? "YourSuperSecretKeyThatIsAtLeast32CharactersLong!";
            var issuer = jwtSettings["Issuer"] ?? "TestApp";
            var audience = jwtSettings["Audience"] ?? "TestAppUsers";

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("sub", user.Id.ToString()),
                new Claim("jti", Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<List<string>> GetUserList()
        {
            // This is a simple method to get all usernames for debugging
            var users = await _userService.GetAllUsersAsync();
            return users.Select(u => u.Username).ToList();
        }

        private string HashPassword(string password)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}
