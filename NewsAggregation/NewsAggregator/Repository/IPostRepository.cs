using NewsAggregator.Entity;

namespace NewsAggregator.Repository
{
	public interface IPostRepository
	{
		Task ConvertUrlToPosts(string url);
	}
}
