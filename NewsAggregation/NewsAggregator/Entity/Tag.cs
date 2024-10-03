namespace NewsAggregator.Entity
{
    public class Tag
    {
		public int Id { get; set; }
		public string Name { get; set; }

		// Navigation property to PostTag
		public ICollection<PostTag> PostTags { get; set; }
	}
}
