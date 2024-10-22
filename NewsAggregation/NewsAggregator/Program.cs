
using NewsAggregator.Job;
using NewsAggregator.Repository;
using NewsAggregator.Repository.Impl;
using NewsAggregator.Service;

namespace NewsAggregator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Register dependencies
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

            builder.Services.AddSingleton<ICategoryRepository>(provider =>
                new CategoryRepository(connectionString, provider.GetRequiredService<ILogger<CategoryRepository>>()));
            builder.Services.AddSingleton<IProviderRepository>(provider =>
                new ProviderRepository(connectionString, provider.GetRequiredService<ILogger<ProviderRepository>>()));
            builder.Services.AddSingleton<ISourceRepository>(provider =>
                new SourceRepository(connectionString, provider.GetRequiredService<ILogger<SourceRepository>>()));
            builder.Services.AddSingleton<IRssScraper, RssScraper>();
            builder.Services.AddSingleton<IPostRepository>(provider =>
                new PostRepository(connectionString, provider.GetRequiredService<ILogger<PostRepository>>(),
                provider.GetRequiredService<ISourceRepository>(), provider.GetRequiredService<ICategoryRepository>()));
            builder.Services.AddScoped<SearchService>();

            builder.Services.AddHostedService<RssScraperHostedService>(provider =>
            {
                var logger = provider.GetRequiredService<ILogger<RssScraperHostedService>>();
                var rssScraper = provider.GetRequiredService<IRssScraper>();
                var postProcessor = provider.GetRequiredService<IPostRepository>();
                var sourceRepository = provider.GetRequiredService<ISourceRepository>();
                var providerRepository = provider.GetRequiredService<IProviderRepository>();
                return new RssScraperHostedService(logger, rssScraper, postProcessor, sourceRepository, providerRepository);
            });

            builder.Services.AddSingleton<QuartzStartup>();

            // Build the service provider once to avoid multiple builds
            using var serviceProvider = builder.Services.BuildServiceProvider();
            var quartzStartup = serviceProvider.GetService<QuartzStartup>();

            // Configure Quartz
            quartzStartup?.ConfigureQuartz(builder.Services);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
