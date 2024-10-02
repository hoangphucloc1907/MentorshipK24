using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewsAggregator.Entity;
using NewsAggregator.Service;

namespace NewsAggregator.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class SourceController : ControllerBase
	{
		private readonly SourceService _sourceService;
		private readonly RssService _rssService;

		public SourceController(SourceService sourceService, RssService rssService)
		{
			_sourceService = sourceService;
			_rssService = rssService;
		}

		[HttpPost("fetch-rss")]
		public IActionResult FetchAndInsertRssData([FromBody] string rssUrl)
		{
			if (string.IsNullOrEmpty(rssUrl))
			{
				return BadRequest("RSS URL is required.");
			}

			try
			{
				List<RSSFeed> feeds = _rssService.FetchRssFeed(rssUrl);
				Source source = new Source
				{
					SourceName = "RSS Source",
					SourceUrl = rssUrl,
					SourceType = "RSS",
					SourceCategoryID = 1, // Set based on your logic
					CreatedAt = DateTime.Now,
					Description = "RSS Feed Source"
				};
				_sourceService.InsertSourceAsync(source);
				_sourceService.InsertRssFeedsAsync(feeds, source.SourceID);
				return Ok();
			}
			catch (Exception ex)
			{
				// Log the exception (you can use a logging framework like Serilog, NLog, etc.)
				// For simplicity, we're just returning the exception message in the response
				return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
			}
		}
	}
}
