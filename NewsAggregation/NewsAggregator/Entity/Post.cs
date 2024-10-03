namespace NewsAggregator.Entity
{
    public class Post
    {
		public int Id { get; set; }
		public string Url { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }

		// Foreign key relationship with Source
		public int SourceID { get; set; }
		public Source Source { get; set; }

		// Navigation property to PostTag
		public ICollection<PostTag> PostTags { get; set; }
	}
}
