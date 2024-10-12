
using NewsAggregator.Repository;

namespace NewsAggregator.Service
{
	public class RssScraperHostedService : BackgroundService
	{
		private readonly ILogger<RssScraperHostedService> _logger;
		private readonly IRssScraper _rssScraper;
		private readonly IPostProcessor _postProcessor;
		private readonly ISourceRepository _sourceRepository;

		public RssScraperHostedService(ILogger<RssScraperHostedService> logger, IRssScraper rssScraper, IPostProcessor postProcessor, ISourceRepository sourceRepository)
		{
			_logger = logger;
			_rssScraper = rssScraper;
			_postProcessor = postProcessor;
			_sourceRepository = sourceRepository;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			_logger.LogInformation("RssScraperHostedService running.");

			while (!stoppingToken.IsCancellationRequested)
			{
				try
				{
					await _rssScraper.ScrapeAndStoreRssData("https://tuoitre.vn/rss.htm");
					await _rssScraper.ScrapeAndStoreRssData("https://vnexpress.net/rss");
					await _rssScraper.ScrapeAndStoreRssData("https://thanhnien.vn/rss.html");
					// Add more URLs as needed

					await ProcessSourceUrls();

					await Task.Delay(TimeSpan.FromHours(1), stoppingToken); // schedule
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "An error occurred while scraping RSS data.");
				}
			}

			_logger.LogInformation("RssScraperHostedService is stopping.");
		}

		private async Task ProcessSourceUrls()
		{
			var sourceUrls = await _sourceRepository.GetSourceUrls();

			var tasks = new List<Task>();
			foreach (var sourceUrl in sourceUrls)
			{
				tasks.Add(_postProcessor.ConvertUrlToPosts(sourceUrl));
			}
			await Task.WhenAll(tasks);
		}
	}
}
