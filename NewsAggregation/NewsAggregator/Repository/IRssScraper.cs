namespace NewsAggregator.Repository
{
	public interface IRssScraper
	{
		Task ScrapeAndStoreRssData(string url);
	}
}
