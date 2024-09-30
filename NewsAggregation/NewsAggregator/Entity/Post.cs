namespace NewsAggregator.Entity
{
    public class Post
    {
        public int PostID { get; set; }
        public int UserID { get; set; }
        public int SquadID { get; set; }
        public int SourceID { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string PostType { get; set; }
        public int ReadingTime { get; set; }
        public string RssLink { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
