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
            var postsTask = GetPosts(searchTerm);
            var tagsTask = GetTags(searchTerm);

            await Task.WhenAll(postsTask, tagsTask);

            var matchedPosts = _searchService.SearchPostsByTitle(searchTerm, postsTask.Result);
            var matchedTags = _searchService.SearchTagsByName(searchTerm, tagsTask.Result);

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
            var posts = new List<Post>();

            await using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand("SELECT TOP 100 Id, Title, ViewCount, Upvote FROM Post WHERE Title LIKE @searchTerm", connection);
                command.Parameters.AddWithValue("@searchTerm", $"%{searchTerm}%");

                await using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    posts.Add(new Post
                    {
                        Id = reader.GetInt32(0),
                        Title = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                        ViewCount = reader.IsDBNull(2) ? 0 : reader.GetInt32(2),
                        Upvote = reader.IsDBNull(3) ? 0 : reader.GetInt32(3)
                    });
                }
            }

            return posts;
        }

        private async Task<List<Tag>> GetTags(string searchTerm)
        {
            var tags = new List<Tag>();

            await using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand("SELECT TOP 100 Id, TagName FROM Tag WHERE TagName LIKE @searchTerm", connection);
                command.Parameters.AddWithValue("@searchTerm", $"%{searchTerm}%");

                await using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    tags.Add(new Tag
                    {
                        Id = reader.GetInt32(0),
                        TagName = reader.IsDBNull(1) ? string.Empty : reader.GetString(1)
                    });
                }
            }

            return tags;
        }
    }
}
