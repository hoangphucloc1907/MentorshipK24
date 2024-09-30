namespace NewsAggregator.Entity
{
    public class Squad
    {
        public int SquadID { get; set; }
        public string SquadName { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public int UserID { get; set; }
        public bool IsPublic { get; set; }
    }
}
