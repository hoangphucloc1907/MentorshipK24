using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewsAggregator.Entity;
using NewsAggregator.Service;
using System.Data.SqlClient;

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
        public async Task<ActionResult<IEnumerable<TagGroup>>> GetTags()
        {
            var tags = await _tagService.GetTagsAsync();

            var groupedTags = tags
                .GroupBy(tag => tag.TagName[0])
                .Select(group => new TagGroup
                {
                    Initial = group.Key,
                    Tags = group.ToList()
                })
                .OrderBy(group => group.Initial)
                .ToList();

            return Ok(groupedTags);
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
    }


    public class TagGroup
    {
        public char Initial { get; set; }
        public List<Tag> Tags { get; set; }
    }
}
