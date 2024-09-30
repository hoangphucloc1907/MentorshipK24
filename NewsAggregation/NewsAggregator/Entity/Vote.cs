namespace NewsAggregator.Entity
{
    public class Vote
    {
        public int VoteID { get; set; }
        public int UserID { get; set; }
        public int PostID { get; set; }
        public string VoteType { get; set; }
    }
}
