using GamedayTracker.Interfaces;
using GamedayTracker.Jobs;
using Microsoft.Extensions.Logging;
using Quartz;
using Serilog;

namespace GamedayTracker.Schedules
{
    public class RealTimeScoresJobScheduler(IScheduler scheduler, 
        IEvaluator evaluatorService, ILogger<RealTimeScoresJobScheduler> logger)
    {
        private readonly IScheduler _scheduler = scheduler;
        private readonly IEvaluator _evaluatorService = evaluatorService;
        private readonly ILogger<RealTimeScoresJobScheduler> _logger = logger;

        public async Task StartAsync()
        {
            var job = JobBuilder.Create<RealTimeScoresJob>()
                .WithIdentity("RealTimeScoresJob", "RealTimeScoresGroup")
                .Build();
            var trigger = TriggerBuilder.Create()
                .WithIdentity("RealTimeScoresTrigger", "RealTimeScoresGroup")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithInterval(_evaluatorService.GetInterval(_evaluatorService.Evaluate(DateTimeOffset.Now)))
                    .RepeatForever())
                .Build();
            _logger.LogInformation($"Starting Daily Headline Job...{job.Key}");
            await _scheduler.ScheduleJob(job, trigger);
            await _scheduler.Start();
        }
    }
}
