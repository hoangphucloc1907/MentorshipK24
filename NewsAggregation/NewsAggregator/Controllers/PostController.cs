using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewsAggregator.Entity;
using System.Data.SqlClient;

namespace NewsAggregator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly string _connectionString;

        public PostController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        [HttpGet("popular")]
        public async Task<ActionResult<IEnumerable<Post>>> GetPopularPosts()
        {
            var posts = new List<Post>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand("SELECT TOP 10 Id, Title, ViewCount " +
                    "FROM Post " +
                    "ORDER BY ViewCount DESC", connection);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        posts.Add(new Post
                        {
                            Id = reader.GetInt32(0),
                            Title = reader.GetString(1),
                            ViewCount = reader.GetInt32(2)
                        });
                    }
                }
            }

            return Ok(posts);
        }

        [HttpGet("upvoted")]
        public async Task<ActionResult<IEnumerable<Post>>> GetTopUpvotedPosts()
        {
            var posts = new List<Post>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand("SELECT TOP 10 Id, Title, Upvote " +
                    "FROM Post " +
                    "ORDER BY Upvote DESC", connection);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        posts.Add(new Post
                        {
                            Id = reader.GetInt32(0),
                            Title = reader.GetString(1),
                            Upvote = reader.GetInt32(2)
                        });
                    }
                }
            }

            return Ok(posts);
        }
    }
}
