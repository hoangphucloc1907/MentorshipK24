
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
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();
			builder.Services.AddControllers();

			// Register dependencies
			var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
			if (string.IsNullOrEmpty(connectionString))
			{
				throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");
			}

			builder.Services.AddSingleton<ICategoryRepository>(provider => new CategoryRepository(connectionString, provider.GetRequiredService<ILogger<CategoryRepository>>()));
			builder.Services.AddSingleton<IProviderRepository>(provider => new ProviderRepository(connectionString, provider.GetRequiredService<ILogger<ProviderRepository>>()));
			builder.Services.AddSingleton<ISourceRepository>(provider => new SourceRepository(connectionString, provider.GetRequiredService<ILogger<SourceRepository>>()));
			builder.Services.AddSingleton<IRssScraper, RssScraper>();
			builder.Services.AddSingleton<IPostRepository>(provider => new PostRepository(connectionString, provider.GetRequiredService<ILogger<PostRepository>>(), provider.GetRequiredService<ISourceRepository>(), provider.GetRequiredService<ICategoryRepository>()));
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

            // Configure Quartz
            QuartzStartup.ConfigureQuartz(builder.Services);

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
