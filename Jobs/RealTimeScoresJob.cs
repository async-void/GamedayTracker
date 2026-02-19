using DSharpPlus;
using DSharpPlus.Entities;
using GamedayTracker.Interfaces;
using GamedayTracker.Services;
using GamedayTracker.Utility;
using Quartz;
using Serilog;
using System.Text;

namespace GamedayTracker.Jobs
{
    public class RealTimeScoresJob(IGameData gameDataService, IDiscordEmbedService embedService, DiscordClient client, IEvaluator evaluator) : IJob
    {
        private readonly IGameData _gameDataService = gameDataService;
        private readonly DiscordClient _client = client;
        public async Task Execute(IJobExecutionContext context)
        {
            Log.Information("Fetching realtime scores....[started]");
            var seasonType = evaluator.Evaluate(DateTimeOffset.Now);

            if (seasonType.Equals(Enums.RealTimeScoresMode.Offseason))
            {
                Log.Information("Fetching realtime scores....[skipped] - season is currently in offseason mode");
                return;
            }

            var scoreboard = await _gameDataService.GetNFLScoresAsync();
            var chnl = await _client.GetChannelAsync(1398021337498390539);

            if (scoreboard.Events.Count > 0)
            {
                var scoresEmbed = await embedService.CreateScoresEmbed(scoreboard);
                var msg = await chnl.SendMessageAsync(scoresEmbed);
                try
                {
                    await chnl.CrosspostMessageAsync(msg);
                    Log.Information("Fetching realtime scores....[success] - scores sent to live-scores channel");
                }
                catch(Exception ex)
                {
                    Log.Error($"Error sending message to live-scores channel: {ex.Message}");
                }

            }
            else
            {
                Log.Error($"Fetching realtime scores....[failed] REASON: could not find scores for season {scoreboard.Season.Year}");
                await chnl.SendMessageAsync($"fetching real time score failed: Season Mode is {seasonType}");
            }
        }
    }
}
