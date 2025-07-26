using GamedayTracker.Jobs;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Schedules
{
    public class DailyStandingsScheduler(IScheduler scheduler, ILogger<DailyStandingsScheduler> logger)
    {
        private readonly IScheduler _scheduler = scheduler;
        private ILogger<DailyStandingsScheduler> _logger = logger;

        public async Task StartAsync()
        {
            var job = JobBuilder.Create<DailyStandingsJob>()
                .WithIdentity("DailyStandingsJob", "DailyStandingsGroup")
                .Build();
            var trigger = TriggerBuilder.Create()
                .WithIdentity("DailyStandingsTrigger", "DailyStandingsGroup")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithInterval(TimeSpan.FromHours(12))
                    .RepeatForever())
                .Build();
            _logger.LogInformation("Scheduling Daily Standings Job with trigger: {Trigger}", trigger.Key);
            await _scheduler.ScheduleJob(job, trigger);
            await _scheduler.Start();
        }
    }
}
