namespace NewsAggregator.Entity
{
    public class Source
    {
        public int SourceID { get; set; }
        public string SourceName { get; set; }
        public string SourceUrl { get; set; }
        public string ApiUrl { get; set; }
        public int SourceCategoryID { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Description { get; set; }

        public string SourceType { get; set; }

	}
}
