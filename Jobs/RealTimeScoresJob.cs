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
    public class RealTimeScoresJob(IGameData gameDataService, IDiscordEmbedService embedService, DiscordClient client) : IJob
    {
        private readonly IGameData _gameDataService = gameDataService;
        private readonly DiscordClient _client = client;
        public async Task Execute(IJobExecutionContext context)
        {
            Log.Information("Fetching realtime scores....[started]");
            //var scoreboard = await _gameDataService.GetCurrentScoreboard();
            var scoreboard = await _gameDataService.GetNFLScoresAsync();
            var liveGames = _gameDataService.GetLiveGames(scoreboard);
           // var liveGameEmbed = embedService.CreateLiveGamesEmbed(liveGames);
            var scoresEmbed = embedService.CreateScoresEmbed(scoreboard);

            if (scoreboard.Events.Count > 0)
            {
                //var sb = new StringBuilder();
                //var unixTimestamp = DateTimeOffset.UtcNow.ToTimestamp();
                //var grouped = scoreboard.Events
                //    .GroupBy(g => g.Date)
                //    .Select(scoreboard => new
                //    {
                //        GameDate = scoreboard.Key,
                //        Opponents = scoreboard.Select(o => new
                //        {
                //            Away = o.Competitions[0].Competitors[0].Team,
                //            Home = o.Competitions[0].Competitors[1].Team,
                //        })
                //    })
                //    .ToList();

                //for (var i = 0; i < grouped.Count; i++)
                //{
                //    sb.Append($"### {grouped[i].GameDate}\n");
                //    for (var j = 0; j < grouped[i].Opponents.Count(); j++)
                //    {
                //        if (grouped[i].Opponents.ElementAt(j).AwayTeam.Score >
                //            grouped[i].Opponents.ElementAt(j).HomeTeam.Score)
                //        {
                //            sb.AppendLine($"{grouped[i].Opponents.ElementAt(j).AwayTeam.Emoji} **{grouped[i].Opponents.ElementAt(j).AwayTeam.Score}** : " +
                //              $"{grouped[i].Opponents.ElementAt(j).HomeTeam.Score} {grouped[i].Opponents.ElementAt(j).HomeTeam.Emoji}");
                //        }
                //        else if (grouped[i].Opponents.ElementAt(j).AwayTeam.Score <
                //            grouped[i].Opponents.ElementAt(j).HomeTeam.Score)
                //        {
                //            sb.AppendLine($"{grouped[i].Opponents.ElementAt(j).AwayTeam.Emoji} {grouped[i].Opponents.ElementAt(j).AwayTeam.Score} : " +
                //              $"**{grouped[i].Opponents.ElementAt(j).HomeTeam.Score}** {grouped[i].Opponents.ElementAt(j).HomeTeam.Emoji}");
                //        }
                //        else
                //        {
                //            sb.AppendLine($"{grouped[i].Opponents.ElementAt(j).AwayTeam.Emoji} {grouped[i].Opponents.ElementAt(j).AwayTeam.Score} : " +
                //              $"{grouped[i].Opponents.ElementAt(j).HomeTeam.Score} {grouped[i].Opponents.ElementAt(j).HomeTeam.Emoji} - tie");
                //        }


                //    }
                //}

                //DiscordComponent[] components =
                //[
                //    new DiscordTextDisplayComponent("Current NFL Scores"),
                //    new DiscordSeparatorComponent(true),
                //   // new DiscordTextDisplayComponent($"{sb}"),
                //    new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
                //    new DiscordSectionComponent(new DiscordTextDisplayComponent($"-# Powered by GamedayTracker ©️ {unixTimestamp}"),
                //        new DiscordButtonComponent(DiscordButtonStyle.Secondary, "donateId", "Donate"))
                //];
                //var container = new DiscordContainerComponent(components);
                //var embed = new DiscordMessageBuilder()
                //    .EnableV2Components()
                //    .AddContainerComponent(container);

                var chnl = await _client.GetChannelAsync(1398021337498390539);
               
                var msg = await chnl.SendMessageAsync(scoresEmbed);
                await chnl.CrosspostMessageAsync(msg);

                Log.Information("Fetching realtime scores....[success] - scores sent to live-scores channel");
            }
            else
            {
                Log.Error($"Fetching realtime scores....[failed] REASON: {scoreboard}");
            }
        }
    }
}
