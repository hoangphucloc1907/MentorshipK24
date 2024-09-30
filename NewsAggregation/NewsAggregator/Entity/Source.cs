namespace NewsAggregator.Entity
{
    public class Source
    {
        public int SourceID { get; set; }
        public string SourceName { get; set; }
        public string Url { get; set; }
        public int SourceCategoryID { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Description { get; set; }

    }
}
