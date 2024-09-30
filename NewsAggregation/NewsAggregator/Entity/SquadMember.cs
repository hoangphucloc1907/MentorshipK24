namespace NewsAggregator.Entity
{
    public class SquadMember
    {
        public int SquadMemberID { get; set; }
        public int SquadID { get; set; }
        public int UserID { get; set; }
        public DateTime JoinedAt { get; set; }

    }
}
