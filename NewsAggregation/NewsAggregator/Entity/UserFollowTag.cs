namespace NewsAggregator.Entity
{
    public class UserFollowTag
    {
		public int Id { get; set; }

		// Foreign key relationship with User (nếu bạn có bảng User)
		public int UserId { get; set; }

		// Foreign key relationship with Tag
		public int TagId { get; set; }
		public Tag Tag { get; set; }

	}
}
