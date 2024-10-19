using NewsAggregator.Repository;
using NewsAggregator.Repository.Impl;
using Quartz;

namespace NewsAggregator.Job
{
    public class FetchRssJob : IJob
    {
        private readonly IPostRepository _postRepository;
        private readonly ISourceRepository _sourceRepository;

        public FetchRssJob(IPostRepository postRepository, ISourceRepository sourceRepository)
        {
            _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
            _sourceRepository = sourceRepository ?? throw new ArgumentNullException(nameof(sourceRepository));
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var sourceUrls = await _sourceRepository.GetSourceUrls();

            var tasks = sourceUrls
                .Where(url => !string.IsNullOrEmpty(url))
                .Select(url => _postRepository.ConvertUrlToPosts(url))
                .ToList();

            await Task.WhenAll(tasks);
        }
    }
}
