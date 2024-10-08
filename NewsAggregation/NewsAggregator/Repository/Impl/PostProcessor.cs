using HtmlAgilityPack;
using NewsAggregator.Entity;
using System.Data.SqlClient;
using System.ServiceModel.Syndication;
using System.Xml;

namespace NewsAggregator.Repository.Impl
{
	public class PostProcessor : IPostProcessor
	{
		private readonly string _connectionString;
		private readonly ILogger<PostProcessor> _logger;
		private readonly ISourceRepository _sourceRepository;
		private readonly ICategoryRepository _categoryRepository;

		public PostProcessor(string connectionString, ILogger<PostProcessor> logger, ISourceRepository sourceRepository, ICategoryRepository categoryRepository)
		{
			_connectionString = connectionString;
			_logger = logger;
			_sourceRepository = sourceRepository;
			_categoryRepository = categoryRepository;
		}

		public async Task ConvertUrlToPosts(string url)
		{
			using (var reader = XmlReader.Create(url))
			{
				var feed = SyndicationFeed.Load(reader);
				var tasks = feed.Items.Select(item => ProcessPostAsync(item, url)).ToList();
				await Task.WhenAll(tasks);
			}

			await ProcessTagsForAllPosts();
		}

		private async Task ProcessPostAsync(SyndicationItem item, string url)
		{
			var title = item.Title.Text;
			var link = item.Links[0].Uri.ToString();
			var guid = item.Id;
			DateTimeOffset pubDate = GetPubDate(item, title);
			var image = GetImageFromItem(item);
			int categoryId = await _categoryRepository.GetCategoryIdFromSource(url);

			var post = new Post
			{
				CategoryId = categoryId,
				Title = title,
				Link = link,
				Guid = guid,
				Pubdate = pubDate,
				Image = image
			};

			int postId = await UpsertPost(post);
		}

		private DateTimeOffset GetPubDate(SyndicationItem item, string title)
		{
			try
			{
				var pubDate = item.PublishDate;
				if (pubDate == DateTimeOffset.MinValue)
				{
					throw new InvalidOperationException("PublishDate is MinValue.");
				}
				return pubDate;
			}
			catch (Exception ex)
			{
				_logger.LogWarning($"Failed to get PublishDate for item '{title}': {ex.Message}. Trying to read from pubDate element.");
				return GetPubDateFromElement(item, title);
			}
		}

		private DateTimeOffset GetPubDateFromElement(SyndicationItem item, string title)
		{
			var pubDateElement = item.ElementExtensions.ReadElementExtensions<string>("pubDate", "").FirstOrDefault();
			if (string.IsNullOrWhiteSpace(pubDateElement))
			{
				_logger.LogWarning($"pubDate element is missing for item '{title}'. Using fallback date.");
				return DateTimeOffset.UtcNow;
			}

			return DateTimeOffset.TryParse(pubDateElement.Trim(), out var pubDate) ? pubDate : DateTimeOffset.UtcNow;
		}

		private string GetImageFromItem(SyndicationItem item)
		{
			var imageLink = item.Links.FirstOrDefault(link => link.RelationshipType == "enclosure" && link.MediaType == "image/jpeg");
			if (imageLink != null)
			{
				return imageLink.Uri.ToString();
			}

			var imageElement = item.ElementExtensions.FirstOrDefault(element => element.OuterName == "enclosure")?.GetObject<XmlElement>();
			return imageElement?.GetAttribute("type") == "image/jpeg" ? imageElement.GetAttribute("url") : string.Empty;
		}

