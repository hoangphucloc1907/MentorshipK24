using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NewsAggregator.Entity;
using System.Collections.Generic;
using System.Data.SqlClient;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NewsAggregator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly string _connectionString;

        public UserController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new ArgumentNullException(nameof(configuration), "Connection string cannot be null");
        }

        // GET: api/<UserController>
        [HttpGet]
        public IEnumerable<User> GetAllUser()
        {
            var result = new List<User>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var cmdQuery = "SELECT Id, Email, Username FROM Users";

                using (var command = new SqlCommand(cmdQuery, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new User
                        {
                            Id = reader.GetInt32(0),
                            Email = reader.GetString(1),
                            Username = reader.GetString(2),
                        });
                    }
                }
            }

            return result;
        }

        // GET api/<UserController>/5
        [HttpGet("{id}")]
        public ActionResult<User> GetUserById(int id)
        {
            User? result = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var cmdQuery = "SELECT Id, Email, Username FROM Users WHERE Id = @Id";

                using (var command = new SqlCommand(cmdQuery, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            result = new User
                            {
                                Id = reader.GetInt32(0),
                                Email = reader.GetString(1),
                                Username = reader.GetString(2),
                            };
                        }
                    }
                }
            }

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        // POST api/<UserController>
        [HttpPost]
        public IActionResult Insert([FromBody] User user)
        {
            if (user == null || string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Username))
            {
                return BadRequest("Invalid user data.");
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var cmdQuery = "INSERT INTO Users (Email, Username) VALUES (@Email, @Username)";

                using (var command = new SqlCommand(cmdQuery, connection))
                {
                    command.Parameters.AddWithValue("@Email", user.Email);
                    command.Parameters.AddWithValue("@Username", user.Username);
                    command.ExecuteNonQuery();
                }
            }

            return Ok("User inserted successfully.");
        }

        // PUT api/<UserController>/5
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] User user)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var cmdQuery = "UPDATE Users SET Username = @Username WHERE Id = @Id";

                using (var command = new SqlCommand(cmdQuery, connection))
                {
                    command.Parameters.AddWithValue("@Username", user.Username);
                    command.Parameters.AddWithValue("@Id", id);

                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected == 0)
                    {
                        return NotFound("User not found.");
                    }
                }
            }

            return Ok("User updated successfully.");
        }

        // DELETE api/<UserController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var cmdQuery = "DELETE FROM Users WHERE Id = @Id";

                using (var command = new SqlCommand(cmdQuery, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected == 0)
                    {
                        return NotFound("User not found.");
                    }
                }
            }
            return Ok("User deleted successfully.");
        }

        [HttpGet("posts/{userId}")]
        public async Task<ActionResult<IEnumerable<Post>>> GetPostsByUserFollow(int userId, int pageNumber = 1, int pageSize = 10)
        {
            var posts = new List<Post>();

            await using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var cmdQuery = @"
                                SELECT DISTINCT p.Id, p.CategoryId, p.Title, p.Link, p.Guid, p.Pubdate, p.Image
                                FROM Post p
                                INNER JOIN PostTag pt ON p.Id = pt.PostId
                                INNER JOIN UserFollowTag uft ON pt.TagId = uft.TagId
                                WHERE uft.UserId = @UserId
                                UNION
                                SELECT DISTINCT p.Id, p.CategoryId, p.Title, p.Link, p.Guid, p.Pubdate, p.Image
                                FROM Post p
                                INNER JOIN UserFollowCategory ufc ON p.CategoryId = ufc.CategoryId
                                WHERE ufc.UserId = @UserId
                                ORDER BY Pubdate DESC
                                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                await using (var command = new SqlCommand(cmdQuery, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    command.Parameters.AddWithValue("@Offset", (pageNumber - 1) * pageSize);
                    command.Parameters.AddWithValue("@PageSize", pageSize);

                    await using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            posts.Add(new Post
                            {
                                Id = reader.GetInt32(0),
                                CategoryId = reader.GetInt32(1),
                                Title = reader.GetString(2),
                                Link = reader.GetString(3),
                                Guid = reader.GetString(4),
                                Pubdate = reader.GetDateTimeOffset(5),
                                Image = reader.IsDBNull(6) ? null! : reader.GetString(6)
                            });
                        }
                    }
                }
            }

            return Ok(posts);
        }
    }
}
