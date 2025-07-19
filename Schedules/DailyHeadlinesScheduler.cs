using GamedayTracker.Jobs;
using Quartz;
using Serilog;

namespace GamedayTracker.Schedules
{
    public class DailyHeadlinesScheduler(ISchedulerFactory schedulerFactory)
    {
        private readonly ISchedulerFactory _schedulerFactory = schedulerFactory;
        public async Task StartAsync()
        {
            var scheduler = await _schedulerFactory.GetScheduler();
            var job = JobBuilder.Create<DailyHeadlineJob>()
                .WithIdentity("DailyHeadlineJob", "NFL News")
                .WithDescription("Job to fetch daily NFL headlines: 24 hour interval")
                .Build();
            var trigger = TriggerBuilder.Create()
                .WithIdentity("DailyHeadlineTrigger", "NFL News")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithInterval(TimeSpan.FromHours(4))
                    .RepeatForever())
                .Build();
            Log.Information($"Starting Daily Headline Job...{job.Key}");
            await scheduler.ScheduleJob(job, trigger);
            await scheduler.Start();
        }
    }
}
