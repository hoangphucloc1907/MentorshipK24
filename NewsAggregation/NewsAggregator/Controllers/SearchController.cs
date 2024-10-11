using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewsAggregator.Entity;
using NewsAggregator.Service;
using System.Data.SqlClient;

namespace NewsAggregator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly SearchService _searchService;

        public SearchController(IConfiguration configuration, SearchService searchService)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new ArgumentNullException(nameof(configuration), "Connection string cannot be null");
            _searchService = searchService;
        }

        [HttpGet("search")]
        public async Task<ActionResult> Search(string searchTerm)
        {
            var posts = await GetPosts(searchTerm);
            var tags = await GetTags(searchTerm);

            var matchedPosts = _searchService.SearchPostsByTitle(searchTerm, posts);
            var matchedTags = _searchService.SearchTagsByName(searchTerm, tags);

            var response = new
            {
                Posts = matchedPosts.Select(post => new
                {
                    post.Id,
                    post.Title,
                    post.ViewCount,
                    post.Upvote
                }),
                Tags = matchedTags.Select(tag => new
                {
                    tag.Id,
                    tag.TagName
                })
            };

            return Ok(response);
        }

        private async Task<List<Post>> GetPosts(string searchTerm)
        {
            await using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand("SELECT Id, Title FROM Post WHERE Title LIKE @searchTerm", connection);
                command.Parameters.AddWithValue("@searchTerm", $"%{searchTerm}%");

                await using var reader = await command.ExecuteReaderAsync();
                var posts = new List<Post>(reader.FieldCount);

                while (await reader.ReadAsync())
                {
                    posts.Add(new Post
                    {
                        Id = reader.GetInt32(0),
                        Title = reader.GetString(1)
                    });
                }

                return posts;
            }
        }

        private async Task<List<Tag>> GetTags(string searchTerm)
        {
            var tags = new List<Tag>();

            await using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand("SELECT Id, TagName FROM Tag WHERE TagName LIKE @searchTerm", connection);
                command.Parameters.AddWithValue("@searchTerm", $"%{searchTerm}%");

                await using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    tags.Add(new Tag
                    {
                        Id = reader.GetInt32(0),
                        TagName = reader.GetString(1)
                    });
                }
            }

            return tags;
        }
    }
}
