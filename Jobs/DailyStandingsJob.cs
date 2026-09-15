using DSharpPlus;
using DSharpPlus.Entities;
using GamedayTracker.Enums;
using GamedayTracker.Extensions;
using GamedayTracker.Interfaces;
using GamedayTracker.Models.NFL;
using GamedayTracker.Services;
using GamedayTracker.Services.Espn;
using GamedayTracker.Utility;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using Quartz;
using System.Text;

namespace GamedayTracker.Jobs
{
    public class DailyStandingsJob(IEvaluator evaluator, IGameData gameDataService, DiscordClient client,
        ILogger<DailyStandingsJob> logger, IEspnClient espnClient) : IJob
    {
        private readonly IEspnClient _espnClient = espnClient;
        public async Task Execute(IJobExecutionContext context)
        {
            await SendDailyStandingsAsync();
        }

        public async Task SendDailyStandingsAsync()
        {
            var seasonType = evaluator.Evaluate(DateTime.Now);
            var chnl = await client.GetChannelAsync(1398735401048608960);

            if (seasonType == RealTimeScoresMode.Offseason)
            {
                await client.SendMessageAsync(chnl, "NFL is currently in Off Season, Daily Standing will begin again once the regular season starts.");
                logger.LogInformation("Offseason Mode.... Skipping DailyStandingsJob");
                return;
            }
            var curSeason = await _espnClient.GetSeasonAsync();
            logger.LogInformation("Fetching daily standings for NFL season {season}.", curSeason.Year);
            var standings = await _espnClient.GetStandingsAsync();

            var sb = new StringBuilder();
            if (standings.Children is null)
            {
                await chnl.SendMessageAsync("Espn hasn't published the current season standings yet.");
                logger.LogInformation("Espn hasn't published the current season standings yet.");
                return;
            }

            if (standings is { } curStandings && curStandings.Children.Count > 0) 
            {
                var timestamp = DateTimeOffset.UtcNow.ToTimestamp();
                for (var i = 0; i < curStandings.Children.Count; i++)
                {
                    var division = standings.Children?.ElementAt(i);
                    var divisionAbbr = division!.Name!.StartsWith("American") ? "AFC" : "NFC";
                    var divEmoji = NflEmojiService.GetEmoji(divisionAbbr);
                    sb.AppendLine($"\r\n**{divisionAbbr}** {divEmoji}\r\n");
                    sb.AppendLine("__`Team\t  W\tL\tPct\tGB`__");

                    if (division is not null)
                    {
                        var stats = division.Standings.Entries
                               .Select(e => e.ToTeamStanding())
                               .OrderByDescending(s => s.Pct)
                               .ThenByDescending(s => s.Wins)
                               .ThenBy(s => s.Loses)
                               .ThenBy(s => s.Abbr)
                               .ToList();

                        foreach (var s in stats)
                        {
                            var emoji = NflEmojiService.GetEmoji(s.Abbr);
                            sb.AppendLine($"{emoji} `{s.Abbr,-3} {s.Wins,4} {s.Loses,4} {s.Pct,7:F3} {s.GB.ToString() ?? "--",4}`");
                        }
                    }
                }

                DiscordComponent[] components =
                [
                    new DiscordTextDisplayComponent($"## NFL Standings\r\n-# {curSeason.Year}"),
                    new DiscordSeparatorComponent(true),
                    new DiscordTextDisplayComponent($"{sb}"),
                    new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
                    new DiscordSectionComponent(new DiscordTextDisplayComponent($"-# Powered by GamedayTracker ©️"),
                        new DiscordButtonComponent(DiscordButtonStyle.Secondary, "donateId", "Donate")),
                    new DiscordTextDisplayComponent($"-# last updated at {timestamp}")
                ];
                var container = new DiscordContainerComponent(components, false, DiscordColor.DarkButNotBlack);
                var embed = new DiscordMessageBuilder()
                    .EnableV2Components()
                    .AddContainerComponent(container);
                
                logger.LogInformation("Sending daily standings for NFL season {season}.", curSeason.Year);
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
