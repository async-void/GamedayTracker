using DSharpPlus;
using DSharpPlus.Entities;
using GamedayTracker.Extensions;
using GamedayTracker.Interfaces;
using GamedayTracker.Services;
using GamedayTracker.Utility;
using Microsoft.Extensions.Logging;
using Quartz;
using System.Text;

namespace GamedayTracker.Jobs
{
    public class DailyStandingsJob(ITeamData teamDataService, IGameData gameDataService, DiscordClient client, ILogger<DailyStandingsJob> logger) : IJob
    {
        

        public async Task Execute(IJobExecutionContext context)
        {
            await SendDailyStandingsAsync();
        }

        public async Task SendDailyStandingsAsync()
        {
            var curSeason = gameDataService.GetCurSeason();
            logger.LogInformation("Fetching daily standings for NFL season {season}.", curSeason.Value);
            var standings = await teamDataService.GetAllTeamStandings(curSeason.Value);
            if (standings.IsOk && standings.Value.Count > 0)
            {
                var sb = new StringBuilder();
                var timestamp = DateTimeOffset.UtcNow.ToTimestamp();
                var grouped = standings.Value
                    .GroupBy(s => s.Division)
                    .Select(standing => new
                    {
                        Division = standing.Key,
                        Teams = standing.Select(t => new
                        {
                            t.TeamName,
                            t.Wins,
                            t.Loses,
                            t.Pct
                        })
                    })
                    .ToList();
                
                for (var i = 0; i < grouped.Count; i++)
                {
                    sb.AppendLine($"-# {grouped[i].Division}");
                    sb.AppendLine("__`Team\t W\t L\tPct`__");
                    for (var j = 0; j < grouped[i].Teams.Count(); j++)
                    {
                        var abbr = grouped[i].Teams.ElementAt(j).TeamName.ToAbbr();
                        var emoji = NflEmojiService.GetEmoji(abbr);
                        sb.AppendLine($"{emoji} `{abbr, -3}:{grouped[i].Teams.ElementAt(j).Wins, 4} {grouped[i].Teams.ElementAt(j).Loses, 4} {grouped[i].Teams.ElementAt(j).Pct, 7}`");
                    }
                }
                DiscordComponent[] components =
                [
                    new DiscordTextDisplayComponent($"## NFL Standings\r\n-# {curSeason.Value}"),
                    new DiscordSeparatorComponent(true),
                    new DiscordTextDisplayComponent($"{sb}"),
                    new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
                    new DiscordSectionComponent(new DiscordTextDisplayComponent($"-# Powered by GamedayTracker ©️ {timestamp}"),
                        new DiscordButtonComponent(DiscordButtonStyle.Secondary, "donateId", "Donate"))
                ];
                var container = new DiscordContainerComponent(components, false, DiscordColor.DarkButNotBlack);
                var embed = new DiscordMessageBuilder()
                    .EnableV2Components()
                    .AddContainerComponent(container);
                var chnl = await client.GetChannelAsync(1398735401048608960);
                logger.LogInformation("Sending daily standings for NFL season {season}.", curSeason.Value);
                var msg = await chnl.SendMessageAsync(embed);

                try
                {
                    await chnl.CrosspostMessageAsync(msg);
                }
                catch (Exception ex)
                {
                    logger.LogInformation("Unable to crosspost daily standings - {message}", ex.Message);
                }
            }
        }
    } 
}
