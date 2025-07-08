using DSharpPlus;
using DSharpPlus.Entities;
using GamedayTracker.Interfaces;
using Quartz;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Jobs
{
    public class RealTimeScoresJob(IGameData gameDataService, DiscordClient client) : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var scoreboard = gameDataService.GetCurrentScoreboard();
           

            if (scoreboard.IsOk && scoreboard.Value.Count > 0)
            {
                var sb = new StringBuilder();
                var grouped = scoreboard.Value
                    .GroupBy(g => g.GameDate)
                    .Select(scoreboard => new
                    {
                        GameDate = scoreboard.Key,
                        Opponents = scoreboard.Select(o => new
                        {
                            o.Opponents!.AwayTeam,
                            o.Opponents.HomeTeam
                        })
                    })
                    .ToList();

                for (var i = 0; i < grouped.Count; i++)
                {
                    sb.Append($"### {grouped[i].GameDate}\n");
                    for (var j = 0; j < grouped[i].Opponents.Count(); j++)
                    {
                        sb.AppendLine($"{grouped[i].Opponents.ElementAt(j).AwayTeam.Emoji} **{grouped[i].Opponents.ElementAt(j).AwayTeam.Name}** : {grouped[i].Opponents.ElementAt(j).AwayTeam.Score} at " +
                            $"{grouped[i].Opponents.ElementAt(j).HomeTeam.Emoji} **{grouped[i].Opponents.ElementAt(j).HomeTeam.Name}** : {grouped[i].Opponents.ElementAt(j).HomeTeam.Score}");
                    }
                }

                foreach (var guild in client.Guilds.Values)
                {
                    var channel = guild.GetDefaultChannel();
                    if (channel is { } chnl)
                    {
                        var embed = new DiscordEmbedBuilder()
                            .WithTitle("Current NFL Scores")
                            .WithDescription(sb.ToString())
                            .WithColor(DiscordColor.Green);

                        await chnl.SendMessageAsync(embed.Build());
                    }
                }
                Log.Information("Fetching realtime scores....[success]");
            }
            else
            {
                Log.Error("Fetching realtime scores....[failed]");
                foreach (var guild in client.Guilds.Values)
                {
                    var channel = guild.GetDefaultChannel();
                    if (channel is { } chnl)
                    {
                        await chnl.SendMessageAsync("``could not fetch real time updated scores...``");
                    }
                }
            }
        }
    }
}
