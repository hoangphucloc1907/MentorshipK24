using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewsAggregator.Entity;
using System.Data.SqlClient;

namespace NewsAggregator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagController : ControllerBase
    {
        private readonly string _connectionString;

        public TagController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TagGroup>>> GetTags()
        {
            var tags = new List<Tag>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand("SELECT Id, TagName FROM Tag ORDER BY TagName", connection);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        tags.Add(new Tag
                        {
                            Id = reader.GetInt32(0),
                            TagName = reader.GetString(1)
                        });
                    }
                }
            }

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

        // API for Popular Tags
        [HttpGet("popular")]
        public async Task<ActionResult<IEnumerable<Tag>>> GetPopularTags()
        {
            var tags = new List<Tag>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                // Query to get popular tags based on total number of followers
                var command = new SqlCommand(@"
                    SELECT T.Id, T.TagName, COUNT(UFT.UserId) AS FollowerCount
                    FROM Tag T
                    JOIN UserFollowTag UFT ON T.Id = UFT.TagId
                    GROUP BY T.Id, T.TagName
                    ORDER BY FollowerCount DESC", connection);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        tags.Add(new Tag
                        {
                            Id = reader.GetInt32(0),
                            TagName = reader.GetString(1)
                        });
                    }
                }
            }

            return Ok(tags);
        }

        // API for Trending Tags (e.g., in the last 7 days)
        [HttpGet("trending")]
        public async Task<ActionResult<IEnumerable<Tag>>> GetTrendingTags()
        {
            var tags = new List<Tag>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                // Query to get trending tags based on follows in the last 7 days
                var command = new SqlCommand(@"
                    SELECT T.Id, T.TagName, COUNT(UFT.UserId) AS FollowerCount
                    FROM Tag T
                    JOIN UserFollowTag UFT ON T.Id = UFT.TagId
                    WHERE UFT.FollowDate >= DATEADD(day, -7, GETDATE())
                    GROUP BY T.Id, T.TagName
                    ORDER BY FollowerCount DESC", connection);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        tags.Add(new Tag
                        {
                            Id = reader.GetInt32(0),
                            TagName = reader.GetString(1)
                        });
                    }
                }
            }

            return Ok(tags);
        }
    }


    public class TagGroup
    {
        public char Initial { get; set; }
        public List<Tag> Tags { get; set; }
    }
}
