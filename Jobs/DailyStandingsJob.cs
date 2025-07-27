using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Net.Gateway;
using GamedayTracker.Extensions;
using GamedayTracker.Interfaces;
using GamedayTracker.Services;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Jobs
{
    public class DailyStandingsJob(ITeamData teamDataService, IGameData gameDataService, DiscordClient client, ILogger<DailyStandingsJob> logger) : IJob
    {
        private readonly ITeamData _teamDataService = teamDataService;
        private readonly IGameData _gameDataService = gameDataService;
        private readonly DiscordClient _client = client;
        private readonly ILogger<DailyStandingsJob> _logger = logger;

        public async Task Execute(IJobExecutionContext context)
        {
            await SendDailyStandingsAsync();
        }

        public async Task SendDailyStandingsAsync()
        {
            var curSeason = _gameDataService.GetCurSeason();
            _logger.LogInformation("Fetching daily standings for NFL season {season}.", curSeason);
            var standings = await _teamDataService.GetAllTeamStandings(curSeason.Value);
            if (standings.IsOk && standings.Value.Count > 0)
            {
                var sb = new StringBuilder();
                var unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
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
                    new DiscordSectionComponent(new DiscordTextDisplayComponent($"-# Powered by GamedayTracker ©️ <t:{unixTimestamp}:F>"),
                        new DiscordButtonComponent(DiscordButtonStyle.Secondary, "donateId", "Donate"))
                ];
                var container = new DiscordContainerComponent(components, false, DiscordColor.DarkButNotBlack);
                var embed = new DiscordMessageBuilder()
                    .EnableV2Components()
                    .AddContainerComponent(container);
                var chnl = await _client.GetChannelAsync(1398735401048608960);
                _logger.LogInformation("Sending daily standings for NFL season {season}.", curSeason);
                var msg = await chnl.SendMessageAsync(embed);
                await chnl.CrosspostMessageAsync(msg);
            }
        }
    } 
}
