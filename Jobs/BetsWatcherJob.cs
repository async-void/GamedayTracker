using GamedayTracker.Enums;
using GamedayTracker.Interfaces;
using GamedayTracker.Models.NFL;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Jobs
{
    public class BetsWatcherJob(IEvaluator seasonTypeEvaluator, IBetting bettingService, ILogger<BetsWatcherJob> logger) : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var seasonType = seasonTypeEvaluator.Evaluate(DateTime.Now);

            if (seasonType is not RealTimeScoresMode.Offseason)
            {
                //if we make it this far then we are in SeasonType.PreGame, SeasonType.LiveGame, or SeasonType.PostGame and we want to check for bets that need to be updated!

                // get all members.
                var bets = await bettingService.GetAllBetsAsync(BetType.Moneyline);
            }
            else
            {
                logger.LogInformation("BetsWatcherJob: offseason mode, skipping BetWatcherJob...");
                return;
            }         
        }
    }
}
