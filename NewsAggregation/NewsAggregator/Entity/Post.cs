namespace NewsAggregator.Entity
{
    public class Post
    {
		public int Id { get; set; } 
		public int CategoryId { get; set; }
		public string Title { get; set; }
		public string Link { get; set; }
		public string Guid { get; set; }
		public DateTimeOffset Pubdate { get; set; }
		public String Image { get; set; }
        public int ViewCount { get; set; }
		public int Upvote { get; set; }
    }
}