		private async Task<int> UpsertPost(Post post)
		{
			using (var connection = new SqlConnection(_connectionString))
			{
				await connection.OpenAsync();

				var selectCommand = new SqlCommand("SELECT Id FROM Post WHERE Guid = @Guid", connection);
				selectCommand.Parameters.AddWithValue("@Guid", post.Guid);

				var result = await selectCommand.ExecuteScalarAsync();
				if (result != null)
				{
					var updateCommand = new SqlCommand("UPDATE Post SET CategoryId = @CategoryId, Title = @Title, Link = @Link, Pubdate = @Pubdate, Image = @Image WHERE Guid = @Guid", connection);
					updateCommand.Parameters.AddWithValue("@CategoryId", post.CategoryId);
					updateCommand.Parameters.AddWithValue("@Title", post.Title);
					updateCommand.Parameters.AddWithValue("@Link", post.Link);
					updateCommand.Parameters.AddWithValue("@Pubdate", post.Pubdate);
					updateCommand.Parameters.AddWithValue("@Image", post.Image ?? (object)DBNull.Value);
					updateCommand.Parameters.AddWithValue("@Guid", post.Guid);

					await updateCommand.ExecuteNonQueryAsync();
					_logger.LogInformation($"Post updated: {post.Title}");
					return (int)result;
				}
				else
				{
					var insertCommand = new SqlCommand("INSERT INTO Post (CategoryId, Title, Link, Guid, Pubdate, Image) OUTPUT INSERTED.Id VALUES (@CategoryId, @Title, @Link, @Guid, @Pubdate, @Image)", connection);
					insertCommand.Parameters.AddWithValue("@CategoryId", post.CategoryId);
					insertCommand.Parameters.AddWithValue("@Title", post.Title);
					insertCommand.Parameters.AddWithValue("@Link", post.Link);
					insertCommand.Parameters.AddWithValue("@Guid", post.Guid);
					insertCommand.Parameters.AddWithValue("@Pubdate", post.Pubdate);
					insertCommand.Parameters.AddWithValue("@Image", post.Image ?? (object)DBNull.Value);

					var postId = await insertCommand.ExecuteScalarAsync();
					_logger.LogInformation($"New post inserted: {post.Title}");
					return (int)postId;
				}
			}
		}

		private async Task ProcessTagsForAllPosts()
		{
			using (var connection = new SqlConnection(_connectionString))
			{
				await connection.OpenAsync();
				var selectCommand = new SqlCommand("SELECT Id, Link FROM Post", connection);
				using (var reader = await selectCommand.ExecuteReaderAsync())
				{
					while (await reader.ReadAsync())
					{
						int postId = reader.GetInt32(0);
						string postLink = reader.GetString(1);
						var tags = await GetTagsFromPostAsync(postLink);
						await InsertTagAsync(postId, tags);
					}
				}
			}
		}

        private async Task<List<string>> GetTagsFromPostAsync(string postLink)
        {
            var tags = new List<string>();
            try
            {
                var httpClient = new HttpClient();
                var html = await httpClient.GetStringAsync(postLink);

                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                var htmlStructure = new Dictionary<string, string[]>
                {
                    { "tuoitreTags", new[] { "//div[@class='detail-tab']/a" } },
                    { "vnexpressTags", new[] { "//div[@class='tags']/h4/a" } }
                };

                foreach (var entry in htmlStructure)
                {
                    var nodes = doc.DocumentNode.SelectNodes(entry.Value[0]);
                    if (nodes != null)
                    {
                        tags.AddRange(nodes.Select(tag => tag.InnerText.Trim()));
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Failed to fetch or parse tags from {postLink}: {ex.Message}");
            }

            return tags;
        }

		private async Task InsertTagAsync(int postId, List<string> tags)
		{
			using (var connection = new SqlConnection(_connectionString))
			{
				await connection.OpenAsync();

				foreach (var tag in tags)
				{
					var selectTagCommand = new SqlCommand("SELECT Id FROM Tag WHERE TagName = @TagName", connection);
					selectTagCommand.Parameters.AddWithValue("@TagName", tag);

					var tagId = await selectTagCommand.ExecuteScalarAsync();

					if (tagId == null)
					{
						var insertTagCommand = new SqlCommand("INSERT INTO Tag (TagName) OUTPUT INSERTED.Id VALUES (@TagName)", connection);
						insertTagCommand.Parameters.AddWithValue("@TagName", tag);
						tagId = await insertTagCommand.ExecuteScalarAsync();
					}

					var insertPostTagCommand = new SqlCommand("INSERT INTO PostTag (PostId, TagId) VALUES (@PostId, @TagId)", connection);
					insertPostTagCommand.Parameters.AddWithValue("@PostId", postId);
					insertPostTagCommand.Parameters.AddWithValue("@TagId", tagId);
					await insertPostTagCommand.ExecuteNonQueryAsync();
				}
			}
		}
	}
}
