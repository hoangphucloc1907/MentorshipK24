namespace NewsAggregator.Entity
{
    public class Bookmark
    {
        public int BookmarkID { get; set; }
        public int PostID { get; set; }
        public int UserID { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
