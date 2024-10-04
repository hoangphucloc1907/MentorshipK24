using HtmlAgilityPack;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace NewsAggregator.Service
{
	public class RssScraperHostedService : BackgroundService
	{
		private readonly ILogger<RssScraperHostedService> _logger;
		private readonly string _connectionString;
		private readonly Dictionary<string, string> _providerXPathMappings;

		public RssScraperHostedService(ILogger<RssScraperHostedService> logger, string connectionString)
		{
			_logger = logger;
			_connectionString = connectionString;
			_providerXPathMappings = new Dictionary<string, string>
							{
								{ "tuoitre.vn", "//ul[@class='list-rss clearfix']/li/a" },
								{ "vnexpress.net", "//ul[@class='list-rss']/li/a" },
								{"thanhnien.vn","//ul[@class='cate-content']/li/a" }
								// Add more mappings as needed
							};
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			_logger.LogInformation("RssScraperHostedService running.");

			while (!stoppingToken.IsCancellationRequested)
			{
				try
				{
					await ScrapeAndStoreRssData("https://tuoitre.vn/rss.htm");
					await ScrapeAndStoreRssData("https://vnexpress.net/rss");
					await ScrapeAndStoreRssData("https://thanhnien.vn/rss.html");
					// Add more URLs as needed

					await Task.Delay(TimeSpan.FromHours(1), stoppingToken); // Run every hour
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "An error occurred while scraping RSS data.");
				}
			}

			_logger.LogInformation("RssScraperHostedService is stopping.");
		}

		private async Task ScrapeAndStoreRssData(string url)
		{
			var web = new HtmlWeb();
			var doc = web.Load(url);

			var uri = new Uri(url);
			var host = uri.Host.Replace("www.", "");

			if (!_providerXPathMappings.TryGetValue(host, out var xPathQuery))
			{
				_logger.LogWarning($"No XPath query found for provider: {host}");
				return;
			}

			var categories = doc.DocumentNode.SelectNodes(xPathQuery);

			if (categories != null)
			{
				foreach (var categoryNode in categories)
				{
					// Fix: If there are span tags or unnecessary words ("RSS", '-'),remove them
					var categoryName = Regex.Replace(categoryNode.InnerText.Trim(), @"<[^>]+>|RSS|-.+", "").Trim();

					var rssUrl = categoryNode.GetAttributeValue("href", string.Empty);

					if (!string.IsNullOrEmpty(categoryName) && !string.IsNullOrEmpty(rssUrl))
					{
						// Automatically add get ID
						int providerId = await GetOrInsertProvider(url);
						int categoryId = await GetOrInsertCategory(categoryName);

						await InsertSource(categoryName, rssUrl, categoryId, providerId);
						Console.WriteLine($"Category '{categoryName}' with RSS URL '{rssUrl}' has been processed.");
					}
				}
			}
		}

		private async Task<int> GetOrInsertCategory(string categoryName)
		{
			using (var connection = new SqlConnection(_connectionString))
			{
				await connection.OpenAsync();

				// Kiểm tra xem category đã tồn tại chưa
				var selectCommand = new SqlCommand("SELECT Id FROM Category WHERE CategoryName = @CategoryName", connection);
				selectCommand.Parameters.AddWithValue("@CategoryName", categoryName);

				var result = await selectCommand.ExecuteScalarAsync();
				if (result != null)
				{
					_logger.LogInformation($"Category found: {categoryName}");
					return (int)result;
				}

				// Nếu không tồn tại, thêm category mới
				var insertCommand = new SqlCommand("INSERT INTO Category (CategoryName) OUTPUT INSERTED.Id VALUES (@CategoryName)", connection);
				insertCommand.Parameters.AddWithValue("@CategoryName", categoryName);

				var insertedId = (int)await insertCommand.ExecuteScalarAsync();
				_logger.LogInformation($"Inserted new category: {categoryName} with ID: {insertedId}");
				return insertedId;
			}
		}

		private async Task<int> GetOrInsertProvider(string url)
		{
			using (var connection = new SqlConnection(_connectionString))
			{
				await connection.OpenAsync();

				var uri = new Uri(url);
				var providerName = uri.Host;

				// Kiểm tra xem provider đã tồn tại chưa
				var selectCommand = new SqlCommand("SELECT Id FROM Provider WHERE Link = @Link", connection);
				selectCommand.Parameters.AddWithValue("@Link", url);

				var result = await selectCommand.ExecuteScalarAsync();
				if (result != null)
				{
					_logger.LogInformation($"Provider found for URL: {url}");
					return (int)result;
				}

				// Nếu không tồn tại, thêm provider mới
				var insertCommand = new SqlCommand("INSERT INTO Provider (ProviderName, Link) OUTPUT INSERTED.Id VALUES (@ProviderName, @Link)", connection);
				insertCommand.Parameters.AddWithValue("@ProviderName", providerName);
				insertCommand.Parameters.AddWithValue("@Link", url);

				var insertedId = (int)await insertCommand.ExecuteScalarAsync();
				_logger.LogInformation($"Inserted new provider for URL: {url} with ID: {insertedId}");
				return insertedId;
			}
		}

		private async Task InsertSource(string sourceName, string sourceUrl, int categoryId, int providerId)
		{
			using (var connection = new SqlConnection(_connectionString))
			{
				await connection.OpenAsync();
				var command = new SqlCommand("INSERT INTO Source (SourceUrl, CategoryId, ProviderId) VALUES (@SourceUrl, @CategoryId, @ProviderId)", connection);
				command.Parameters.AddWithValue("@SourceUrl", sourceUrl);
				command.Parameters.AddWithValue("@CategoryId", categoryId);
				command.Parameters.AddWithValue("@ProviderId", providerId);

				await command.ExecuteNonQueryAsync();
			}
		}
	}
}
