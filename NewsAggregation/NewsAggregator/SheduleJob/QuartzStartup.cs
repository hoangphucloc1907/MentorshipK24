using Quartz.Impl;
using Quartz.Spi;
using Quartz;

namespace NewsAggregator.Job
{
    public class QuartzStartup
    {
        public static void ConfigureQuartz(IServiceCollection services)
        {
            services.AddSingleton<IJobFactory, SingletonJobFactory>();
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();

            // Add our job
            services.AddSingleton<FetchRssJob>();
            services.AddSingleton(new JobSchedule(
                jobType: typeof(FetchRssJob),
                cronExpression: "0 7 23,0 * * ?")); // Run every day at 11:07pm and 12am

            services.AddHostedService<QuartzHostedService>();
        }
    }
}
