using GamedayTracker.Interfaces;
using GamedayTracker.Jobs;
using GamedayTracker.Services;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Schedules
{
    public class BetsWatcherScheduler(ISchedulerFactory schedulerFactory)
    {
        private readonly ISchedulerFactory _schedulerFactory = schedulerFactory;
        public async Task StartAsync()
        {
            var scheduler = await _schedulerFactory.GetScheduler();
            var job = JobBuilder.Create<BetsWatcherJob>()
                .WithIdentity("BetsWatcherJob", "Bets Watcher")
                .WithDescription("watches all member bets, calculates winners after game is complete")
                .Build();
            var trigger = TriggerBuilder.Create()
                .WithIdentity("BetsWatcherJob", "Bets Watcher Trigger")
                .WithDescription("Trigger to run Bets Watcher Job every 10 minutes")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithInterval(TimeSpan.FromHours(12))
                    .RepeatForever())
                .Build();
            await scheduler.ScheduleJob(job, trigger);
            await scheduler.Start();
        }
    }
}
