namespace NewsAggregator.Repository
{
	public interface ICategoryRepository
	{
		Task<int> GetOrInsertCategory(string categoryName);
		Task<int> GetCategoryIdFromSource(string sourceUrl);
	}
}
