using DSharpPlus;
using DSharpPlus.Entities;
using GamedayTracker.Enums;
using GamedayTracker.Extensions;
using GamedayTracker.Interfaces;
using GamedayTracker.Services;
using GamedayTracker.Utility;
using Microsoft.Extensions.Logging;
using Quartz;
using System.Text;

namespace GamedayTracker.Jobs
{
    public class DailyStandingsJob(IEvaluator evaluator, ITeamData teamDataService, IGameData gameDataService, DiscordClient client, ILogger<DailyStandingsJob> logger) : IJob
    {
        

        public async Task Execute(IJobExecutionContext context)
        {
            await SendDailyStandingsAsync();
        }

        public async Task SendDailyStandingsAsync()
        {
            var seasonType = evaluator.Evaluate(DateTime.Now);

            if (seasonType == RealTimeScoresMode.Offseason)
            {
                logger.LogInformation("Offseason Mode.... Skipping DailyStandingsJob");
                return;
            }
            var curSeason = gameDataService.GetCurSeason();
            logger.LogInformation("Fetching daily standings for NFL season {season}.", curSeason.Value);
            var standings = await gameDataService.GetNFLStandingsAsync();
            var sb = new StringBuilder();

            if (standings is { }  curStandings && curStandings.Children.Count > 0) 
            {
                var timestamp = DateTimeOffset.UtcNow.ToTimestamp();
                for (var i = 0; i < curStandings.Children.Count; i++)
                {
                    var division = standings.Children.ElementAt(i);
                    sb.AppendLine($"\r\n**{division.Name}**\r\n");
                    sb.AppendLine("__`Team\t W\t L\t Pct`__");
                    // Sort the entries BEFORE looping
                    var sorted = division.Standings.Entries
                        .OrderByDescending(e => double.Parse(e.Stats[10].DisplayValue)) // win %
                        .ThenByDescending(e => int.Parse(e.Stats[11].DisplayValue))     // wins
                        .ThenBy(e => int.Parse(e.Stats[3].DisplayValue))                // losses
                        .ThenBy(e => e.Team.Abbreviation)                               // final tiebreaker
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
