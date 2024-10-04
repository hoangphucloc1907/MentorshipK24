namespace NewsAggregator.Entity
{
    public class Post
    {
		public int Id { get; set; } // Primary Key
		public int CategoryId { get; set; }
		public string Title { get; set; }
		public string Link { get; set; }
		public string Guid { get; set; }
		public DateTime Pubdate { get; set; }
		public byte[] Image { get; set; }
	}
}
