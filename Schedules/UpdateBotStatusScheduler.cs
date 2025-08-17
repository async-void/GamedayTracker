using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Schedules
{
    public class UpdateBotStatusScheduler(ISchedulerFactory schedulerFactory)
    {
        private readonly ISchedulerFactory _schedulerFactory = schedulerFactory;
        public async Task StartAsync()
        {
            var scheduler = await _schedulerFactory.GetScheduler();
            var job = JobBuilder.Create<Jobs.DailyNumbersJob>()
                .WithIdentity("UpdateBotStatusJob", "Update Bot Status")
                .WithDescription("Job to update bot status , recalculates guild count.")
                .Build();
            var trigger = TriggerBuilder.Create()
                .WithIdentity("UpdateBotStatusJob", "Bot Status Trigger")
                .WithDescription("Trigger to run Update Bot Status Job every 10 minutes")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithInterval(TimeSpan.FromMinutes(10))
                    .RepeatForever())
                .Build();
            await scheduler.ScheduleJob(job, trigger);
            await scheduler.Start();
        }
    }
}
