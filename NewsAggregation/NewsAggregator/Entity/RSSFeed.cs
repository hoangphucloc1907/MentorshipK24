namespace NewsAggregator.Entity
{
	public class RSSFeed
	{
		public int Id { get; set; }
		public string RSSFeedUrl { get; set; }
		public int SourceID { get; set; } // Khóa ngoại từ bảng Sources
	}
}