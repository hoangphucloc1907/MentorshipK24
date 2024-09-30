using Microsoft.AspNetCore.Mvc;
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
            var connection = new SqlConnection(connectionString);

            connection.Open();
            var cmdQuery = "select * from Users";

            var command = new SqlCommand(cmdQuery, connection);
            var reader = command.ExecuteReader();
            var result = new List<User>();

            while (reader.Read())
            {
                result.Add(new User
                {
                    UserID = reader.GetInt32(0),
                    Email = reader.GetString(1),
                    Username = reader.GetString(2),
               
                });
            }
            connection.Close();

            return result;
        }

        // GET api/<UserController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<UserController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<UserController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<UserController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
