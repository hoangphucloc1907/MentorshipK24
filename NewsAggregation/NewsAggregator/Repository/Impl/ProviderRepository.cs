using System.Data.SqlClient;

namespace NewsAggregator.Repository.Impl
{
    public class ProviderRepository : IProviderRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<ProviderRepository> _logger;

        public ProviderRepository(string connectionString, ILogger<ProviderRepository> logger)
        {
            _connectionString = connectionString;
            _logger = logger;
        }

        public async Task<int> GetOrInsertProvider(string url)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var uri = new Uri(url);
                var providerName = uri.Host;

                var selectCommand = new SqlCommand("SELECT Id FROM Provider WHERE Link = @Link", connection);
                selectCommand.Parameters.AddWithValue("@Link", url);

                var result = await selectCommand.ExecuteScalarAsync();
                if (result != null)
                {
                    _logger.LogInformation($"Provider found for URL: {url}");
                    return (int)result;
                }

                var insertCommand = new SqlCommand("INSERT INTO Provider (ProviderName, Link) OUTPUT INSERTED.Id VALUES (@ProviderName, @Link)", connection);
                insertCommand.Parameters.AddWithValue("@ProviderName", providerName);
                insertCommand.Parameters.AddWithValue("@Link", url);

                var insertedId = await insertCommand.ExecuteScalarAsync() as int?;
                if (insertedId.HasValue)
                {
                    _logger.LogInformation($"Inserted new provider for URL: {url} with ID: {insertedId.Value}");
                    return insertedId.Value;
                }
                throw new InvalidOperationException("Failed to insert provider.");
            }
        }
    }
}
