using HtmlAgilityPack;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.ServiceModel.Syndication;
using System.Xml;
using NewsAggregator.Entity;
using System.Globalization;

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
					{ "thanhnien.vn", "//ul[@class='cate-content']/li/a" }
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

					await ProcessSourceUrls();

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
			var doc = await web.LoadFromWebAsync(url);

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
					// Fix: If there are span tags or unnecessary words ("RSS", '-'), remove them
					var categoryName = Regex.Replace(categoryNode.InnerText.Trim(), @"<[^>]+>|RSS|-.+", "").Trim();

					var rssUrl = categoryNode.GetAttributeValue("href", string.Empty);

					if (!string.IsNullOrEmpty(categoryName) && !string.IsNullOrEmpty(rssUrl))
					{
						// Check if the URL is relative and make it a full path if necessary
						if (!rssUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
						{
							rssUrl = new Uri(uri, rssUrl).ToString();
						}

						// Automatically add get ID
						int providerId = await GetOrInsertProvider(url);
						int categoryId = await GetOrInsertCategory(categoryName);

						await InsertSource(rssUrl, categoryId, providerId);
						_logger.LogInformation($"Category '{categoryName}' with RSS URL '{rssUrl}' has been processed.");
					}
				}
			}
		}

		private async Task<int> GetOrInsertCategory(string categoryName)
		{
			using (var connection = new SqlConnection(_connectionString))
			{
				await connection.OpenAsync();

				// Check if category already exists
				var selectCommand = new SqlCommand("SELECT Id FROM Category WHERE CategoryName = @CategoryName", connection);
				selectCommand.Parameters.AddWithValue("@CategoryName", categoryName);

				var result = await selectCommand.ExecuteScalarAsync();
				if (result != null)
				{
					_logger.LogInformation($"Category found: {categoryName}");
					return (int)result;
				}

				// If not, insert new category
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

		private async Task<int> GetOrInsertProvider(string url)
		{
			using (var connection = new SqlConnection(_connectionString))
			{
				await connection.OpenAsync();

				var uri = new Uri(url);
				var providerName = uri.Host;

				// Check if provider already exists
				var selectCommand = new SqlCommand("SELECT Id FROM Provider WHERE Link = @Link", connection);
				selectCommand.Parameters.AddWithValue("@Link", url);

				var result = await selectCommand.ExecuteScalarAsync();
				if (result != null)
				{
					_logger.LogInformation($"Provider found for URL: {url}");
					return (int)result;
				}

				// If not, insert new provider
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

		private async Task InsertSource(string sourceUrl, int categoryId, int providerId)
		{
			using (var connection = new SqlConnection(_connectionString))
			{
				await connection.OpenAsync();

				// Check if source already exists
				var selectCommand = new SqlCommand("SELECT Id FROM Source WHERE SourceUrl = @SourceUrl", connection);
				selectCommand.Parameters.AddWithValue("@SourceUrl", sourceUrl);

				var result = await selectCommand.ExecuteScalarAsync();
				if (result != null)
				{
					_logger.LogInformation($"Source already exists: {sourceUrl}");
					return;
				}

				// If not, insert new source
				var insertCommand = new SqlCommand("INSERT INTO Source (SourceUrl, CategoryId, ProviderId) VALUES (@SourceUrl, @CategoryId, @ProviderId)", connection);
				insertCommand.Parameters.AddWithValue("@SourceUrl", sourceUrl);
				insertCommand.Parameters.AddWithValue("@CategoryId", categoryId);
				insertCommand.Parameters.AddWithValue("@ProviderId", providerId);

				await insertCommand.ExecuteNonQueryAsync();
				_logger.LogInformation($"Inserted new source: {sourceUrl}");
			}
		}

		private async Task ProcessSourceUrls()
		{
			var sourceUrls = await GetSourceUrls();

			var tasks = new List<Task>();
			foreach (var sourceUrl in sourceUrls)
			{
				tasks.Add(ConvertUrlToPosts(sourceUrl));
			}
			await Task.WhenAll(tasks);
		}

		private async Task<List<string>> GetSourceUrls()
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

		private async Task ConvertUrlToPosts(string url)
		{
			using (var reader = XmlReader.Create(url))
			{
				var feed = SyndicationFeed.Load(reader);

				var tasks = new List<Task>();

				foreach (var item in feed.Items)
				{
					var title = item.Title.Text;
					var link = item.Links[0].Uri.ToString();
					var guid = item.Id;

					// Get pubDate from item
					DateTimeOffset pubDate = GetPubDate(item, title);

					var image = GetImageFromItem(item);
					int categoryId = await GetCategoryIdFromSource(url);

					tasks.Add(UpsertPost(new Post
					{
						CategoryId = categoryId,
						Title = title,
						Link = link,
						Guid = guid,
						Pubdate = pubDate,
						Image = image
					}));
				}

				await Task.WhenAll(tasks);
			}
		}

		private DateTimeOffset GetPubDate(SyndicationItem item, string title)
		{
			// Try to get PublishDate from SyndicationItem
			DateTimeOffset pubDate;

			// Check PublishDate property first
			try
			{
				pubDate = item.PublishDate;

				// If PublishDate is not valid, try to get from pubDate element
				if (pubDate == DateTimeOffset.MinValue)
				{
					throw new InvalidOperationException("PublishDate is MinValue.");
				}
			}
			catch (Exception ex) // Catch any unexpected errors
			{
				_logger.LogWarning($"Failed to get PublishDate for item '{title}': {ex.Message}. Trying to read from pubDate element.");
				pubDate = GetPubDateFromElement(item, title);
			}

			return pubDate;
		}

		private DateTimeOffset GetPubDateFromElement(SyndicationItem item, string title)
		{
			var pubDateElement = item.ElementExtensions.ReadElementExtensions<string>("pubDate", "").FirstOrDefault();

			if (string.IsNullOrWhiteSpace(pubDateElement))
			{
				_logger.LogWarning($"pubDate element is missing for item '{title}'. Using fallback date.");
				return DateTimeOffset.UtcNow;  // Or you can choose another default value
			}

			// Convert date time from string
			if (DateTimeOffset.TryParse(pubDateElement.Trim(), out var pubDate))
			{
				return pubDate;
			}

			// Log warning if parsing fails
			_logger.LogWarning($"Failed to parse pubDate element for item '{title}'. Using current date.");
			return DateTimeOffset.UtcNow;  // Or you can choose another default value
		}

		private async Task<int> GetCategoryIdFromSource(string sourceUrl)
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

		private string GetImageFromItem(SyndicationItem item)
		{
			// Check links in item
			foreach (var link in item.Links)
			{
				// Check if link has type "enclosure"
				if (link.RelationshipType == "enclosure" && link.MediaType == "image/jpeg")
				{
					return link.Uri.ToString(); // Return image URL
				}
			}

			// If not found, check in ElementExtensions
			foreach (var element in item.ElementExtensions)
			{
				if (element.OuterName == "enclosure")
				{
					var xmlElement = element.GetObject<XmlElement>();
					if (xmlElement.GetAttribute("type") == "image/jpeg")
					{
						return xmlElement.GetAttribute("url"); // Return URL from attribute
					}
				}
			}

			return string.Empty; // Return empty string if no image found
		}

		private async Task UpsertPost(Post post)
		{
			using (var connection = new SqlConnection(_connectionString))
			{
				await connection.OpenAsync();

				// Check if post already exists
				var selectCommand = new SqlCommand("SELECT Id FROM Post WHERE Guid = @Guid", connection);
				selectCommand.Parameters.AddWithValue("@Guid", post.Guid);

				var result = await selectCommand.ExecuteScalarAsync();
				if (result != null)
				{
					// Update the existing post
					var updateCommand = new SqlCommand("UPDATE Post SET CategoryId = @CategoryId, Title = @Title, Link = @Link, Pubdate = @Pubdate, Image = @Image WHERE Guid = @Guid", connection);
					updateCommand.Parameters.AddWithValue("@CategoryId", post.CategoryId);
					updateCommand.Parameters.AddWithValue("@Title", post.Title);
					updateCommand.Parameters.AddWithValue("@Link", post.Link);
					updateCommand.Parameters.AddWithValue("@Pubdate", post.Pubdate);
					updateCommand.Parameters.AddWithValue("@Image", post.Image ?? (object)DBNull.Value);
					updateCommand.Parameters.AddWithValue("@Guid", post.Guid);

					await updateCommand.ExecuteNonQueryAsync();
					_logger.LogInformation($"Post updated: {post.Title}");
				}
				else
				{
					// Insert the new post
					var insertCommand = new SqlCommand("INSERT INTO Post (CategoryId, Title, Link, Guid, Pubdate, Image) VALUES (@CategoryId, @Title, @Link, @Guid, @Pubdate, @Image)", connection);
					insertCommand.Parameters.AddWithValue("@CategoryId", post.CategoryId);
					insertCommand.Parameters.AddWithValue("@Title", post.Title);
					insertCommand.Parameters.AddWithValue("@Link", post.Link);
					insertCommand.Parameters.AddWithValue("@Guid", post.Guid);
					insertCommand.Parameters.AddWithValue("@Pubdate", post.Pubdate);
					insertCommand.Parameters.AddWithValue("@Image", post.Image ?? (object)DBNull.Value);

					await insertCommand.ExecuteNonQueryAsync();
					_logger.LogInformation($"New post inserted: {post.Title}");
				}
			}
		}
	}
}
