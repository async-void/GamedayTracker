using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Schedules
{
    public class DailyNumbersScheduler(ISchedulerFactory schedulerFactory)
    {
        private readonly ISchedulerFactory _schedulerFactory = schedulerFactory;
        public async Task StartAsync()
        {
            var scheduler = await _schedulerFactory.GetScheduler();
            var job = JobBuilder.Create<Jobs.DailyNumbersJob>()
                .WithIdentity("DailyNumbersJob", "Daily Numbers")
                .WithDescription("Job to generate daily numbers: 2 hour interval")
                .Build();
            var trigger = TriggerBuilder.Create()
                .WithIdentity("DailyNumbersTrigger", "Daily Numbers")
                .WithDescription("Trigger to run Daily Numbers Job every 2 hours")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithInterval(TimeSpan.FromHours(2))
                    .RepeatForever())
                .Build();
            await scheduler.ScheduleJob(job, trigger);
            await scheduler.Start();
        }
    }
}
