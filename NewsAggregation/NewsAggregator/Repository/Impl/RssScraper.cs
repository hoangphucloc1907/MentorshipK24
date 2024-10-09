using HtmlAgilityPack;
using NewsAggregator.Repository;
using System.Text.RegularExpressions;

namespace NewsAggregator.Repository.Impl
{
    public class RssScraper : IRssScraper
    {
        private readonly ILogger<RssScraper> _logger;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProviderRepository _providerRepository;
        private readonly ISourceRepository _sourceRepository;
        private readonly Dictionary<string, string> _providerXPathMappings;

        public RssScraper(ILogger<RssScraper> logger, ICategoryRepository categoryRepository, IProviderRepository providerRepository, ISourceRepository sourceRepository)
        {
            _logger = logger;
            _categoryRepository = categoryRepository;
            _providerRepository = providerRepository;
            _sourceRepository = sourceRepository;
            _providerXPathMappings = new Dictionary<string, string>
            {
                { "tuoitre.vn", "//ul[@class='list-rss clearfix']/li/a" },
                { "vnexpress.net", "//ul[@class='list-rss']/li/a" },
                { "thanhnien.vn", "//ul[@class='cate-content']/li/a" }
                // Add more mappings as needed
            };
        }

        public async Task ScrapeAndStoreRssData(string url)
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
                    var categoryName = Regex.Replace(categoryNode.InnerText.Trim(), @"<[^>]+>|RSS|-.+", "").Trim();
                    var rssUrl = categoryNode.GetAttributeValue("href", string.Empty);

                    if (!string.IsNullOrEmpty(categoryName) && !string.IsNullOrEmpty(rssUrl))
                    {
                        if (!rssUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                        {
                            rssUrl = new Uri(uri, rssUrl).ToString();
                        }

                        int providerId = await _providerRepository.GetOrInsertProvider(url);
                        int categoryId = await _categoryRepository.GetOrInsertCategory(categoryName);

                        await _sourceRepository.InsertSource(rssUrl, categoryId, providerId);
                        _logger.LogInformation($"Category '{categoryName}' with RSS URL '{rssUrl}' has been processed.");
                    }
                }
            }
        }
    }
}
