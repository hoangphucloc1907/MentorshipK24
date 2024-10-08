using System.Data.SqlClient;

namespace NewsAggregator.Repository.Impl
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<CategoryRepository> _logger;

        public CategoryRepository(string connectionString, ILogger<CategoryRepository> logger)
        {
            _connectionString = connectionString;
            _logger = logger;
        }

        public async Task<int> GetCategoryIdFromSource(string sourceUrl)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand("SELECT CategoryId FROM Source WHERE SourceUrl = @SourceUrl", connection);
                command.Parameters.AddWithValue("@SourceUrl", sourceUrl);

                var result = await command.ExecuteScalarAsync();
                if (result != null)
                {
                    return (int)result;
                }
                throw new InvalidOperationException($"No CategoryId found for SourceUrl: {sourceUrl}");
            }
        }

        public async Task<int> GetOrInsertCategory(string categoryName)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var selectCommand = new SqlCommand("SELECT Id FROM Category WHERE CategoryName = @CategoryName", connection);
                selectCommand.Parameters.AddWithValue("@CategoryName", categoryName);

                var result = await selectCommand.ExecuteScalarAsync();
                if (result != null)
                {
                    _logger.LogInformation($"Category found: {categoryName}");
                    return (int)result;
                }

                var insertCommand = new SqlCommand("INSERT INTO Category (CategoryName) OUTPUT INSERTED.Id VALUES (@CategoryName)", connection);
                insertCommand.Parameters.AddWithValue("@CategoryName", categoryName);

                var insertedId = await insertCommand.ExecuteScalarAsync() as int?;
                if (insertedId.HasValue)
                {
                    _logger.LogInformation($"Inserted new category: {categoryName} with ID: {insertedId.Value}");
                    return insertedId.Value;
                }
                throw new InvalidOperationException("Failed to insert category.");
            }
        }
    }
}
