using Quartz.Impl;
using Quartz.Spi;
using Quartz;

namespace NewsAggregator.Job
{
    public class QuartzStartup
    {
        private readonly IConfiguration _configuration;

        public QuartzStartup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureQuartz(IServiceCollection services)
        {
            services.AddSingleton<IJobFactory, SingletonJobFactory>();
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();

            // Add our job
            services.AddSingleton<FetchRssJob>();

            var cronExpression = _configuration["Quartz:CronExpression"];

            services.AddSingleton(new JobSchedule(
                jobType: typeof(FetchRssJob),
                cronExpression: cronExpression)); // Run based on configuration

            services.AddHostedService<QuartzHostedService>();
        }
    }
}
