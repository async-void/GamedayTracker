using DSharpPlus;
using DSharpPlus.Entities;
using GamedayTracker.Interfaces;
using Quartz;
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
            var sb = new StringBuilder();

            if (scoreboard.IsOk && scoreboard.Value.Count > 0)
            {
                var grouped = scoreboard.Value
                    .GroupBy(g => g.GameDate)
                    .Select(scoreboard => new
                    {
                        GameDate = scoreboard.Key,
                        Opponents = scoreboard.Select(o => new
                        {
                            AwayTeam = o.Opponents.AwayTeam,
                            HomeTeam = o.Opponents.HomeTeam
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
            }
        }
    }
}
