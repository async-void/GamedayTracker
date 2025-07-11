using GamedayTracker.Jobs;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    .WithInterval(TimeSpan.FromMinutes(1)) //TODO: Run daily
                    .RepeatForever())
                .Build();
            await scheduler.Start();
            await scheduler.ScheduleJob(job, trigger);
        }
    }
}
