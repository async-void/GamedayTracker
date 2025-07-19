using GamedayTracker.Interfaces;
using GamedayTracker.Jobs;
using Quartz;
using Serilog;

namespace GamedayTracker.Schedules
{
    public class RealTimeScoresJobScheduler(IScheduler scheduler, 
        IEvaluator evaluatorService, IServiceProvider services)
    {
        private readonly IScheduler _scheduler = scheduler;
        private readonly IEvaluator _evaluatorService = evaluatorService;
        private readonly IServiceProvider _services = services;

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
            Log.Information($"Starting Daily Headline Job...{job.Key}");
            await _scheduler.ScheduleJob(job, trigger);
            await _scheduler.Start();
        }
    }
}
