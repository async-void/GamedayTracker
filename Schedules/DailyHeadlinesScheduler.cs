using GamedayTracker.Jobs;
using GamedayTracker.Models;
using Quartz;

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
                .Build();
            var trigger = TriggerBuilder.Create()
                .WithIdentity("DailyHeadlineTrigger", "NFL News")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithInterval(TimeSpan.FromHours(24)) //TODO: Run daily
                    .RepeatForever())
                .Build();
            Serilog.Log.Information($"Starting Daily Headline Job...{job.Key}");
            await scheduler.Start();
            await scheduler.ScheduleJob(job, trigger);
        }
    }
}
