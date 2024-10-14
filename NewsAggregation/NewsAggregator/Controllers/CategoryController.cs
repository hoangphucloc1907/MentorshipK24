using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewsAggregator.Service;

namespace NewsAggregator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly CategoryService _categoryService;

        public CategoryController(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            _categoryService = new CategoryService(connectionString);
        }

        [HttpPost("followCategory")]
        public async Task<IActionResult> FollowCategory([FromBody] FollowCategoryRequest request)
        {
            if (request == null || request.UserId <= 0 || request.CategoryId <= 0)
            {
                return BadRequest("Invalid follow category data.");
            }

            await _categoryService.FollowCategoryAsync(request.UserId, request.CategoryId);
            return Ok("Category followed successfully.");
        }

        [HttpPost("unfollowCategory")]
        public async Task<IActionResult> UnfollowCategory([FromBody] FollowCategoryRequest request)
        {
            if (request == null || request.UserId <= 0 || request.CategoryId <= 0)
            {
                return BadRequest("Invalid unfollow category data.");
            }

            await _categoryService.UnfollowCategoryAsync(request.UserId, request.CategoryId);
            return Ok("Category unfollowed successfully.");
        }
    }

    public class FollowCategoryRequest
    {
        public int UserId { get; set; }
        public int CategoryId { get; set; }
    }
}
