using System.Data.SqlClient;

namespace NewsAggregator.Service
{
    public class CategoryService
    {
        private readonly string _connectionString;

        public CategoryService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task FollowCategoryAsync(int userId, int categoryId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = "INSERT INTO UserFollowCategory (UserId, CategoryId) VALUES (@UserId, @CategoryId)";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    command.Parameters.AddWithValue("@CategoryId", categoryId);

                    connection.Open();
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task UnfollowCategoryAsync(int userId, int categoryId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = "DELETE FROM UserFollowCategory WHERE UserId = @UserId AND CategoryId = @CategoryId";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    command.Parameters.AddWithValue("@CategoryId", categoryId);

                    connection.Open();
                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
