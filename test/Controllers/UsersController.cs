using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using test.Services;

namespace test.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("online")]
        [Authorize]
        public async Task<ActionResult> GetOnlineUsers()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                var onlineUsers = users.Select(u => new
                {
                    id = u.Id,
                    username = u.Username,
                    email = u.Email,
                    isActive = u.IsActive
                }).ToList();

                return Ok(onlineUsers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching users", error = ex.Message });
            }
        }
    }
}

