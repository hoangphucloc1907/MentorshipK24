namespace NewsAggregator.Repository
{
	public interface ISourceRepository
	{
		Task InsertSource(string sourceUrl, int categoryId, int providerId);
		Task<List<string>> GetSourceUrls();
	}
}

