using NewsAggregator.Entity;

namespace NewsAggregator.Repository
{
	public interface IPostProcessor
	{
		Task ConvertUrlToPosts(string url);
	}
}
