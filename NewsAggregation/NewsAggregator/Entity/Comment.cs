namespace NewsAggregator.Entity
{
    public class Comment
    {
        public int CommentID { get; set; }
        public int PostID { get; set; }
        public int? ParentCommentID { get; set; }
        public int UserID { get; set; }
        public string Content { get; set; }
        public DateTime UpsertedAt { get; set; }
    }
}
