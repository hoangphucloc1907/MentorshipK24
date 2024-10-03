namespace NewsAggregator.Entity
{
	public class Source
	{
		public int Id { get; set; }
		public string SourceName { get; set; }
		public string SourceUrl { get; set; }
		public string ApiUrl { get; set; }
		public string Description { get; set; }
		public string SourceType { get; set; }

		// Foreign key relationship with SourceCategory
		public int SourceCategoryID { get; set; }
		public SourceCategory SourceCategory { get; set; }

		// Navigation property to RSSFeeds
		public ICollection<RSSFeed> RSSFeeds { get; set; }
	}
}
