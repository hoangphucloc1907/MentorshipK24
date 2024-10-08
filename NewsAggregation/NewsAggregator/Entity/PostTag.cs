﻿namespace NewsAggregator.Entity
{
    public class PostTag
    {
		public int Id { get; set; }

		// Foreign key relationship with Post
		public int PostId { get; set; }
		public Post Post { get; set; }

		// Foreign key relationship with Tag
		public int TagId { get; set; }
		public Tag Tag { get; set; }

	}
}
