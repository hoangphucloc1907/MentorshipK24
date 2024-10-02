namespace NewsAggregator.Entity
{
    public class RSSFeed
    {
		public int RSSFeedID { get; set; }
		public int SourceID { get; set; }
		public string RSSFeedUrl { get; set; }
		public string FeedName { get; set; }
		public int TagID { get; set; }
		public DateTime CreatedAt { get; set; }
	}
}