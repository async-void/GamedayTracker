using System.ComponentModel;
using ChalkDotNET;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Entities;
using GamedayTracker.Extensions;
using GamedayTracker.Interfaces;
using GamedayTracker.Services;

namespace GamedayTracker.SlashCommands.Stats
{
    public class TeamStatsSlashCommand(ITeamData teamDataService)
    {
        [Command("teamstats")]
        [Description("Get the Team Statistics")]
        public async Task GetTeamStats(SlashCommandContext ctx, [Parameter("team")] string teamName, [Parameter("season")] int season)
        {
            await ctx.DeferResponseAsync();
            //var s = await teamDataService.GetStatsAsync(season);
            var stats = await teamDataService.GetTeamStatsAsync(season, teamName);
            if (!stats.IsOk)
            {
                //TODO: build response here
                Console.WriteLine(
                    $"{Chalk.Yellow($"[{DateTimeOffset.UtcNow}]")} {Chalk.Yellow($"[Gameday Tracker]")} {Chalk.DarkBlue("[INFO]")} {Chalk.DarkGray($"{stats.Error.ErrorMessage}")}");
                await ctx.EditResponseAsync($"Error: {stats.Error}");
                return;
            }

            var teamAbbr = teamName.ToAbbr();
            var teamEmoji = NflEmojiService.GetEmoji(teamAbbr);
            var logoUrl = LogoPathService.GetLogoPath(teamAbbr);
            DiscordComponent[] components =
            [
                new DiscordTextDisplayComponent($"**{teamEmoji} {teamName} {stats.Value.Season} Stats**"),
                new DiscordSeparatorComponent(true),
                new DiscordSectionComponent(new DiscordTextDisplayComponent($"Team: **{stats.Value.TeamName}**{teamEmoji}\rGms: **{stats.Value.GamesPlayed}**\rTotal Pts: **{stats.Value.TotalPoints}**\r" +
                                                                            $"Pts/G: **{stats.Value.PointsPerGame}**\rRushYds: **{stats.Value.RushYardsTotal:#,##0}**\r" +
                                                                            $"RYds/G: **{stats.Value.RushPerGame}**\rPassYds: **{stats.Value.PassYardsTotal:#,##0}**\r" +
                                                                            $"PYds/G: **{stats.Value.PassYardsPerGame}**\rTotal Yds: **{stats.Value.TotalYards:#,##0}**\r" +
                                                                            $"Yds/G: **{stats.Value.YardsPerGame}**"), new DiscordThumbnailComponent(logoUrl)),
                
                new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
                new DiscordSectionComponent(new DiscordTextDisplayComponent($"Gameday Tracker ©️ {DateTime.UtcNow.ToLongDateString()}"),  new DiscordButtonComponent(DiscordButtonStyle.Success, "donateId", "Donate")),
            ];

            var container = new DiscordContainerComponent(components, false, DiscordColor.DarkGray);
            var msgBuilder = new DiscordMessageBuilder()
                .EnableV2Components()
                .AddContainerComponent(container);
            await ctx.RespondAsync(msgBuilder);
        }
    }
}
