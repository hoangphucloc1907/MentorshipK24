using NewsAggregator.Entity;
using System.Data.SqlClient;

namespace NewsAggregator.Service
{
	public class SourceService
	{
		private readonly string _connectionString;

		public SourceService(string connectionString)
		{
			_connectionString = connectionString;
		}

		public async Task InsertSourceAsync(Source source)
		{
			try
			{
				using (SqlConnection connection = new SqlConnection(_connectionString))
				{
					await connection.OpenAsync();

					string query = "INSERT INTO Sources (SourceName, SourceUrl, SourceType, SourceCategoryID, CreatedAt, Description, ApiUrl) " +
								   "VALUES (@SourceName, @SourceUrl, @SourceType, @SourceCategoryID, @CreatedAt, @Description, @ApiUrl)";

					using (SqlCommand command = new SqlCommand(query, connection))
					{
						command.Parameters.AddWithValue("@SourceName", source.SourceName);
						command.Parameters.AddWithValue("@SourceUrl", source.SourceUrl);
						command.Parameters.AddWithValue("@SourceType", source.SourceType);
						command.Parameters.AddWithValue("@SourceCategoryID", source.SourceCategoryID);
						command.Parameters.AddWithValue("@CreatedAt", source.CreatedAt);
						command.Parameters.AddWithValue("@Description", source.Description);
						command.Parameters.AddWithValue("@ApiUrl", source.ApiUrl);

						await command.ExecuteNonQueryAsync();
					}
				}
			}
			catch (SqlException ex)
			{
				// Log error or handle it appropriately
				throw new Exception("Error while inserting source", ex);
			}
		}

		public async Task InsertRssFeedsAsync(List<RSSFeed> feeds, int sourceId)
		{
			try
			{
				using (SqlConnection connection = new SqlConnection(_connectionString))
				{
					await connection.OpenAsync();

					foreach (var feed in feeds)
					{
						string query = "INSERT INTO RSSFeeds (SourceID, RSSFeedUrl, TagID, FeedName, CreatedAt) " +
									   "VALUES (@SourceID, @RSSFeedUrl, @TagID, @FeedName, @CreatedAt)";

						using (SqlCommand command = new SqlCommand(query, connection))
						{
							command.Parameters.AddWithValue("@SourceID", sourceId);
							command.Parameters.AddWithValue("@RSSFeedUrl", feed.RSSFeedUrl);
							command.Parameters.AddWithValue("@TagID", feed.TagID);
							command.Parameters.AddWithValue("@FeedName", feed.FeedName);
							command.Parameters.AddWithValue("@CreatedAt", feed.CreatedAt);

							await command.ExecuteNonQueryAsync();
						}
					}
				}
			}
			catch (SqlException ex)
			{
				// Log error or handle it appropriately
				throw new Exception("Error while inserting RSS feeds", ex);
			}
		}
	}
}
