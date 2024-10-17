using Microsoft.AspNetCore.Mvc;
using NewsAggregator.Entity;
using NewsAggregator.Service;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsAggregator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagController : ControllerBase
    {
        private readonly TagService _tagService;

        public TagController(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            _tagService = new TagService(connectionString);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TagGroup>>> GetTags(int pageNumber, int pageSize)
        {
            var tags = await _tagService.GetTagsAsync(pageNumber, pageSize);

            var groupedTags = new Dictionary<char, List<Tag>>();

            foreach (var tag in tags)
            {
                var initial = tag.TagName[0];

                if (!groupedTags.ContainsKey(initial))
                {
                    groupedTags[initial] = new List<Tag>();
                }

                groupedTags[initial].Add(tag);
            }

            var sortedTags = groupedTags.OrderBy(group => group.Key)
                                        .Select(group => new TagGroup
                                        {
                                            Initial = group.Key,
                                            Tags = group.Value
                                        })
                                        .ToList();

            return Ok(sortedTags);
        }

        [HttpGet("popular")]
        public async Task<ActionResult<IEnumerable<Tag>>> GetPopularTags()
        {
            var tags = await _tagService.GetPopularTagsAsync();
            return Ok(tags);
        }

        [HttpGet("trending")]
        public async Task<ActionResult<IEnumerable<Tag>>> GetTrendingTags()
        {
            var tags = await _tagService.GetTrendingTagsAsync();
            return Ok(tags);
        }

        [HttpPost("follow")]
        public async Task<IActionResult> FollowTag([FromBody] FollowTagRequest request)
        {
            if (request == null || request.UserId <= 0 || request.TagId <= 0)
            {
                return BadRequest("Invalid follow tag data.");
            }

            await _tagService.FollowTagAsync(request.UserId, request.TagId);
            return Ok("Tag followed successfully.");
        }

        [HttpPost("unfollow")]
        public async Task<IActionResult> UnfollowTag([FromBody] FollowTagRequest request)
        {
            if (request == null || request.UserId <= 0 || request.TagId <= 0)
            {
                return BadRequest("Invalid unfollow tag data.");
            }

            await _tagService.UnfollowTagAsync(request.UserId, request.TagId);
            return Ok("Tag unfollowed successfully.");
        }

        
    }

    public class TagGroup
    {
        public char Initial { get; set; }
        public List<Tag> Tags { get; set; }
    }

    public class FollowTagRequest
    {
        public int UserId { get; set; }
        public int TagId { get; set; }
    }

    
}
