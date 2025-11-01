using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using test.Models;
using test.Services;

namespace test.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProblemsController : ControllerBase
    {
        private readonly IProblemService _problemService;

        public ProblemsController(IProblemService problemService)
        {
            _problemService = problemService;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<Problem>>> GetProblems([FromQuery] string? category = null)
        {
            try
            {
                List<Problem> problems;
                if (string.IsNullOrEmpty(category) || category.ToLower() == "all")
                {
                    problems = await _problemService.GetAllProblemsAsync();
                }
                else
                {
                    problems = await _problemService.GetProblemsByCategoryAsync(category);
                }

                return Ok(problems);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching problems", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Problem>> GetProblem(string id)
        {
            try
            {
                var problem = await _problemService.GetProblemByIdAsync(id);
                if (problem == null)
                {
                    return NotFound(new { message = "Problem not found" });
                }

                return Ok(problem);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching the problem", error = ex.Message });
            }
        }

        [HttpGet("categories")]
        [Authorize]
        public async Task<ActionResult<List<Category>>> GetCategories()
        {
            try
            {
                var categories = await _problemService.GetAllCategoriesAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching categories", error = ex.Message });
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Problem>> CreateProblem([FromBody] Problem problem)
        {
            try
            {
                var createdProblem = await _problemService.CreateProblemAsync(problem);
                return CreatedAtAction(nameof(GetProblem), new { id = createdProblem.Id }, createdProblem);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the problem", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<Problem>> UpdateProblem(string id, [FromBody] Problem problem)
        {
            try
            {
                var updatedProblem = await _problemService.UpdateProblemAsync(id, problem);
                return Ok(updatedProblem);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the problem", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> DeleteProblem(string id)
        {
            try
            {
                var result = await _problemService.DeleteProblemAsync(id);
                if (!result)
                {
                    return NotFound(new { message = "Problem not found" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the problem", error = ex.Message });
            }
        }
    }
}
