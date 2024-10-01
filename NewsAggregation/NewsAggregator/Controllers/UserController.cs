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
		// GET: api/<UserController>
		[HttpGet]
		public IEnumerable<User> Get()
		{
			var connectionString = "Data Source=DESKTOP-CS11CFJ\\SQLEXPRESS;Initial Catalog=NewsAggregator;Integrated Security=True;";
			var result = new List<User>();

			using (var connection = new SqlConnection(connectionString))
			{
				connection.Open();
				var cmdQuery = "SELECT UserID, Email, Username FROM Users";

				using (var command = new SqlCommand(cmdQuery, connection))
				using (var reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						result.Add(new User
						{
							UserID = reader.GetInt32(0),
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
		public ActionResult<User> Get(int id)
		{
			var connectionString = "Data Source=DESKTOP-CS11CFJ\\SQLEXPRESS;Initial Catalog=NewsAggregator;Integrated Security=True;";
			User result = null;

			using (var connection = new SqlConnection(connectionString))
			{
				connection.Open();
				var cmdQuery = "SELECT UserID, Email, Username FROM Users WHERE UserID = @UserID";

				using (var command = new SqlCommand(cmdQuery, connection))
				{
					command.Parameters.AddWithValue("@UserID", id);

					using (var reader = command.ExecuteReader())
					{
						if (reader.Read())
						{
							result = new User
							{
								UserID = reader.GetInt32(0),
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
		public IActionResult Post([FromBody] User user)
		{
			if (user == null || string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Username))
			{
				return BadRequest("Invalid user data.");
			}

			var connectionString = "Data Source=DESKTOP-CS11CFJ\\SQLEXPRESS;Initial Catalog=NewsAggregator;Integrated Security=True;";

			using (var connection = new SqlConnection(connectionString))
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
		public IActionResult Put(int id, [FromBody] User user)
		{
			var connectionString = "Data Source=DESKTOP-CS11CFJ\\SQLEXPRESS;Initial Catalog=NewsAggregator;Integrated Security=True;";

			using (var connection = new SqlConnection(connectionString))
			{
				connection.Open();
				var cmdQuery = "UPDATE Users SET Username = @Username WHERE UserID = @UserID";

				using (var command = new SqlCommand(cmdQuery, connection))
				{
					command.Parameters.AddWithValue("@Username", user.Username);
					command.Parameters.AddWithValue("@UserID", id);

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
			var connectionString = "Data Source=DESKTOP-CS11CFJ\\SQLEXPRESS;Initial Catalog=NewsAggregator;Integrated Security=True;";

			using (var connection = new SqlConnection(connectionString))
			{
				connection.Open();
				var cmdQuery = "DELETE FROM Users WHERE UserID = @UserID";

				using (var command = new SqlCommand(cmdQuery, connection))
				{
					command.Parameters.AddWithValue("@UserID", id);

					int rowsAffected = command.ExecuteNonQuery();
					if (rowsAffected == 0)
					{
						return NotFound("User not found.");
					}
				}
			}
			return Ok("User deleted successfully.");
		}
	}
}
