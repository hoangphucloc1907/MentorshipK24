namespace NewsAggregator.Repository
{
	public interface IProviderRepository
	{
		Task<int> GetOrInsertProvider(string url);
	}
}
