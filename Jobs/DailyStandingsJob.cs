using DSharpPlus;
using DSharpPlus.Entities;
using GamedayTracker.Enums;
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
                await chnl.SendMessageAsync("Unable to fetch standings from ESPN API. Please check the API or try again later.");
                logger.LogInformation("Unable to fetch standings from the Espn Api, Season Mode is {seasonMode}", seasonType);
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
                    sb.AppendLine("__`Team\t  W\t L\t Pct`__");

                    if (division is not null)
                    {
                        var sorted = division.Standings.Entries
                           .OrderByDescending(e => double.Parse(e.Stats[10].DisplayValue))
                           .ThenByDescending(e => int.Parse(e.Stats[11].DisplayValue))
                           .ThenBy(e => int.Parse(e.Stats[3].DisplayValue))
                           .ThenBy(e => e.Team.Abbreviation)
                           .ToList();

                        foreach (var entry in sorted)
                        {
                            var teamName = entry.Team.Name;
                            var wins = entry.Stats[11].DisplayValue;
                            var losses = entry.Stats[3].DisplayValue;
                            var winPercent = entry.Stats[10].DisplayValue;
                            var teamAbbr = entry.Team.Abbreviation ?? "default";
                            var emoji = NflEmojiService.GetEmoji(teamAbbr);

                            sb.AppendLine($"{emoji} `{teamAbbr,-3} {wins,4} {losses,4} {winPercent,7}`");
                        }
                    }
                }

                DiscordComponent[] components =
                [
                    new DiscordTextDisplayComponent($"## NFL Standings\r\n-# {curSeason.Year}"),
                    new DiscordSeparatorComponent(true),
                    new DiscordTextDisplayComponent($"{sb}"),
                    new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
                    new DiscordSectionComponent(new DiscordTextDisplayComponent($"-# Powered by GamedayTracker ©️ {timestamp}"),
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

            //var standings = await teamDataService.GetAllTeamStandings(curSeason.Value);
            //if (standings.IsOk && standings.Value.Count > 0)
            //{
               
            //    var timestamp = DateTimeOffset.UtcNow.ToTimestamp();
            //    var grouped = standings.Value
            //        .GroupBy(s => s.Division)
            //        .Select(standing => new
            //        {
            //            Division = standing.Key,
            //            Teams = standing.Select(t => new
            //            {
            //                t.TeamName,
            //                t.Wins,
            //                t.Loses,
            //                t.Pct
            //            })
            //        })
            //        .ToList();
                
            //    for (var i = 0; i < grouped.Count; i++)
            //    {
            //        sb.AppendLine($"-# {grouped[i].Division}");
            //        sb.AppendLine("__`Team\t W\t L\tPct`__");
            //        for (var j = 0; j < grouped[i].Teams.Count(); j++)
            //        {
            //            var abbr = grouped[i].Teams.ElementAt(j).TeamName.ToAbbr();
            //            var emoji = NflEmojiService.GetEmoji(abbr);
            //            sb.AppendLine($"{emoji} `{abbr, -3}:{grouped[i].Teams.ElementAt(j).Wins, 4} {grouped[i].Teams.ElementAt(j).Loses, 4} {grouped[i].Teams.ElementAt(j).Pct, 7}`");
            //        }
            //    }
            //    DiscordComponent[] components =
            //    [
            //        new DiscordTextDisplayComponent($"## NFL Standings\r\n-# {curSeason.Value}"),
            //        new DiscordSeparatorComponent(true),
            //        new DiscordTextDisplayComponent($"{sb}"),
            //        new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
            //        new DiscordSectionComponent(new DiscordTextDisplayComponent($"-# Powered by GamedayTracker ©️ {timestamp}"),
            //            new DiscordButtonComponent(DiscordButtonStyle.Secondary, "donateId", "Donate"))
            //    ];
            //    var container = new DiscordContainerComponent(components, false, DiscordColor.DarkButNotBlack);
            //    var embed = new DiscordMessageBuilder()
            //        .EnableV2Components()
            //        .AddContainerComponent(container);
            //    var chnl = await client.GetChannelAsync(1398735401048608960);
            //    logger.LogInformation("Sending daily standings for NFL season {season}.", curSeason.Value);
            //    var msg = await chnl.SendMessageAsync(embed);

            //    try
            //    {
            //        await chnl.CrosspostMessageAsync(msg);
            //    }
            //    catch (Exception ex)
            //    {
            //        logger.LogInformation("Unable to crosspost daily standings - {message}", ex.Message);
            //    }
            //}
        }
    } 
}
