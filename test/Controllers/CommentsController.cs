using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using test.Models;
using test.Services;

namespace test.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentsController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<Comment>>> GetComments()
        {
            try
            {
                var comments = await _commentService.GetAllCommentsAsync();
                return Ok(comments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching comments", error = ex.Message });
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Comment>> CreateComment([FromBody] CreateCommentRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Content))
                {
                    return BadRequest(new { message = "Comment content is required" });
                }

                var username = User.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown";
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";

                var comment = await _commentService.CreateCommentAsync(request.Content, username, userId);
                return Ok(comment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the comment", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> DeleteComment(string id)
        {
            try
            {
                var result = await _commentService.DeleteCommentAsync(id);
                if (!result)
                {
                    return NotFound(new { message = "Comment not found" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the comment", error = ex.Message });
            }
        }
    }

    public class CreateCommentRequest
    {
        public string Content { get; set; } = string.Empty;
    }
}

