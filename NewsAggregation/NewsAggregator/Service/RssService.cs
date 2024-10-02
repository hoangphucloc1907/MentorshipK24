using System.ServiceModel.Syndication;
using System.Xml;
using NewsAggregator.Entity;

namespace NewsAggregator.Service
{
	public class RssService
	{
		public List<RSSFeed> FetchRssFeed(string rssUrl)
		{
			List<RSSFeed> feeds = new List<RSSFeed>();

			using (XmlReader reader = XmlReader.Create(rssUrl))
			{
				SyndicationFeed feed = SyndicationFeed.Load(reader);
				foreach (var item in feed.Items)
				{
					RSSFeed rssFeed = new RSSFeed
					{
						RSSFeedUrl = rssUrl,
						FeedName = item.Title.Text,
						CreatedAt = item.PublishDate.DateTime,
						TagID = 1
					};
					feeds.Add(rssFeed);
				}
			}

			return feeds;
		}
	}
}
