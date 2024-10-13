using NewsAggregator.Entity;
using System.Data.SqlClient;

namespace NewsAggregator.Service
{
    public class TagService
    {
        private readonly string _connectionString;

        public TagService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<Tag>> GetTagsAsync()
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

            return tags;
        }

        public async Task<List<Tag>> GetPopularTagsAsync()
        {
            var tags = new List<Tag>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

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

            return tags;
        }

        public async Task<List<Tag>> GetTrendingTagsAsync()
        {
            var tags = new List<Tag>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

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

            return tags;
        }

        public async Task FollowTagAsync(int userId, int tagId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var command = new SqlCommand(@"
                                INSERT INTO UserFollowTag (UserId, TagId, FollowDate)
                                VALUES (@UserId, @TagId, GETDATE())", connection);

                command.Parameters.AddWithValue("@UserId", userId);
                command.Parameters.AddWithValue("@TagId", tagId);

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task UnfollowTagAsync(int userId, int tagId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var command = new SqlCommand(@"
                                DELETE FROM UserFollowTag
                                WHERE UserId = @UserId AND TagId = @TagId", connection);

                command.Parameters.AddWithValue("@UserId", userId);
                command.Parameters.AddWithValue("@TagId", tagId);

                await command.ExecuteNonQueryAsync();
            }
        }
    }
}
