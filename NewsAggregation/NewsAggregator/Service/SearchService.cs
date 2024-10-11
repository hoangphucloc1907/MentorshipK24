using NewsAggregator.Entity;
using FuzzySharp;

namespace NewsAggregator.Service
{
    public class SearchService
    {
        public IEnumerable<Post> SearchPostsByTitle(string searchTerm, List<Post> posts)
        {
            // Fuzzy search posts by title
            var topResults = Process.ExtractTop(searchTerm, posts.Select(p => p.Title).ToList(), limit: 5);

            // Return the matched posts based on the original objects
            return posts.Where(post => topResults.Select(result => result.Value).Contains(post.Title));
        }

        public IEnumerable<Tag> SearchTagsByName(string searchTerm, List<Tag> tags)
        {
            // Fuzzy search tags by tag name
            var topResults = Process.ExtractTop(searchTerm, tags.Select(t => t.TagName).ToList(), limit: 5);

            // Return the matched tags based on the original objects
            return tags.Where(tag => topResults.Select(result => result.Value).Contains(tag.TagName));
        }
    }
}
