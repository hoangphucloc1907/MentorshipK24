﻿namespace NewsAggregator.Entity
{
	public class Source
	{
		public int Id { get; set; } 
		public string SourceUrl { get; set; }
		public int CategoryId { get; set; }
		public int ProviderId { get; set; }
	}
}
