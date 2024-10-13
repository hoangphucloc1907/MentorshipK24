using NewsAggregator.Entity;
using System.Data.SqlClient;

namespace NewsAggregator.Repository
{
    public class UserRepository
    {
        private readonly string _connectionString;

        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void InsertUser(User user)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var cmdQuery = "INSERT INTO Users (Email, Username, Password, Role) VALUES (@Email, @Username, @Password, @Role)";

                using (var command = new SqlCommand(cmdQuery, connection))
                {
                    command.Parameters.AddWithValue("@Email", user.Email);
                    command.Parameters.AddWithValue("@Username", user.Username);
                    command.Parameters.AddWithValue("@Password", user.Password);
                    command.Parameters.AddWithValue("@Role", (int)user.Role);
                    command.ExecuteNonQuery();
                }
            }
        }

        public User? GetUserByEmail(string email)
        {
            User? result = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var cmdQuery = "SELECT Id, Email, Username, Password, Role FROM Users WHERE Email = @Email";

                using (var command = new SqlCommand(cmdQuery, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            result = new User
                            {
                                Id = reader.GetInt32(0),
                                Email = reader.GetString(1),
                                Username = reader.GetString(2),
                                Password = reader.GetString(3),
                                Role = (UserRole)reader.GetInt32(4)
                            };
                        }
                    }
                }
            }

            return result;
        }
    }
}
