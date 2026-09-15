using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Entities;
using GamedayTracker.Enums;
using GamedayTracker.Extensions;
using GamedayTracker.Interfaces;
using GamedayTracker.Services;
using GamedayTracker.Services.Espn;
using GamedayTracker.Utility;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.Text;

namespace GamedayTracker.SlashCommands.NFL
{
    public class StandingsSlashCommand(IEspnClient espnClient, IEvaluator evaluator, ILogger<StandingsSlashCommand> logger)
    {
        private readonly IEspnClient _espnClient = espnClient;
        private readonly IEvaluator _evaluator = evaluator;
        private readonly ILogger<StandingsSlashCommand> _logger = logger;

        [Command("standings")]
        [Description("get season Team Standings")]
        public async Task GetStandings(SlashCommandContext ctx)
        {
            await ctx.DeferResponseAsync();
            var seasonType = _evaluator.Evaluate(DateTime.Now);

            if (seasonType == RealTimeScoresMode.Offseason)
            {
                await ctx.RespondAsync("NFL is currently in Off Season, Daily Standing will begin again once the regular season starts.");
                _logger.LogInformation("Offseason Mode.... Skipping DailyStandingsJob");
                return;
            }
            var curSeason = await _espnClient.GetSeasonAsync();
            _logger.LogInformation("Fetching daily standings for NFL season {season}.", curSeason.Year);
            var standings = await _espnClient.GetStandingsAsync();

            var sb = new StringBuilder();
            if (standings.Children is null)
            {
                await ctx.RespondAsync("Espn hasn't published the current season standings yet.");
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
                    sb.AppendLine("__`Team\t  W\t L\t Pct`__");

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
                            sb.AppendLine($"{emoji} `{s.Abbr,-3} {s.Wins,4} {s.Loses,4} {s.Pct,7:F3}`");
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

                await ctx.RespondAsync(embed);
            }




            //var testStandings = await gameData.GetNFLStandingsAsync();
            //var standings = await teamDataService.GetAllTeamStandings(season);
            //var unixTimestamp = DateTimeOffset.UtcNow.ToTimestamp();
            //if (standings.IsOk && standings.Value.Count > 0)
            //{
            //    var sb = new StringBuilder();
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
            //            sb.AppendLine($"{emoji} `{abbr,-3}:{grouped[i].Teams.ElementAt(j).Wins,4} {grouped[i].Teams.ElementAt(j).Loses,4} {grouped[i].Teams.ElementAt(j).Pct,7}`");
            //        }
            //    }
            //    DiscordComponent[] components =
            //    [
            //        new DiscordTextDisplayComponent($"## NFL Standings\r\n-# {season}"),
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
            //    await ctx.RespondAsync(new DiscordInteractionResponseBuilder(embed));
            //}
            //else
            //{
            //    DiscordComponent[] components =
            //    [
            //        new DiscordTextDisplayComponent($"**ERROR**"),
            //        new DiscordSeparatorComponent(true),
            //        new DiscordTextDisplayComponent($"Could not find data for season {season}"),
            //        new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
            //        new DiscordSectionComponent(new DiscordTextDisplayComponent($"-# Gameday Tracker ©️ {unixTimestamp}"),
            //                                new DiscordButtonComponent(DiscordButtonStyle.Secondary, "donateId", "Donate"))
            //    ];
            //    var container = new DiscordContainerComponent(components, false, DiscordColor.DarkRed);
            //    var message = new DiscordInteractionResponseBuilder()
            //        .EnableV2Components()
            //        .AddContainerComponent(container);

            //    await ctx.RespondAsync(new DiscordInteractionResponseBuilder(message));
            //}

        }
    }
}
