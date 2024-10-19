
using NewsAggregator.Repository;

namespace NewsAggregator.Service
{
    public class RssScraperHostedService : BackgroundService
    {
        private readonly ILogger<RssScraperHostedService> _logger;
        private readonly IRssScraper _rssScraper;
        private readonly IPostRepository _postProcessor;
        private readonly ISourceRepository _sourceRepository;
        private readonly IProviderRepository _providerRepository;

        public RssScraperHostedService(ILogger<RssScraperHostedService> logger, IRssScraper rssScraper, IPostRepository postProcessor, ISourceRepository sourceRepository, IProviderRepository providerRepository)
        {
            _logger = logger;
            _rssScraper = rssScraper;
            _postProcessor = postProcessor;
            _sourceRepository = sourceRepository;
            _providerRepository = providerRepository;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("RssScraperHostedService running.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var providerLinks = await _providerRepository.GetLinkProvider();
                    foreach (var link in providerLinks)
                    {
                        await _rssScraper.ScrapeAndStoreRssData(link);
                    }
                    //await ProcessSourceUrls();

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
