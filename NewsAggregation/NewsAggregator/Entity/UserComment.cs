namespace NewsAggregator.Entity
{
    public class UserComment
    {
        public int CommentID { get; set; }
        public int UserID { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

    }
}
