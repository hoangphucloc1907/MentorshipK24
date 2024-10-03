namespace NewsAggregator.Entity
{
    public class SourceCategory
    {
		public int Id { get; set; }
		public string CategoryName { get; set; }

		// Navigation property to Sources
		public ICollection<Source> Sources { get; set; }
	}
}
