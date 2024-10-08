using System.Data.SqlClient;

namespace NewsAggregator.Repository.Impl
{
    public class SourceRepository : ISourceRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<SourceRepository> _logger;

        public SourceRepository(string connectionString, ILogger<SourceRepository> logger)
        {
            _connectionString = connectionString;
            _logger = logger;
        }

        public async Task InsertSource(string sourceUrl, int categoryId, int providerId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var selectCommand = new SqlCommand("SELECT Id FROM Source WHERE SourceUrl = @SourceUrl", connection);
                selectCommand.Parameters.AddWithValue("@SourceUrl", sourceUrl);

                var result = await selectCommand.ExecuteScalarAsync();
                if (result != null)
                {
                    _logger.LogInformation($"Source already exists: {sourceUrl}");
                    return;
                }

                var insertCommand = new SqlCommand("INSERT INTO Source (SourceUrl, CategoryId, ProviderId) VALUES (@SourceUrl, @CategoryId, @ProviderId)", connection);
                insertCommand.Parameters.AddWithValue("@SourceUrl", sourceUrl);
                insertCommand.Parameters.AddWithValue("@CategoryId", categoryId);
                insertCommand.Parameters.AddWithValue("@ProviderId", providerId);

                await insertCommand.ExecuteNonQueryAsync();
                _logger.LogInformation($"Inserted new source: {sourceUrl}");
            }
        }

        public async Task<List<string>> GetSourceUrls()
        {
            var sourceUrls = new List<string>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand("SELECT SourceUrl FROM Source", connection);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        sourceUrls.Add(reader.GetString(0));
                    }
                }
            }

            return sourceUrls;
        }
    }
}
