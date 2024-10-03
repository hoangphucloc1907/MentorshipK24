namespace NewsAggregator.Entity
{
	public class RSSFeedItem
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public string Link { get; set; }
		public int RSSFeedId { get; set; } // Khóa ngoại từ bảng RSSFeeds
	}
}