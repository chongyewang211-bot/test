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

        [HttpPost("register")]
        public async Task<ActionResult<LoginResponse>> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var userExists = await _userService.UserExistsAsync(request.Username);
                if (userExists)
                {
                    return BadRequest(new { message = "Username already exists" });
                }

                var user = await _userService.CreateUserAsync(request.Username, request.Email, request.Password);
                
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
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating user", error = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
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
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Database connection error. Please check MongoDB connection.", error = ex.Message });
            }
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
    }
}
