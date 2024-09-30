namespace NewsAggregator.Entity
{
    public class Tag
    {
        public int TagID { get; set; }
        public int CategoryTagID { get; set; }
        public string TagName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
